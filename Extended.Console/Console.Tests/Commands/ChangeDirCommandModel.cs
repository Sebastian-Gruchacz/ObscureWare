namespace ConsoleApplication1.Commands
{
    using Obscureware.Console.Commands.Model;

    /// <summary>
    /// The change dir command model.
    /// </summary>
    [CommandDescriptorFor(typeof(ChangeDirCommand))]
    [CommandName("cd")]
    [CommandDescription(@"Moves Current Directory specific way.")]
    public class ChangeDirCommandModel : CommandModel
    {
        // TODO: parameters...
    }
}