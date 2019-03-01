using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            RepetitionTable repetitionTable = new RepetitionTable();

            MoveList moves = position.GetAllMoves();

            int moveCount = moves.Count;

            int alpha = -1000000;
            int beta = 10000000;
            int score;
            Move bestMove = new Move();
            PvNode bestNode = null;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();

            for (int depth = maxPly-1; depth < maxPly; depth++) {
                for (int i = 0; i < moveCount; i++)
                {
                    Move move = moves.Get(i);
                    
                    if (position.IsLegalMove(move, pinnedPiecesBitboard))
                    {
                        BoardState previousState = new BoardState(move, position);
                        position.DoMove(move);
                        PvNode node = new PvNode(move);
                        score = -AlphaBeta(position, -beta, -alpha, depth, repetitionTable, node);
                        position.UndoMove(previousState);

                        if (score > alpha)
                        {
                            bestMove = move;
                            bestNode = node;
                            alpha = score;
                        }
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            StringBuilder sb = new StringBuilder();
            while (bestNode != null)
            {
                sb.Append(bestNode.Move);
                sb.Append("\r\n");
                bestNode = bestNode.Next;
            }
            Debug.Write(sb.ToString());
            return bestMove;
        }

        public int AlphaBeta(Position position, int alpha, int beta, int depth, RepetitionTable repetitionTable, PvNode pvNode)
        {
            ulong zobrist = position.Zobrist;
            if (depth == 0)
            {
                return Quiesce(position, alpha, beta, depth, repetitionTable);
            }

            // Check for 3-repetition draw
            if (repetitionTable.AddPosition(zobrist) >= 3)
            {
                repetitionTable.RemovePosition(position.Zobrist);
                return 0;
            }

            

            bool noMoves = true;

            MoveList moves = position.GetAllMoves();
            int moveCount = moves.Count;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    noMoves = false;
                    PvNode node = new PvNode(move);
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);
         
                    int score = -AlphaBeta(position, -beta, -alpha, depth - 1, repetitionTable, node);
                 
                    position.UndoMove(previousState);

                    if (score > alpha)
                    {
                        alpha = score;
                        pvNode.Next = node;
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
                return position.GetCheckersBitboard() == 0 ? 0 : -50000 - depth;
            }
            return alpha;
        }

        public int Quiesce(Position position, int alpha, int beta, int depth, RepetitionTable repetitionTable)
        {
            // Check for 3-repetition draw
            ulong zobrist = position.Zobrist;
            if (repetitionTable.AddPosition(zobrist) >= 3)
            {
                repetitionTable.RemovePosition(zobrist);
                return 0;
            }

            int standPat = Evaluate.EvaluatePosition(position);
            if (standPat > alpha)
            {
                alpha = standPat;
            }
            if (beta <= alpha)
            {
                repetitionTable.RemovePosition(zobrist);
                return alpha;
            }

            Bitboard checkersBitboard = position.GetCheckersBitboard();
            MoveList moves = new MoveList();
            if (checkersBitboard != 0)
            {
                position.GetEvasionMoves(moves, checkersBitboard);
            }
            else
            {
                position.GetMoves(
                    MoveType.Captures,
                    moves,
                    position.GetPieceBitboard(position.Opponent(), Position.ALL_PIECES));
            }
            int moveCount = moves.Count;
            bool noMoves = true;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    if (checkersBitboard == 0 && position.See(move) < 0)
                        continue;
                    noMoves = false;
                    BoardState previousState = new BoardState(move, position);
                    
                    position.DoMove(move);
                    int score = -Quiesce(position, -beta, -alpha, depth-1, repetitionTable);
                    position.UndoMove(previousState);
                    if (score > alpha)
                    {
                        alpha = score;
                    }
                    if (beta <= alpha)
                    {
                        break;
                    }
                }
            }
            repetitionTable.RemovePosition(zobrist);
            if (noMoves)
            {
                return checkersBitboard != 0 ? -50000 - depth : standPat;
            }
            return alpha;
        }
    }
}
