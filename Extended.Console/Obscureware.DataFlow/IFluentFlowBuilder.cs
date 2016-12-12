namespace Obscureware.Console.Commands.Blocks
{
    using System;
    using System.Linq.Expressions;

    public interface IFluentFlowBuilder
    {
        IFluentFlow BuildFlow();
        IFluentFlowBuilder Link(string sourceId);
        IFluentFlowBuilder Link<TBlockSource>() where TBlockSource : ProcessingBlockBase;
        IFluentFlowBuilder Link<TBlockSource, TBlockSource2>()
            where TBlockSource : ProcessingBlockBase
            where TBlockSource2 : ProcessingBlockBase;
        IFluentFlowBuilder Link<TBlockSource, TBlockSource2, TBlockSource3>()
            where TBlockSource : ProcessingBlockBase
            where TBlockSource2 : ProcessingBlockBase
            where TBlockSource3 : ProcessingBlockBase;

        IFluentFlowBuilder To(string targetId);
        IFluentFlowBuilder To<TBlockTarget>()
            where TBlockTarget : ProcessingBlockBase;
        IFluentFlowBuilder To<TBlockTarget, TBlockTarget2>()
            where TBlockTarget : ProcessingBlockBase
            where TBlockTarget2 : ProcessingBlockBase;
        IFluentFlowBuilder When<TTuple>(Expression<Predicate<TTuple>> condition);

        IFluentFlowBuilder OverrideCreate<TBlock>(ProcessingBlockOptions options, string id = null) where TBlock : ProcessingBlockBase;
        IFluentFlowBuilder OverrideCreate<TBlock>(Func<TBlock> factoryFunction, string id = null) where TBlock : ProcessingBlockBase;
    }
}