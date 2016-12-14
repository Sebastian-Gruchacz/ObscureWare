namespace Obscureware.Console.Commands.Internals
{
    using System.Collections.Generic;
    using ObscureWare.Console;
    using Operations.Tables;

    public class OutputManager : ICommandOutput
    {
        private readonly IConsole _consoleInstance;
        private readonly CommandEngineStyles _engineStyles;
        private DataTablePrinter _tablePrinter;

        public OutputManager(IConsole consoleInstance, CommandEngineStyles engineStyles)
        {
            this._consoleInstance = consoleInstance;
            this._engineStyles = engineStyles;
            this._tablePrinter = new DataTablePrinter(consoleInstance);
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

        /// <inheritdoc />
        public void PrintSimpleTable<T>(DataTable<T> filesTable)
        {
            this._tablePrinter.PrintAsSimpleTable(filesTable, this._engineStyles.HelpHeader, this._engineStyles.HelpDefinition);
        }

        public void PrintWarning(string message)
        {
            this._consoleInstance.WriteLine(this._engineStyles.Error, message);
        }
    }
}