namespace ConsoleApplication1.Commands
{
    using Obscureware.Console.Commands.Model;

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
        [CommandSwitch(typeof(DirectoryListMode), "m", DefaultValue = DirectoryListMode.CurrentDir)]
        [CommandDescription("Specifies which predefined directory location shall be listed.")]
        // TODO: list help for switches.
        public DirectoryListMode Mode { get; set; }

        [Mandatory(false)]
        [CommandValueFlag(ValueFlagType.Text, "f", "F")]
        //[CommandUnnamedOption(0)]
        [CommandDescription("Specifies filter for enumerated files. Does not apply to folders listing.")]
        public string Filter { get; set; }
    }
}