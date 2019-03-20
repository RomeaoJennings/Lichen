using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Lichen.Model;

namespace Lichen.AI
{
    using Bitboard = UInt64;

    public class Evaluate2 : IEvaluator
    {
        private static readonly int[] materialValues = { 10000, 900, 500, 350, 325, 100, 0 };

        private static readonly int[] attackBonus = { 9, 9, 5, 3, 3, 2 };

        private static readonly int[] pawnFileBonus = { 1, 2, 3, 4, 4, 3, 2, 1 };
        private static readonly int[,] passedPawnBonus = { { 0, 128, 64, 32, 16, 8, 4, 0 }, { 0, 4, 8, 16, 32, 64, 128, 0 } };



        public const int isolatedPawnPenalty = 5;
        public const int stackedPawnPenalty = 5;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EvaluatePosition(Position position)
        {
            int score = 0;

            Bitboard notWhite = ~position.GetPieceBitboard(Position.WHITE, Position.ALL_PIECES);
            Bitboard notBlack = ~position.GetPieceBitboard(Position.BLACK, Position.ALL_PIECES);

            // King mobility
            int whiteKingSquare = Bitboards.BitScanForward(position.GetPieceBitboard(Position.WHITE, Position.KING));
            int blackKingSquare = Bitboards.BitScanForward(position.GetPieceBitboard(Position.BLACK, Position.KING));
            int whiteKing = Bitboards.CountBits(notWhite & Bitboards.KingBitboards[whiteKingSquare]);
            int blackKing = Bitboards.CountBits(notBlack & Bitboards.KingBitboards[blackKingSquare]);

            // Queen evalution
            int whiteQueen = EvaluateSliders(ref position, Position.WHITE, Position.QUEEN);
            int blackQueen = EvaluateSliders(ref position, Position.BLACK, Position.QUEEN);

            // Rook Evaluation
            int whiteRook = EvaluateSliders(ref position, Position.WHITE, Position.ROOK);
            int blackRook = EvaluateSliders(ref position, Position.BLACK, Position.ROOK);

            // Bishop Evaluation
            int whiteBishop = EvaluateSliders(ref position, Position.WHITE, Position.BISHOP);
            int blackBishop = EvaluateSliders(ref position, Position.BLACK, Position.BISHOP);

            // Knight Evaluation
            int whiteKnight = EvaluateKnights(ref position, Position.WHITE);
            int blackKnight = EvaluateKnights(ref position, Position.BLACK);

            int whitePawn = EvaluatePawns(ref position, Position.WHITE);
            int blackPawn = EvaluatePawns(ref position, Position.BLACK);

            score = whitePawn + whiteKnight + whiteBishop + whiteRook + whiteQueen + whiteKing;
            score -= (blackPawn + blackKnight + blackBishop + blackRook + blackQueen + blackKing);
            if (position.PlayerToMove == Position.BLACK)
                score *= -1;

            return score;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EvaluateKnights(ref Position position, int color)
        {
            Bitboard knights = position.GetPieceBitboard(color, Position.KNIGHT);
            int score = 0;
            while (knights != 0)
            {
                int knightSquare = knights.BitScanForward();
                Bitboards.PopLsb(ref knights);
                EvaluateKnight(ref position, ref knightSquare, ref color, ref score);
            }
            return score;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EvaluateKnight(ref Position position, ref int square, ref int color, ref int score)
        {
            Bitboard destinations = Bitboards.KnightBitboards[square];
            Bitboard myPieces = position.GetPieceBitboard(color, Position.ALL_PIECES);

            score += materialValues[Position.KNIGHT];

            while (destinations != 0)
            {
                int attackSquare = destinations.BitScanForward();
                Bitboards.PopLsb(ref destinations);
                int attackedPiece = position.GetPieceSquares()[attackSquare];
                if (attackedPiece == Position.EMPTY)
                {
                    score += 1; // Base mobility bonus
                }
                // Bonuses for attacking opponent pieces
                else if ((Bitboards.SquareBitboards[attackSquare] & myPieces) == 0)
                {
                    score += 2;
                }
                // Bonuses for defending own pieces
                else
                {
                    score += attackBonus[attackedPiece];
                }
            }
        }

        public int EvaluateSliders(ref Position position, int color, int pieceType)
        {
            Bitboard sliders = position.GetPieceBitboard(color, pieceType);
            int score = 0;
            while (sliders != 0)
            {
                int sliderSquare = sliders.BitScanForward();
                Bitboards.PopLsb(ref sliders);
                EvaluateSlider(ref position, ref color, ref pieceType, ref sliderSquare, ref score);
            }
            return score;
        }

        public void EvaluateSlider(ref Position position, ref int color, ref int pieceType, ref int square, ref int score)
        {
            Bitboard destinations = position.GetSlidingPieceBitboard(square, pieceType);
            Bitboard myPieces = position.GetPieceBitboard(color, Position.ALL_PIECES);

            score += materialValues[pieceType];

            while (destinations != 0)
            {
                int attackSquare = destinations.BitScanForward();
                Bitboards.PopLsb(ref destinations);
                int attackedPiece = position.GetPieceSquares()[attackSquare];
                if (attackedPiece == Position.EMPTY)
                {
                    score += 1; // Base mobility bonus
                }
                // Bonuses for attacking opponent pieces
                else if ((Bitboards.SquareBitboards[attackSquare] & myPieces) == 0)
                {
                    score += 2;
                }
                // Bonuses for defending own pieces
                else
                {
                    score += attackBonus[attackedPiece];
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EvaluatePawns(ref Position position, int color)
        {
            int score = 0;

            Bitboard myPawns = position.GetPieceBitboard(color, Position.PAWN);
            Bitboard myPieces = position.GetPieceBitboard(color, Position.ALL_PIECES);

            Bitboard opponentPawns =
                position.GetPieceBitboard(color == Position.WHITE ? Position.BLACK : Position.WHITE, Position.PAWN);

            Bitboard allPawns = myPawns | opponentPawns;

            Bitboard pawns = myPawns;

            while (pawns != 0)
            {
                score += materialValues[Position.PAWN];

                int currSquare = pawns.BitScanForward();
                Bitboards.PopLsb(ref pawns);

                // Add bonus based on file of pawn (to promote taking towards the center)
                score += pawnFileBonus[Bitboards.GetColumn(currSquare)];

                // Analyze stacked pawns
                Bitboard filePawns = Bitboards.ColumnBitboards[currSquare] & myPawns;

                if ((filePawns & (filePawns - 1)) != 0) // More than 1 pawn in this rank
                {
                    score -= stackedPawnPenalty;
                }

                // Analyze isolated pawns
                if ((Bitboards.IsolatedPawnBitboards[currSquare] & myPawns) == 0)
                {
                    score -= isolatedPawnPenalty;
                }

                // Analyze passed pawns
                if ((Bitboards.PassedPawnBitboards[color, currSquare] & allPawns) == 0)
                {
                    score += passedPawnBonus[color, Bitboards.GetRow(currSquare)];
                }

                Bitboard pawnAttacks = Bitboards.PawnBitboards[color, currSquare];
                while (pawnAttacks != 0)
                {
                    int attackSquare = pawnAttacks.BitScanForward();
                    Bitboards.PopLsb(ref pawnAttacks);
                    int attackedPiece = position.GetPieceSquares()[attackSquare];
                    if (attackedPiece == Position.EMPTY || (myPieces & Bitboards.SquareBitboards[attackSquare]) != 0)
                    { 
                        continue;
                    }
                    score += attackBonus[attackedPiece];
                }
            }
            return score;
        }
    }
}
