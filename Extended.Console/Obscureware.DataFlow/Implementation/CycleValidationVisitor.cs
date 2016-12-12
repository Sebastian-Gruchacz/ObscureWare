namespace Obscureware.Console.Commands.Blocks
{
    using System.Collections.Generic;
    using System.Linq;

    public class CycleValidationVisitor : IFlowVisitor
    {
        private readonly Stack<string> _visitedBlocks = new Stack<string>();

        public void Visit(IFluentFlow flow)
        {
            this.Visit(flow.StartBlock);
        }

        public void Visit(ProcessingBlockBase item)
        {
            if (this._visitedBlocks.Contains(item.ReadableId))
            {
                var blocks = this._visitedBlocks.Reverse().ToList();
                var cycleStartIndex = blocks.IndexOf(item.ReadableId);

                throw new InvalidFlowException(string.Format("Cycle detected on blocks {0}",
                    string.Join(", ", blocks.Skip(cycleStartIndex))));
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