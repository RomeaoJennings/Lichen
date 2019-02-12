using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    public struct Move
    {
        public int SourceSquare;
        public int DestinationSquare;
        public int CapturePiece;

        public Move(int source, int destination, int capture)
        {
            SourceSquare = source;
            DestinationSquare = destination;
            CapturePiece = capture;
        }

        public static bool operator==(Move m1, Move m2)
        {
            return 
                m1.DestinationSquare == m2.DestinationSquare && 
                m1.SourceSquare == m2.SourceSquare &&
                m1.CapturePiece == m2.CapturePiece;
        }

        public static bool operator!=(Move m1, Move m2)
        {
            return !(m1 == m2);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Move))
                return (Move)obj == this;
            return false;
        }

        public override int GetHashCode()
        {
            return 
                (SourceSquare.GetHashCode() << 8) ^ 
                (DestinationSquare.GetHashCode() << 16) ^ 
                CapturePiece.GetHashCode();
        }
    }
}
