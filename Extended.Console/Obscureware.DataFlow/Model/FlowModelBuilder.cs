namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class FlowModelBuilder : IFluentFlowBuilder
    {
        private readonly IProcessingBlockFactory _blockFactory;

        private readonly Dictionary<string, ProcessingBlockBase> _blocks =
            new Dictionary<string, ProcessingBlockBase>(StringComparer.InvariantCultureIgnoreCase);

        // used in order to traverse blocks in order of first appearance in flow
        private readonly List<ProcessingBlockBase> _orderedBlocks = new List<ProcessingBlockBase>();

        private readonly Stack<BlockLink> _recentLinks = new Stack<BlockLink>();
        private string[] _sourceIds;

        // note XL - might add additional connection validation
        public FlowModelBuilder(IProcessingBlockFactory blockFactory)
        {
            this._blockFactory = blockFactory;
        }

        public virtual IFluentFlow BuildFlow()
        {
            return new ProcessingFlow(this._orderedBlocks, this._blockFactory.CancellationTokenSource);
        }

        public virtual IFluentFlowBuilder Link(string sourceId)
        {
            this.ValidateInExisting(sourceId);
            this._recentLinks.Clear();
            this._sourceIds = new[] { sourceId };
            return this;
        }

        public virtual IFluentFlowBuilder Link<TBlockSource>()
            where TBlockSource : ProcessingBlockBase
        {
            this._recentLinks.Clear();
            this._sourceIds = new[] { this.CreateDefaultBlockWithId<TBlockSource>() };
            return this;
        }

        public IFluentFlowBuilder Link<TBlockSource, TBlockSource2>()
            where TBlockSource : ProcessingBlockBase where TBlockSource2 : ProcessingBlockBase
        {
            this._recentLinks.Clear();
            this._sourceIds = new[]
                             {
                                 this.CreateDefaultBlockWithId<TBlockSource>(),
                                 this.CreateDefaultBlockWithId<TBlockSource2>()
                             };

            return this;
        }

        public IFluentFlowBuilder Link<TBlockSource, TBlockSource2, TBlockSource3>()
            where TBlockSource : ProcessingBlockBase where TBlockSource2 : ProcessingBlockBase where TBlockSource3 : ProcessingBlockBase
        {
            this._recentLinks.Clear();
            this._sourceIds = new[] { this.CreateDefaultBlockWithId<TBlockSource>(), this.CreateDefaultBlockWithId<TBlockSource2>(), this.CreateDefaultBlockWithId<TBlockSource3>() };

            return this;
        }

        private string CreateDefaultBlockWithId<TBlockSource>()
            where TBlockSource : ProcessingBlockBase
        {
            var id = GetDefaultIdFromType<TBlockSource>();
            if (!this._blocks.ContainsKey(id))
            {
                var block = this._blockFactory.Create<TBlockSource>();
                if (block == null)
                {
                    throw new InvalidOperationException(string.Format("Failed to create block of type {0}", typeof(TBlockSource).Name));
                }

                block.ReadableId = id;
                this._blocks.Add(id, block);
                this._orderedBlocks.Add(block);
            }

            return id;
        }

        public IFluentFlowBuilder To(string targetId)
        {
            this.ValidateInExisting(targetId);
            if (this._sourceIds == null || !this._sourceIds.Any())
            {
                throw new InvalidOperationException("Source block not set. Use Link() method before To()");
            }

            var targetBlock = this.GetBlock(targetId);

            foreach (var source in this._sourceIds)
            {
                var sourceBlock = this.GetBlock(source);
                if (sourceBlock == null)
                {
                    throw new InvalidOperationException(string.Format("target block with id {0} not found", source));
                }

                this._recentLinks.Push(BlockLink.Link(sourceBlock, targetBlock));
            }

            return this;
        }

        public IFluentFlowBuilder To<TBlockTarget>() where TBlockTarget : ProcessingBlockBase
        {

            if (this._sourceIds == null || !this._sourceIds.Any())
            {
                throw new InvalidOperationException("Source block not set. Use Link() method before To()");
            }

            var targetBlock = this.GetBlock(this.CreateDefaultBlockWithId<TBlockTarget>());
            if (targetBlock == null)
            {
                throw new InvalidOperationException(string.Format("target block with of type {0} not found", typeof(TBlockTarget).Name));
            }

            foreach (var source in this._sourceIds)
            {
                var sourceBlock = this.GetBlock(source);
                if (sourceBlock == null)
                {
                    throw new InvalidOperationException(string.Format("target block with id {0} not found", source));
                }

                this._recentLinks.Push(BlockLink.Link(sourceBlock, targetBlock));
            }

            return this;
        }

        public IFluentFlowBuilder To<TBlockTarget, TBlockTarget2>()
            where TBlockTarget : ProcessingBlockBase
            where TBlockTarget2 : ProcessingBlockBase
        {
            this.To<TBlockTarget>();
            this.To<TBlockTarget2>();

            return this;
        }

        public virtual IFluentFlowBuilder When<TTuple>(Expression<Predicate<TTuple>> condition)
        {
            if (!this._recentLinks.Any())
            {
                throw new InvalidOperationException("No links exist to connect condition to");
            }

            while (this._recentLinks.Any())
            {
                var link = this._recentLinks.Pop();
                if (link.Condition != null)
                {
                    throw new InvalidOperationException("Condition already set on link. Would be overriden");
                }

                link.Condition = new PredicateCondition<TTuple>(condition);
            }

            return this;
        }

        private void ValidateInExisting(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            if (!this._blocks.ContainsKey(id))
            {
                throw new InvalidOperationException(string.Format("Block with custom id {0} not created using OverrideCreate", id));
            }
        }

        private ProcessingBlockBase GetBlock(string id)
        {
            if (!this._blocks.ContainsKey(id))
            {
                throw new ArgumentException(string.Format("Could not find block with id {0}", id));
            }

            return this._blocks[id];
        }

        public virtual IFluentFlowBuilder OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null)
            where TBlock : ProcessingBlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._blocks.ContainsKey(id))
            {
                throw new ApplicationException(string.Format("Block with id {0} already exists in this flow", id));
            }

            this._blockFactory.OverrideCreate<TBlock>(options, id);
            var block = this._blockFactory.Create<TBlock>(id);

            this._blocks.Add(id, block);
            this._orderedBlocks.Add(block);

            return this;
        }


        public virtual IFluentFlowBuilder OverrideCreate<TBlock>(Func<TBlock> factoryFunction, string id = null)
            where TBlock : ProcessingBlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._blocks.ContainsKey(id))
            {
                throw new ApplicationException(string.Format("Block with id {0} already exists in this flow", id));
            }

            this._blockFactory.OverrideCreate(factoryFunction, id);

            var block = this._blockFactory.Create<TBlock>(id);

            block.ReadableId = id;
            this._blocks.Add(id, block);
            this._orderedBlocks.Add(block);

            return this;
        }

        private static string GetDefaultIdFromType<TBlock>() where TBlock : ProcessingBlockBase
        {
            return typeof(TBlock).Name;
        }
    }
}