using Obscureware.DataFlow;

namespace Obscureware.Console.Commands.Blocks
{
    using DataFlow.Model;

    public interface IFlowNavigator
    {
        void Visit(IFlow flow);
        void Visit(BlockBase item);
        void Visit(ILink link);
        void Visit(ICondition condition);
    }
}