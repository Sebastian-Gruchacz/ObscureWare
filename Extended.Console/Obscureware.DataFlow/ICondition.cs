namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using DataFlow.Implementation;

    public interface ICondition
    {
        string SerializedCondition { get; }
        Predicate<DataFlowToken> Condition { get; }
    }
}