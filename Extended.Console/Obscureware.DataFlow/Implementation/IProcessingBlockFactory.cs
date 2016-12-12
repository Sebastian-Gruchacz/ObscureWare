namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Threading;

    public interface IProcessingBlockFactory
    {
        TBlock Create<TBlock>(string id = null) where TBlock : ProcessingBlockBase;

        CancellationTokenSource CancellationTokenSource { get; }

        void OverrideCreate<TBlock>(Func<TBlock> factoryFunc, string id = null)
            where TBlock : ProcessingBlockBase;

        void OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null)
            where TBlock : ProcessingBlockBase;
    }
}