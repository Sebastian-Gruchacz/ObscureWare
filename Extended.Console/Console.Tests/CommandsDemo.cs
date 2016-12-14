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
            var options = new CommandParserOptions
                {
                    FlagCharacters = new string[] {@"\", "-"},
                    SwitchCharacters = new string[] {@"-", "--"},
                    OptionArgumentMode = CommandOptionArgumentMode.Separated,
                    //OptionArgumentJoinCharacater = ':', // not used because of: CommandOptionArgumentMode.Separated
                    AllowFlagsAsOneArgument = false,
                    CommandsSensitivenes = CommandCaseSensitivenes.Insensitive,
                    UnnamedOptionsMode = UnnamedOptionsMode.EndOnly, // TODO: let the command decide ?
                };

            var engine = CommandEngine.BuildEngineForManualSelection(options, typeof(DirCommand), typeof(ClsCommand));

            //engine.Styles = new CommandEngineStyles
            //{
            //    // custom styles go here
            //};

            bool executedProperly = engine.ExecuteCommand(context, console, new[] {"dir", @"\f", "-m", "CurrentDir", "*.*" });
            //engine.ExecuteCommand(context, console, new[] { "cls" });
            engine.ExecuteCommand(context, console, new[] { "diraa" });
            engine.ExecuteCommand(context, console, new[] { "help" });
            engine.ExecuteCommand(context, console, new[] { "dir", "-h" });
        }
    }
}
