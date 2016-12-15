namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System.Reflection;
    using Model;

    /// <summary>
    /// Property parser for flags (BOOL)
    /// </summary>
    internal class FlagPropertyParser : BasePropertyParser
    {
        public FlagPropertyParser(PropertyInfo flagProperty) : base(flagProperty)
        {

        }

        /// <inheritdoc />
        protected override void DoApply(ICommandParserOptions options, CommandModel model, string[] args, ref int argIndex)
        {
            this.TargetProperty.SetValue(model, true);
        }
    }
}