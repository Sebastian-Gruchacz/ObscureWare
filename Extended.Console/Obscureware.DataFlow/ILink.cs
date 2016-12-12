namespace Obscureware.Console.Commands.Blocks
{
    public interface ILink
    {
        ProcessingBlockBase Source { get; }
        ProcessingBlockBase Target { get; }
        ICondition Condition { get; set; }
    }
}