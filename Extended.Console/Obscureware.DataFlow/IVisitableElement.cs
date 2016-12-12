namespace Obscureware.Console.Commands.Blocks
{
    public interface IVisitableElement
    {
        void Accept(IFlowVisitor visitor);
    }
}