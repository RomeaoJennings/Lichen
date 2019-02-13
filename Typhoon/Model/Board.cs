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
        private CastleRights castleRights;
        private int halfMoveClock;
        private int fullMoveNumber;
        private Bitboard enPassentBitboard;

        public int Opponent { get { return playerToMove == WHITE ? BLACK : WHITE; } }

        public Bitboard AllPiecesBitboard
        {
            get { return pieces[WHITE, ALL_PIECES] | pieces[BLACK, ALL_PIECES]; }
        }

        public int HalfMoveClock
        {
            get { return halfMoveClock; }
        }

        public int FullMoveNumber
        {
            get { return fullMoveNumber; }
        }

        public Bitboard EnPassentBitboard
        {
            get { return enPassentBitboard; }
        }

        public Board()
        {
            NewGame();
        }

        private void NewGame()
        {
            playerToMove = WHITE;
            castleRights = CastleRights.All;
            fullMoveNumber = 1;
            halfMoveClock = 0;
            enPassentBitboard = 0;

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

        public Bitboard GetPieceBitboard(int color, int pieceType)
        {
            return pieces[color, pieceType];
        }

        public int[] GetPieceSquares()
        {
            int[] result = new int[64];
            Array.Copy(squares, result, NUM_SQUARES);
            return result;
        }

        public void GetMoves()
        {
            List<Move> moves = new List<Move>();
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

        public void GetPawnMoves(List<Move> list, int color, Bitboard destinationBitboard)
        {
            // TODO: EnPassent
            // TODO: Promotion


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

            if (b1.castleRights != b2.castleRights)
                return false;

            if (b1.halfMoveClock != b2.halfMoveClock)
                return false;

            if (b1.fullMoveNumber != b2.fullMoveNumber)
                return false;
            if (b1.enPassentBitboard != b2.enPassentBitboard)
                return false;

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
            hashCode ^= castleRights.GetHashCode();
            hashCode ^= fullMoveNumber;
            hashCode ^= halfMoveClock;
            hashCode ^= enPassentBitboard.GetHashCode();
            return hashCode;
        }

        #endregion

        public static Board FromFEN(string fen)
        {
            Board result = new Board();
            result.pieces = new Bitboard[2, 7];
            const string whitePieces = "KQRBNP";
            const string blackPieces = "kqrbnp";
            try
            {
                string[] elements = fen.Split(' ');
                string squares = elements[0].Replace("/", string.Empty);
                int square = 63;
                // Process squares
                foreach (char curr in squares)
                { 
                    // Blank Squares
                    if (curr >= '0' && curr <= '9')
                    {
                        square -= int.Parse(curr.ToString());
                        continue;
                    }
                    // Black Pieces
                    else if (curr >='b' && curr <= 'r')
                    {
                        Bitboard toAdd = Bitboards.SquareBitboards[square];
                        result.pieces[BLACK, blackPieces.IndexOf(curr)] |= toAdd;
                        result.pieces[BLACK, ALL_PIECES] |= toAdd;
                    }
                    // White Pieces
                    else
                    {
                        Bitboard toAdd = Bitboards.SquareBitboards[square];
                        result.pieces[WHITE, whitePieces.IndexOf(curr)] |= toAdd;
                        result.pieces[WHITE, ALL_PIECES] |= toAdd;
                    }
                    square--;
                }
                result.InitPieceSquares();

                result.playerToMove = elements[1] == "w" ? WHITE : BLACK;
                result.castleRights = CastleRights.FromFEN(elements[2]);
                result.enPassentBitboard = 0;
                if (elements[3] != "-")
                {
                    result.enPassentBitboard = Bitboards.SquareBitboards[Bitboards.GetSquareFromName(elements[3])];
                }
                result.halfMoveClock = int.Parse(elements[4]);
                result.fullMoveNumber = int.Parse(elements[5]);
                return result;
            }
            catch (Exception e)
            {
                throw new ArgumentException($"Invalid FEN string: {fen}", e);
            }
        }
    }
}
