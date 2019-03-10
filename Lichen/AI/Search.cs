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

    public class Search
    {
        public const int INITIAL_ALPHA = -10000000;
        public const int INITIAL_BETA = 10000000;
        public const int CHECKMATE = -50000;
        public const int NULL_MOVE_REDUCTION = 2;
        public const int NUMBER_OF_KILLERS = 2;

        private long nodeCounter;
        private long nodesPerSecond;
        private readonly TranspositionTable transpositionTable;
        private Move[] principalVariation;
        private int[,,] killerCounts;
        private Move[,] killerMoves;


        public long Nodes { get { return nodeCounter; } }
        public long NodesPerSecond { get { return nodesPerSecond; } }
        public TranspositionTable TranspositionTable { get { return transpositionTable; } }
        public Move[] PrincipalVariation { get { return principalVariation; } }


#pragma warning disable S3264 // Events should be invoked (they are invoked through delegate).
        public event EventHandler<SearchCompletedEventArgs> IterationCompleted;
        public event EventHandler<SearchCompletedEventArgs> SearchCompleted;
#pragma warning restore S3264 // Events should be invoked

        public Search()
        {
            transpositionTable = new TranspositionTable();
        }

        public Search(TranspositionTable tt)
        {
            transpositionTable = tt;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExtractPrincipalVariation(int length, Position position)
        {
            Stack<BoardState> previousState = new Stack<BoardState>(length);
            principalVariation = new Move[length];

            for (int i = 0; i < length; i++)
            {
                TableEntry entry;
                if (transpositionTable.GetEntry(position.Zobrist, out entry))
                {
                    principalVariation[i] = entry.BestMove;
                    previousState.Push(new BoardState(entry.BestMove, position));
                    position.DoMove(entry.BestMove);
                }
                else
                {
                    break;
                }
            }
            while (previousState.Count > 0)
            {
                position.UndoMove(previousState.Pop());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int[] GetMoveScores(MoveList moves, Move? hashMove, Position position, int depth)
        {
            int[] values = { 0, 900, 500, 350, 325, 100 };
            int count = moves.Count;
            int[] result = new int[count];
            int[] squares = position.GetPieceSquares();
            for (int i = 0; i < count; i++)
            {
                Move move = moves.Get(i);
                int capturePiece = move.CapturePiece();
                if (hashMove != null && move == hashMove)
                {
                    result[i] = -10000;
                }
                else if (move == killerMoves[depth, 0])
                {
                    result[i] = -9999;
                }
                else if (move == killerMoves[depth, 1])
                {
                    result[i] = -9998;
                }
                else if (capturePiece != Position.EMPTY)
                {
                    result[i] = values[squares[move.OriginSquare()]] - values[capturePiece];
                }
                else
                {
                    result[i] = -squares[move.OriginSquare()];
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnIterationCompleted(int ply, int score, bool searchComplete = false)
        {
            EventHandler<SearchCompletedEventArgs> eventToCall = searchComplete ? SearchCompleted : IterationCompleted;
            if (eventToCall != null)
            {
                eventToCall(this, new SearchCompletedEventArgs(
                    ply + 1, score, principalVariation, nodeCounter, nodesPerSecond, transpositionTable.TableUsage));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move IterativeDeepening(int maxPly, Position position)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int score;
            nodeCounter = 1;
            int alpha = 0;
            int previousScore = 0;
            Move bestMove = new Move();


            
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            MoveList moves = position.GetAllMoves();
            int moveCount = moves.Count;

            for (int depth = 0; depth < maxPly; depth++)
            {
                killerCounts = new int[maxPly, 64, 64];
                killerMoves = new Move[maxPly, NUMBER_OF_KILLERS];
                int beta = INITIAL_BETA;
                alpha = INITIAL_ALPHA;

                TableEntry ttEntry;
                Move? hashMove = null;


                if (transpositionTable.GetEntry(position.Zobrist, out ttEntry))
                {
                    hashMove = ttEntry.BestMove;
                }

                moves.Sort(hashMove, killerMoves[0, 0], killerMoves[0, 1]);
                for (int i = 0; i < moveCount; i++)
                {
                    Move move = moves.Get(i);
                    if (position.IsLegalMove(move, pinnedPiecesBitboard))
                    {
                        BoardState previousState = new BoardState(move, position);
                        position.DoMove(move);

                        // If depth is zero no previous score to use.  Therefore do full window search
                        if (depth == 0)
                        {
                            score = -AlphaBeta(
                                position,
                                0 - beta,
                                0 - alpha,
                                depth
                            );
                        }
                        else
                        {
                            int aspirationAlphaDelta = 50;
                            int aspirationBetaDelta = 50;
                            bool failed;
                            int aspireAlpha = previousScore - aspirationAlphaDelta;
                            int aspireBeta = previousScore + aspirationBetaDelta;
                            do
                            {
                                failed = false;

                                score = -AlphaBeta(
                                    position,
                                    0 - aspireBeta,
                                    0 - aspireAlpha,
                                    depth);
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
                        if (score > alpha)
                        {
                            bestMove = move;
                            alpha = score;
                        }
                    }
                }
                previousScore = alpha;
                transpositionTable.AddEntry(position.Zobrist, alpha, NodeType.Exact, depth + 1, bestMove);
                nodesPerSecond = Nodes * 1000 / Math.Max(1L, stopwatch.ElapsedMilliseconds);
                ExtractPrincipalVariation(depth + 1, position);
                OnIterationCompleted(depth, alpha);
            }
            stopwatch.Stop();
            OnIterationCompleted(maxPly - 1, alpha, true);
            return bestMove;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AlphaBeta(
            Position position,
            int alpha,
            int beta,
            int depth,
            bool nullMoveAllowed = true
        )
        {
            int originalAlpha = alpha;
            int score;
            // Check transposition table
            TableEntry ttEntry;
            Move? hashMove = null;
            if (transpositionTable.GetEntry(position.Zobrist, out ttEntry))
            {
                hashMove = ttEntry.BestMove;
                if (ttEntry.Depth == depth) // Change this once we get TT stable.
                {
                    if (ttEntry.NodeType == NodeType.Exact)
                    {
                        return ttEntry.Score;
                    }
                    else if (ttEntry.NodeType == NodeType.LowerBound)
                    {
                        if (ttEntry.Score > alpha)
                        {
                            alpha = ttEntry.Score;
                        }
                    }
                    else if (ttEntry.Score < beta) // Upper Bound Node
                    {
                        beta = ttEntry.Score;
                    }
                }
            }

            if (depth == 0)
            {
                return Quiesce(position, alpha, beta, depth);
            }

            nodeCounter++;

            if (position.PositionIsThreefoldDraw())
            {
                return 0;
            }

            ////Null move pruning
            //if (depth > NULL_MOVE_REDUCTION && nullMoveAllowed && position.GetCheckersBitboard() == 0)
            //{
            //    BoardState bs = position.DoNullMove();
            //    score = -AlphaBeta(position, 0 - beta, 1 - beta, depth - NULL_MOVE_REDUCTION - 1, false);
            //    position.UndoNullMove(bs);
            //    if (score >= beta)
            //    {
            //        return beta;
            //    }
            //}

            int current = INITIAL_ALPHA;
            bool noMoves = true;
            Move bestMove = new Move();
            MoveList moves = position.GetAllMoves();
            int moveCount = moves.Count;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();

            moves.Sort(hashMove, killerMoves[depth, 0], killerMoves[depth, 1]);
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);

                    // Perform PVS Search.  Full window on first move, and null windows on remaining moves.
                    if (noMoves)
                    {
                        noMoves = false;
                        score = -AlphaBeta(position, 0 - beta, 0 - alpha, depth - 1);
                    }
                    else
                    {
                        score = -AlphaBeta(position, -1 - alpha, 0 - alpha, depth - 1);
                        // If null move search fails, search again with full window.
                        if (score > alpha && score < beta)
                            score = -AlphaBeta(position, 0 - beta, 0 - alpha, depth - 1);
                    }
                    position.UndoMove(previousState);
                    if (score >= current)
                    {
                        current = score;
                        bestMove = move;
                        if (score >= alpha)
                        {
                            alpha = score;
                        }
                        if (score >= beta)
                        {
                            if (move.CapturePiece() == Position.EMPTY)
                            {
                                UpdateKillerMoves(move, depth);
                            }
                            break;
                        }
                    }
                }
            }

            if (noMoves) // No moves were legal.  Therefore it is checkmate or stalemate.
            {
                return position.GetCheckersBitboard() == 0 ? 0 : CHECKMATE - depth;
            }
            NodeType nodeType;
            if (current <= originalAlpha)
            {
                nodeType = NodeType.UpperBound;
            }
            else if (current >= beta)
            {
                nodeType = NodeType.LowerBound;
            }
            else
            {
                // Add PV Node to PvTable and return.
                nodeType = NodeType.Exact;
            }
            // Add bound nodes to transposition table.
            transpositionTable.AddEntry(position.Zobrist, alpha/* TODO: SHould this be current? */, nodeType, depth, bestMove);
            return current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            moves.Sort(null, killerMoves[0, 0], killerMoves[0, 1]);
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
                    int score = -Quiesce(position, 0 - beta, 0 - alpha, depth - 1);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateKillerMoves(Move move, int depth)
        {
            int origin = move.OriginSquare();
            int destination = move.DestinationSquare();
            ++killerCounts[depth, origin, destination];
            int newCount = killerCounts[depth, origin, destination];
            Move firstKiller = killerMoves[depth, 0];

            // Is the current killer better than the second stored killer?
            if (newCount > killerCounts[depth, firstKiller.OriginSquare(), firstKiller.DestinationSquare()])
            {
                killerMoves[depth, 1] = firstKiller;
                killerMoves[depth, 0] = move;
                return;
            }
            Move secondKiller = killerMoves[depth, 1];
            if (firstKiller != move && newCount > killerCounts[depth,secondKiller.OriginSquare(), secondKiller.DestinationSquare()])
            {
                killerMoves[depth, 1] = move;
            }
        }
    }
}
