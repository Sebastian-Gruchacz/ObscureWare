﻿namespace ConsoleApplication1
{
    using Commands;
    using ObscureWare.Console;
    using Obscureware.Console.Commands;

    public class TestCommands
    {
        public TestCommands(IConsole console)
        {
            ConsoleContext context = new ConsoleContext();
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

            var engine =
                CommandEngineBuilder.Build()
                    //.WithCommands(typeof(DirCommand), typeof(ClsCommand), typeof(ExitCommand), typeof(ChangeDirUpCommand), typeof(ChangeDirCommand))
                    .WithCommandsFromAssembly(this.GetType().Assembly)
                    .UsingOptions(options)
                    .UsingStyles(CommandEngineStyles.DefaultStyles)
                    .ConstructForConsole(console);


            //engine.Styles = new CommandEngineStyles
            //{
            //    // custom styles go here
            //};

            bool executedProperly = engine.ExecuteCommand(context, @"dir \d -m CurrentDir -f *.*");
            //engine.ExecuteCommand(context, console, "cls" );
            engine.ExecuteCommand(context, @"diraa");
            engine.ExecuteCommand(context, @"-help");
            engine.ExecuteCommand(context, @"dir -h");


            // now manual testing ;-)
            engine.Run(context);
        }
    }
}
