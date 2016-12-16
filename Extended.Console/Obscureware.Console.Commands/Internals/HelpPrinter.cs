namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Obscureware.Console.Commands.Internals.Parsers;

    using ObscureWare.Console;

    internal class HelpPrinter : IKeyWordProvider
    {
        private static readonly string[] BaseInlineHelpCommands = new[] { "?", "help", "h" };
        private static readonly IEqualityComparer<string> SensitiveComparer = new SensitiveStringComparer();
        private static readonly IEqualityComparer<string> InsensitiveComparer = new InsensitiveStringComparer();

        private readonly ICommandParserOptions _options;
        private readonly CommandEngineStyles _styles;
        private readonly IConsole _console;
        private readonly string[] _allInlineHelpOptions;
        private readonly IEqualityComparer<string> _commandNameComparer;

        public HelpPrinter(ICommandParserOptions options, CommandEngineStyles styles, IConsole console) // TODO: extract IHelpStyles
        {
            this._options = options;
            this._styles = styles;
            this._console = console;
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
        public void PrintHelpOnHelp()
        {
            this._console.WriteText(this._styles.Default, "To get list of available commands type ");
            for (int i = 0; i < this._allInlineHelpOptions.Length; i++)
            {
                if (i > 0)
                {
                    this._console.WriteText(this._styles.Default, i == this._allInlineHelpOptions.Length - 1 ? " or " : ", ");
                }

                this._console.WriteText(this._styles.HelpDefinition, this._allInlineHelpOptions[i]);
            }

            this._console.WriteLine(this._styles.Default, ".");
        }

        /// <summary>
        /// Prints global help and list of available commands.
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="arguments">Extra help arguments. Not used at the moment.</param>
        public void PrintGlobalHelp(IEnumerable<CommandInfo> commands, IEnumerable<string> arguments)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            this._console.WriteLine(this._styles.HelpHeader, "Available commands:");

            foreach (var cmdInfo in commands)
            {
                this._console.WriteText(this._styles.HelpDefinition, cmdInfo.ModelBuilder.CommandName + "\t\t");
                this._console.WriteLine(this._styles.HelpDescription, cmdInfo.ModelBuilder.CommandDescription);

                // TODO: expose and print description in nice way - justified paragraph or else... Tables?
            }

            this._console.WriteLine();
            this._console.WriteLine(this._styles.Default, $"All command names are case {this._options.CommandsSensitivenes.ToString().ToLower()}.");
            this._console.WriteText(this._styles.Default, "To receive syntax help about particular command use \"");
            this._console.WriteText(this._styles.HelpDefinition, $"<commandName> {this._options.FlagCharacters.SelectRandom()}{BaseInlineHelpCommands.SelectRandom()}");
            this._console.WriteText(this._styles.Default, "\" or \"");
            this._console.WriteText(this._styles.HelpDefinition, $"{this._options.FlagCharacters.SelectRandom()}{BaseInlineHelpCommands.SelectRandom()} <commandName>");
            this._console.WriteLine(this._styles.Default, "\" syntax.");
            this._console.WriteLine();
        }

        public IEnumerable<string> GetCommandKeyWords()
        {
            return BaseInlineHelpCommands;
        }

        public IEnumerable<string> GetOptionKeyWords()
        {
            return BaseInlineHelpCommands;
        }
    }
}