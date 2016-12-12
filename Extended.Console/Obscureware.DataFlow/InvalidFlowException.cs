namespace Obscureware.DataFlow
{
    using System;

    public class InvalidFlowException : Exception
    {
        public InvalidFlowException(string message) : base(message)
        {
        }
    }
}