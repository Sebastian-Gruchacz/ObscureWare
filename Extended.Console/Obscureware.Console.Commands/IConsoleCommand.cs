namespace Obscureware.Console.Commands
{
    public interface IConsoleCommand
    {
        void Execute(object contextObject, ICommandOutput output, object runtimeModel); // TODO: add output (and input for interactive!) manipulators

        // TODO: this is static command. also required are interactive one and random-printing one (asynchronous partial operations...)
    }
}