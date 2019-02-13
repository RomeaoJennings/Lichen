using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    public struct Move : IComparable
    {
        public readonly int SourceSquare;
        public readonly int DestinationSquare;
        public readonly int CapturePiece;
        public readonly int PromotionType;

        public Move(int source, int destination, int capture = Board.EMPTY, int promotionType = Board.EMPTY)
        {
            SourceSquare = source;
            DestinationSquare = destination;
            CapturePiece = capture;
            PromotionType = promotionType;
        }

        public static bool operator ==(Move m1, Move m2)
        {
            return
                m1.DestinationSquare == m2.DestinationSquare &&
                m1.SourceSquare == m2.SourceSquare &&
                m1.CapturePiece == m2.CapturePiece &&
                m1.PromotionType == m2.PromotionType;
        }

        public static bool operator !=(Move m1, Move m2)
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
                SourceSquare << 8 ^
                DestinationSquare << 16 ^
                CapturePiece << 4 ^
                PromotionType;
        }


        public int CompareTo(object obj)
        {
            try
            {
                Move that = (Move)obj;
                int source = SourceSquare - that.SourceSquare;
                if (source != 0)
                {
                    return source;
                }
                int dest = DestinationSquare - that.DestinationSquare;
                if (dest != 0)
                {
                    return dest;
                }
                return PromotionType - that.PromotionType;
            }
            catch (InvalidCastException)
            {
                throw new NotSupportedException("Comparison with non-Move objects is not supported.");
            }
        }

        public override string ToString()
        {
            string pieceStr = "KQRBNP";

            StringBuilder sb = new StringBuilder();
            sb.Append(Bitboards.GetNameFromSquare(SourceSquare));
            if (CapturePiece != Board.EMPTY)
            {
                sb.Append('x');
                sb.Append(pieceStr[CapturePiece]);
            }
            else
            {
                sb.Append('-');
            }
            sb.Append(Bitboards.GetNameFromSquare(DestinationSquare));
            if (PromotionType != Board.EMPTY)
            {
                sb.Append('=');
                sb.Append(pieceStr[PromotionType]);
            }
            return sb.ToString();
        }
    }
}
