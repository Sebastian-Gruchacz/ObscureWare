namespace Obscureware.Console.Commands.Blocks
{
    using System;

    public class InvalidFlowException : Exception
    {
        public InvalidFlowException(string message) : base(message)
        {
        }
    }
}