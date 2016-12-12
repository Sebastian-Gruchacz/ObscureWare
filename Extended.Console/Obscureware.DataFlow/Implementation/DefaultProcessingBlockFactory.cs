namespace Obscureware.DataFlow.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Obscureware.Console.Commands.Blocks;

    public class DefaultProcessingBlockFactory : IProcessingBlockFactory
    {
        private readonly Dictionary<string, Func<ProcessingBlockBase>> _overridenFactory
            = new Dictionary<string, Func<ProcessingBlockBase>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, ProcessingBlockOptions> _overridenOptions
            = new Dictionary<string, ProcessingBlockOptions>(StringComparer.InvariantCultureIgnoreCase);

        public ProcessingBlockOptions DefaultOptions { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public FlowExceptionManager Manager { get; set; }

        public DefaultProcessingBlockFactory()
        {
            this.DefaultOptions = ProcessingBlockOptions.New();
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Manager = new FlowExceptionManager();
        }

        public ProcessingBlockBase Create(Type blockType, string id = null)
        {
            // if id is not null we require override
            if (!string.IsNullOrWhiteSpace(id) && !this._overridenFactory.ContainsKey(id) && !this._overridenOptions.ContainsKey(id))
            {
                throw new InvalidOperationException(string.Format("Block with id {0} not created with OverrideCreate method", id));
            }

            id = id ?? this.GetDefaultIdFromType(blockType);

            ProcessingBlockBase block;
            // if default id in overrides - return override
            if (this._overridenFactory.ContainsKey(id))
            {
                block = (ProcessingBlockBase)this._overridenFactory[id]();
                if (block == null)
                {
                    throw new InvalidFlowException(string.Format("Overwrite with id {0} returned null instead of block", id));
                }

                if (block.Options == null)
                {
                    block.Options = this.DefaultOptions;
                }
            }
            else
            {
                var currentOptions = this._overridenOptions.ContainsKey(id) ? this._overridenOptions[id] : this.DefaultOptions;

                block = this.CreateNewInstance(blockType);
                block.Options = currentOptions;
            }

            block.Set(this.Manager, this.CancellationTokenSource);
            block.ReadableId = id ?? this.GetDefaultIdFromType(blockType);

            return block;
        }

        private ProcessingBlockBase CreateNewInstance(Type blockType)
        {
            var ctor = blockType.GetConstructor(new Type[0]);

            if (ctor == null)
            {
                throw new InvalidFlowException(string.Format("Block {0} not exposing recognized constructor", this.GetDefaultIdFromType(blockType)));
            }

            return ctor.Invoke(null) as ProcessingBlockBase;
        }

        private string GetDefaultIdFromType(Type blockType)
        {
            return blockType.FullName;
        }

        public TBlock Create<TBlock>(string id = null)
            where TBlock : ProcessingBlockBase
        {
            // if id is not null we require override
            if (!string.IsNullOrWhiteSpace(id) && !this._overridenFactory.ContainsKey(id) && !this._overridenOptions.ContainsKey(id))
            {
                throw new InvalidOperationException(string.Format("Block with id {0} not created with OverrideCreate method", id));
            }

            id = id ?? GetDefaultIdFromType<TBlock>();

            TBlock block;
            // if default id in overrides - return override
            if (this._overridenFactory.ContainsKey(id))
            {
                block = (TBlock)this._overridenFactory[id]();
                if (block == null)
                {
                    throw new InvalidFlowException(string.Format("Overwrite with id {0} returned null instead of block", id));
                }

                if (block.Options == null)
                {
                    block.Options = this.DefaultOptions;
                }
            }
            else
            {
                var currentOptions = this._overridenOptions.ContainsKey(id) ? this._overridenOptions[id] : this.DefaultOptions;

                block = this.CreateNewInstance<TBlock>();
                block.Options = currentOptions;
            }

            block.Set(this.Manager, this.CancellationTokenSource);
            block.ReadableId = id ?? GetDefaultIdFromType<TBlock>();

            return block;
        }

        protected virtual TBlock CreateNewInstance<TBlock>()
            where TBlock : ProcessingBlockBase
        {
            var ctor = typeof(TBlock).GetConstructor(new Type[0]);

            if (ctor == null)
            {
                throw new InvalidFlowException(string.Format("Block {0} not exposing recognized constructor", GetDefaultIdFromType<TBlock>()));
            }

            return ctor.Invoke(null) as TBlock;
        }

        public void OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null)
            where TBlock : ProcessingBlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._overridenOptions.ContainsKey(id) || this._overridenFactory.ContainsKey(id))
            {
                throw new ApplicationException(string.Format("Block with id {0} already overwritten", id));
            }

            this._overridenOptions.Add(id, this.PrepareOptions(options));
        }

        public void OverrideCreate<TBlock>(Func<TBlock> factoryFunc, string id = null)
            where TBlock : ProcessingBlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._overridenOptions.ContainsKey(id) || this._overridenFactory.ContainsKey(id))
            {
                throw new ApplicationException(string.Format("Block with id {0} already overwritten", id));
            }

            this._overridenFactory.Add(id, factoryFunc);
        }

        private static string GetDefaultIdFromType<TBlock>() where TBlock : ProcessingBlockBase
        {
            return typeof(TBlock).FullName;
        }

        private ProcessingBlockOptions PrepareOptions(ProcessingBlockOptions options)
        {
            return options ?? this.DefaultOptions;
        }
    }
}