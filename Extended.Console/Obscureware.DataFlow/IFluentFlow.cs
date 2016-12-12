namespace Obscureware.Console.Commands.Blocks
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFluentFlow : IVisitableElement
    {
        IList<ProcessingBlockBase> ProcesingBlocks { get; }
        ProcessingBlockBase StartBlock { get; }
        void Post(FlowToken token);

        Task GetCompletionTask();
    }
}