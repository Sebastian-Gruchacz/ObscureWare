namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System;
    using System.Globalization;
    using System.Reflection;
    using Converters;
    using Model;

    internal class UnnamedPropertyParser : BasePropertyParser
    {
        private readonly int _argumentIndex;
        private readonly ArgumentConverter _converter;

        public UnnamedPropertyParser(int argumentIndex, PropertyInfo propertyInfo, ArgumentConverter converter) : base(propertyInfo)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            this._argumentIndex = argumentIndex;
            this._converter = converter;
        }

        public int ArgumentIndex
        {
            get { return this._argumentIndex; }
        }

        /// <inheritdoc />
        protected override void DoApply(ICommandParserOptions options, CommandModel model, string[] args, ref int argIndex)
        {
            this.TargetProperty.SetValue(model, this._converter.TryConvert(args[argIndex], CultureInfo.CurrentUICulture)); // TODO: proper Engine / console culture everywhere, passing
        }
    }
}