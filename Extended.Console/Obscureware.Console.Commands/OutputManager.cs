using System;
using System.Collections.Generic;
using ObscureWare.Console;

namespace Obscureware.Console.Commands
{
    public class OutputManager : ICommandOutput
    {
        private readonly IConsole _consoleInstance;

        public OutputManager(IConsole consoleInstance)
        {
            _consoleInstance = consoleInstance;
        }

        public void PrintResultLines(IEnumerable<string> results)
        {
            throw new NotImplementedException();
        }
    }
}