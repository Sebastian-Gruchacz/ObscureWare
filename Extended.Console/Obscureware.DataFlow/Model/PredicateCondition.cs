namespace Obscureware.DataFlow.Model
{
    using System;
    using System.Linq.Expressions;
    using Implementation;

    public class PredicateCondition<TTuple> : ICondition
    {
        public string SerializedCondition { get; private set; }

        public Predicate<DataFlowToken> Condition { get; private set; }

        public PredicateCondition(Expression<Predicate<TTuple>> condition)
        {
            this.SerializedCondition = $"{typeof(TTuple).Name}.{condition}";
            var conditionOnTuple = condition.Compile();
            this.Condition = o => conditionOnTuple(o.Get<TTuple>());
        }

        public void Accept(IFlowNavigator navigator)
        {
            navigator.Visit(this);
        }
    }
}