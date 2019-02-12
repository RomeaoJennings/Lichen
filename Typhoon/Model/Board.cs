using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    using Bitboard = UInt64;

    public class Board
    {
        public const int WHITE = 0;
        public const int BLACK = 1;
        public const int NUM_SQUARES = 64;

        public const int EMPTY = -1;
        public const int KING = 0;
        public const int QUEEN = 1;
        public const int ROOK = 2;
        public const int BISHOP = 3;
        public const int KNIGHT = 4;
        public const int PAWN = 5;
        public const int ALL_PIECES = 6;

        private Bitboard[,] pieces;
        private int[] squares;
        private int playerToMove;

        public int Opponent { get { return playerToMove == WHITE ? BLACK : WHITE; } }

        public Bitboard AllPiecesBitboard
        {
            get { return pieces[WHITE, ALL_PIECES] | pieces[BLACK, ALL_PIECES]; }
        }

        public Board()
        {
            NewGame();
        }

        private void NewGame()
        {
            playerToMove = WHITE;

            pieces = new Bitboard[2, 7];
            pieces[WHITE, PAWN] = 0xFF00UL;
            pieces[WHITE, KNIGHT] = 0x42UL;
            pieces[WHITE, BISHOP] = 0x24UL;
            pieces[WHITE, ROOK] = 0x81UL;
            pieces[WHITE, QUEEN] = 0x10UL;
            pieces[WHITE, KING] = 0x8UL;
            pieces[WHITE, ALL_PIECES] = 0xFFFF;

            pieces[BLACK, PAWN] = 0xFF000000000000UL;
            pieces[BLACK, KNIGHT] = 0x4200000000000000UL;
            pieces[BLACK, BISHOP] = 0x2400000000000000UL;
            pieces[BLACK, ROOK] = 0x8100000000000000UL;
            pieces[BLACK, QUEEN] = 0x1000000000000000UL;
            pieces[BLACK, KING] = 0x800000000000000UL;
            pieces[BLACK, ALL_PIECES] = 0xFFFF000000000000;

            InitPieceSquares();
        }

        

        private void InitPieceSquares()
        {
            squares = new int[NUM_SQUARES];
            for (int i = 0; i < NUM_SQUARES; i++)
            {
                squares[i] = EMPTY;
            }

            for (int side = 0; side < 2; side++)
            {
                for (int piece = 0; piece < 6; piece++)
                {
                    Bitboard curr = pieces[side, piece];
                    while (curr != 0)
                    {
                        squares[Bitboards.BitScanForward(curr)] = piece;
                        Bitboards.PopLsb(ref curr);
                    }
                }
            }
        }



        public void GenerateMovesFromBitboard(List<Move> list, Bitboard attacks, int sourceSquare)
        {
            while (attacks != 0)
            {
                int destinationSquare = Bitboards.BitScanForward(attacks);
                Bitboards.PopLsb(ref attacks);
                list.Add(new Move(sourceSquare, destinationSquare, squares[destinationSquare]));
            }
        }

        public void GetStepPieceMoves(List<Move> list, Bitboard moves, int square, Bitboard destinationBitboard)
        {
            Bitboard attacks = moves & destinationBitboard;
            GenerateMovesFromBitboard(list, attacks, square);
        }

        public void GetSlidingPieceMoves(List<Move> list, int pieceType, int square, Bitboard destinationBitboard)
        {
            Bitboard attacks = 0;
            if (pieceType == ROOK || pieceType == QUEEN)
            {
                attacks = Bitboards.GetRookMoveBitboard(square, destinationBitboard);
            }
            if (pieceType == BISHOP || pieceType == QUEEN)
            {
                attacks |= Bitboards.GetBishopMoveBitboard(square, destinationBitboard);
            }
            GenerateMovesFromBitboard(list, attacks, square);
        }

        #region Equality and HashCode Functions

        public static bool operator ==(Board b1, Board b2)
        {
            if (b1.playerToMove != b2.playerToMove)
                return false;

            // Compare piece bitboards
            for (int i = 0; i < ALL_PIECES; i++)
            {
                if (b1.pieces[WHITE, i] != b2.pieces[WHITE, i])
                    return false;
                if (b1.pieces[BLACK, i] != b2.pieces[BLACK, i])
                    return false;
            }
            return true;
        }

        public static bool operator !=(Board b1, Board b2)
        {
            return !(b1 == b2);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Board))
            {
                return this == (Board)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = playerToMove.GetHashCode();
            for (int i = 0; i < ALL_PIECES; i++)
            {
                hashCode ^= pieces[WHITE, i].GetHashCode();
                hashCode ^= pieces[BLACK, i].GetHashCode();
            }
            return hashCode;
        }

        #endregion
    }
}
