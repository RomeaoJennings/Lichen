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
            Board board = Board.FromFEN(fen);
            MoveList actualMoves = new MoveList();
            if (destinationBitboard == 0)
                destinationBitboard = ~board.GetPieceBitboard(color, Board.ALL_PIECES);
            if (captureFunc)
                board.GetAllPawnCaptureMoves(actualMoves, color, destinationBitboard);
            else
                board.GetAllPawnPushMoves(actualMoves, color, destinationBitboard);
            TestArrayEquality(expectedMoves, actualMoves.ToArray());
        }

    }
}
