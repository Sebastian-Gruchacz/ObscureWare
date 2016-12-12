namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal class TplFlowBuildingVisitor : IFlowVisitor
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        private TplFlowBuildingVisitor(CancellationTokenSource cancellationTokenSource)
        {
            this._cancellationTokenSource = cancellationTokenSource;
        }

        public static TplFlowBuildingVisitor Create(CancellationTokenSource tokenSource)
        {
            return new TplFlowBuildingVisitor(tokenSource);
        }

        public void Visit(IFluentFlow flow)
        {
            foreach (var block in flow.ProcesingBlocks)
            {
                this.Visit(block);
                foreach (var link in block.IncommingLinks)
                {
                    this.Visit(link);
                }
            }
        }

        public void Visit(ProcessingBlockBase block)
        {
            if (block.IncommingLinks.Count() <= 1)
            {
                return;
            }

            Task.WhenAll(block.IncommingLinks.Select(o => o.Source.Completion))
                .ContinueWith(o =>
                    {
                        // if any faults cancell all data
                        if (o.IsFaulted)
                        {
                            block.Fault(o.Exception);
                            this._cancellationTokenSource.Cancel();
                        }
                        else block.Complete();
                    });

        }

        public void Visit(ILink link)
        {
            var shouldPropagate = this.ShouldPropagateCompletionByDefault(link);

            // connect TPL blocks
            if (link.Condition == null)
            {
                link.Source.Link(link.Target, shouldPropagate);
                return;
            }

            Predicate<FlowToken> conditionFunc = o => o.IsProcessingFinished || link.Condition.Condition(o);

            link.Source.Link(link.Target, shouldPropagate, conditionFunc);
        }

        private bool ShouldPropagateCompletionByDefault(ILink link)
        {
            // propagate completion only when linking to block 
            // that has only single parent
            // when has more - will have to wait for both to finish

            return link.Target.IncommingLinks.Count() <= 1;
        }

        public void Visit(ICondition condition)
        {
        }
    }
}