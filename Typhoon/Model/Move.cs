using System;
using System.Diagnostics;
using System.Text;
using System.Runtime.CompilerServices;

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

        private readonly int move;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int OriginSquare()
        {
            return move & ORIGIN_MASK;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int DestinationSquare()
        {
            return (move & DEST_MASK) >> DEST_SHIFT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CapturePiece()
        {
            return (move & CAPTURE_MASK) >> CAPTURE_SHIFT;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int PromotionType()
        {
            return (move & PROM_MASK) >> PROM_SHIFT;     
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnPassent()
        {
            return (move & EP_MASK) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCastle()
        {
            return (move & CASTLE_MASK) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CastleDirection()
        {
            return (move & CASTLE_DIR_MASK) == 0 ? 0 : 1; // TODO: Switch to SHIFT
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move(int origin, int destination, int capture = Position.EMPTY, int promotionType = Position.EMPTY)
        {
            move = promotionType << 3;
            move |= capture;
            move <<= 6;
            move |= destination;
            move <<= 6;
            move |= origin;

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        // TODO: Add Validation
        public static Move FromLongAlgebraic(Position position, string move)
        {
            int[] squares = position.GetPieceSquares();

            int originSquare = Bitboards.GetSquareFromName(move.Substring(0, 2));
            int destinationSquare = Bitboards.GetSquareFromName(move.Substring(2, 2));
            int capturePiece = squares[destinationSquare];
            int movedPiece = squares[originSquare];
            
            // Castle Moves
            if (movedPiece == Position.KING && Bitboards.SquareDistance[originSquare, destinationSquare] > 1)
            {
                int castleDirection = Bitboards.GetColumn(destinationSquare) == 6 ? Position.KING : Position.QUEEN;
                return new Move(originSquare, destinationSquare, false, true, castleDirection);
            }
            // En Passent and Promotion
            if (movedPiece == Position.PAWN)
            {
                // En Passent
                if (Bitboards.GetColumn(originSquare) != Bitboards.GetColumn(destinationSquare) &&
                    capturePiece == Position.EMPTY)
                {
                    return new Move(originSquare, destinationSquare, true, false);
                }
                else if (move.Length > 4)
                {
                    const string promotionMap = "-qrbn";
                    int promotionPiece = promotionMap.IndexOf(move[4]);
                    return new Move(originSquare, destinationSquare, capturePiece, promotionPiece);
                }
            }
            // Everything else
            return new Move(originSquare, destinationSquare, capturePiece);
        }

        public static bool operator ==(Move m1, Move m2)
        {
            return m1.move == m2.move;
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
            return move;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Move that = (Move)obj;
                int origin = OriginSquare() - that.OriginSquare();
                if (origin != 0)
                {
                    return origin;
                }
                int dest = DestinationSquare() - that.DestinationSquare();
                if (dest != 0)
                {
                    return dest;
                }
                return PromotionType() - that.PromotionType();
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
            sb.Append(Bitboards.GetNameFromSquare(OriginSquare()));
            sb.Append(Bitboards.GetNameFromSquare(DestinationSquare()));
            if (PromotionType() != Position.EMPTY)
            {
                sb.Append('=');
                sb.Append(pieceStr[PromotionType()]);
            }
            return sb.ToString();
        }
    }
}
