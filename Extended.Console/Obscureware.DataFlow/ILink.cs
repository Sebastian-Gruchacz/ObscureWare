namespace Obscureware.Console.Commands.Blocks
{
    using DataFlow.Model;

    public interface ILink
    {
        BlockBase Source { get; }
        BlockBase Target { get; }
        ICondition Condition { get; set; }
    }
}