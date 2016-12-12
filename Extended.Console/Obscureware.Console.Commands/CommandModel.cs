namespace Obscureware.Console.Commands
{
    public abstract class CommandModel
    {
        public string[] RawParameters { get; internal set; }
    }
}