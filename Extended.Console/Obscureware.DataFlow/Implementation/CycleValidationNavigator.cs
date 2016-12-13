namespace Obscureware.DataFlow.Implementation
{
    using System.Collections.Generic;
    using System.Linq;
    using Console.Commands.Blocks;
    using DataFlow;
    using Model;

    /// <summary>
    /// Detects erroneous design of the flow that forms cycle(s).
    /// </summary>
    public class CycleValidationNavigator : IFlowNavigator
    {
        private readonly Stack<string> _visitedBlocks = new Stack<string>();

        public void Visit(IFlow flow)
        {
            this.Visit(flow.StartBlock);
        }

        public void Visit(BlockBase item)
        {
            // TODO: this method is generally quite inefficient, but safe and fast enough for typical flow of several nodes

            if (this._visitedBlocks.Contains(item.ReadableId))
            {
                var blocks = this._visitedBlocks.Reverse().ToList();
                var cycleStartIndex = blocks.IndexOf(item.ReadableId);

                throw new InvalidFlowException($"Cycle detected on blocks {string.Join(", ", blocks.Skip(cycleStartIndex))}");
            }

            this._visitedBlocks.Push(item.ReadableId);

            foreach (var link in item.OutgoingLinks)
            {
                this.Visit(link);
            }

            this._visitedBlocks.Pop();
        }

        public void Visit(ILink link)
        {
            this.Visit(link.Target);
        }

        public void Visit(ICondition condition)
        {
        }
    }
}