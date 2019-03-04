using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Typhoon.Model;

namespace Typhoon.Search
{
    using Bitboard = UInt64;

    public static class Evaluate
    {
        private static readonly int[] materialValues = { 10000, 900, 500, 330, 310, 100, 0 };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EvaluatePosition(Position position)
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
            int whiteQueen = EvaluateSlidingPiece(position, Position.WHITE, Position.QUEEN, 900, 1, ref notWhite);
            int blackQueen = EvaluateSlidingPiece(position, Position.BLACK, Position.QUEEN, 900, 1, ref notBlack);

            // Rook Evaluation
            int whiteRook = EvaluateSlidingPiece(position, Position.WHITE, Position.ROOK, 500, 3, ref notWhite);
            int blackRook = EvaluateSlidingPiece(position, Position.BLACK, Position.ROOK, 500, 3, ref notBlack);

            // Bishop Evaluation
            int whiteBishop = EvaluateSlidingPiece(position, Position.WHITE, Position.BISHOP, 325, 8, ref notWhite);
            int blackBishop = EvaluateSlidingPiece(position, Position.BLACK, Position.BISHOP, 325, 8, ref notBlack);

            // Knight Evaluation
            int whiteKnight = EvaluateKnights(position, Position.WHITE, 8, ref notWhite);
            int blackKnight = EvaluateKnights(position, Position.BLACK, 8, ref notBlack);

            // For now, do not include pawn mobility
            int whitePawn = 100 * Bitboards.CountBits(position.GetPieceBitboard(Position.WHITE, Position.PAWN));
            int blackPawn = 100 * Bitboards.CountBits(position.GetPieceBitboard(Position.BLACK, Position.PAWN));

            
            score = whitePawn + whiteKnight + whiteBishop + whiteRook + whiteQueen + whiteKing;
            score -= (blackPawn + blackKnight + blackBishop + blackRook + blackQueen + blackKing);
            if (position.PlayerToMove == Position.BLACK)
                score *= -1;
            
            return score;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EvaluateKnights(Position position, int color, int mobilityMultiplier, ref Bitboard destinationBitboard)
        {
            Bitboard pieces = position.GetPieceBitboard(color, Position.KNIGHT);
            int score = 0;
            while (pieces != 0)
            {
                score += 300; // Material value
                int square = pieces.BitScanForward();
                Bitboards.PopLsb(ref pieces);
                score += mobilityMultiplier * (Bitboards.CountBits(destinationBitboard & Bitboards.KnightBitboards[square]));
            }
            return score;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EvaluateSlidingPiece(Position position, int color, int pieceType, int materialValue, int mobilityMultiplier, ref Bitboard destinationBitboard)
        {
            Bitboard pieces = position.GetPieceBitboard(color, pieceType);
            int score = 0;
            while (pieces != 0)
            {
                score += materialValue;
                int square = pieces.BitScanForward();
                Bitboards.PopLsb(ref pieces);

                Bitboard attacks = 0;
                if (pieceType == Position.ROOK || pieceType == Position.QUEEN)
                {
                    attacks = Bitboards.GetRookMoveBitboard(square, position.AllPiecesBitboard);
                }
                if (pieceType == Position.BISHOP || pieceType == Position.QUEEN)
                {
                    attacks |= Bitboards.GetBishopMoveBitboard(square, position.AllPiecesBitboard);
                }
                attacks &= destinationBitboard;
                score += Bitboards.CountBits(attacks) * mobilityMultiplier;
            }
            return score;
        }
    }
}
