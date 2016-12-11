namespace Obscureware.Console.Commands
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using ObscureWare.Console;
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    public class CommandEngine
    {
        private CommandEngineStyles _styles;
        private static readonly string[] InlineHelpCommands = new[] { "?", "help", "h" };
        private readonly Dictionary<string, CommandInfo> _commands;


        // NO default public constructor - by design
        private CommandEngine(Type[] commands)
        {
            this.Styles = CommandEngineStyles.DefaultStyles; // use default even if user not defines any

            this._commands = this.CheckCommands(commands);
        }

        private Dictionary<string, CommandInfo> CheckCommands(Type[] commands)
        {
            Dictionary<string, CommandInfo> result = new Dictionary<string, CommandInfo>();

            ConsoleCommandBuilder builder = new ConsoleCommandBuilder();
            foreach (var commandType in commands)
            {
                Tuple<string, ModelBuilder, IConsoleCommand> cmd = builder.ValidateAndBuildCommand(commandType);
                result.Add(cmd.Item1.ToUpper(), new CommandInfo(cmd.Item3, cmd.Item2));
            }

            return result;
        }

        public string[] FlagCharacters { get; set; }
        public string[] SwitchCharacters { get; set; }
        public CommandOptionArgumentMode OptionArgumentMode { get; set; }
        public char OptionArgumentJoinCharacater { get; set; }
        public bool AllowFlagsAsOneArgument { get; set; }
        public UnnamedOptionsMode UnnamedOptionsMode { get; set; }

        /// <summary>
        /// Specifies styles used by CommandEngine to color the output and help.
        /// </summary>
        public CommandEngineStyles Styles
        {
            get { return _styles; }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));

                _styles = value;
            }
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
            if (this.IsGlobalHelpRequested(cmdName))
            {
                this.PrintGlobalHelp(commandLineArguments.Skip(1));
                return false;
            }

            CommandInfo cmd = this.FindCommand(cmdName);
            if (cmd == null)
            {
                consoleInstance.WriteLine(this.Styles.Warning, "Unknown command.");
                this.PrintHelpOnHelp(consoleInstance);
                return false;
            }

            if (commandLineArguments.Length > 1) // TODO: first or any? And ignore all the other syntax...
            {
                if (this.IsCommandHelpRequested(commandLineArguments[1]))
                {
                    this.PrintCommandHelp(cmd.Command, commandLineArguments.Skip(2)); // pass all remaining options only for detail-full syntax help (if available / implemented)
                    return false;
                }
            }

            // This might crash-throw if invalid types defined. Fine.
            var model = this.BuildModelForCommand(cmd, commandLineArguments.Skip(1));
            var outputManager = new OutputManager(consoleInstance);

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



        private bool IsGlobalHelpRequested(string cmdName)
        {
            // TODO: more options
            return cmdName.Equals("help", StringComparison.InvariantCultureIgnoreCase);
        }



        private bool IsCommandHelpRequested(string commandLineArgument)
        {
            // TODO: implement using InlineHelpCommands
            return false;
        }

        private CommandInfo FindCommand(string cmdName)
        {
            CommandInfo cmd;
            if (this._commands.TryGetValue(cmdName.ToUpper(), out cmd))
            {
                return cmd;
            }

            return null;
        }

        private void PrintGlobalHelp(IEnumerable<string> skip)
        {
            throw new NotImplementedException();
        }

        private void PrintHelpOnHelp(IConsole console)
        {
            console.WriteText(this.Styles.Default, "To get list of available commands type ");
            var availableCommands = InlineHelpCommands.Select(cm => this.SwitchCharacters.First() + cm).ToArray();
            for (int i = 0; i < availableCommands.Length; i++)
            {
                if (i > 0)
                {
                    console.WriteText(this.Styles.Default, i == availableCommands.Length - 1 ? " or " : ", ");
                }

                console.WriteText(this.Styles.HelpDefinition, availableCommands[i]);
            }

            console.WriteLine(this.Styles.Default, ".");
        }

        private void PrintCommandHelp(IConsoleCommand cmd, IEnumerable<string> skip)
        {
            throw new NotImplementedException();
        }

        private object BuildModelForCommand(CommandInfo cmdInfo, IEnumerable<string> arguments)
        {
            return cmdInfo.ModelBuilder.BuildModel(arguments);
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="commands"></param>
        /// <returns></returns>
        public static CommandEngine BuildEngineForManualSelection(params Type[] commands)
        {
            return new CommandEngine(commands);
        }

        public static CommandEngine BuildEngineWithAutodiscovery(params Assembly[] assembliesToScan)
        {
            throw new NotImplementedException();
        }
    }
}