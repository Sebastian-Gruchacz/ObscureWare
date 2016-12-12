namespace ConsoleApplication1
{
    using System.IO;
    using ObscureWare.Console;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Obscureware.Console.Commands;

    public class TestCommands
    {
        public TestCommands(IConsole console)
        {
            ComsoleContext context = new ComsoleContext();

            var engine = CommandEngine.BuildEngineForManualSelection(typeof(DirCommand));
            engine.FlagCharacters = new string[] { @"\", "-" };
            engine.SwitchCharacters = new string[] { @"-", "--" };
            engine.OptionArgumentMode = CommandOptionArgumentMode.Separated;
            //engine.OptionArgumentJoinCharacater = ':'; // not used because of: CommandOptionArgumentMode.Separated
            engine.AllowFlagsAsOneArgument = false;
            engine.CommandsSensitivenes = CommandCaseSensitivenes.Insensitive;
            engine.UnnamedOptionsMode = UnnamedOptionsMode.EndOnly; // TODO: let the command decide ?
            //engine.Styles = new CommandEngineStyles
            //{
            //    // custom styles go here
            //};

            bool executedProperly = engine.ExecuteCommand(context, console, new[] {"dir", @"\f", "-m", "CurrentDir", "*.*" });
            engine.ExecuteCommand(context, console, new[] {"diraa" });
            engine.ExecuteCommand(context, console, new[] { "help" });
            engine.ExecuteCommand(context, console, new[] { "dir", "-h" });
        }
    }

    public class ComsoleContext
    {
    }

    [CommandModel(typeof(DirCommandModel))]
    public class DirCommand : IConsoleCommand
    {
        public void Execute(object contextObject, ICommandOutput output, object runtimeModel)
        {
            var model = runtimeModel as DirCommandModel; // necessary to avoid Generic-inheritance troubles...

            // TODO: custom filters normalization?

            switch (model.Mode)
            {
                case ListMode.CurrentDir:
                {
                    this.ListCurrentFolder(contextObject, output, model);
                    break;
                }
                case ListMode.CurrentLocalState:
                    break;
                case ListMode.CurrentRemoteHead:
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

    [CommandDescriptorFor(typeof(DirCommand))]
    [CommandName("dir")]
    [CommandDescription(@"Lists files withing current folder or repository state, depending on selected options.")]
    public class DirCommandModel : CommandModel
    {
        [Mandatory(false)]
        [CommandFlag(false, "d", "D")]
        // TODO: Name Attribute? Or just use activation letters for help/syntax display?
        [CommandDescription("When set specifies whether directories shall be listed too.")]
        public bool IncludeFolders { get; set; }

        [Mandatory()]
        [CommandSwitch(typeof(ListMode), "m", DefaultValue = ListMode.CurrentDir)]
        [CommandDescription("Specifies which predefined directory location shall be listed.")]
        // TODO: list help for switches.
        public ListMode Mode { get; set; }

        [Mandatory(false)]
        [CommandValueFlag(ValueFlagType.Text, "f", "F")]
        //[CommandUnnamedOption(0)]
        [CommandDescription("Specifies filter for enumerated files. Does not apply to folders listing.")]
        public string Filter { get; set; }
    }

    public enum ListMode
    {
        CurrentDir,

        CurrentLocalState,

        CurrentRemoteHead
    }
}
