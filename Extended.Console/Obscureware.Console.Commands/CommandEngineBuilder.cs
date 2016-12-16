namespace Obscureware.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Obscureware.Console.Commands.Internals;

    using ObscureWare.Console;

    /// <summary>
    /// Fluent-syntax based CommandEngine builder
    /// </summary>
    public class CommandEngineBuilder
    {
        private CommandParserOptions _options;

        private CommandEngineStyles _styles;

        private readonly List<Type> _commands;

        private CommandEngineBuilder()
        {
            _commands = new List<Type>();

            // TODO: add standard commands
        }

        public CommandEngineBuilder UsingStyles(CommandEngineStyles styles)
        {
            if (styles == null)
            {
                throw new ArgumentNullException(nameof(styles));
            }

            this._styles = styles;

            return this;
        }

        public CommandEngineBuilder WithOptions(CommandParserOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this._options = options;

            return this;
        }

        public CommandEngineBuilder WithCommandsFromAssembly(Assembly asm)
        {
            if (asm == null)
            {
                throw new ArgumentNullException(nameof(asm));
            }


            // TODO: verify and check comamnds

            return this;
        }

        public CommandEngineBuilder WithCommands(params Type[] commandTypes)
        {
            if (commandTypes == null)
            {
                throw new ArgumentNullException(nameof(commandTypes));
            }
            

            this._commands.AddRange(commandTypes);


            return this;
        }


        public ICommandEngine ConstructForConsole(IConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            if (this._options == null)
            {
                throw new InvalidOperationException("Could not construct engine without providing Options object.");
            }

            if (this._styles == null)
            {
                throw new InvalidOperationException("Could not construct engine without providing Styles object.");
            }

            // TODO: build Helper print!
            // TODO: keyword check already !

            return new CommandEngine(new CommandManager(this._commands.ToArray()), this._options, this._styles, console);
        }

        /// <summary>
        /// Start fluent syntax
        /// </summary>
        /// <returns></returns>
        public static CommandEngineBuilder Build()
        {
            return new CommandEngineBuilder();
        }
    }
}
