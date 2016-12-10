namespace Obscureware.Console.Commands
{
    public abstract class CommandModel
    {
        protected CommandModel(string[] rawParameters)
        {
            RawParameters = rawParameters;
        }

        public string[] RawParameters { get; }

        // TODO: provide extra detailed help, syntaxes, samples, etc...
    }
}