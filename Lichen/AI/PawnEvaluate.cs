using System;
using System.Runtime.CompilerServices;

using Lichen.Model;

namespace Lichen.AI
{
    using Bitboard = UInt64;

    public class PawnEvaluate : IEvaluator
    {
        public readonly int[] pawnFileBonus = { 1, 2, 3, 4, 4, 3, 2, 1 };
        public readonly int[,] passedPawnBonus = { { 0, 128, 64, 32, 16, 8, 4, 0 }, { 0, 4, 8, 16, 32, 64, 128, 0 } };

        public const int isolatedPawnPenalty = 5;
        public const int stackedPawnPenalty = 5;



        private readonly Evaluate baseEval = new Evaluate();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EvaluatePosition(Position position)
        {
           int score = 
                EvaluatePawnStructure(position, Position.WHITE) - 
                EvaluatePawnStructure(position, Position.BLACK);
            if (position.PlayerToMove == Position.BLACK)
                score = -score;
            return score + baseEval.EvaluatePosition(position);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int EvaluatePawnStructure(Position position, int color)
        {
            int score = 0;

            Bitboard myPawns = position.GetPieceBitboard(color, Position.PAWN);

            Bitboard opponentPawns = 
                position.GetPieceBitboard(color == Position.WHITE ? Position.BLACK : Position.WHITE, Position.PAWN);

            Bitboard allPawns = myPawns | opponentPawns;

            Bitboard pawns = myPawns;

            while (pawns != 0)
            {
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
            }
            return score;
        }
    }
}
