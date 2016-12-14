namespace Obscureware.Console.Commands
{
    using Internals;
    using System.Collections.Generic;

    using ObscureWare.Console;
    using System;
    using System.Linq;
    using System.Reflection;

    public class CommandEngine
    {
        private readonly CommandManager _commandManager;
        private CommandEngineStyles _styles;
        private HelpPrinter _helpPrinter;
        private ICommandParserOptions _options;

        // TODO: move console to constructor and inject to dependants instead of through functions.
        // NO default public constructor - by design
        private CommandEngine(Type[] commands, ICommandParserOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
            this.Styles = CommandEngineStyles.DefaultStyles; // use default even if user not defines any
            this._commandManager = new CommandManager(commands) { CommandsSensitivenes = this.Options.CommandsSensitivenes };
        }

        public ICommandParserOptions Options
        {
            get { return this._options; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                this._options = value;
                this.OnOptionsChanged();
            }
        }

        /// <summary>
        /// Specifies styles used by CommandEngine to color the output and help.
        /// </summary>
        public CommandEngineStyles Styles
        {
            get { return this._styles; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                this._styles = value;
                this.OnStylesChanged();
            }
        }

        private void OnStylesChanged()
        {
            if (this.Options != null && this.Styles != null)
            {
                this._helpPrinter = new HelpPrinter(this.Options, this.Styles);
            }

            // TODO: more?
        }

        private void OnOptionsChanged()
        {
            if (this.Options != null && this.Styles != null)
            {
                this._helpPrinter = new HelpPrinter(this.Options, this.Styles);
            }
            // TODO: more...
        }

        // https://en.wikipedia.org/wiki/Command-line_interface


        /// <summary></summary>
        /// <param name="context">Shared context object, that will be</param>
        /// <param name="consoleInstance"></param>
        /// <param name="commandLineArguments"></param>
        public bool ExecuteCommand(object context, IConsole consoleInstance, string[] commandLineArguments)
        {
            if (consoleInstance == null) throw new ArgumentNullException(nameof(consoleInstance));
            if (commandLineArguments == null) throw new ArgumentNullException(nameof(commandLineArguments));
            if (commandLineArguments.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(commandLineArguments));

            // ...
            string cmdName = commandLineArguments[0];
            if (this._helpPrinter.IsGlobalHelpRequested(cmdName))
            {
                this.PrintGlobalHelp(consoleInstance, commandLineArguments.Skip(1));
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmdName))
            {
                return false; // just ignore
            }

            CommandInfo cmd = this._commandManager.FindCommand(cmdName);
            if (cmd == null)
            {
                consoleInstance.WriteLine(this.Styles.Warning, $"Unknown command => \"{cmdName}\".");
                this._helpPrinter.PrintHelpOnHelp(consoleInstance);
                return false;
            }

            if (commandLineArguments.Length > 1) // TODO: first or any? And ignore all the other syntax...
            {
                if (this._helpPrinter.IsCommandHelpRequested(commandLineArguments[1]))
                {
                    this.PrintCommandHelp(consoleInstance, cmd, commandLineArguments.Skip(2)); // pass all remaining options only for detail-full syntax help (if available / implemented)
                    return false;
                }
            }

            // This might crash-throw if invalid types defined. Fine.
            var model = this.BuildModelForCommand(cmd, commandLineArguments.Skip(1));
            var outputManager = new OutputManager(consoleInstance, this.Styles);

            try
            {
                cmd.Command.Execute(context, outputManager, model); // skip only cmdName itself
            }
            catch (Exception)
            {
                // TODO: print error
                return false;
            }

            return true;
        }

        private void PrintGlobalHelp(IConsole console, IEnumerable<string> arguments)
        {
            this._helpPrinter.PrintGlobalHelp(console, this._commandManager.GetAll(), arguments);
        }

        private void PrintCommandHelp(IConsole console, CommandInfo cmd, IEnumerable<string> skip)
        {
            this._helpPrinter.PrintCommandHelp(console, cmd.ModelBuilder);
        }

        private object BuildModelForCommand(CommandInfo cmdInfo, IEnumerable<string> arguments)
        {
            this.Options.ValidateParserOptions();

            return cmdInfo.ModelBuilder.BuildModel(arguments, this.Options);
        }

        ///  <summary>
        ///
        ///  </summary>
        /// <param name="options"></param>
        /// <param name="commands"></param>
        /// <returns></returns>
        public static CommandEngine BuildEngineForManualSelection(CommandParserOptions options, params Type[] commands)
        {
            return new CommandEngine(commands, options);
        }

        public static CommandEngine BuildEngineWithAutodiscovery(params Assembly[] assembliesToScan)
        {
            throw new NotImplementedException();
        }
    }
}