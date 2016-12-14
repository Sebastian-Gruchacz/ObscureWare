namespace ConsoleApplication1
{
    using System;

    using Obscureware.Console.Commands;
    public class ConsoleContext : ICommandEngineContext
    {
        public bool ShallTerminate { get; set; }

        public string GetCurrentPrompt()
        {
            return $"{Environment.CurrentDirectory}\\";
        }
    }
}