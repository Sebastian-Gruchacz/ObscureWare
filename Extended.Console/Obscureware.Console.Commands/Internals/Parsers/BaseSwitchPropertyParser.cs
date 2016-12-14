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
        protected BaseSwitchPropertyParser(PropertyInfo propertyInfo, uint expectedArguments = 1) : base(propertyInfo)
        {
            this._expectedArguments = expectedArguments;
            // TODO: more parameters for switch perhaps, Converter?
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
            else
            {
                if (this._expectedArguments != 1)
                {
                    throw new BadImplementationException("Options in Merged or Joined mode could only be pairs. Those modes not support more than one value for option.", this.GetType());
                }

                string firstArg = this.SafelyGetNextArg(args, ref argIndex);
                string[] parts = firstArg.Split(options.OptionArgumentJoinCharacater);

                this.DoApplySwitch(model, parts.Skip(1).ToArray()); // will support multi-values as well...
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