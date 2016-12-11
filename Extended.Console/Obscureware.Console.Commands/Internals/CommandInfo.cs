namespace Obscureware.Console.Commands.Internals
{
    internal class CommandInfo
    {
        public IConsoleCommand Command { get; private set; }

        public ModelBuilder ModelBuilder { get; private set; }


        public CommandInfo(IConsoleCommand commandInstance, ModelBuilder modelBuilder)
        {
            this.Command = commandInstance;
            this.ModelBuilder = modelBuilder;

            // TODO: store model help printer?

            // TODO: store model parser generated routines / expressions?

            // Probably ModelInfo must be more complicated than just simple type, perhaps dedicated builder class?
        }
    }
}