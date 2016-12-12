﻿namespace Obscureware.Console.Commands.Model
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CommandValueFlagAttribute : Attribute
    {
        /// <summary>
        /// Gets expected value type for the flag. Target property MUST match desired type.
        /// </summary>
        public ValueFlagType ValueFlagType { get; private set; }

        /// <summary>
        /// Gets strings / letters that will enable this flag.
        /// </summary>
        public string[] CommandLiterals { get; private set; }

        public CommandValueFlagAttribute(ValueFlagType valueType, params string[] commandLiterals)
        {
            this.ValueFlagType = valueType;
            this.CommandLiterals = commandLiterals;
        }
    }
}