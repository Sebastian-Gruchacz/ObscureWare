namespace ObscureWare.Console
{
    using System.Drawing;

    /// <summary>
    /// Handy class to remember settings of special-case color pairs
    /// </summary>
    public class ConsoleFontColor
    {
        /// <summary>
        /// Initializes new instance of ConsoleFontColor tuple
        /// </summary>
        /// <param name="foreColor"></param>
        /// <param name="bgColor"></param>
        public ConsoleFontColor(Color foreColor, Color bgColor)
        {
            BgColor = bgColor;
            ForeColor = foreColor;
        }

        /// <summary>
        /// Foreground color
        /// </summary>
        public Color ForeColor { get; private set; }

        /// <summary>
        /// Background color
        /// </summary>
        public Color BgColor { get; private set; }
    }
}