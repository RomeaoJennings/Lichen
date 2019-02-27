using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    using Bitboard = UInt64;
    public delegate void PawnMoveDelegate(List<Move> moves, int color, Bitboard destinationBitboard);
    public static class TestUtils
    {
        public static void TestArrayEquality<T>(T[] expected, T[] actual)
        {
            if (typeof(IComparable).IsAssignableFrom(typeof(T)))
            {
                Array.Sort(expected);
                Array.Sort(actual);
            }

            Assert.AreEqual(expected.Length, actual.Length, "Arrays are not the same length");
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i], $"Error at index: {i}.");
            }
        }

        public static void TestPawnMoves(bool captureFunc, string fen, Move[] expectedMoves, int color, Bitboard destinationBitboard = 0)
        {
            Position board = Position.FromFen(fen);
            MoveList actualMoves = new MoveList();
            if (destinationBitboard == 0)
                destinationBitboard = ~board.GetPieceBitboard(color, Position.ALL_PIECES);
            if (captureFunc)
                board.GetAllPawnCaptureMoves(actualMoves, color, destinationBitboard);
            else
                board.GetAllPawnPushMoves(actualMoves, color, destinationBitboard);
            TestArrayEquality(expectedMoves, actualMoves.ToArray());
        }

        public static string StripMoveNumsFromFen(string fen)
        {
            int lastSpace = fen.LastIndexOf(' ');
            int secondLastSpace = fen.LastIndexOf(' ', lastSpace - 1);
            return fen.Substring(0, secondLastSpace);
        }

        public static bool CompareFen(string fen1, string fen2)
        {
            return StripMoveNumsFromFen(fen1) == StripMoveNumsFromFen(fen2);
        }

        public static ulong NextULong(this Random random)
        {
            uint upper = (uint)random.Next();
            uint lower = (uint)random.Next();
            return ((ulong)upper) << 32 | lower;
        }
    }
}
