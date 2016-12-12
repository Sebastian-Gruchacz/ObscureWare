using System.Collections.Generic;

namespace Obscureware.Console.Commands
{
    public interface ICommandOutput
    {
        /// <summary>
        /// Prints to the output given lines using Result Styling
        /// </summary>
        /// <param name="results"></param>
        void PrintResultLines(IEnumerable<string> results);

        void Clear();
    }
}