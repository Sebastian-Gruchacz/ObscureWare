namespace Obscureware.Console.Commands.Model
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CommandValueFlagAttribute : Attribute
    {
        /// <summary>
        /// Gets strings / letters that will enable this flag.
        /// </summary>
        public string[] CommandLiterals { get; private set; }

        public CommandValueFlagAttribute(params string[] commandLiterals)
        {
            this.CommandLiterals = commandLiterals;
        }
    }
}