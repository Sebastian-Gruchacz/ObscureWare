namespace Obscureware.DataFlow
{
    using System;
    using Implementation;

    public interface ICondition
    {
        string SerializedCondition { get; }
        Predicate<DataFlowToken> Condition { get; }
    }
}