namespace Obscureware.DataFlow.Logging
{
    using System;
    using Console.Commands.Blocks;
    using Implementation;
    using NLog;

    /// <summary>
    /// Implements <see cref="IDataFlowLogger"/> using NLog <see cref="Logger"/> implementation. Plain wrapper type.
    /// </summary>
    public class DataFlowNLogger : IDataFlowLogger
    {
        private readonly Logger _currentBlockLogger;

        public DataFlowNLogger(Logger currentBlockLogger)
        {
            this._currentBlockLogger = currentBlockLogger;
        }

        /// <inheritdoc cref="IDataFlowLogger"/>
        public void Trace(DataFlowToken token, string formatter, params object[] arguments)
        {
            this._currentBlockLogger.Trace(string.Concat(token.TokenId, " ", formatter), arguments);
        }

        /// <inheritdoc cref="IDataFlowLogger"/>
        public void Error(DataFlowToken token, string formatter, params object[] arguments)
        {
            this._currentBlockLogger.Error(string.Concat(token.TokenId, " ", formatter), arguments);
        }

        /// <inheritdoc cref="IDataFlowLogger"/>
        public void Error(DataFlowToken token, string message)
        {
            this._currentBlockLogger.Error(string.Concat(token.TokenId, " ", message));
        }

        /// <inheritdoc cref="IDataFlowLogger"/>
        public void Error(DataFlowToken token, Exception exception)
        {
            this._currentBlockLogger.Error(exception, token.TokenId.ToString());
        }

        /// <inheritdoc cref="IDataFlowLogger"/>
        public void Error(DataFlowToken token, Exception exception, string message)
        {
            this._currentBlockLogger.Error(exception, string.Concat(token.TokenId, " ", message));
        }

        /// <inheritdoc cref="IDataFlowLogger"/>
        public void Warn(DataFlowToken token, string formatter, params object[] arguments)
        {
            this._currentBlockLogger.Warn(string.Concat(token.TokenId, " ", formatter), arguments);
        }
    }
}