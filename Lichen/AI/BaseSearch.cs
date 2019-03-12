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

    public class BaseSearch
    {
        public const int INITIAL_ALPHA = -10000000;
        public const int INITIAL_BETA = 10000000;
        public const int CHECKMATE = -50000;
        public const int NULL_MOVE_REDUCTION = 2;
        public const int NUMBER_OF_KILLERS = 2;

        private long nodeCounter;
        private long nodesPerSecond;
        private Move[] principalVariation;
        private int[,,] killerCounts;
        private Move[,] killerMoves;
        private TranspositionTable transpositionTable;


        public long Nodes { get { return nodeCounter; } }
        public long NodesPerSecond { get { return nodesPerSecond; } }
        public Move[] PrincipalVariation { get { return principalVariation; } }


#pragma warning disable S3264 // Events should be invoked (they are invoked through delegate).
        public event EventHandler<SearchCompletedEventArgs> IterationCompleted;
        public event EventHandler<SearchCompletedEventArgs> SearchCompleted;
#pragma warning restore S3264 // Events should be invoked


        public BaseSearch()
        {
            transpositionTable = new TranspositionTable();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExtractPrincipalVariation(int length, PvNode pvNode)
        {
            principalVariation = new Move[length+1];
            int cntr = length;
            while (pvNode != null)
            {
                
                principalVariation[cntr] = pvNode.Move;
                cntr--;
                pvNode = pvNode.Next;
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
                    ply + 1, score, principalVariation, nodeCounter, nodesPerSecond, 0/*transpositionTable.TableUsage*/));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move IterativeDeepening(int maxPly, Position position)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int score;
            nodeCounter = 1;
            int alpha = INITIAL_ALPHA;
            Move bestMove = new Move();

            PvNode node, pvNode = null;
            
            Bitboard pinnedPiecesBitboard = position.GetPinnedPiecesBitboard();
            MoveList moves = position.GetAllMoves();
            int moveCount = moves.Count;

            for (int depth = 0; depth < maxPly; depth++)
            {
                killerCounts = new int[maxPly, 64, 64];
                killerMoves = new Move[maxPly, NUMBER_OF_KILLERS];
                int beta = INITIAL_BETA;
                alpha = INITIAL_ALPHA;

                Move? hashMove = null;
                if (depth > 0)
                    hashMove = principalVariation[depth];

                moves.Sort(hashMove, killerMoves[0, 0], killerMoves[0, 1]); // TODO: Killer moves are null here.  Change this...
                for (int i = 0; i < moveCount; i++)
                {
                    Move move = moves.Get(i);
                    if (position.IsLegalMove(move, pinnedPiecesBitboard))
                    {
                        node = new PvNode(move);
                        BoardState previousState = new BoardState(move, position);
                        position.DoMove(move);

                        score = -AlphaBeta(position, 0 - beta, 0 - alpha, depth, node);


                        position.UndoMove(previousState);
                        if (score > alpha)
                        {
                            bestMove = move;
                            alpha = score;
                            pvNode = node;
                        }
                    }
                }
                nodesPerSecond = Nodes * 1000 / Math.Max(1L, stopwatch.ElapsedMilliseconds);
                ExtractPrincipalVariation(depth + 1, pvNode);
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
            PvNode parentPv,
            bool nullMoveAllowed = true
        )
        {
            int score;

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

            moves.Sort(hashMove/*principalVariation[depth-1]*/, killerMoves[depth, 0], killerMoves[depth, 1]);
            ttEntry = new TableEntry(position.Zobrist,depth);
            for (int i = 0; i < moveCount; i++)
            {
                Move move = moves.Get(i);
                if (position.IsLegalMove(move, pinnedPiecesBitboard))
                {
                    noMoves = false;
                    PvNode node = new PvNode(move);
                    BoardState previousState = new BoardState(move, position);
                    position.DoMove(move);

                    score = -AlphaBeta(position, 0 - beta, 0 - alpha, depth - 1, node);
                    position.UndoMove(previousState);
                    if (score > current)
                    {
                        current = score;
                        if (score > alpha)
                        {
                            alpha = score;
                            parentPv.Next = node;
                            ttEntry.NodeType = NodeType.Exact;
                            ttEntry.BestMove = move;
                        }
                        if (score >= beta)
                        {
                            ttEntry.NodeType = NodeType.LowerBound;
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
