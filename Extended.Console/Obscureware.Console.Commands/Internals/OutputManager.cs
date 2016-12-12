namespace Obscureware.Console.Commands.Internals
{
    using System.Collections.Generic;
    using ObscureWare.Console;

    public class OutputManager : ICommandOutput
    {
        private readonly IConsole _consoleInstance;

        public OutputManager(IConsole consoleInstance)
        {
            this._consoleInstance = consoleInstance;
        }

        public void PrintResultLines(IEnumerable<string> results)
        {
            // TODO: improve!

            foreach (var result in results)
            {
                this._consoleInstance.WriteLine(result);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            this._consoleInstance.Clear();
        }
    }
}