namespace Obscureware.DataFlow.Model
{
    using System;

    public class BlockLink : ILink
    {
        private BlockLink(BlockBase source, BlockBase target, ICondition condition)
        {
            this.Source = source;
            this.Target = target;
            this.Condition = condition;
        }

        public static BlockLink Link(BlockBase source, BlockBase target, ICondition condition = null)
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

        public void Accept(IFlowNavigator navigator)
        {
            navigator.Visit(this);
        }

        public BlockBase Source { get; private set; }
        public BlockBase Target { get; private set; }
        public ICondition Condition { get; set; }
    }
}