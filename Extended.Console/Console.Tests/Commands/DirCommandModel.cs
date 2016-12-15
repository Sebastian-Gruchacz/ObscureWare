namespace ConsoleApplication1.Commands
{
    using Obscureware.Console.Commands.Model;

    [CommandDescriptorFor(typeof(DirCommand))]
    [CommandName("dir")]
    [CommandDescription(@"Lists files withing current folder or repository state, depending on selected options.")]
    public class DirCommandModel : CommandModel
    {
        [Mandatory(false)]
        [CommandFlag("d", "D")]
        // TODO: Name Attribute? Or just use activation letters for help/syntax display?
        [CommandDescription("When set specifies whether directories shall be listed too.")]
        public bool IncludeFolders { get; set; }

        [Mandatory()]
        [CommandSwitch(typeof(DirectoryListMode), "m", DefaultValue = DirectoryListMode.CurrentDir)]
        [CommandDescription("Specifies which predefined directory location shall be listed.")]
        // TODO: list help for switches.
        // TODO: more switch types?
        // TODO: runtime support switch auto-complete. Sourced through ModelBuilder & Parser
        public DirectoryListMode Mode { get; set; }

        [Mandatory(false)]
        [CommandValueFlag("f", "F")]
        [CommandDescription("Specifies filter for enumerated files. Does not apply to folders listing.")]
        // TODO: runtime support for some values / unnamed values autocompletion? sourced through command itself...
        public string Filter { get; set; }
    }

    // TODO: add sorting
}