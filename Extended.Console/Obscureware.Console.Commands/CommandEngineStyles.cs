using System.Drawing;
using ObscureWare.Console;

namespace Obscureware.Console.Commands
{
    /// <summary>
    /// Contains various color scheme settings used by CommandEngine
    /// </summary>
    public class CommandEngineStyles
    {
        public static CommandEngineStyles DefaultStyles
        {
            get
            {
                return new CommandEngineStyles
                {
                    Default = new ConsoleFontColor(Color.DarkGray, Color.Black),
                    Error = new ConsoleFontColor(Color.DarkRed, Color.Black),
                    Warning = new ConsoleFontColor(Color.Orange, Color.Black),

                    HelpHeader = new ConsoleFontColor(Color.Black, Color.DarkGray),
                    HelpBody = new ConsoleFontColor(Color.LightGray, Color.Black),
                    HelpDefinition = new ConsoleFontColor(Color.White, Color.Black),
                    HelpDescription = new ConsoleFontColor(Color.LightGray, Color.Black),
                    HelpSyntax = new ConsoleFontColor(Color.White, Color.DarkBlue),
                    Prompt = new ConsoleFontColor(Color.Yellow, Color.DarkBlue)
                };
            }
        }

        // TODO: split this class into specialized classes / interfaces (help styles, messaging styles, table styles etc.)

        public ConsoleFontColor HelpSyntax { get; set; }

        public ConsoleFontColor HelpDescription { get; set; }

        public ConsoleFontColor HelpDefinition { get; set; }

        public ConsoleFontColor HelpBody { get; set; }

        public ConsoleFontColor HelpHeader { get; set; }

        public ConsoleFontColor Warning { get; set; }

        public ConsoleFontColor Error { get; set; }

        public ConsoleFontColor Default { get; set; }

        public ConsoleFontColor Prompt { get;  set; }
    }
}