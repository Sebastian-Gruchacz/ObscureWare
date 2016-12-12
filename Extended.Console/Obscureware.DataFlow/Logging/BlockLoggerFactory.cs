namespace Obscureware.Console.Commands.Blocks
{
    using System;

    using NLog;

    public static class BlockLoggerFactory
    {
        public static BlockLogger Create(Type blockType)
        {
            return new BlockLogger(LogManager.GetLogger(blockType.FullName));
        }
    }
}