using System;
using System.Diagnostics;
using System.Text;

namespace Typhoon.Model
{
    public struct Move : IComparable
    {
        public readonly int OriginSquare;
        public readonly int DestinationSquare;
        public readonly int CapturePiece;
        public readonly int PromotionType;
        public readonly bool IsEnPassent;
        public readonly bool IsCastle;
        public readonly int CastleDirection;

        public Move(int origin, int destination, int capture = Board.EMPTY, int promotionType = Board.EMPTY)
        {
            OriginSquare = origin;
            DestinationSquare = destination;
            CapturePiece = capture;
            PromotionType = promotionType;
            IsEnPassent = false;
            IsCastle = false;
            CastleDirection = 0;
        }

        public Move(int origin, int destination, bool enPassent, bool castle, int castleDirection = 0)
        {
            // Must be castle or enPassent
            Debug.Assert(enPassent != castle);

            OriginSquare = origin;
            DestinationSquare = destination;
            PromotionType = Board.EMPTY;
            CastleDirection = castleDirection;
            if (enPassent)
            {
                CapturePiece = Board.EMPTY;
                IsEnPassent = true;
                IsCastle = false;
            }
            else
            {
                CapturePiece = Board.EMPTY;
                IsEnPassent = false;
                IsCastle = true;

            }
        }

        public static bool operator ==(Move m1, Move m2)
        {
            return
                m1.DestinationSquare == m2.DestinationSquare &&
                m1.OriginSquare == m2.OriginSquare &&
                m1.CapturePiece == m2.CapturePiece &&
                m1.PromotionType == m2.PromotionType &&
                m1.IsEnPassent == m2.IsEnPassent &&
                m1.IsCastle == m2.IsCastle;
        }

        public static bool operator !=(Move m1, Move m2)
        {
            return !(m1 == m2);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Move))
            {
                return (Move)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash =
                OriginSquare << 8 ^
                DestinationSquare << 16 ^
                CapturePiece << 4 ^
                PromotionType;
            if (IsCastle)
            {
                hash ^= 0x63826364;
            }

            if (IsEnPassent)
            {
                hash ^= 0x12345654;
            }

            return hash;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Move that = (Move)obj;
                int origin = OriginSquare - that.OriginSquare;
                if (origin != 0)
                {
                    return origin;
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
            sb.Append(Bitboards.GetNameFromSquare(OriginSquare));
            if (CapturePiece != Board.EMPTY)
            {
                sb.Append('x');
                sb.Append(pieceStr[CapturePiece]);
            }
            else if (IsEnPassent)
            {
                sb.Append("xP(EP)");
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
