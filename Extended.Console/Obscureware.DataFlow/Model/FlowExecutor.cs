namespace Obscureware.DataFlow.Model
{
    using System;
    using System.Threading.Tasks;
    using DataFlow;
    using Implementation;

    /// <summary>
    /// Class responsible for execution of previously build Flow
    /// </summary>
    public class FlowExecutor
    {
        private readonly Action<IFlowBuilder> _flowFactory;

        private readonly IBlockFactory _blockFactory;

        private IFlow _flow;

        public FlowExecutor(Action<IFlowBuilder> flowFactory, IBlockFactory blockFactory = null)
        {
            this._flowFactory = flowFactory;
            this._blockFactory = blockFactory ?? new DefaultBlockFactory();
        }

        public async Task PostToFlowAsync(DataFlowToken token)
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