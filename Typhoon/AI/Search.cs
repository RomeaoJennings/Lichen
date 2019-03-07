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
        public const int INITIAL_ALPHA = -10000000;
        public const int INITIAL_BETA = 10000000;
        public const int CHECKMATE = -50000;

        private int nodeCounter;
        private int nodesPerSecond;

        public int Nodes { get { return nodeCounter; } }
        public int NodesPerSecond { get { return nodesPerSecond; } }



        public event EventHandler<SearchCompletedEventArgs> IterationCompleted;
        public event EventHandler<SearchCompletedEventArgs> SearchCompleted;

        protected void OnIterationCompleted(int ply, int score, bool searchComplete = false)
        {
            EventHandler<SearchCompletedEventArgs> eventToCall = searchComplete ? SearchCompleted : IterationCompleted;
            if (eventToCall != null)
            {
                eventToCall(this, new SearchCompletedEventArgs(
                    ply + 1, score, principalVariations[ply], nodeCounter, nodesPerSecond));
            }
        }



        // An array of principal variations at each ply of the ID framework.
        private PvNode[] principalVariations;

        public Move IterativeDeepening(int maxPly, Position position)
        {
            nodeCounter = 1;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            principalVariations = new PvNode[maxPly];

            MoveList moves = position.GetAllMoves();

            int moveCount = moves.Count;
            int score;
            Move bestMove = new Move();
            PvNode bestNode = new PvNode();
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            int alpha=0;
            int previousScore = 0;
            for (int depth = 0; depth < maxPly; depth++)
            {

                alpha = INITIAL_ALPHA;
                int beta = INITIAL_BETA;
                bool isPvLine = true;
                if (isPvLine)
                {
                    moves.SwapPvNode(bestNode.Move);
                }

                for (int i = 0; i < moveCount; i++)
                {
                    Move move = moves.Get(i);
                    if (position.IsLegalMove(move, pinnedPiecesBitboard))
                    {
                        BoardState previousState = new BoardState(move, position);
                        position.DoMove(move);
                        PvNode node = new PvNode(move);

                        // If depth is zero no previous score to use.  Therefore do full window search
                        if (depth == 0)
                        {
                            score = -AlphaBeta(
                                position,
                                -beta,
                                -alpha,
                                depth,
                                isPvLine,
                                node,
                                bestNode.Next);
                        }
                        else
                        {
                            int aspirationAlphaDelta = 100;
                            int aspirationBetaDelta = 100;
                            bool failed;
                            int aspireAlpha = previousScore - aspirationAlphaDelta;
                            int aspireBeta = previousScore + aspirationBetaDelta;
                            do
                            {
                                failed = false;
                                
                                score = -AlphaBeta(
                                    position,
                                    -aspireBeta,
                                    -aspireAlpha,
                                    depth,
                                    isPvLine,
                                    node,
                                    bestNode.Next);
                                if (score <= aspireAlpha)
                                {
                                    aspirationAlphaDelta <<= 2;
                                    aspireAlpha = previousScore - aspirationAlphaDelta;
                                    failed = true;
                                }
                                else if (score >= aspireBeta)
                                {
                                    aspirationBetaDelta <<= 2;
                                    aspireBeta = previousScore + aspirationBetaDelta;
                                    failed = true;
                                }
                            } while (failed);
                            
                        }
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
                previousScore = alpha;
                principalVariations[depth] = bestNode;
                nodesPerSecond = (int)(Nodes * 1000 / Math.Max(1L, stopwatch.ElapsedMilliseconds));
                OnIterationCompleted(depth, alpha);
            }
            stopwatch.Stop();
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
                return Quiesce(position, alpha, beta, depth);
            }

            nodeCounter++;

            if (position.PositionIsThreefoldDraw())
            {
                return 0;
            }

            int current = INITIAL_ALPHA;
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
                    
                    PvNode node = new PvNode(move);
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);
                    int score;

                    // Perform PVS Search.  Full window on first move, and null windows on remaining moves.
                    if (noMoves)
                    {
                        noMoves = false;
                        score = -AlphaBeta(position, -beta, -alpha, depth - 1, isPvLine, node, lastPv?.Next);
                    }
                    else
                    {
                        score = -AlphaBeta(position, -alpha - 1, -alpha, depth - 1, isPvLine, node, lastPv?.Next);
                        // If null move search fails, search again with full window.
                        if (score > alpha && score < beta)
                            score= -AlphaBeta(position, -beta, -alpha, depth - 1, isPvLine, node, lastPv?.Next);
                    }

                    position.UndoMove(previousState);
                    isPvLine = false;
                    if (score >= current)
                    {
                        current = score;
                        pvNode.Next = node;
                        if (score >= alpha)
                        {
                            alpha = score;
                        }
                        if (score >= beta)
                        {
                            break;
                        }
                    }
                }
            }

            if (noMoves) // No moves were legal.  Therefore it is checkmate or stalemate.
            {
                return position.GetCheckersBitboard() == 0 ? 0 : CHECKMATE - depth;
            }
            return current;
        }

        public int Quiesce(Position position, int alpha, int beta, int depth)
        {
            nodeCounter++;
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
