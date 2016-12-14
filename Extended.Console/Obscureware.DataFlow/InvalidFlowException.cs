namespace Obscureware.DataFlow
{
    using System;

    [Serializable]
    public class InvalidFlowException : Exception
    {
        public InvalidFlowException(string message) : base(message)
        {
        }
    }
}