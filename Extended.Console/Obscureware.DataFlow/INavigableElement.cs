namespace Obscureware.DataFlow
{
    public interface INavigableElement
    {
        void Accept(IFlowNavigator navigator);
    }
}