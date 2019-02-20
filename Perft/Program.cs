using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typhoon.Model;

namespace Perft
{
    static class Program
    {
        static void Main(string[] args)
        {
            Board board = Board.FromFEN("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
           
                Perft(board, 6);
           
        }

        static void Perft(Board board, int depth)
        {
            Debug.Assert(depth >= 1);
            ulong pinned = board.GetPinnedPiecesBitboard();
            var moves = board.GetAllMoves().FindAll(m => board.IsLegalMove(m, pinned));
            ulong total = 0;
            if (depth == 1)
            {
                foreach (var move in moves)
                    Console.WriteLine(move);
                Console.WriteLine($"Total Moves: {moves.Count}");
                return;
            }
            foreach (var move in moves)
            {
                ulong nodes = 0;
                BoardState bs = new BoardState(board.CastleRights, board.EnPassentBitboard, move);
                board.DoMove(move);
                CountNodes(board, depth - 1, ref nodes);
                Console.WriteLine($"Move: {move}: {nodes}");
                total += nodes;
                board.UndoMove(bs);
            }
            Console.WriteLine($"Total Nodes: {total}");
        }

        static void CountNodes(Board board, int depth, ref ulong nodes)
        {
            ulong pinned = board.GetPinnedPiecesBitboard();
            var moves = board.GetAllMoves().FindAll(m => board.IsLegalMove(m, pinned));

            if (depth == 1)
            {
                nodes += (ulong)moves.Count;
            }
            else
            {
                
                foreach (var move in moves)
                {
                    BoardState bs = new BoardState(board.CastleRights, board.EnPassentBitboard, move);
                    board.DoMove(move);
                    CountNodes(board, depth - 1, ref nodes);
                    board.UndoMove(bs);
                }
            }
        }
    }
}
