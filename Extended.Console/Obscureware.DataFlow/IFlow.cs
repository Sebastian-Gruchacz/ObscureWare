namespace Obscureware.DataFlow
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Implementation;
    using Model;

    public interface IFlow : INavigableElement
    {
        IList<BlockBase> ProcesingBlocks { get; }
        BlockBase StartBlock { get; }
        void Post(DataFlowToken token);

        Task GetCompletionTask();
    }
}