namespace ConsoleApplication1
{
    using Commands;
    using ObscureWare.Console;
    using Obscureware.Console.Commands;

    public class TestCommands
    {
        public TestCommands(IConsole console)
        {
            ComsoleContext context = new ComsoleContext();

            var engine = CommandEngine.BuildEngineForManualSelection(typeof(DirCommand), typeof(ClsCommand));
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
            engine.ExecuteCommand(context, console, new[] { "cls" });
            engine.ExecuteCommand(context, console, new[] { "diraa" });
            engine.ExecuteCommand(context, console, new[] { "help" });
            engine.ExecuteCommand(context, console, new[] { "dir", "-h" });
        }
    }
}
