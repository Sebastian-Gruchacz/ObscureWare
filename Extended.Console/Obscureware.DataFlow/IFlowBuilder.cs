namespace Obscureware.DataFlow
{
    using System;
    using System.Linq.Expressions;
    using Console.Commands.Blocks;
    using Model;

    public interface IFlowBuilder
    {
        IFlow BuildFlow();
        IFlowBuilder Link(string sourceId);
        IFlowBuilder Link<TBlockSource>() where TBlockSource : BlockBase;
        IFlowBuilder Link<TBlockSource, TBlockSource2>()
            where TBlockSource : BlockBase
            where TBlockSource2 : BlockBase;
        IFlowBuilder Link<TBlockSource, TBlockSource2, TBlockSource3>()
            where TBlockSource : BlockBase
            where TBlockSource2 : BlockBase
            where TBlockSource3 : BlockBase;

        IFlowBuilder To(string targetId);
        IFlowBuilder To<TBlockTarget>()
            where TBlockTarget : BlockBase;
        IFlowBuilder To<TBlockTarget, TBlockTarget2>()
            where TBlockTarget : BlockBase
            where TBlockTarget2 : BlockBase;
        IFlowBuilder When<TTuple>(Expression<Predicate<TTuple>> condition);

        IFlowBuilder OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null) where TBlock : BlockBase;
        IFlowBuilder OverrideCreate<TBlock>(Func<TBlock> factoryFunction, string id = null) where TBlock : BlockBase;
    }
}