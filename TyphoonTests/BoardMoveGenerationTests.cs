using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    using Bitboard = UInt64;

    [TestClass]
    public class BoardMoveGenerationTests
    {
        [TestMethod]
        [TestCategory("Move Generation")]
        public void Test_GenerateMovesFromBitboard()
        {
            Board board = Board.FromFEN("r1bqkbnr/2p2ppp/p2p4/1p2p1B1/3nP3/3P1N2/PPPN1PPP/R2QKBR1 w Qkq b6 0 7");
            Move[] expectedMoves = {
                new Move(Board.F3, Board.D4, Board.KNIGHT),
                new Move(Board.F3, Board.E5, Board.PAWN),
                new Move(Board.F3, Board.H4, Board.EMPTY)
            };
            Array.Sort(expectedMoves);

            List<Move> actualMoves = new List<Move>();
            Bitboard attacks = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES)
                & Bitboards.KnightBitboards[Board.F3];
            int square = Board.F3;

            board.GenerateMovesFromBitboard(actualMoves, attacks, square);
            actualMoves.Sort();

            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());

            // TODO: ADD ADDITIONAL TESTS
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void Test_GetAllStepPieceMoves()
        {
            Board board = Board.FromFEN("r1bqkbnr/2p2ppp/p2p4/1p2p1B1/3nP3/3P1N2/PPPN1PPP/R2QKBR1 w Qkq b6 0 7");
            Move[] expectedMoves = {
                new Move(Board.F3, Board.D4, Board.KNIGHT),
                new Move(Board.F3, Board.E5, Board.PAWN),
                new Move(Board.F3, Board.H4, Board.EMPTY),
                new Move(Board.D2, Board.B1, Board.EMPTY),
                new Move(Board.D2, Board.B3, Board.EMPTY),
                new Move(Board.D2, Board.C4, Board.EMPTY)
            };
            Array.Sort(expectedMoves);

            List<Move> actualMoves = new List<Move>();
            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);

            board.GetAllStepPieceMoves(actualMoves, Board.KNIGHT, Board.WHITE, destinationBitboard);
            actualMoves.Sort();

            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());

            //TODO: Add additional tests
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void Test_GetSlidingPieceMoves_WithBishop()
        {
            Board board = Board.FromFEN("r1bqkb1r/pppp1ppp/2n2n2/1B2p3/P3P3/3P4/1PP2PPP/RNBQK1NR b KQkq - 0 4");
            Move[] expectedMoves =
            {
                new Move(Board.B5, Board.A6),
                new Move(Board.B5, Board.C6, Board.KNIGHT),
                new Move(Board.B5, Board.C4)
            };
            Array.Sort(expectedMoves);
            List<Move> actualMoves = new List<Move>();
            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);
            board.GetSlidingPieceMoves(actualMoves, Board.BISHOP, Board.B5, board.AllPiecesBitboard, destinationBitboard);
            actualMoves.Sort();

            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void Test_GetSlidingPieceMoves_WithQueen()
        {
            Board board = Board.FromFEN("r1bqkb1r/pppp1ppp/2n5/1B2p2n/P3P3/3P4/1PP2PPP/RNBQK1NR w KQkq - 1 5");
            Move[] expectedMoves =
            {
                new Move(Board.D1, Board.D2),
                new Move(Board.D1, Board.E2),
                new Move(Board.D1, Board.F3),
                new Move(Board.D1, Board.G4),
                new Move(Board.D1, Board.H5, Board.KNIGHT)
        };
            Array.Sort(expectedMoves);
            List<Move> actualMoves = new List<Move>();

            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);
            board.GetSlidingPieceMoves(actualMoves, Board.QUEEN, Board.D1, board.AllPiecesBitboard, destinationBitboard);
            actualMoves.Sort();

            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());

        }
    }
}