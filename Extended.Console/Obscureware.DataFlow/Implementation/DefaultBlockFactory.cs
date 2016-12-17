namespace Obscureware.DataFlow.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Model;

    public class DefaultBlockFactory : IBlockFactory
    {
        private readonly Dictionary<string, Func<BlockBase>> _overridenFactory
            = new Dictionary<string, Func<BlockBase>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, ProcessingBlockOptions> _overridenOptions
            = new Dictionary<string, ProcessingBlockOptions>(StringComparer.InvariantCultureIgnoreCase);

        public ProcessingBlockOptions DefaultOptions { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        public FlowExceptionManager Manager { get; set; } // TODO: proper interface

        public DefaultBlockFactory()
        {
            this.DefaultOptions = ProcessingBlockOptions.New();
            this.CancellationTokenSource = new CancellationTokenSource();
            this.Manager = new FlowExceptionManager();
        }

        public BlockBase Create(Type blockType, string id = null)
        {
            // if id is not null we require override
            if (!string.IsNullOrWhiteSpace(id) && !this._overridenFactory.ContainsKey(id) && !this._overridenOptions.ContainsKey(id))
            {
                throw new InvalidOperationException($"Block with id {id} not created with OverrideCreate method");
            }

            id = id ?? this.GetDefaultIdFromType(blockType);

            BlockBase block;
            // if default id in overrides - return override
            if (this._overridenFactory.ContainsKey(id))
            {
                block = (BlockBase)this._overridenFactory[id]();
                if (block == null)
                {
                    throw new InvalidFlowException($"Overwrite with id {id} returned null instead of block");
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
            block.ReadableId = id ?? this.GetDefaultIdFromType(blockType); // TODO: fix logic in this method

            return block;
        }

        private BlockBase CreateNewInstance(Type blockType)
        {
            var ctor = blockType.GetConstructor(new Type[0]);

            if (ctor == null)
            {
                throw new InvalidFlowException($"Block {this.GetDefaultIdFromType(blockType)} not exposing recognized constructor");
            }

            return ctor.Invoke(null) as BlockBase;
        }

        private string GetDefaultIdFromType(Type blockType)
        {
            return blockType.FullName;
        }

        public TBlock Create<TBlock>(string id = null)
            where TBlock : BlockBase
        {
            // if id is not null we require override
            if (!string.IsNullOrWhiteSpace(id) && !this._overridenFactory.ContainsKey(id) && !this._overridenOptions.ContainsKey(id))
            {
                throw new InvalidOperationException($"Block with id {id} not created with OverrideCreate method");
            }

            id = id ?? GetDefaultIdFromType<TBlock>();

            TBlock block;
            // if default id in overrides - return override
            if (this._overridenFactory.ContainsKey(id))
            {
                block = (TBlock)this._overridenFactory[id]();
                if (block == null)
                {
                    throw new InvalidFlowException($"Overwrite with id {id} returned null instead of block");
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
            where TBlock : BlockBase
        {
            var ctor = typeof(TBlock).GetConstructor(new Type[0]);

            if (ctor == null)
            {
                throw new InvalidFlowException($"Block {GetDefaultIdFromType<TBlock>()} not exposing recognized constructor");
            }

            return ctor.Invoke(null) as TBlock;
        }

        public void OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null)
            where TBlock : BlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._overridenOptions.ContainsKey(id) || this._overridenFactory.ContainsKey(id))
            {
                throw new ApplicationException($"Block with id {id} already overwritten");
            }

            this._overridenOptions.Add(id, this.PrepareOptions(options));
        }

        public void OverrideCreate<TBlock>(Func<TBlock> factoryFunc, string id = null)
            where TBlock : BlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._overridenOptions.ContainsKey(id) || this._overridenFactory.ContainsKey(id))
            {
                throw new ApplicationException($"Block with id {id} already overwritten");
            }

            this._overridenFactory.Add(id, factoryFunc);
        }

        private static string GetDefaultIdFromType<TBlock>() where TBlock : BlockBase
        {
            return typeof(TBlock).FullName;
        }

        private ProcessingBlockOptions PrepareOptions(ProcessingBlockOptions options)
        {
            return options ?? this.DefaultOptions;
        }
    }
}