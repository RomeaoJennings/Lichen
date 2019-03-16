using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Lichen.Model;

namespace Lichen.AI
{
    using Bitboard = UInt64;

    public class Search : IComparer<Move>
    {
        public const int INITIAL_ALPHA = -10000000;
        public const int INITIAL_BETA = 10000000;
        public const int CHECKMATE = -50000;
        public const int NULL_MOVE_REDUCTION = 2;
        public const int NUMBER_OF_KILLERS = 2;

        private readonly IEvaluator positionEvaluator;
        private long nodeCounter;
        private long nodesPerSecond;
        private Move[] principalVariation;
        private int lastScore;
        private int lastSearchDepth;
        private int[,,] killerCounts;
        private Move[,] killerMoves;
        private int[,] historyCounts;
        private readonly TranspositionTable transpositionTable;
        private bool mateIsFound;
        private int mateDistance;
        private bool outOfSearchTime;
        private Timer searchTimer;

        public bool MateIsFound { get { return mateIsFound; } }
        public int MateDistance { get { return mateDistance; } }
        public long Nodes { get { return nodeCounter; } }
        public long NodesPerSecond { get { return nodesPerSecond; } }
        public Move[] PrincipalVariation { get { return principalVariation; } }


#pragma warning disable S3264 // Events should be invoked (they are invoked through delegate).
        public event EventHandler<SearchCompletedEventArgs> IterationCompleted;
        public event EventHandler<SearchCompletedEventArgs> SearchCompleted;
#pragma warning restore S3264 // Events should be invoked


        public Search()
        {
            transpositionTable = new TranspositionTable();
            positionEvaluator = new PawnEvaluate();
        }

        public Search(TranspositionTable tt)
        {
            transpositionTable = tt;
            positionEvaluator = new PawnEvaluate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExtractPrincipalVariation(int length, Position position)
        {
            List<Move> pvList = new List<Move>();
            Stack<BoardState> previousPositions = new Stack<BoardState>();
            bool keepGoing = true;
            int cnt = 0;
            // Search through the transposition table for best moves. Each time we find one, do that move, and continue the search.
            do
            {
                TableEntry ttEntry;
                cnt++;
                if (transpositionTable.GetEntry(position.Zobrist, out ttEntry) && ttEntry.BestMove != Move.EmptyMove)
                {
                    pvList.Add(ttEntry.BestMove);
                    previousPositions.Push(new BoardState(ttEntry.BestMove, position));
                    position.DoMove(ttEntry.BestMove);
                }
                else
                {
                    keepGoing = false;
                }

            } while (keepGoing && cnt < length);
            
            // Unwind moves done from PV.
            while (previousPositions.Count > 0)
            {
                position.UndoMove(previousPositions.Pop());
            }
            principalVariation = pvList.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnIterationCompleted(int ply, int score, bool searchComplete = false)
        {
            if (mateIsFound)
            {
                score = mateDistance;
            }
            EventHandler<SearchCompletedEventArgs> eventToCall = searchComplete ? SearchCompleted : IterationCompleted;
            if (eventToCall != null)
            {
                eventToCall(this, new SearchCompletedEventArgs(
                    ply + 1, score, mateIsFound, principalVariation, nodeCounter, nodesPerSecond, transpositionTable.TableUsage));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move IterativeDeepening(int maxPly, int maxTimeMs, Position position)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            outOfSearchTime = false;
            searchTimer = new Timer();
            if (maxTimeMs > 0) // Zero or negative if no max time
            {
                searchTimer.Interval = maxTimeMs;
                searchTimer.Elapsed += OnSearchTimeout;
                searchTimer.Start();
            }

            int alpha = INITIAL_ALPHA;
            Move bestMove = new Move();

            try
            {
                transpositionTable.NewSearch(); // Increment TT epoch to remove ancient nodes upon collisions with new ones.
                mateIsFound = false;
                int score;
                nodeCounter = 1;

                Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
                MoveList moves = position.GetAllMoves();
                int moveCount = moves.Count;

                for (int depth = 0; depth < maxPly; depth++)
                {
                    killerCounts = new int[maxPly, 64, 64];
                    killerMoves = new Move[maxPly, NUMBER_OF_KILLERS];
                    historyCounts = new int[6, 64];

                    int beta = INITIAL_BETA;
                    alpha = INITIAL_ALPHA;

                    Move? hashMove = null;
                    TableEntry ttEntry;
                    if (transpositionTable.GetEntry(position.Zobrist, out ttEntry) && ttEntry.BestMove != Move.EmptyMove)
                    {
                        hashMove = ttEntry.BestMove;
                    }

                    moves.Sort(this, hashMove, null, null);
                    for (int i = 0; i < moveCount; i++)
                    {
                        Move move = moves.Get(i);
                        if (position.IsLegalMove(move, pinnedPiecesBitboard))
                        {
                            BoardState previousState = new BoardState(move, position);
                            position.DoMove(move);

                            score = -AlphaBeta(position, 0 - beta, 0 - alpha, depth);


                            position.UndoMove(previousState);
                            if (score > alpha)
                            {
                                bestMove = move;
                                alpha = score;
                            }
                        }
                    }
                    transpositionTable.AddEntry(position.Zobrist, alpha, NodeType.Exact, depth + 1, bestMove);
                    nodesPerSecond = Nodes * 1000 / Math.Max(1L, stopwatch.ElapsedMilliseconds);

                    lastScore = alpha;
                    lastSearchDepth = depth + 1;
                    ExtractPrincipalVariation(lastSearchDepth, position);

                    if (CheckmateFound(ref alpha))
                    {
                        if (alpha > 40000)
                        {
                            int matePly = 1 + depth - alpha - CHECKMATE;
                            
                            matePly++;
                            matePly >>= 1;
                            mateIsFound = true;
                            mateDistance = matePly;
                        }
                        else
                        {
                            int matePly = alpha - CHECKMATE - depth + 1;
                           
                            matePly >>= 1;
                            mateIsFound = true;
                            mateDistance = matePly;
                        }
                    }
                    OnIterationCompleted(depth, alpha);
                }
            }
            catch (TimeoutException) { }
            finally
            {
                stopwatch.Stop();
                OnIterationCompleted(lastSearchDepth, lastScore, true);
                searchTimer.Stop();
                searchTimer.Dispose();
            }
            return bestMove;
        }

        private void OnSearchTimeout(object sender, ElapsedEventArgs e)
        {
            outOfSearchTime = true;
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
            if (outOfSearchTime)
            {
                throw new TimeoutException();
            }

            int score;

            if (position.PositionIsThreefoldDraw()) // TODO: Add 50 move draw.
            {
                return 0;
            }

            TableEntry ttEntry;
            Move? hashMove = null;
            if (transpositionTable.GetEntry(position.Zobrist, out ttEntry))
            {
                if (ttEntry.BestMove != Move.EmptyMove)
                    hashMove = ttEntry.BestMove;
                if (ttEntry.Depth >= depth)
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

            if (depth <= 0)
            {
                return Quiesce(position, alpha, beta, 0);
            }

            nodeCounter++;
            int current = INITIAL_ALPHA;
            bool noMoves = true;

            //Null Move Heuristic
            if (nullMoveAllowed && position.GetCheckersBitboard() == 0)
            {
                BoardState prevPosition = position.DoNullMove();
                score = -AlphaBeta(position, -beta, -beta + 1, depth - 3, false);
                position.UndoNullMove(prevPosition);
                if (score >= beta)
                {
                    return beta;
                }
            }

            MoveList moves = position.GetAllMoves();
            int moveCount = moves.Count;
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();

            moves.Sort(this, hashMove, killerMoves[depth, 0], killerMoves[depth, 1]);
            ttEntry = new TableEntry(position.Zobrist,depth);
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);

                    score = -AlphaBeta(position,  -beta, -alpha, depth - 1);
                    
                    position.UndoMove(previousState);
                    noMoves = false;
                    if (score > current)
                    {
                        current = score;
                        if (score > alpha)
                        {
                            alpha = score;
                            ttEntry.NodeType = NodeType.Exact;
                            ttEntry.BestMove = move;
                        }
                        if (score >= beta)
                        {
                            ttEntry.NodeType = NodeType.LowerBound;
                            if (move.CapturePiece() == Position.EMPTY)
                            {
                                historyCounts[position.GetPieceSquares()[move.OriginSquare()], move.DestinationSquare()] += (1 << depth);
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

            ttEntry.Score = current;
            transpositionTable.AddEntry(ttEntry);

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

            int standPat = positionEvaluator.EvaluatePosition(position);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckmateFound(ref int score)
        {
            return score > 40000 || score < -40000;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(Move x, Move y)
        {
            int xCapture = x.CapturePiece();
            int yCapture = y.CapturePiece();

            if (xCapture == Position.EMPTY)
            {
                if (yCapture != Position.EMPTY)
                {
                    return 1;
                }
                else // Both quiet moves, so order moves according to history heuristic.
                {
                    return historyCounts[y.MovedPiece(), y.DestinationSquare()] - historyCounts[x.MovedPiece(), x.DestinationSquare()];
                }
            }
            else // X is capture move
            {
                if (yCapture == Position.EMPTY)
                {
                    return -1;
                }
                else // Both captures, so sort by MVV/LVA
                {
                    if (xCapture == yCapture) // Same victim so sort by attacker
                    {
                        
                        return y.MovedPiece() - x.MovedPiece();
                    }
                    else // Different victims, so sort victim
                    {
                        return xCapture - yCapture;
                    }
                }
            }
        }
    }
}
