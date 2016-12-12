namespace Obscureware.Console.Commands.Blocks
{
    using System;

    using NLog;

    public class BlockLogger : IBlockLogger
    {
        private readonly Logger _currentBlockLogger;

        public BlockLogger(Logger currentBlockLogger)
        {
            this._currentBlockLogger = currentBlockLogger;
        }

        public void Trace(FlowToken token, string formatMessage, params object[] arguments)
        {
            this._currentBlockLogger.Trace(string.Concat(token.TokenId, " ", formatMessage), arguments);
        }

        public void Error(FlowToken token, string formatMessage, params object[] arguments)
        {
            this._currentBlockLogger.Error(string.Concat(token.TokenId, " ", formatMessage), arguments);
        }

        public void Error(FlowToken token, string message)
        {
            this._currentBlockLogger.Error(string.Concat(token.TokenId, " ", message));
        }

        public void Error(FlowToken token, Exception exception)
        {
            this._currentBlockLogger.Error(token.TokenId.ToString(), exception);
        }

        public void Error(FlowToken token, Exception exception, string message)
        {
            this._currentBlockLogger.Error(string.Concat(token.TokenId, " ", message), exception);
        }

        public void Warn(FlowToken token, string formatMessage, params object[] arguments)
        {
            this._currentBlockLogger.Warn(string.Concat(token.TokenId, " ", formatMessage), arguments);
        }
    }
}