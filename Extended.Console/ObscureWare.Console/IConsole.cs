namespace ObscureWare.Console
{
    using System.Drawing;

    /// <summary>
    /// Encapsulates some typical console operations with interface. Expected usages - simple system console, with low resolution and limited colors or graphical console.
    /// </summary>
    /// <remarks>Deliberately removed all formatting overloads that System.Console has. All of this can be done before call.</remarks>
    public interface IConsole
    {
        /// <summary>
        /// Writes specific text at given position, and using given colors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="foreColor"></param>
        /// <param name="bgColor"></param>
        void WriteText(int x, int y, string text, Color foreColor, Color bgColor);

        /// <summary>
        /// Clears entire visible console area (window)
        /// </summary>
        void Clear();

        /// <summary>
        /// Writes given text at current cursor position using given colors
        /// </summary>
        /// <param name="text"></param>
        void WriteText(ConsoleFontColor colors, string text);

        /// <summary>
        /// Writes given text at current cursor position using most recent colors
        /// </summary>
        /// <param name="text"></param>
        void WriteText(string text);

        /// <summary>
        /// Writes given line of text at current cursor position using given colors
        /// </summary>
        /// <param name="text"></param>
        void WriteLine(ConsoleFontColor colors, string text);

        /// <summary>
        /// Writes given text at current cursor position using most recent colors
        /// </summary>
        /// <param name="text"></param>
        void WriteLine(string text);

        /// <summary>
        /// Sets pair of colors to be used by following <see cref="WriteText*"/> calls
        /// </summary>
        /// <param name="foreColor"></param>
        /// <param name="bgColor"></param>
        void SetColors(Color foreColor, Color bgColor);

        /// <summary>
        /// Positions cursor at specific position ON THE SCREEN, not in the buffer.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetCursorPosition(int x, int y);

        /// <summary>
        /// Writes single character at current cursor position using most recent colors
        /// </summary>
        /// <param name="character"></param>
        void WriteText(char character);

        // TODO: read operations

        // TODO: async rerad-write operations

        /// <summary>
        /// Gets vertical size of console Window
        /// </summary>
        int WindowHeight { get; }

        /// <summary>
        /// Gets horizontal size of console Window
        /// </summary>
        int WindowWidth { get; }

        /// <summary>
        /// Reads one text line synchronously.
        /// </summary>
        /// <returns></returns>
        string ReadLine();
        void WriteLine();

        void SetColors(ConsoleFontColor style);
    }
}
