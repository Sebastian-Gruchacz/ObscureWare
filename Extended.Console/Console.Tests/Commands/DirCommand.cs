namespace ConsoleApplication1.Commands
{
    using System;
    using System.IO;
    using Obscureware.Console.Commands;
    using Obscureware.Console.Commands.Model;
    using Obscureware.Console.Operations.Tables;

    [CommandModel(typeof(DirCommandModel))]
    public class DirCommand : IConsoleCommand
    {
        public void Execute(object contextObject, ICommandOutput output, object runtimeModel)
        {
            var model = runtimeModel as DirCommandModel; // necessary to avoid Generic-inheritance troubles...

            // TODO: custom filters normalization?

            switch (model.Mode)
            {
                case DirectoryListMode.CurrentDir:
                {
                    this.ListCurrentFolder(contextObject, output, model);
                    break;
                }
                case DirectoryListMode.CurrentLocalState:
                    break;
                case DirectoryListMode.CurrentRemoteHead:
                    break;
                default:
                    break;
            }
        }

        private void ListCurrentFolder(object contextObject, ICommandOutput output, DirCommandModel parameters)
        {
            string filter = string.IsNullOrWhiteSpace(parameters.Filter) ? "*.*" : parameters.Filter;
            string basePath = Environment.CurrentDirectory;

            DataTable<string> filesTable = new DataTable<string>(
                new ColumnInfo("Name", ColumnAlignment.Left),
                new ColumnInfo("Size", ColumnAlignment.Right),
                new ColumnInfo("Last Modification", ColumnAlignment.Right)
                );

            var baseDir = new DirectoryInfo(basePath);
            if (parameters.IncludeFolders)
            {
                var dirs = baseDir.GetDirectories(filter, SearchOption.TopDirectoryOnly);
                foreach (var dirInfo in dirs)
                {
                    filesTable.AddRow(dirInfo.FullName, new []{ dirInfo.Name, "<DIR>", Directory.GetLastWriteTime(dirInfo.FullName).ToShortDateString()});
                }
            }

            var files = baseDir.GetFiles(filter, SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in files)
            {
                filesTable.AddRow(fileInfo.FullName, new [] { fileInfo.Name, fileInfo.Length.ToString(), File.GetLastWriteTime(fileInfo.FullName).ToShortDateString() });
            }

            // TODO: add number to xBytes string for Length

            output.PrintSimpleTable(filesTable);
        }
    }
}