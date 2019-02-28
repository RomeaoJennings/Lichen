using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typhoon.Model;

namespace Typhoon.Search
{
    using Bitboard = UInt64;

    public class Search
    {
        public Move IterativeDeepening(int maxPly, Position position)
        {
            MoveList moves;
            Bitboard checkersBitboard = position.GetCheckersBitboard();
            RepetitionTable repetitionTable = new RepetitionTable();

            if (checkersBitboard != 0)
            {
                moves = new MoveList();
                position.GetEvasionMoves(moves, checkersBitboard);
            }
            else
            {
                moves = position.GetAllMoves();
            }

            int moveCount = moves.Count;

            int alpha = -1000000;
            int beta = 10000000;
            int score;
            Move bestMove = new Move();
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();

            for (int depth = 0; depth < maxPly; depth++) {
                for (int i = 0; i < moveCount; i++)
                {
                    Move move = moves.Get(i);
                    if (position.IsLegalMove(move, pinnedPiecesBitboard))
                    {
                        BoardState previousState = new BoardState(move, position);
                        position.DoMove(move);
                        score = -AlphaBeta(position, -beta, -alpha, depth, repetitionTable);
                        position.UndoMove(previousState);

                        if (score > alpha)
                        {
                            bestMove = move;
                            alpha = score;
                        }
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            return bestMove;
        }

        public int AlphaBeta(Position position, int alpha, int beta, int depth, RepetitionTable repetitionTable)
        {
            // Check for 3-repetition draw
            if (repetitionTable.AddPosition(position.Zobrist) >= 3)
            {
                repetitionTable.RemovePosition(position.Zobrist);
                return 0;
            }

            if (depth == 0)
            {
                return Evaluate.EvaluatePosition(position);
            }

            bool noMoves = true;

            MoveList moves;
            Bitboard checkersBitboard = position.GetCheckersBitboard();
            if (checkersBitboard != 0)
            {
                moves = new MoveList();
                position.GetEvasionMoves(moves, checkersBitboard);
            }
            else
            {
                moves = position.GetAllMoves();
            }
            int moveCount = moves.Count;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);
                    int score = -AlphaBeta(position, -beta, -alpha, depth - 1, repetitionTable);
                    position.UndoMove(previousState);

                    if (score > alpha)
                    {
                        alpha = score;
                        noMoves = false;
                    }
                    if (score >= beta)
                    {
                        break;
                    }
                }
            }

            repetitionTable.RemovePosition(position.Zobrist);
            if (noMoves) // No moves were legal.  Therefore it is checkmate or stalemate.
            {
                return checkersBitboard == 0 ? 0 : -50000 - depth;
            }
            return alpha;
        }
    }
}
