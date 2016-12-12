namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public abstract class ProcessingBlockBase : ICommandBlock, IVisitableElement
    {
        private readonly object _initLock = new object();

        private ProcessingBlockOptions _options;
        public ProcessingBlockOptions Options
        {
            get { return this._options; }
            set
            {
                bool isInited = this._transformation != null;
                if (isInited)
                {
                    throw new InvalidOperationException("Can not change options after block was initialized");
                }

                this._options = value;
            }
        }

        private TransformManyBlock<FlowToken, FlowToken> _transformation;
        private FlowExceptionManager _flowExceptionManager = new FlowExceptionManager();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        protected BlockLogger BlockLogger { get; private set; }

        protected event EventHandler OnAllTokenProcessed;
        protected event EventHandler<FinishedTokenReceivedEventArgs> OnFinishedTokenReceived;

        protected ProcessingBlockBase(ProcessingBlockOptions options)
        {
            this.Options = options;
            this.BlockLogger = BlockLoggerFactory.Create(this.GetType());
        }

        protected ProcessingBlockBase()
        {
            this.BlockLogger = BlockLoggerFactory.Create(this.GetType());
        }

        private void Init()
        {
            lock (this._initLock)
            {
                if (this.Options == null)
                {
                    throw new InvalidOperationException("Options can not be null");
                }

                if (this._transformation != null)
                {
                    return;
                }

                var tplOptions = new ExecutionDataflowBlockOptions
                                     {
                                         CancellationToken = this._cancellationTokenSource.Token,
                                         MaxDegreeOfParallelism = this.Options.MaxDegreeOfParallelism
                                     };

                this._transformation = new TransformManyBlock<FlowToken, FlowToken>(o => this.TransformInternal(o), tplOptions);
            }

            this.EntryPoint = this._transformation;
            this.Output = this._transformation;

            if (this.Options.TurnOnBroadCast)
            {
                var broadcastBlock = new BroadcastBlock<FlowToken>(o => new FlowToken(o), new DataflowBlockOptions
                                                                                              {
                                                                                                  CancellationToken = this._cancellationTokenSource.Token
                                                                                              });

                this._transformation.LinkTo(broadcastBlock, new DataflowLinkOptions { PropagateCompletion = true });
                this.Output = broadcastBlock;
            }

            // call when the block finishes (all tokens were processed)
            var completionSource = new TaskCompletionSource<string>();
            this.Output.Completion.ContinueWith(o =>
                {
                    if (o.IsCanceled)
                    {
                        completionSource.SetCanceled();
                        return;
                    }

                    if (o.IsFaulted && o.Exception != null)
                    {
                        completionSource.SetException(o.Exception);
                        return;
                    }

                    if (this.OnAllTokenProcessed != null)
                    {
                        this.OnAllTokenProcessed(this, EventArgs.Empty);
                    }

                    completionSource.SetResult(string.Empty);
                });

            this.Completion = completionSource.Task;
        }

        private IEnumerable<FlowToken> PrepareResult(IEnumerable<FlowToken> result)
        {
            if (!this.Options.DoResultCheck)
            {
                return result;
            }

            var isLastBlock = !this.OutgoingLinks.Any();
            var materialized = (result ?? new List<FlowToken>()).ToList();

            // When last block - return empty result.
            // The block finishes only when output queue is empty.
            return materialized.Any() && isLastBlock ? Enumerable.Empty<FlowToken>() : materialized;
        }

        private IEnumerable<FlowToken> TransformInternal(FlowToken token)
        {
            Stopwatch perfCount = new Stopwatch();
            try
            {
                perfCount.Start();

                if (token.IsProcessingFinished)
                {
                    if (this.OnFinishedTokenReceived != null)
                    {
                        this.OnFinishedTokenReceived(this, new FinishedTokenReceivedEventArgs(token));
                    }

                    // just pass token to the next block - ignore processing of current block
                    // we want to pass for instance to the end block that will process results
                    return PrepareResult(new[] { token });
                }

                try
                {
                    return this.PrepareResult(this.Transform(token));
                }
                catch (Exception ex)
                {
                    token.IsProcessingFinished = true;
                    this.BlockLogger.Error(token, ex);

                    this._flowExceptionManager.IncremenetExceptionCount();

                    if (this._flowExceptionManager.IsCritical(ex) || this._flowExceptionManager.IsExceptionCountExceeded())
                    {
                        this._cancellationTokenSource.Cancel();
                        throw;
                    }

                    return PrepareResult(new[] { token });
                }
            }
            finally
            {
                perfCount.Stop();
                PerformanceMonitor.RegisterExecution(this.GetType().Name, perfCount.ElapsedTicks);
            }
        }

        public abstract IEnumerable<FlowToken> Transform(FlowToken token);

        public void Set(FlowExceptionManager flowExceptionManager, CancellationTokenSource cancellationTokenSource)
        {
            if (flowExceptionManager == null)
            {
                throw new ArgumentNullException("flowExceptionManager");
            }
            if (cancellationTokenSource == null)
            {
                throw new ArgumentNullException("cancellationTokenSource");
            }

            if (this._transformation != null)
            {
                // cancellation token would be ignored
                throw new InvalidOperationException("Can not call Set method after block has been initialized.");
            }

            this._flowExceptionManager = flowExceptionManager;
            this._cancellationTokenSource = cancellationTokenSource;
        }

        private ITargetBlock<FlowToken> _entryPoint;
        private ITargetBlock<FlowToken> EntryPoint
        {
            get
            {
                this.Init();
                return this._entryPoint;
            }
            set
            {
                this._entryPoint = value;
            }
        }

        private ISourceBlock<FlowToken> _output;
        private ISourceBlock<FlowToken> Output
        {
            get { this.Init(); return this._output; }
            set { this._output = value; }
        }

        public bool Post(object token)
        {
            return this.EntryPoint.Post(token as FlowToken);
        }

        public object Receive()
        {
            return this.Output.Receive();
        }

        public void Complete()
        {
            this.EntryPoint.Complete();
        }

        public void Fault(Exception exception)
        {
            this.EntryPoint.Fault(exception);
        }

        public void Link(ProcessingBlockBase target, bool propagare, Predicate<FlowToken> prediate = null)
        {
            var options = new DataflowLinkOptions
                              {
                                  PropagateCompletion = propagare
                              };

            if (prediate != null)
            {
                this.Output.LinkTo(target.EntryPoint, options, prediate);
                return;
            }

            this.Output.LinkTo(target.EntryPoint, options);
        }

        public Task Completion { get; private set; }

        public void OnReceived(ITargetBlock<object> onReceiveBlock)
        {
            var consumer = onReceiveBlock;
            this.Output.LinkTo(consumer, new DataflowLinkOptions { PropagateCompletion = true });
        }

        public bool IsCompleted
        {
            get { return this.Completion.IsCompleted; }
        }

        #region Flow linking functions
        public void Accept(IFlowVisitor visitor)
        {
            visitor.Visit(this);
        }

        public string ReadableId { get; set; }
        private readonly List<ILink> _links = new List<ILink>();
        public IEnumerable<ILink> IncommingLinks { get { return this._links.Where(o => o.Target == this); } }
        public IEnumerable<ILink> OutgoingLinks { get { return this._links.Where(o => o.Source == this); } }

        public void AddLink(ILink link)
        {
            if (link == null)
            {
                throw new ArgumentNullException();
            }
            if (link.Source != this && link.Target != this)
            {
                throw new ArgumentException("Must be either incomming or outgoing link");
            }

            this._links.Add(link);
        }
        #endregion
    }
}