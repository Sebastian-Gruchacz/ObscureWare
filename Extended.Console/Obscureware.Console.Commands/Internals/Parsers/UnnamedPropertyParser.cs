namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System.Reflection;
    using Model;

    internal class UnnamedPropertyParser : BasePropertyParser
    {
        private readonly int _argumentIndex;

        public UnnamedPropertyParser(int argumentIndex, PropertyInfo propertyInfo) : base(propertyInfo)
        {
            this._argumentIndex = argumentIndex;
        }

        public int ArgumentIndex
        {
            get { return this._argumentIndex; }
        }

        /// <inheritdoc />
        protected override void DoApply(CommandModel model, string[] args, ref int argIndex)
        {
            this.TargetProperty.SetValue(model, args[argIndex]);
        }
    }
}