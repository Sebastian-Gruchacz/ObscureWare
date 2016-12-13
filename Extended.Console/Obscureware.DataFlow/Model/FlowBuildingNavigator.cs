namespace Obscureware.DataFlow.Model
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Console.Commands.Blocks;
    using DataFlow;
    using Implementation;

    /// <summary>
    /// Navigator class that build real execution flow from the model
    /// </summary>
    internal class FlowBuildingNavigator : IFlowNavigator
    {
        private readonly CancellationTokenSource _cancellationTokenSource;

        private FlowBuildingNavigator(CancellationTokenSource cancellationTokenSource)
        {
            this._cancellationTokenSource = cancellationTokenSource;
        }

        public static FlowBuildingNavigator Create(CancellationTokenSource tokenSource)
        {
            return new FlowBuildingNavigator(tokenSource);
        }

        public void Visit(IFlow flow)
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

        public void Visit(BlockBase block)
        {
            if (block.IncommingLinks.Count() <= 1)
            {
                return;
            }

            Task.WhenAll(block.IncommingLinks.Select(o => o.Source.Completion))
                .ContinueWith(o =>
                    {
                        // if any faults - cancel all data
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

            Predicate<DataFlowToken> conditionFunc = o => o.HasTerminated || link.Condition.Condition(o);

            link.Source.Link(link.Target, shouldPropagate, conditionFunc);
        }

        private bool ShouldPropagateCompletionByDefault(ILink link)
        {
            // Propagate completion only when linking to block that has only single parent.
            // When it has more - it shall wait for both / all of them to finish

            return link.Target.IncommingLinks.Count() <= 1;
        }

        public void Visit(ICondition condition)
        {
        }
    }
}