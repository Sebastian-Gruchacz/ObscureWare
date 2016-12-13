namespace Obscureware.DataFlow
{
    using System;
    using System.Threading;
    using Console.Commands.Blocks;
    using Model;

    public interface IBlockFactory
    {
        /// <summary>
        /// Creates new instance of specific <see cref="TBlock"/> type.
        /// </summary>
        /// <typeparam name="TBlock"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        TBlock Create<TBlock>(string id = null)
            where TBlock : BlockBase;

        /// <summary>
        /// Shared instance <see cref="CancellationTokenSource"/> enabling cancellation of all pending flows.
        /// </summary>
        CancellationTokenSource CancellationTokenSource { get; }

        void OverrideCreate<TBlock>(Func<TBlock> factoryFunc, string id = null)
            where TBlock : BlockBase;

        void OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null)
            where TBlock : BlockBase;
    }
}