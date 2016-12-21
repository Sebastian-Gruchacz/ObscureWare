// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleColorsHelper.cs" company="Obscureware Solutions">
// MIT License
//
// Copyright(c) 2015-2016 Sebastian Gruchacz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>
// <summary>
//   Provides routines used to manipulate console colors.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ObscureWare.Console
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class used to manage system's Console colors
    /// </summary>
    public class ConsoleColorsHelper : IDisposable
    {
        private const float COLOR_WEIGHT_HUE = 47.5f;
        private const float COLOR_WEIGHT_SATURATION = 28.75f;
        private const float COLOR_WEIGHT_BRIGHTNESS = 23.75f;

        private const float COLOR_WEIGHT_RED = 28.5f;
        private const float COLOR_WEIGHT_GREEN = 28.5f;
        private const float COLOR_WEIGHT_BLUE = 23.75f;

        // final weight - how color weights over (under?) "luminosity"
        private const float COLOR_PROPORTION = 0.5f; //100f / 255f;

        private readonly IntPtr _hConsoleOutput;

        private readonly ConcurrentDictionary<Color, ConsoleColor> _knownMappings = new ConcurrentDictionary<Color, ConsoleColor>();

        private KeyValuePair<ConsoleColor, Color>[] _colorBuffer;

        /// <summary>
        /// Initializes new instance of ConsoleColorsHelper class
        /// </summary>
        public ConsoleColorsHelper()
        {
            // TODO: second instance created is crashing. Find out why and how to fix it / prevent. In the worst case - hidden control instance singleton and separation of concerns. => good for testing color engine alone.
            this._hConsoleOutput = NativeMethods.GetStdHandle(NativeMethods.STD_OUTPUT_HANDLE); // 7
            if (this._hConsoleOutput == NativeMethods.INVALID_HANDLE)
            {
                throw new SystemException("GetStdHandle->WinError: #" + Marshal.GetLastWin32Error());
            }

            this._colorBuffer = this.GetCurrentColorset();
        }

        /// <summary>
        /// Tries to find the closest match for given RGB color among current set of colors used by System.Console
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        /// <remarks>Influenced by http://stackoverflow.com/questions/1720528/what-is-the-best-algorithm-for-finding-the-closest-color-in-an-array-to-another</remarks>
        public ConsoleColor FindClosestColor(Color color)
        {
            ConsoleColor cc;
            if (this._knownMappings.TryGetValue(color, out cc))
            {
                return cc;
            }

            cc = this._colorBuffer.OrderBy(kp => this.ColorMatching(color, kp.Value)).First().Key;
            this._knownMappings.TryAdd(color, cc);
            return cc;
        }

        private float ColorMatching(Color srcColor, Color destColor)
        {
            var sh = srcColor.GetHue();
            var ss = srcColor.GetSaturation();
            var sb = srcColor.GetBrightness();
            var dh = destColor.GetHue();
            var ds = destColor.GetSaturation();
            var db = destColor.GetBrightness();

            var sr = srcColor.R;
            var sg = srcColor.G;
            var sc = srcColor.B;
            var dr = destColor.R;
            var dg = destColor.G;
            var dc = destColor.B;

            float result = (float) Math.Sqrt(
                Math.Abs(sh - dh)/(COLOR_WEIGHT_HUE)*COLOR_PROPORTION +
                Math.Abs(ss - ds)/(COLOR_WEIGHT_SATURATION)*COLOR_PROPORTION +
                Math.Abs(sb - db)/(COLOR_WEIGHT_BRIGHTNESS)*COLOR_PROPORTION +
                Math.Abs(sr - dr)/(COLOR_WEIGHT_RED)*COLOR_PROPORTION +
                Math.Abs(sg - dg)/(COLOR_WEIGHT_GREEN)*COLOR_PROPORTION +
                Math.Abs(sc - dc)/(COLOR_WEIGHT_BLUE)*COLOR_PROPORTION);

            return result;
        }

        /// <summary>
        /// Replaces default (or previous...) values of console colors with new RGB values.
        /// </summary>
        /// <param name="mappings"></param>
        public void ReplaceConsoleColors(params Tuple<ConsoleColor, Color>[] mappings)
        {
            var csbe = this.GetConsoleScreenBufferInfoEx();

            foreach (var mapping in mappings)
            {
                SetNewColorDefinition(ref csbe, mapping.Item1, mapping.Item2);
            }

            // strange, needs to be done because window is shrunken somehow
            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;

            bool brc = NativeMethods.SetConsoleScreenBufferInfoEx(this._hConsoleOutput, ref csbe);
            if (!brc)
            {
                throw new SystemException("SetConsoleScreenBufferInfoEx->WinError: #" + Marshal.GetLastWin32Error());
            }

            this.ResetColorCache();
        }

        /// <summary>
        /// Replaces default (or previous...) single value of console color with new RGB value.
        /// </summary>
        /// <param name="color">Console named color</param>
        /// <param name="rgbColor">New RGB value to be used under this color name</param>
        public void ReplaceConsoleColor(ConsoleColor color, Color rgbColor)
        {
            var csbe = this.GetConsoleScreenBufferInfoEx();

            SetNewColorDefinition(ref csbe, color, rgbColor);

            // strange, needs to be done because window is shrunken somehow
            ++csbe.srWindow.Bottom;
            ++csbe.srWindow.Right;

            bool brc = NativeMethods.SetConsoleScreenBufferInfoEx(this._hConsoleOutput, ref csbe);
            if (!brc)
            {
                throw new SystemException("SetConsoleScreenBufferInfoEx->WinError: #" + Marshal.GetLastWin32Error());
            }

            this.ResetColorCache();
        }

        private void ResetColorCache()
        {
            // remove cache, new mappings are required
            this._colorBuffer = this.GetCurrentColorset();
            this._knownMappings.Clear();
        }

        private NativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX GetConsoleScreenBufferInfoEx()
        {
            NativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX csbe = new NativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX();
            csbe.cbSize = Marshal.SizeOf(csbe); // 96 = 0x60

            bool brc = NativeMethods.GetConsoleScreenBufferInfoEx(this._hConsoleOutput, ref csbe);
            if (!brc)
            {
                throw new SystemException("GetConsoleScreenBufferInfoEx->WinError: #" + Marshal.GetLastWin32Error());
            }
            return csbe;
        }

        private static void SetNewColorDefinition(ref NativeMethods.CONSOLE_SCREEN_BUFFER_INFO_EX csbe, ConsoleColor color, Color rgbColor)
        {
            // Eh... Ugly code here...

            var r = rgbColor.R;
            var g = rgbColor.G;
            var b = rgbColor.B;

            switch (color)
            {
                case ConsoleColor.Black:
                    csbe.black = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkBlue:
                    csbe.darkBlue = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkGreen:
                    csbe.darkGreen = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkCyan:
                    csbe.darkCyan = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkRed:
                    csbe.darkRed = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkMagenta:
                    csbe.darkMagenta = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkYellow:
                    csbe.darkYellow = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Gray:
                    csbe.gray = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.DarkGray:
                    csbe.darkGray = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Blue:
                    csbe.blue = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Green:
                    csbe.green = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Cyan:
                    csbe.cyan = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Red:
                    csbe.red = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Magenta:
                    csbe.magenta = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.Yellow:
                    csbe.yellow = new NativeMethods.COLORREF(r, g, b);
                    break;
                case ConsoleColor.White:
                    csbe.white = new NativeMethods.COLORREF(r, g, b);
                    break;
            }
        }

        private KeyValuePair<ConsoleColor, Color>[] GetCurrentColorset()
        {
            var csbe = this.GetConsoleScreenBufferInfoEx();

            return new[]
            {
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Black, csbe.black.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkBlue, csbe.darkBlue.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkGreen, csbe.darkGreen.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkCyan, csbe.darkCyan.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkRed, csbe.darkRed.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkMagenta, csbe.darkMagenta.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkYellow, csbe.darkYellow.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Gray, csbe.gray.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.DarkGray, csbe.darkGray.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Blue, csbe.blue.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Green, csbe.green.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Cyan, csbe.cyan.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Red, csbe.red.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Magenta, csbe.magenta.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.Yellow, csbe.yellow.GetColor()),
                new KeyValuePair<ConsoleColor, Color>(ConsoleColor.White, csbe.white.GetColor()),
            };
        }

        #region IDsiposable implementation

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ConsoleColorsHelper()
        {
            // NOTE: Leave out the finalizer altogether if this class doesn't 
            // own unmanaged resources itself, but leave the other methods
            // exactly as they are. 
            this.Dispose(false);
        }

        /// <summary>
        /// Actual disposing method
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources
            if (this._hConsoleOutput != NativeMethods.INVALID_HANDLE)
            {
                NativeMethods.CloseHandle(this._hConsoleOutput);
            }
        }

        #endregion IDsiposable implementation

        /// <summary>
        /// Returns actual ARGB color stored at console enumerated colors.
        /// </summary>
        /// <param name="cc"></param>
        /// <returns></returns>
        public Color GetCurrentConsoleColor(ConsoleColor cc)
        {
            return this._colorBuffer.Single(pair => pair.Key == cc).Value;
        }
    }
}