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
            System.Console.OutputEncoding = Encoding.Unicode;
            System.Console.InputEncoding = Encoding.Unicode;

            _consoleColorsHelper = helper ?? new ConsoleColorsHelper();

            if (isFullScreen)
            {
                this.SetConsoleWindowToFullScreen();

                // now can calculate how large could be full-screen buffer

                // SG: (-2) On Win8 the only working way to keep borders on the screen :(
                // (-1) required on Win10 though :(
                this.WindowSize = new Point(System.Console.LargestWindowWidth - 2, System.Console.LargestWindowHeight - 1);

                // setting full-screen
                System.Console.BufferWidth = WindowSize.X;
                System.Console.WindowWidth = WindowSize.X;
                System.Console.BufferHeight = WindowSize.Y;
                System.Console.WindowHeight = WindowSize.Y;
                System.Console.SetWindowPosition(0, 0);
            }

            this.WindowWidth = System.Console.WindowWidth;
            this.WindowHeight = System.Console.WindowHeight;
        }

        private void SetConsoleWindowToFullScreen()
        {
            // http://www.codeproject.com/Articles/4426/Console-Enhancements
            SetWindowPosition(
                0,
                0,
                Screen.PrimaryScreen.WorkingArea.Width - (2 * 16),
                Screen.PrimaryScreen.WorkingArea.Height - (2 * 16) - SystemInformation.CaptionHeight);
        }

        public void WriteText(int x, int y, string text, Color foreColor, Color bgColor)
        {
            PositionCursor(x, y);
            SetColors(foreColor, bgColor);
            WriteText(text);
        }

        public void WriteText(string text)
        {
            System.Console.Write(text);
        }

        public void SetColors(Color foreColor, Color bgColor)
        {
            System.Console.ForegroundColor = _consoleColorsHelper.FindClosestColor(foreColor);
            System.Console.BackgroundColor = _consoleColorsHelper.FindClosestColor(bgColor);
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public void PositionCursor(int x, int y)
        {
            System.Console.SetCursorPosition(x, y);
        }

        public void WriteText(char character)
            {
            System.Console.Write(character);
        }

        public int WindowHeight { get; }
        public int WindowWidth { get; }

        public void ReplaceConsoleColor(ConsoleColor color, Color rgbColor)
        {
            _consoleColorsHelper.ReplaceConsoleColor(color, rgbColor);
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