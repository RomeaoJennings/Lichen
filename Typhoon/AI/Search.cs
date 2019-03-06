using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typhoon.Model;

namespace Typhoon.AI
{
    using Bitboard = UInt64;

    public class Search
    {
        public event EventHandler<SearchCompletedEventArgs> IterationCompleted;
        public event EventHandler<SearchCompletedEventArgs> SearchCompleted;

        protected void OnIterationCompleted(int ply, int score, bool searchComplete = false)
        {
            if (searchComplete)
            {
                if (SearchCompleted != null)
                {
                    SearchCompleted(this, new SearchCompletedEventArgs(ply + 1, score, principalVariations[ply]));
                }
            }
            else
            {
                if (IterationCompleted != null)
                {
                    IterationCompleted(this, new SearchCompletedEventArgs(ply + 1, score, principalVariations[ply]));
                }
            }
        }


        public const int INITIAL_ALPHA = -10000000;
        public const int INITIAL_BETA = 10000000;
        public const int CHECKMATE = -50000;

        private int[] nodes;
        private int[] qnodes;

        private int currNodes;
        private int currQNodes;

        // An array of principal variations at each ply of the ID framework.
        private PvNode[] principalVariations;

        public Move IterativeDeepening(int maxPly, Position position)
        {
            Stopwatch sw = new Stopwatch();
            principalVariations = new PvNode[maxPly];
            nodes = new int[maxPly];
            qnodes = new int[maxPly];

            MoveList moves = position.GetAllMoves();

            int moveCount = moves.Count;
            int score;
            Move bestMove = new Move();
            PvNode bestNode = new PvNode();
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            int alpha=0;
            for (int depth = 0; depth < maxPly; depth++)
            {

                alpha = INITIAL_ALPHA;
                int beta = INITIAL_BETA;
                bool isPvLine = true;
                currNodes = 0;
                currQNodes = 0;
                for (int i = 0; i < moveCount; i++)
                {
                    if (isPvLine)
                    {
                        moves.SwapPvNode(bestNode.Move);
                    }
                    Move move = moves.Get(i);

                    if (position.IsLegalMove(move, pinnedPiecesBitboard))
                    {
                        BoardState previousState = new BoardState(move, position);
                        position.DoMove(move);
                        PvNode node = new PvNode(move);
                        score = -AlphaBeta(
                            position,
                            -beta,
                            -alpha,
                            depth,
                            isPvLine,
                            node,
                            bestNode.Next);
                        position.UndoMove(previousState);
                        isPvLine = false;
                        if (score > alpha)
                        {
                            bestMove = move;
                            bestNode = node;
                            alpha = score;
                        }
                    }
                }
                principalVariations[depth] = bestNode;
                nodes[depth] = currNodes;
                qnodes[depth] = currQNodes - currNodes;
                OnIterationCompleted(depth, alpha);
            }
            for (int i = 0; i < maxPly; i++)
            {
                Debug.WriteLine($"Ply {i + 1}: Nodes: {nodes[i]} Q-Nodes: {qnodes[i]}");
            }

            StringBuilder sb = new StringBuilder();
            while (bestNode != null)
            {
                sb.Append(bestNode.Move);
                sb.Append(" ");
                bestNode = bestNode.Next;
            }
            Debug.WriteLine(sb.ToString());
            OnIterationCompleted(maxPly - 1, alpha, true);
            return bestMove;
        }

        public int AlphaBeta(
            Position position,
            int alpha,
            int beta,
            int depth,
            bool isPvLine,
            PvNode pvNode,
            PvNode lastPv)
        {
            if (depth == 0)
            {
                currNodes++;
                return Quiesce(position, alpha, beta, depth);
            }
            if (position.PositionIsThreefoldDraw())
            {
                return 0;
            }

            bool noMoves = true;
            MoveList moves = position.GetAllMoves();
            int moveCount = moves.Count;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            if (isPvLine && lastPv != null)
            {
                moves.SwapPvNode(lastPv.Move);
            }

            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    noMoves = false;
                    PvNode node = new PvNode(move);
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);

                    int score = -AlphaBeta(position, -beta, -alpha, depth - 1, isPvLine, node, lastPv?.Next);

                    position.UndoMove(previousState);
                    isPvLine = false;
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

            if (noMoves) // No moves were legal.  Therefore it is checkmate or stalemate.
            {
                return position.GetCheckersBitboard() == 0 ? 0 : CHECKMATE - depth;
            }
            return alpha;
        }

        public int Quiesce(Position position, int alpha, int beta, int depth)
        {
            currQNodes++;
            if (position.PositionIsThreefoldDraw())
            {
                return 0;
            }

            int standPat = Evaluate.EvaluatePosition(position);
            if (standPat > alpha)
            {
                alpha = standPat;
            }
            if (beta <= alpha)
            {
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
                    int score = -Quiesce(position, -beta, -alpha, depth - 1);
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

            if (noMoves)
            {
                return checkersBitboard != 0 ? CHECKMATE - depth : standPat;
            }
            return alpha;
        }
    }
}
