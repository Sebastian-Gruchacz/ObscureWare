namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Obscureware.Console.Commands.Internals.Parsers;

    using ObscureWare.Console;

    internal class HelpPrinter
    {
        private static readonly string[] BaseInlineHelpCommands = new[] { "?", "help", "h" };
        private static readonly IEqualityComparer<string> SensitiveComparer = new SensitiveStringComparer();
        private static readonly IEqualityComparer<string> InsensitiveComparer = new InsensitiveStringComparer();

        private readonly ICommandParserOptions _options;
        private readonly CommandEngineStyles _styles;
        private readonly string[] _allInlineHelpOptions;
        private readonly IEqualityComparer<string> _commandNameComparer;

        public HelpPrinter(ICommandParserOptions options, CommandEngineStyles styles) // TODO: extract IHelpStyles
        {
            this._options = options;
            this._styles = styles;
            this._allInlineHelpOptions = CommandsSyntaxHelpers.Combine(this._options.FlagCharacters, BaseInlineHelpCommands, ((s, s1) => s + s1)).ToArray();
            this._commandNameComparer = (options.CommandsSensitivenes == CommandCaseSensitivenes.Sensitive)
                ? SensitiveComparer
                : InsensitiveComparer;
        }

        /// <summary>
        /// Returns true, if given command is Global-Help command
        /// </summary>
        /// <param name="cmdName"></param>
        /// <returns></returns>
        public bool IsGlobalHelpRequested(string cmdName)
        {
            // TODO: reject registration of commands that would be in conflict with build-in commands like "help"
            return this._allInlineHelpOptions.Contains(cmdName, this._commandNameComparer);
        }

        /// <summary>
        /// Returns true, if user requested help about command details.
        /// </summary>
        /// <param name="firstArgument"></param>
        /// <returns></returns>
        public bool IsCommandHelpRequested(string firstArgument)
        {
            // TODO: also improve syntax for both "help <command>" and "<command> -help" (or "/?" is configured such). Update help message in PrintGlobalHelp()
            return this._allInlineHelpOptions.Contains(firstArgument, this._commandNameComparer);
        }

        /// <summary>
        /// Prints full (?) help about particular command.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="cmdModelBuilder"></param>
        public void PrintCommandHelp(IConsole console, ModelBuilder cmdModelBuilder)
        {
            console.WriteLine(this._styles.Error, $"Function {nameof(this.PrintCommandHelp)} is not yet implemented.");
        }

        /// <summary>
        /// Prints generic help information.
        /// </summary>
        /// <param name="console"></param>
        public void PrintHelpOnHelp(IConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            var availableHelpCommands = CommandsSyntaxHelpers.Combine(this._options.SwitchCharacters, BaseInlineHelpCommands, ((s, s1) => s + s1)).ToArray();

            console.WriteText(this._styles.Default, "To get list of available commands type ");
            for (int i = 0; i < availableHelpCommands.Length; i++)
            {
                if (i > 0)
                {
                    console.WriteText(this._styles.Default, i == availableHelpCommands.Length - 1 ? " or " : ", ");
                }

                console.WriteText(this._styles.HelpDefinition, availableHelpCommands[i]);
            }

            console.WriteLine(this._styles.Default, ".");
        }

        /// <summary>
        /// Prints global help and list of available commands.
        /// </summary>
        /// <param name="console"></param>
        /// <param name="commands"></param>
        /// <param name="arguments">Extra help arguments. Not used at the moment.</param>
        public void PrintGlobalHelp(IConsole console, IEnumerable<CommandInfo> commands, IEnumerable<string> arguments)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            console.WriteLine(this._styles.HelpHeader, "Available commands:");

            foreach (var cmdInfo in commands)
            {
                console.WriteText(this._styles.HelpDefinition, cmdInfo.ModelBuilder.CommandName + "\t\t");
                console.WriteLine(this._styles.HelpDescription, cmdInfo.ModelBuilder.CommandDescription);

                // TODO: expose and print description in nice way - justified paragraph or else... Tables?
            }

            console.WriteLine();
            console.WriteLine(this._styles.Default, $"All command names are case {this._options.CommandsSensitivenes.ToString().ToLower()}.");
            console.WriteText(this._styles.Default, "To receive syntax help about particular command use \"");
            console.WriteText(this._styles.HelpDefinition, $"<commandName> {this._options.SwitchCharacters.First()}h");

            // TODO: add alternative syntax: -help <commandName>

            console.WriteLine(this._styles.Default, "\" syntax.");
            console.WriteLine();
        }
    }
}