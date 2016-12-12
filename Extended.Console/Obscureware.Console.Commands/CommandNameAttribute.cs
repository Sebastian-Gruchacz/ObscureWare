using System;

namespace Obscureware.Console.Commands
{
    /// <summary>
    /// Specifies command name
    /// </summary>
    public class CommandNameAttribute : Attribute
    {
        public string CommandName { get; private set; }

        public CommandNameAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }
}