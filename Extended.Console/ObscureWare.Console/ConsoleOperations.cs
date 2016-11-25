using System.Runtime.CompilerServices;
using System.Text;

namespace ObscureWare.Console
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;

    public class ConsoleOperations
    {
        private readonly IConsole _console;

        public ConsoleOperations(IConsole console)
        {
            _console = console;
        }

        public bool WriteTextBox(Rectangle textArea, string text, FrameStyle frameDef)
        {
            int boxWidth = textArea.Width;
            int boxHeight = textArea.Height;
            LimitBoxDimensions(textArea.X, textArea.Y, ref boxWidth, ref boxHeight);
            Debug.Assert(boxWidth >= 3);
            Debug.Assert(boxHeight >= 3);
            WriteTextBoxFrame(textArea.X, textArea.Y, boxWidth, boxHeight, frameDef);
            return WriteTextBox(textArea.X + 1, textArea.Y + 1, boxWidth - 2, boxHeight - 2, text, frameDef.TextColor);
        }

        private void WriteTextBoxFrame(int boxX, int boxY, int boxWidth, int boxHeight, FrameStyle frameDef)
        {
            _console.SetColors(frameDef.FrameColor.ForeColor, frameDef.FrameColor.BgColor);
            _console.PositionCursor(boxX, boxY);
            _console.WriteText(frameDef.TopLeft);
            for (int i = 1; i < boxWidth - 1; i++)
            {
                _console.WriteText(frameDef.Top);
            }
            _console.WriteText(frameDef.TopRight);
            string body = frameDef.Left + new string(frameDef.BackgroundFiller, boxWidth - 2) + frameDef.Right;
            for (int j = 1; j < boxHeight - 1; j++)
            {
                _console.PositionCursor(boxX, boxY + j);
                _console.WriteText(body);
            }
            _console.PositionCursor(boxX, boxY + boxHeight - 1);
            _console.WriteText(frameDef.BottomLeft);
            for (int i = 1; i < boxWidth - 1; i++)
            {
                _console.WriteText(frameDef.Bottom);
            }
            _console.WriteText(frameDef.BottomRight);
        }

        public bool WriteTextBox(Rectangle textArea, string text, ConsoleFontColor colorDef)
        {
            return WriteTextBox(textArea.X, textArea.Y, textArea.Width, textArea.Height, text, colorDef);
        }

        public bool WriteTextBox(int x, int y, int boxWidth, int boxHeight, string text, ConsoleFontColor colorDef)
        {
            this.LimitBoxDimensions(x, y, ref boxWidth, ref boxHeight); // so do not have to check for this every line is drawn...
            _console.PositionCursor(x, y);
            _console.SetColors(colorDef.ForeColor, colorDef.BgColor);

            string[] lines = SplitText(text, boxWidth);
            int i;
            for (i = 0; i < lines.Length && i < boxHeight; ++i)
            {
                _console.PositionCursor(x, y + i);
                WriteJustified(lines[i], boxWidth);
            }

            return i == lines.Length;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="maxTableWidth">0 - to limit by windows size</param>
        /// <param name="headers"></param>
        /// <param name="values"></param>
        /// <param name="style"></param>
        public void WriteTabelaricData(int x, int y, int maxTableWidth, string[] headers, IEnumerable<string[]> values,
            TableStyle style)
        {
            this.LimitTableDimensions(x, ref maxTableWidth); // so do not have to check for this every line is drawn...
            _console.PositionCursor(x, y);

            // table calculations - fitting content


            // display header

            // start writing rows... Perhaps try increasing buffer
            TabelericDisplay(headers, values.ToArray());

            //throw new NotImplementedException("later...");
        }



        private void WriteJustified(string text, int boxWidth)
        {
            if (text.Length == boxWidth)
            {
                System.Console.Write(text);
            }
            else
            {
                string[] parts = text.Split(new string[] {@" ", @"\t"}, StringSplitOptions.RemoveEmptyEntries); // both split and clean
                if (parts.Length == 1)
                {
                    System.Console.Write(text); // we cannot do anything about one long word...
                }
                else
                {
                    int cleanedLength = parts.Select(s => s.Length).Sum() + parts.Length - 1;
                    int remainingBlanks = boxWidth - cleanedLength;
                    if (remainingBlanks > cleanedLength/2)
                    {
                        System.Console.Write(text); // text is way too short to expandf it, keep to th eleft
                    }
                    else
                    {
                        int longerSpacesCount = (int) Math.Floor((decimal) remainingBlanks/(parts.Length - 1));
                        if (longerSpacesCount > 1)
                        {
                            decimal remainingLowerSpacesJoins = remainingBlanks - (longerSpacesCount*(parts.Length - 1));
                            if (remainingLowerSpacesJoins > 0)
                            {
                                int longerQty = parts.Length - longerSpacesCount;
                                System.Console.Write(
                                    string.Join(new string(' ', longerSpacesCount), parts.Take(longerQty + 1)) +
                                    string.Join(new string(' ', longerSpacesCount - 1), parts.Skip(longerQty + 1)));
                            }
                            else
                            {
                                // all gaps equal
                                System.Console.Write(string.Join(new string(' ', longerSpacesCount), parts));
                            }
                        }
                        else
                        {
                            System.Console.Write(
                                string.Join(new string(' ', 2), parts.Take(remainingBlanks + 1)) +
                                string.Join(new string(' ', 1), parts.Skip(remainingBlanks + 1)));
                        }
                    }
                }
            }
        }

        private string[] SplitText(string text, int boxWidth)
        {
            // TODO: move it to some external toolset?
            // used this imperfect solution for now: http://stackoverflow.com/a/1678162
            // this will not work properly for long words
            // this is not able to properly break the words in the middle to optimize space...

            int offset = 0;
            var lines = new List<string>();
            while (offset < text.Length)
            {
                int index = text.LastIndexOf(" ", Math.Min(text.Length, offset + boxWidth));
                string line = text.Substring(offset, (index - offset <= 0 ? text.Length : index) - offset);
                offset += line.Length + 1;
                lines.Add(line);
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Limits box dimensions to actual window sizes (avoid overlapping AND exceptions...)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void LimitBoxDimensions(int x, int y, ref int width, ref int height)
        {
            if (x + width > _console.WindowWidth)
            {
                width = _console.WindowWidth - x;
            }
            if (y + height > _console.WindowHeight)
            {
                height = _console.WindowHeight - y;
            }
        }

        private void LimitTableDimensions(int x, ref int maxTableWidth)
        {
            if (maxTableWidth == 0 || x + maxTableWidth > _console.WindowWidth)
            {
                maxTableWidth = _console.WindowWidth - x;
            }
        }



        protected void TabelericDisplay(string[] header, string[][] values)
        {
            var columns = BuildDisplayColumnInfo(header, values).ToArray();
            PrintTable(columns, values, rebuildColumnSizes: false);
            //if (realValues.Any())
            //{
            //    sb.AppendLine("While actual values are:");
            //    PrintTable(sb, columns, realValues, rebuildColumnSizes: false);
            //}
            //else
            //{
            //    sb.AppendLine("While there are no actual values...");
            //}

            //var s = sb.ToString();
            ////Debug.WriteLine(s);
            //return s;
        }

        private static IEnumerable<TextColumnInfo> BuildDisplayColumnInfo(string[] headerValues, string[][] values)
        {
            TextColumnInfo[] textColumns = new TextColumnInfo[headerValues.Length];
            int index = 0;
            foreach (var header in headerValues)
            {
                textColumns[index] = new TextColumnInfo(header, values.Select(row => row[index]).ToArray());
                index++;
            }

            return textColumns;
        }

        private void PrintTable(TextColumnInfo[] textColumns, string[][] rows,
            bool rebuildColumnSizes = true)
        {
            if (rebuildColumnSizes)
            {
                for (int i = 0; i < textColumns.Length; ++i )
                {
                    var columnInfo = textColumns[i];
                    columnInfo.UpdateWithNewValues(rows.Select(row => row[i]).ToArray());
                }
            }

            _console.WriteText('|');

            //.AppendLine("\t| " + string.Join(" | ", textColumns.Select(col => col.GetDisplayHeader())) + " |");
            //foreach (var value in collection)
            //{
            //    sb.AppendLine("\t| " + string.Join(" | ", textColumns.Select(col => col.GetDisplayValue(value))) + " |");
            //}
        }

        /// <summary>
        /// Prints data as simple, frame-less table
        /// </summary>
        /// <param name="header"></param>
        /// <param name="rows"></param>
        /// <param name="tableHeaderColor"></param>
        /// <param name="tableRowColor"></param>
        public void PrintAsSimpleTable(string[] header, string[][] rows, ConsoleFontColor tableHeaderColor, ConsoleFontColor tableRowColor)
        {
            int[] rowSizes = this.CalculateRequiredRowSizes(header, rows);
            if (rowSizes.All(rs => rs <= this._console.WindowWidth))
            {
                // cool, table fits to the screen
                int index = 0;
                string formatter = string.Join(" ", rowSizes.Select(size => $"{{{index++},-{size}}}"));
                this._console.WriteLine(tableHeaderColor, string.Format(formatter, header));
                foreach (string[] row in rows)
                {
                    // TODO: add missing cells...
                    this._console.WriteLine(tableRowColor, string.Format(formatter, row));
                }
            }
            else
            {
                throw new NotImplementedException(); // TODO: oversized tables...
            }
        }

        private int[] CalculateRequiredRowSizes(string[] header, string[][] rows)
        {
            int [] result = new int[header.Length];

            // headers room
            for (int i = 0; i < header.Length; ++i)
            {
                result[i] = Math.Max(result[i], header[i].Length + 1); // + 1 for column spacing

                foreach (string[] row in rows)
                {
                    if (i < row.Length) // some data might be missing, headers no
                    {
                        result[i] = Math.Max(result[i], (row[i]?.Length + 1) ?? 1); // + 1 for column spacing
                    }
                }
            }

            return result;
        }
    }
}
