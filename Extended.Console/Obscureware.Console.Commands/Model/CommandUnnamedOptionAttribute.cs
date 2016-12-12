namespace Obscureware.Console.Commands.Model
{
    using System;

    public class CommandUnnamedOptionAttribute : Attribute
    {
        public int ArgumentIndex { get; set; }

        public CommandUnnamedOptionAttribute(int argumentIndex)
        {
            this.ArgumentIndex = argumentIndex;
        }
    }
}