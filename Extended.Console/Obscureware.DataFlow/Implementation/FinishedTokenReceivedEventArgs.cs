namespace Obscureware.Console.Commands.Blocks
{
    using System;

    public class FinishedTokenReceivedEventArgs : EventArgs
    {
        public FinishedTokenReceivedEventArgs(FlowToken token)
        {
            this.Token = token;
        }

        public FlowToken Token { get; private set; }
    }
}