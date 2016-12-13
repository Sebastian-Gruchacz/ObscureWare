namespace Obscureware.DataFlow
{
    using Console.Commands.Blocks;

    public interface INavigableElement
    {
        void Accept(IFlowNavigator navigator);
    }
}