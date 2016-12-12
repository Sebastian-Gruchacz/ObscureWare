namespace Obscureware.Console.Commands.Blocks
{
    using System;

    public class BlockLink : ILink
    {
        private BlockLink(ProcessingBlockBase source, ProcessingBlockBase target, ICondition condition)
        {
            this.Source = source;
            this.Target = target;
            this.Condition = condition;
        }

        public static BlockLink Link(ProcessingBlockBase source, ProcessingBlockBase target, ICondition condition = null)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }
            if (source == target)
            {
                throw new ArgumentException("Cant link block to itself");
            }

            var link = new BlockLink(source, target, condition);

            source.AddLink(link);
            target.AddLink(link);

            return link;
        }

        public void Accept(IFlowVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ProcessingBlockBase Source { get; private set; }
        public ProcessingBlockBase Target { get; private set; }
        public ICondition Condition { get; set; }
    }
}