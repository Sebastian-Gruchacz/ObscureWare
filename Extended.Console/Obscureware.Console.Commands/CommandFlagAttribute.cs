using System;

namespace Obscureware.Console.Commands
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CommandFlagAttribute : Attribute
    {
        /// <summary>
        /// Gets whether flag is set to TRUE by default and when user specifies it - causes it to be disabled.
        /// </summary>
        public bool IsEnabledByDefault { get; private set; }

        /// <summary>
        /// Gets strings / letters that will enable this flag.
        /// </summary>
        public string[] CommandLiterals { get; private set; }

        public CommandFlagAttribute(bool isEnabledByDefault = false, params string[] commandLiterals)
        {
            IsEnabledByDefault = isEnabledByDefault;
            CommandLiterals = commandLiterals;
        }
    }
}
