namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Model;

    internal abstract class BaseSwitchPropertyParser : BasePropertyParser
    {
        private readonly string[] _switchLetters;

        private readonly uint _expectedArguments;

        /// <inheritdoc />
        protected BaseSwitchPropertyParser(PropertyInfo propertyInfo, string[] switchLetters, uint expectedArguments = 1)
            : base(propertyInfo)
        {
            if (switchLetters == null) throw new ArgumentNullException(nameof(switchLetters));
            if (switchLetters.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(switchLetters));

            this._switchLetters = switchLetters;
            this._expectedArguments = expectedArguments;
        }

        protected override void DoApply(ICommandParserOptions options, CommandModel model, string[] args, ref int argIndex)
        {
            // read more arguments if necessary
            if (options.OptionArgumentMode == CommandOptionArgumentMode.Separated)
            {
                string[] optionArguments = new string[this._expectedArguments];
                for (int i = 0; i < optionArguments.Length; ++i)
                {
                    argIndex++; // skip option arg itself
                    optionArguments[i] = this.SafelyGetNextArg(args, ref argIndex);
                }

                this.DoApplySwitch(model, optionArguments);
            }
            else if (options.OptionArgumentMode == CommandOptionArgumentMode.Joined)
            {
                string firstArg = this.SafelyGetNextArg(args, ref argIndex);
                string[] parts = firstArg.Split(options.OptionArgumentJoinCharacater);

                this.DoApplySwitch(model, parts.Skip(1).ToArray()); // will support multi-values as well...
            }
            else
            {
                if (this._expectedArguments != 1)
                {
                    throw new BadImplementationException(
                        "Options in Merged mode can only have one value. This mode could not support more.",
                        this.GetType());
                }

                string firstArg = this.SafelyGetNextArg(args, ref argIndex);
                string[] combinations = CommandsSyntaxHelpers.Combine(options.SwitchCharacters, this._switchLetters, (s, s1) => s + s1).ToArray();
                string remainder = firstArg.CutLeftAny(combinations);

                this.DoApplySwitch(model, new []{ remainder });
            }

        }

        private string SafelyGetNextArg(string[] args, ref int argIndex)
        {
            if (argIndex >= args.Length)
            {
                throw new InvalidOperationException("Command line provided less arguments that Option expected.");
            }

            return args[argIndex++];
        }

        protected abstract void DoApplySwitch(CommandModel model, string[] switchArguments);
    }
}