namespace ConsoleApplication1.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Obscureware.Console.Commands;
    using Obscureware.Console.Commands.Model;

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
            List<string> results = new List<string>();

            //TODO use Directory/FileInfo to get more data and filename only
            if (parameters.IncludeFolders)
            {
                var dirs = Directory.GetDirectories(basePath, filter, SearchOption.TopDirectoryOnly);
                results.AddRange(dirs);
            }

            var files = Directory.GetFiles(basePath, filter, SearchOption.TopDirectoryOnly);
            results.AddRange(files);

            // TODO: add more columns and print as a table

            output.PrintResultLines(results); // TODO: formatting selection...
        }
    }
}