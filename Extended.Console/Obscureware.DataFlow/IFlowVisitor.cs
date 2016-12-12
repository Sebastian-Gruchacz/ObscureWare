namespace Obscureware.Console.Commands.Blocks
{
    public interface IFlowVisitor
    {
        void Visit(IFluentFlow flow);
        void Visit(ProcessingBlockBase item);
        void Visit(ILink link);
        void Visit(ICondition condition);
    }
}