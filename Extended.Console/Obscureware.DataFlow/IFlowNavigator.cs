namespace Obscureware.DataFlow
{
    using Model;

    public interface IFlowNavigator
    {
        void Visit(IFlow flow);
        void Visit(BlockBase item);
        void Visit(ILink link);
        void Visit(ICondition condition);
    }
}