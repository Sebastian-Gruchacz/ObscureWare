namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using Converters;
    using Model;

    internal class CustomValueSwitchParser : BaseSwitchPropertyParser
    {
        private readonly ArgumentConverter _converter;

        public CustomValueSwitchParser(PropertyInfo propertyInfo, CommandValueFlagAttribute valueAtt, ArgumentConverter converter) : base(propertyInfo, valueAtt.CommandLiterals)
        {
            this._converter = converter;
        }

        /// <inheritdoc />
        protected override void DoApplySwitch(CommandModel model, string[] switchArguments)
        {
            if (switchArguments.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(switchArguments));
            }
            string valueText = switchArguments[0];

            object value = this._converter.TryConvert(valueText, CultureInfo.CurrentUICulture);

            this.TargetProperty.SetValue(model, value);
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValidValues()
        {
            yield break; // custom values switch does not have predefined values, ofcourse.
        }
    }
}