namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Model;

    internal abstract class BaseSwitchPropertyParser : BasePropertyParser
    {
        private readonly uint _expectedArguments;

        /// <inheritdoc />
        protected BaseSwitchPropertyParser(PropertyInfo propertyInfo, uint expectedArguments = 1)
            : base(propertyInfo)
        {
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

                throw new NotImplementedException();
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