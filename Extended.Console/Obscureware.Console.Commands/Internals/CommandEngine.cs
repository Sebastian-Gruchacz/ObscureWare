﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandEngine.cs" company="Obscureware Solutions">
// MIT License
// 
// Copyright(c) 2016 Sebastian Gruchacz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.  
// </copyright>
// <summary>
//   Defines the ICommandEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ObscureWare.Console;

    /// <summary>
    /// The command engine internal implementation.
    /// </summary>
    internal class CommandEngine : ICommandEngine
    {
        private readonly CommandManager _commandManager;

        private readonly IConsole _console;

        private readonly CommandEngineStyles _styles;

        private readonly HelpPrinter _helpPrinter;

        private readonly ICommandParserOptions _options;

        // NO default public constructor - by design
        internal CommandEngine(CommandManager commandManager, ICommandParserOptions options, CommandEngineStyles styles, IConsole console)
        {
            if (commandManager == null)
            {
                throw new ArgumentNullException(nameof(commandManager));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (styles == null)
            {
                throw new ArgumentNullException(nameof(styles));
            }
            if (console == null)
            {
                throw new ArgumentNullException(nameof(console));
            }

            this._commandManager = commandManager;
            this._console = console;
            this._options = options;
            this._styles = styles;

            this._helpPrinter = new HelpPrinter(this._options, this._styles); // TODO: inject from above too! it's required in builder already
        }

        /// <inheritdoc />
        public bool ExecuteCommand(object context, string commandLine)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (string.IsNullOrWhiteSpace(commandLine))
            {
                return true;
            }

            string[] commandLineArguments = CommandLineUtilities.SplitCommandLine(commandLine).ToArray();

            // ...
            string cmdName = commandLineArguments[0];
            if (this._helpPrinter.IsGlobalHelpRequested(cmdName))
            {
                // TODO: Global, or command help?

                this.PrintGlobalHelp(this._console, commandLineArguments.Skip(1));
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmdName))
            {
                return false; // just ignore
            }

            CommandInfo cmd = this._commandManager.FindCommand(cmdName);
            if (cmd == null)
            {
                this._console.WriteLine(this._styles.Warning, $"Unknown command => \"{cmdName}\".");
                this._helpPrinter.PrintHelpOnHelp(this._console);
                return false;
            }

            // TODO: first or any? And ignore all the other syntax...
            if (commandLineArguments.Length > 1) 
            {
                if (this._helpPrinter.IsCommandHelpRequested(commandLineArguments[1]))
                {
                    this.PrintCommandHelp(this._console, cmd, commandLineArguments.Skip(2)); // pass all remaining options only for detail-full syntax help (if available / implemented)
                    return false;
                }
            }

            // This might crash-throw if invalid types defined. Fine.
            var model = this.BuildModelForCommand(cmd, commandLineArguments.Skip(1));
            var outputManager = new OutputManager(this._console, this._styles);

            if (model == null)
            {
                // bad syntax
                return false;
            }

            try
            {
                cmd.Command.Execute(context, outputManager, model); // skip only cmdName itself
            }
            catch (Exception ex)
            {
                this._console.WriteLine(this._styles.Error, "An exception occurred during command execution:");
                this._console.WriteLine(this._styles.Error, ex.ToString());

                // TODO: log also to file?
                return false;
            }

            return true;
        }

        public void Run(ICommandEngineContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            while (!context.ShallTerminate)
            {
                this.DisplayPrompt(this._console, context.GetCurrentPrompt()); // TODO: perhaps multi-color prompt support?

                string cmdString = this._console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmdString))
                {
                    continue;
                }

                this.ExecuteCommand(context, cmdString);
            }
        }

        private void DisplayPrompt(IConsole consoleInstnace, string promptText)
        {
            consoleInstnace.WriteLine();
            consoleInstnace.WriteText(this._styles.Prompt, promptText);
            consoleInstnace.SetColors(this._styles.Default);
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
            return cmdInfo.ModelBuilder.BuildModel(arguments, this._options);
        }
    }
}