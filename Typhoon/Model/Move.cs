using System;
using System.Diagnostics;
using System.Text;

namespace Typhoon.Model
{
    public struct Move : IComparable
    {
        public const int ORIGIN_MASK = 0x3F;
        public const int DEST_MASK = 0xFC0;
        public const int DEST_SHIFT = 6;
        public const int CAPTURE_MASK = 0x7000;
        public const int CAPTURE_SHIFT = 12;
        public const int PROM_MASK = 0x38000;
        public const int PROM_SHIFT = 15;
        public const int EP_MASK = 0x40000;
        public const int CASTLE_MASK = 0x80000;
        public const int CASTLE_DIR_MASK = 0x100000;

        private int move;

        public int OriginSquare
        {
            get
            {
                return move & ORIGIN_MASK;
            }
        }
        public int DestinationSquare
        {
            get
            {
                return (move & DEST_MASK) >> DEST_SHIFT;
            }
        }
        public int CapturePiece
        {
            get
            {
                return (move & CAPTURE_MASK) >> CAPTURE_SHIFT;
            }
        }
        public int PromotionType
        {
            get
            {
                return (move & PROM_MASK) >> PROM_SHIFT;
            }
        }
        public bool IsEnPassent
        {
            get
            {
                return (move & EP_MASK) != 0;
            }
        }
        public bool IsCastle
        {
            get
            {
                return (move & CASTLE_MASK) != 0;
            }
        }
        public int CastleDirection
        {
            get
            {
                return move & CASTLE_DIR_MASK;
            }
        }

        public Move(int origin, int destination, int capture = Board.EMPTY, int promotionType = Board.EMPTY)
        {
            move = promotionType << 3;
            move |= capture;
            move <<= 6;
            move |= destination;
            move <<= 6;
            move |= origin;

        }

        public Move(int origin, int destination, bool enPassent, bool castle, int castleDirection = 0)
        {
            // Must be castle or enPassent
            Debug.Assert(enPassent != castle);

            move = destination << 6;
            move |= origin;
            move |= 0x3F000; // capture and promotion are empty;

            if (enPassent)
                move |= EP_MASK;
            else
            {
                move |= CASTLE_MASK;
                move |= (castleDirection << 20);
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
