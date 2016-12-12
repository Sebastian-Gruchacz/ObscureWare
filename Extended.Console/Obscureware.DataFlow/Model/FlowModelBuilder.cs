namespace Obscureware.DataFlow.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Console.Commands.Blocks;
    using DataFlow;

    public class FlowModelBuilder : IFlowBuilder
    {
        private readonly IBlockFactory _blockFactory;

        private readonly Dictionary<string, BlockBase> _blocks =
            new Dictionary<string, BlockBase>(StringComparer.InvariantCultureIgnoreCase);

        // used in order to traverse blocks in order of first appearance in flow
        private readonly List<BlockBase> _orderedBlocks = new List<BlockBase>();

        private readonly Stack<BlockLink> _recentLinks = new Stack<BlockLink>();
        private string[] _sourceIds;

        // note XL - might add additional connection validation
        public FlowModelBuilder(IBlockFactory blockFactory)
        {
            this._blockFactory = blockFactory;
        }

        public virtual IFlow BuildFlow()
        {
            return new ProcessingFlow(this._orderedBlocks, this._blockFactory.CancellationTokenSource);
        }

        public virtual IFlowBuilder Link(string sourceId)
        {
            this.ValidateInExisting(sourceId);
            this._recentLinks.Clear();
            this._sourceIds = new[] { sourceId };
            return this;
        }

        public virtual IFlowBuilder Link<TBlockSource>()
            where TBlockSource : BlockBase
        {
            this._recentLinks.Clear();
            this._sourceIds = new[] { this.CreateDefaultBlockWithId<TBlockSource>() };
            return this;
        }

        public IFlowBuilder Link<TBlockSource, TBlockSource2>()
            where TBlockSource : BlockBase where TBlockSource2 : BlockBase
        {
            this._recentLinks.Clear();
            this._sourceIds = new[]
                             {
                                 this.CreateDefaultBlockWithId<TBlockSource>(),
                                 this.CreateDefaultBlockWithId<TBlockSource2>()
                             };

            return this;
        }

        public IFlowBuilder Link<TBlockSource, TBlockSource2, TBlockSource3>()
            where TBlockSource : BlockBase where TBlockSource2 : BlockBase where TBlockSource3 : BlockBase
        {
            this._recentLinks.Clear();
            this._sourceIds = new[] { this.CreateDefaultBlockWithId<TBlockSource>(), this.CreateDefaultBlockWithId<TBlockSource2>(), this.CreateDefaultBlockWithId<TBlockSource3>() };

            return this;
        }

        private string CreateDefaultBlockWithId<TBlockSource>()
            where TBlockSource : BlockBase
        {
            var id = GetDefaultIdFromType<TBlockSource>();
            if (!this._blocks.ContainsKey(id))
            {
                var block = this._blockFactory.Create<TBlockSource>();
                if (block == null)
                {
                    throw new InvalidOperationException($"Failed to create block of type {typeof(TBlockSource).Name}");
                }

                block.ReadableId = id;
                this._blocks.Add(id, block);
                this._orderedBlocks.Add(block);
            }

            return id;
        }

        public IFlowBuilder To(string targetId)
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
                    throw new InvalidOperationException($"target block with id {source} not found");
                }

                this._recentLinks.Push(BlockLink.Link(sourceBlock, targetBlock));
            }

            return this;
        }

        public IFlowBuilder To<TBlockTarget>() where TBlockTarget : BlockBase
        {

            if (this._sourceIds == null || !this._sourceIds.Any())
            {
                throw new InvalidOperationException("Source block not set. Use Link() method before To()");
            }

            var targetBlock = this.GetBlock(this.CreateDefaultBlockWithId<TBlockTarget>());
            if (targetBlock == null)
            {
                throw new InvalidOperationException($"Target block with of type {typeof(TBlockTarget).Name} not found");
            }

            foreach (var source in this._sourceIds)
            {
                var sourceBlock = this.GetBlock(source);
                if (sourceBlock == null)
                {
                    throw new InvalidOperationException($"target block with id {source} not found");
                }

                this._recentLinks.Push(BlockLink.Link(sourceBlock, targetBlock));
            }

            return this;
        }

        public IFlowBuilder To<TBlockTarget, TBlockTarget2>()
            where TBlockTarget : BlockBase
            where TBlockTarget2 : BlockBase
        {
            this.To<TBlockTarget>();
            this.To<TBlockTarget2>();

            return this;
        }

        public virtual IFlowBuilder When<TTuple>(Expression<Predicate<TTuple>> condition)
        {
            if (!this._recentLinks.Any())
            {
                throw new InvalidOperationException("No links exist to connect condition to.");
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
                throw new ArgumentNullException(nameof(id));
            }

            if (!this._blocks.ContainsKey(id))
            {
                throw new InvalidOperationException($"Block with custom id {id} not created using OverrideCreate");
            }
        }

        private BlockBase GetBlock(string id)
        {
            if (!this._blocks.ContainsKey(id))
            {
                throw new ArgumentException($"Could not find block with id {id}", nameof(id));
            }

            return this._blocks[id];
        }

        public virtual IFlowBuilder OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null)
            where TBlock : BlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._blocks.ContainsKey(id))
            {
                throw new ApplicationException($"Block with id {id} already exists in this flow");
            }

            this._blockFactory.OverrideCreate<TBlock>(options, id);
            var block = this._blockFactory.Create<TBlock>(id);

            this._blocks.Add(id, block);
            this._orderedBlocks.Add(block);

            return this;
        }


        public virtual IFlowBuilder OverrideCreate<TBlock>(Func<TBlock> factoryFunction, string id = null)
            where TBlock : BlockBase
        {
            id = id ?? GetDefaultIdFromType<TBlock>();

            if (this._blocks.ContainsKey(id))
            {
                throw new ApplicationException($"Block with id {id} already exists in this flow");
            }

            this._blockFactory.OverrideCreate(factoryFunction, id);

            var block = this._blockFactory.Create<TBlock>(id);

            block.ReadableId = id;
            this._blocks.Add(id, block);
            this._orderedBlocks.Add(block);

            return this;
        }

        private static string GetDefaultIdFromType<TBlock>() where TBlock : BlockBase
        {
            return typeof(TBlock).Name;
        }
    }
}