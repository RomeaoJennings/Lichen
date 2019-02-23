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
            Board board = new Board();
            Perft(board, 8);
        }

        static void Perft(Board board, int depth)
        {
            Debug.Assert(depth >= 1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ulong pinned = board.GetPinnedPiecesBitboard();
            var moves = board.GetAllMoves();
            int mvCnt = moves.Count;
            ulong total = 0;
            if (depth == 1)
            {
                int cnt = 0;

                for (int i=0;i<mvCnt;i++)
                {
                    var move = moves.Get(i);
                    if (board.IsLegalMove(move, pinned))
                    {
                        Console.WriteLine(move);
                        cnt++;
                    }
                }
                Console.WriteLine($"Total Moves: {cnt}");
                return;
            }
             
            for (int i=0;i<mvCnt;i++)
            {
                var move = moves.Get(i);
                ulong nodes = 0;
                BoardState bs = new BoardState(board.CastleRights, board.EnPassentBitboard, move);
                board.DoMove(move);
                CountNodes(board, depth - 1, ref nodes);
                Console.WriteLine($"Move: {move}: {nodes}");
                total += nodes;
                board.UndoMove(bs);
            }
            sw.Stop();
            
            Console.WriteLine($"Total Nodes: {total}");
            Console.WriteLine($"Elapsed Time: {sw.Elapsed}");
            Console.WriteLine($"Nodes Per Second: {(total / (ulong)sw.ElapsedMilliseconds) * 1000  }");
        }

        static void CountNodes(Board board, int depth, ref ulong nodes)
        {
            ulong pinned = board.GetPinnedPiecesBitboard();
            var moves = board.GetAllMoves();
            int mvCnt = moves.Count;
            if (depth == 1)
            {

                for (int i=0;i<mvCnt;i++)
                    if (board.IsLegalMove(moves.Get(i),pinned))
                        nodes++;
            }
            else
            {
                for (int i=0;i<mvCnt;i++)
                {
                    var move = moves.Get(i);
                    if (board.IsLegalMove(move, pinned))
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
}
