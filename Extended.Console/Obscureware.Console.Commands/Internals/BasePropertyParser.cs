namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Reflection;
    using Model;

    internal abstract class BasePropertyParser
    {
        public PropertyInfo TargetProperty { get; private set; }

        protected BasePropertyParser(PropertyInfo flagProperty)
        {
            if (flagProperty == null)
            {
                throw new ArgumentNullException(nameof(flagProperty));
            }

            this.TargetProperty = flagProperty;
        }

        public void Apply(CommandModel model, string[] args, ref int argIndex)
        {
            if (model.GetType() != this.TargetProperty.DeclaringType)
            {
                throw new InvalidOperationException("Incompatible model type.");
            }

            this.DoApply(model, args, ref argIndex);
        }

        protected abstract void DoApply(CommandModel model, string[] args, ref int argIndex);
    }
}