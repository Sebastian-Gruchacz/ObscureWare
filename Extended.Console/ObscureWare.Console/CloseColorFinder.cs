// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CloseColorFinder.cs" company="Obscureware Solutions">
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
//   Defines the CloseColorFinder class responsible for color matching routines.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ObscureWare.Console
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    /// <summary>
    /// Class responsible for finding closest color index from given console colors array.
    /// </summary>
    public class CloseColorFinder
    {
        private const float COLOR_WEIGHT_HUE = 47.5f;

        private const float COLOR_WEIGHT_SATURATION = 28.75f;

        private const float COLOR_WEIGHT_BRIGHTNESS = 23.75f;

        private const float COLOR_WEIGHT_RED = 28.5f;

        private const float COLOR_WEIGHT_GREEN = 28.5f;

        private const float COLOR_WEIGHT_BLUE = 23.75f;

        // final weight - how color weights over (under?) "luminosity"
        private const float COLOR_PROPORTION = 0.5f; //100f / 255f;

        private readonly ConcurrentDictionary<Color, ConsoleColor> _knownMappings = new ConcurrentDictionary<Color, ConsoleColor>();

        private readonly KeyValuePair<ConsoleColor, Color>[] _colorBuffer;

        public CloseColorFinder(KeyValuePair<ConsoleColor, Color>[] colorBuffer)
        {
            this._colorBuffer = colorBuffer;
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

        /// <summary>
        /// Returns actual ARGB color stored at console enumerated colors.
        /// </summary>
        /// <param name="cc">Enumeration-index in console colors</param>
        /// <returns>ARGB color.</returns>
        public Color GetCurrentConsoleColor(ConsoleColor cc)
        {
            return this._colorBuffer.Single(pair => pair.Key == cc).Value;
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

            float result = (float)Math.Sqrt(
                (Math.Abs(sh - dh) / COLOR_WEIGHT_HUE * COLOR_PROPORTION) +
                (Math.Abs(ss - ds) / COLOR_WEIGHT_SATURATION * COLOR_PROPORTION) +
                (Math.Abs(sb - db) / COLOR_WEIGHT_BRIGHTNESS * COLOR_PROPORTION) +
                (Math.Abs(sr - dr) / COLOR_WEIGHT_RED * COLOR_PROPORTION) +
                (Math.Abs(sg - dg) / COLOR_WEIGHT_GREEN * COLOR_PROPORTION) +
                (Math.Abs(sc - dc) / COLOR_WEIGHT_BLUE * COLOR_PROPORTION));

            return result;
        }
    }
}