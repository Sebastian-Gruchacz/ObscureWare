namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ObscureWare.Console;

    internal class HelpPrinter
    {
        private static readonly string[] BaseInlineHelpCommands = new[] { "?", "help", "h" };

        private readonly ICommandParserOptions _options;
        private readonly CommandEngineStyles _styles;
        private readonly string[] _allInlineHelpOptions;

        public HelpPrinter(ICommandParserOptions options, CommandEngineStyles styles) // TODO: extract IHelpStyles
        {
            this._options = options;
            this._styles = styles;

            this._allInlineHelpOptions = BaseInlineHelpCommands.SelectMany(txt => this._options.FlagCharacters.Select(f => f + txt)).ToArray();
        }

        public bool IsGlobalHelpRequested(string cmdName)
        {
            // TODO: more options available???
            return cmdName.Equals("help", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool IsCommandHelpRequested(string firstArgument)
        {
            return this._allInlineHelpOptions.Contains(firstArgument); // TODO: sensitiveness comparer
        }


        public void PrintCommandHelp(IConsole console, ModelBuilder cmdModelBuilder)
        {
            console.WriteLine(this._styles.Error, $"Function {nameof(this.PrintCommandHelp)} is not yet implemented.");
        }

        public void PrintHelpOnHelp(IConsole console)
        {
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            var availableHelpCommands = BaseInlineHelpCommands.Select(cm => this._options.SwitchCharacters.First() + cm).ToArray();

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
        ///
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

                // TODO: expose and print description in nice way - justified paragraph or else...
            }

            console.WriteLine();
            console.WriteLine(this._styles.Default, $"All command names are case {this._options.CommandsSensitivenes.ToString().ToLower()}.");
            console.WriteText(this._styles.Default, "To receive syntax help about particular command use \"");
            console.WriteText(this._styles.HelpDefinition, "<commandName> -h");
            console.WriteLine(this._styles.Default, "\" syntax.");
            console.WriteLine();
        }
    }
}