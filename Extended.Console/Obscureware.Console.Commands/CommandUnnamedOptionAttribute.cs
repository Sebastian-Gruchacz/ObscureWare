using System;

namespace Obscureware.Console.Commands
{
    public class CommandUnnamedOptionAttribute : Attribute
    {
        public int ArgumentIndex { get; set; }

        public CommandUnnamedOptionAttribute(int argumentIndex)
        {
            ArgumentIndex = argumentIndex;
        }
    }
}