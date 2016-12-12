namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Linq.Expressions;

    public class PredicateCondition<TTuple> : ICondition
    {
        public string SerializedCondition { get; private set; }

        public Predicate<FlowToken> Condition { get; private set; }

        public PredicateCondition(Expression<Predicate<TTuple>> condition)
        {
            this.SerializedCondition = string.Format("{0}.{1}", typeof(TTuple).Name, condition);
            var conditionOnTuple = condition.Compile();
            this.Condition = o => conditionOnTuple(o.Get<TTuple>());
        }

        public void Accept(IFlowVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}