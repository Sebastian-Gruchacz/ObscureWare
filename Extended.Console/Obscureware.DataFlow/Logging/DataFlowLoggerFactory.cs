namespace Obscureware.DataFlow.Logging
{
    using System;
    using NLog;

    /// <summary>
    /// Constructs logger for Data-Flow execution.
    /// </summary>
    public static class DataFlowLoggerFactory
    {
        public static IDataFlowLogger Create(Type blockType)
        {
            // TODO: use configuration if possible
            return new DataFlowNLogger(LogManager.GetLogger(blockType.FullName));
        }
    }
}