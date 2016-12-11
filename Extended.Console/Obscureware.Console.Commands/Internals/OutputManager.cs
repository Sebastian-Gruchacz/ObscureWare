namespace Obscureware.Console.Commands.Internals
{
    using System.Collections.Generic;
    using ObscureWare.Console;

    public class OutputManager : ICommandOutput
    {
        private readonly IConsole _consoleInstance;

        public OutputManager(IConsole consoleInstance)
        {
            _consoleInstance = consoleInstance;
        }

        public void PrintResultLines(IEnumerable<string> results)
        {
            // TODO: improve!

            foreach (var result in results)
            {
                _consoleInstance.WriteLine(result);
            }
        }
    }
}