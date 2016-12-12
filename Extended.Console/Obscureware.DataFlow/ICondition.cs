namespace Obscureware.Console.Commands.Blocks
{
    using System;

    public interface ICondition
    {
        string SerializedCondition { get; }
        Predicate<FlowToken> Condition { get; }
    }
}