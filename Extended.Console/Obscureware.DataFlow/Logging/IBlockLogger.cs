namespace Obscureware.Console.Commands.Blocks
{
    using System;

    /// <summary>
    /// Prepends all messages with Unique Id from FlowToken
    /// </summary>
    public interface IBlockLogger
    {
        void Trace(FlowToken token, string formatMessage, params object[] arguments);
        void Warn(FlowToken token, string formatMessage, params object[] arguments);
        void Error(FlowToken token, string formatMessage, params object[] arguments);
        void Error(FlowToken token, Exception exception);
        void Error(FlowToken token, Exception exception, string message);
    }
}