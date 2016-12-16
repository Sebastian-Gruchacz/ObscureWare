namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Model;

    internal class EnumSwitchParser : BaseSwitchPropertyParser
    {
        private readonly Type _enumType;

        private readonly string[] _validValues;

        public EnumSwitchParser(PropertyInfo propertyInfo, CommandSwitchAttribute switchAttribute) : base(propertyInfo, switchAttribute.CommandLiterals)
        {
            if (switchAttribute == null)
            {
                throw new ArgumentNullException(nameof(switchAttribute));
            }

            this._enumType = switchAttribute.SwitchBaseType;
            this._validValues = Enum.GetNames(this._enumType);
        }

        /// <inheritdoc />
        protected override void DoApplySwitch(CommandModel model, string[] switchArguments)
        {
            if (switchArguments.Length == 0)
            {
                throw new ArgumentException("Value cannot be an empty collection.", nameof(switchArguments));
            }
            string enumText = switchArguments[0];

            object enumValue = Enum.Parse(this._enumType, enumText, true); // might fail, TODO: Try finding something better than exception during parsing suer input...

            this.TargetProperty.SetValue(model, enumValue);
        }

        /// <inheritdoc />
        public override IEnumerable<string> GetValidValues()
        {
            return this._validValues;
        }
    }
}