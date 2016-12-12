using Obscureware.DataFlow;

namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using DataFlow.Implementation;
    using DataFlow.Model;

    internal class ProcessingFlow : IFlow
    {
        protected CancellationTokenSource CancellationTokenSource { get; private set; }
        protected FlowModelBuilder Builder { get; set; }
        protected IFlowNavigator DefaultNavigator { get; set; }
        protected IFlowNavigator ValidationNavigator { get; set; }
        private bool _isModelInited;
        private bool _isStackBlockCompleted;
        public IList<BlockBase> ProcesingBlocks { get; private set; }

        public virtual BlockBase StartBlock
        {
            get
            {
                var entryBlocks = this.ProcesingBlocks.Where(o => !o.IncommingLinks.Any()).ToList();
                if (!this.ProcesingBlocks.Any())
                {
                    throw new InvalidOperationException("Empty flow. No block was created with the block");
                }
                if (!entryBlocks.Any())
                {
                    throw new InvalidOperationException("No single entry block. One block should not have incomming connection.");
                }
                if (entryBlocks.Count() > 1)
                {
                    throw new InvalidOperationException(string.Format("Multiple entry blocks {0}. Should be only one.",
                        string.Join(",", entryBlocks.Select(o => o.ReadableId))));
                }

                return entryBlocks.Single();
            }
        }

        public ProcessingFlow(IList<BlockBase> blocks, CancellationTokenSource tokenSource)
        {
            this.ProcesingBlocks = blocks;
            this.CancellationTokenSource = tokenSource;
            this.DefaultNavigator = FlowBuildingNavigator.Create(tokenSource);
            this.ValidationNavigator = new CycleValidationNavigator();
        }

        private void Init()
        {
            if (!this._isModelInited)
            {
                this.Accept(this.ValidationNavigator);
                this.Accept(this.DefaultNavigator);
                this._isModelInited = true;
            }
        }

        public void Post(DataFlowToken token)
        {
            this.Init();

            var wasQueued = this.StartBlock.Post(token);
            if (!wasQueued)
            {
                throw new InvalidFlowException("Message not accepted by the flow. Probably token send after flow is completed.");
            }
        }

        public async Task GetCompletionTask()
        {

            this.Init();

            if (!this._isStackBlockCompleted)
            {
                this.StartBlock.Complete();
                this._isStackBlockCompleted = true;
            }

            try
            {

                await Task.WhenAll(this.ProcesingBlocks.Select(o => o.Completion).ToArray());
            }
            catch (Exception)
            {
                this.CancellationTokenSource.Cancel();

                throw;
            }

        }

        public void Accept(IFlowNavigator navigator)
        {
            navigator.Visit(this);
        }
    }
}