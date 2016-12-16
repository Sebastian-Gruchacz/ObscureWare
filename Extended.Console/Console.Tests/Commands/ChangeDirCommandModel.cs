namespace ConsoleApplication1.Commands
{
    using Obscureware.Console.Commands;
    using Obscureware.Console.Commands.Model;

    /// <summary>
    /// The change dir command model.
    /// </summary>
    [CommandDescriptorFor(typeof(ChangeDirCommand))]
    [CommandName("cd")]
    [CommandDescription(@"Moves Current Directory specific way.")]
    public class ChangeDirCommandModel : CommandModel
    {
        [OptionName(@"target")]
        [Mandatory(false)]
        [CommandUnnamedOption(0)]
        [CommandDescription("Specifies how directory shall be changed. Nothing or '.' will remain in current folder. '..' Will go one level up. '\\' will immediately jump to the root. Anything else means subdirectory or exact location - if has rooted format..")]
        public string Target { get; set; }
    }
}