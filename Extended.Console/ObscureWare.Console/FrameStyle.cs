namespace ObscureWare.Console
{
    /// <summary>
    /// Definition of on-screen frame
    /// </summary>
    public class FrameStyle
    {
        internal enum FramePiece : byte
        {
            TopLeft = 0,
            Top,
            TopRight,
            Left,
            Right,
            BottomLeft,
            Bottom,
            BottomRight
        }

        private readonly char[] _frameChars;

        public FrameStyle(ConsoleFontColor frameColor, ConsoleFontColor textColor, string frameChars, char backgroundFiller)
        {
            FrameColor = frameColor;
            TextColor = textColor;
            BackgroundFiller = backgroundFiller;
            _frameChars = frameChars.ToCharArray();
        }

        public ConsoleFontColor FrameColor { get; private set; }

        public ConsoleFontColor TextColor { get; private set; }

        public char BackgroundFiller { get; private set; }

        public char TopLeft => _frameChars[(byte)FramePiece.TopLeft];

        public char Top => _frameChars[(byte)FramePiece.Top];

        public char TopRight => _frameChars[(byte)FramePiece.TopRight];

        public char Left => _frameChars[(byte)FramePiece.Left];

        public char Right => _frameChars[(byte)FramePiece.Right];

        public char BottomLeft => _frameChars[(byte)FramePiece.BottomLeft];

        public char Bottom => _frameChars[(byte)FramePiece.Bottom];

        public char BottomRight => _frameChars[(byte)FramePiece.BottomRight];
    }
}