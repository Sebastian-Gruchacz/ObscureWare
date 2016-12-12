namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Threading.Tasks;

    using Obscureware.DataFlow.Implementation;

    public class FlowExecutor
    {
        private readonly Action<IFluentFlowBuilder> _flowFactory;

        private readonly IProcessingBlockFactory _blockFactory;

        private IFluentFlow _flow;

        public FlowExecutor(Action<IFluentFlowBuilder> flowFactory, IProcessingBlockFactory blockFactory = null)
        {
            this._flowFactory = flowFactory;
            this._blockFactory = blockFactory ?? new DefaultProcessingBlockFactory();
        }

        public async Task PostToFlowAsync(FlowToken token)
        {
            if (this._flow == null)
            {
                var builder = new FlowModelBuilder(this._blockFactory);
                this._flowFactory(builder);
                this._flow = builder.BuildFlow();
            }

            this._flow.Post(token);

            await this._flow.GetCompletionTask();
        }
    }
}