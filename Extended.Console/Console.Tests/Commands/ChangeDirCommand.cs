namespace ConsoleApplication1.Commands
{
    using System;
    using System.IO;

    using Obscureware.Console.Commands;
    using Obscureware.Console.Commands.Model;

    [CommandModel(typeof(ChangeDirCommandModel))]
    public class ChangeDirCommand : IConsoleCommand
    {
        /// <inheritdoc />
        public void Execute(object contextObject, ICommandOutput output, object runtimeModel)
        {

            // TODO: switch model options: root, up, specific folder, sub-folder

            //DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
            //if (dir.FullName != dir.Root.FullName)
            //{
            //    Environment.CurrentDirectory = dir.Parent.FullName;
            //    //(contextObject as ConsoleContext).
            //}
        }
    }
}