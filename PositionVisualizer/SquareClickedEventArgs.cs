using System;

namespace UserInterface
{
    public class SquareClickedEventArgs : EventArgs
    {
        private readonly int sq;
        public int Square
        {
            get { return sq; }
        }

        public SquareClickedEventArgs(int square)
        {
            sq = square;
        }
    }
}
