namespace Obscureware.Console.Commands
{
    public interface ICommandEngineContext
    {
        /// <summary>
        /// Returns true, when context indicates that application shall finish execution - i.e. stop prompting user for more commands
        /// </summary>
        bool ShallTerminate { get; set; }

        /// <summary>
        /// Shall produce text, that will be displayed to the user as a prompt.
        /// </summary>
        /// <returns></returns>
        string GetCurrentPrompt();
    }
}