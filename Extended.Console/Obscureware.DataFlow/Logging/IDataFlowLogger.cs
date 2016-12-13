
namespace Obscureware.DataFlow.Logging
{
    using System;
    using Implementation;
    using Console.Commands.Blocks;
    using JetBrains.Annotations;

    /// <summary>
    /// Prepends all messages with Unique Id from DataFlowToken
    /// </summary>
    public interface IDataFlowLogger
    {
        /// <summary>
        /// Writes trace message from Data Flow
        /// </summary>
        /// <param name="token"></param>
        /// <param name="formatter"></param>
        /// <param name="arguments"></param>
        void Trace([NotNull]DataFlowToken token, [NotNull]string formatter, params object[] arguments);

        /// <summary>
        /// Writes warning message from Data Flow
        /// </summary>
        /// <param name="token"></param>
        /// <param name="formatter"></param>
        /// <param name="arguments"></param>
        void Warn([NotNull]DataFlowToken token, [NotNull]string formatter, params object[] arguments);

        /// <summary>
        /// Writes error message from Data Flow
        /// </summary>
        /// <param name="token"></param>
        /// <param name="formatter"></param>
        /// <param name="arguments"></param>
        void Error([NotNull]DataFlowToken token, [NotNull]string formatter, params object[] arguments);

        /// <summary>
        /// Writes error message from Data Flow
        /// </summary>
        /// <param name="token"></param>
        /// <param name="exception"></param>
        void Error([NotNull]DataFlowToken token, [NotNull]Exception exception);

        /// <summary>
        /// Writes error message from Data Flow
        /// </summary>
        /// <param name="token"></param>
        /// <param name="exception"></param>
        /// <param name="message"></param>
        void Error([NotNull]DataFlowToken token, [NotNull]Exception exception, [NotNull]string message);
    }
}