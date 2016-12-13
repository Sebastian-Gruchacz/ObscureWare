namespace Obscureware.DataFlow.Implementation
{
    using System;

    /// <summary>
    /// Used when receiving for processing token that have (been) already terminated
    /// </summary>
    public class TerminatedTokenReceivedEventArgs : EventArgs
    {
        public TerminatedTokenReceivedEventArgs(DataFlowToken terminatedToken)
        {
            this.TerminatedToken = terminatedToken;
        }

        /// <summary>
        /// Token instance.
        /// </summary>
        public DataFlowToken TerminatedToken { get; private set; }
    }
}