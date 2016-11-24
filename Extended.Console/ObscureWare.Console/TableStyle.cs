namespace ObscureWare.Console
{
    public class TableStyle
    {
        internal enum TablePiece : byte
        {
            TopLeft = 0,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
            HeaderSeparatorWithFrame,
            HeaderSeparatorWithoutFrame,
            ColumnsSeparator,
            TopConnector,
            BottomConnector
        }

        private readonly char[] _frameChars;

        public TableStyle(ConsoleFontColor frameColor, ConsoleFontColor headerColor, ConsoleFontColor oddRowColor, ConsoleFontColor evenRowColor,
            string frameChars, char backgroundFiller, TableLargeRowContentBehavior behaviour)
        {
            FrameColor = frameColor;
            HeaderColor = headerColor;
            OddRowColor = oddRowColor;
            EvenRowColor = evenRowColor;
            BackgroundFiller = backgroundFiller;
            Behaviour = behaviour;
            _frameChars = frameChars.ToCharArray();
        }

        public ConsoleFontColor FrameColor { get; private set; }
        public ConsoleFontColor HeaderColor { get; private set; }
        public ConsoleFontColor OddRowColor { get; private set; }
        public ConsoleFontColor EvenRowColor { get; private set; }

        public char BackgroundFiller { get; private set; }
        public TableLargeRowContentBehavior Behaviour { get; private set; }

        public char TopLeft => _frameChars[(byte)TablePiece.TopLeft];

        public char Top => _frameChars[(byte)TablePiece.Top];

        public char TopRight => _frameChars[(byte)TablePiece.TopRight];

        public char Left => _frameChars[(byte)TablePiece.Left];

        public char Right => _frameChars[(byte)TablePiece.Right];

        public char BottomLeft => _frameChars[(byte)TablePiece.BottomLeft];

        public char Bottom => _frameChars[(byte)TablePiece.Bottom];

        public char BottomRight => _frameChars[(byte)TablePiece.BottomRight];

        public char HeaderSeparatorFrame => _frameChars[(byte)TablePiece.HeaderSeparatorWithFrame];

        public char HeaderSeparatorCell => _frameChars[(byte)TablePiece.HeaderSeparatorWithoutFrame];

        public char ColumnSeparator => _frameChars[(byte)TablePiece.ColumnsSeparator];

        public char TopConnector => _frameChars[(byte)TablePiece.TopConnector];

        public char BottomConnector => _frameChars[(byte)TablePiece.BottomConnector];
    }

    /// <summary>
    /// Specifies how large content of table rows will be treated
    /// </summary>
    public enum TableLargeRowContentBehavior
    {
        /// <summary>
        /// Cut to fit with ellipsis
        /// </summary>
        Ellipsis,

        /// <summary>
        /// Multi-lined
        /// </summary>
        Wrap // TODO:
    }
}