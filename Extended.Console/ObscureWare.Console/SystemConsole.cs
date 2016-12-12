namespace ObscureWare.Console
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    /// <summary>
    /// Wraps System.Console with IConsole interface methods
    /// </summary>
    public class SystemConsole : IConsole
    {
        private readonly ConsoleColorsHelper _consoleColorsHelper;

        /// <summary>
        /// In characters...
        /// </summary>
        public Point WindowSize { get; }

        public SystemConsole(ConsoleColorsHelper helper, bool isFullScreen)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            this._consoleColorsHelper = helper ?? new ConsoleColorsHelper();

            if (isFullScreen)
            {
                this.SetConsoleWindowToFullScreen();

                // now can calculate how large could be full-screen buffer

                // SG: (-2) On Win8 the only working way to keep borders on the screen :(
                // (-1) required on Win10 though :(
                this.WindowSize = new Point(Console.LargestWindowWidth - 2, Console.LargestWindowHeight - 1);

                // setting full-screen
                Console.BufferWidth = this.WindowSize.X;
                Console.WindowWidth = this.WindowSize.X;
                Console.BufferHeight = this.WindowSize.Y;
                Console.WindowHeight = this.WindowSize.Y;
                Console.SetWindowPosition(0, 0);
            }

            this.WindowWidth = Console.WindowWidth;
            this.WindowHeight = Console.WindowHeight;
        }

        private void SetConsoleWindowToFullScreen()
        {
            // http://www.codeproject.com/Articles/4426/Console-Enhancements
            this.SetWindowPosition(
                0,
                0,
                Screen.PrimaryScreen.WorkingArea.Width - (2 * 16),
                Screen.PrimaryScreen.WorkingArea.Height - (2 * 16) - SystemInformation.CaptionHeight);
        }

        public void WriteText(int x, int y, string text, Color foreColor, Color bgColor)
        {
            this.SetCursorPosition(x, y);
            this.SetColors(foreColor, bgColor);
            this.WriteText(text);
        }

        private void WriteText(string text)
        {
            Console.Write(text);
        }

        void IConsole.WriteText(string text)
        {
            this.WriteText(text);
        }

        public void WriteLine(ConsoleFontColor colors, string text)
        {
            this.SetColors(colors.ForeColor, colors.BgColor);
            Console.WriteLine(text);
        }

        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }

        public void SetColors(Color foreColor, Color bgColor)
        {
            Console.ForegroundColor = this._consoleColorsHelper.FindClosestColor(foreColor);
            Console.BackgroundColor = this._consoleColorsHelper.FindClosestColor(bgColor);
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void WriteText(ConsoleFontColor colors, string text)
        {
            this.SetColors(colors.ForeColor, colors.BgColor);
            Console.Write(text);
        }

        public void SetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(x, y);
        }

        public Point GetCursorPosition()
        {
            return new Point(Console.CursorLeft, Console.CursorTop);
        }

        public void WriteText(char character)
        {
            Console.Write(character);
        }

        public int WindowHeight { get; }

        public int WindowWidth { get; }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void SetColors(ConsoleFontColor style)
        {
            this.SetColors(style.ForeColor, style.BgColor);
        }

        public void ReplaceConsoleColor(ConsoleColor color, Color rgbColor)
        {
            this._consoleColorsHelper.ReplaceConsoleColor(color, rgbColor);
        }

        #region PInvoke

        [DllImport("user32")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, int flags);

        [DllImport("kernel32")]
        private static extern IntPtr GetConsoleWindow();

        private const int SWP_NOZORDER = 0x4;
        private const int SWP_NOACTIVATE = 0x10;

        /// <summary>
        /// Sets the console window location and size in pixels
        /// </summary>
        public void SetWindowPosition(int x, int y, int width, int height)
        {
            IntPtr hwnd = GetConsoleWindow();
            SetWindowPos(hwnd, IntPtr.Zero, x, y, width, height, SWP_NOZORDER | SWP_NOACTIVATE);
            // no release handle?
        }

        #endregion PInvoke
    }
}