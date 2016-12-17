namespace Obscureware.DataFlow
{
    using Model;

    public interface ILink
    {
        BlockBase Source { get; }
        BlockBase Target { get; }
        ICondition Condition { get; set; }
    }
}