using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region SquareNames

        public const int A1 = 7;
        public const int B1 = 6;
        public const int C1 = 5;
        public const int D1 = 4;
        public const int E1 = 3;
        public const int F1 = 2;
        public const int G1 = 1;
        public const int H1 = 0;

        public const int A2 = 15;
        public const int B2 = 14;
        public const int C2 = 13;
        public const int D2 = 12;
        public const int E2 = 11;
        public const int F2 = 10;
        public const int G2 = 9;
        public const int H2 = 8;

        public const int A3 = 23;
        public const int B3 = 22;
        public const int C3 = 21;
        public const int D3 = 20;
        public const int E3 = 19;
        public const int F3 = 18;
        public const int G3 = 17;
        public const int H3 = 16;

        public const int A4 = 31;
        public const int B4 = 30;
        public const int C4 = 29;
        public const int D4 = 28;
        public const int E4 = 27;
        public const int F4 = 26;
        public const int G4 = 25;
        public const int H4 = 24;

        public const int A5 = 39;
        public const int B5 = 38;
        public const int C5 = 37;
        public const int D5 = 36;
        public const int E5 = 35;
        public const int F5 = 34;
        public const int G5 = 33;
        public const int H5 = 32;

        public const int A6 = 47;
        public const int B6 = 46;
        public const int C6 = 45;
        public const int D6 = 44;
        public const int E6 = 43;
        public const int F6 = 42;
        public const int G6 = 41;
        public const int H6 = 40;

        public const int A7 = 55;
        public const int B7 = 54;
        public const int C7 = 53;
        public const int D7 = 52;
        public const int E7 = 51;
        public const int F7 = 50;
        public const int G7 = 49;
        public const int H7 = 48;

        public const int A8 = 63;
        public const int B8 = 62;
        public const int C8 = 61;
        public const int D8 = 60;
        public const int E8 = 59;
        public const int F8 = 58;
        public const int G8 = 57;
        public const int H8 = 56;

        #endregion


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
                        squares[curr.BitScanForward()] = piece;
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
                int destinationSquare = attacks.BitScanForward();
                Bitboards.PopLsb(ref attacks);
                list.Add(new Move(sourceSquare, destinationSquare, squares[destinationSquare]));
            }
        }

        public void GetAllStepPieceMoves(List<Move> list, int pieceType, int color, Bitboard destinationBitboard)
        {
            Debug.Assert(pieceType == KING || pieceType == KNIGHT);
            Debug.Assert(color == WHITE || color == BLACK);

            Bitboard[] attackBitboards = pieceType == KING ? Bitboards.KingBitboards : Bitboards.KnightBitboards;

            Bitboard piece = pieces[color, pieceType];
            while (piece != 0)
            {
                int square = piece.BitScanForward();
                Bitboards.PopLsb(ref piece);
                Bitboard attacksBitboard = attackBitboards[square] & destinationBitboard;
                GenerateMovesFromBitboard(list, attacksBitboard, square);
            }
        }

        public void GetSlidingPieceMoves(
            List<Move> list,
            int pieceType,
            int square,
            Bitboard allPieces,
            Bitboard destinationBitboard)
        {
            Bitboard attacks = 0;
            if (pieceType == ROOK || pieceType == QUEEN)
            {
                attacks = Bitboards.GetRookMoveBitboard(square, allPieces);
            }
            if (pieceType == BISHOP || pieceType == QUEEN)
            {
                attacks |= Bitboards.GetBishopMoveBitboard(square, allPieces);
            }
            attacks &= destinationBitboard;
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
                    else if (curr >= 'b' && curr <= 'r')
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
