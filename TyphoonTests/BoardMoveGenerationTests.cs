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
        public void GenerateMovesFromBitboard()
        {
            Board board = Board.FromFEN("r1bqkbnr/2p2ppp/p2p4/1p2p1B1/3nP3/3P1N2/PPPN1PPP/R2QKBR1 w Qkq b6 0 7");
            Move[] expectedMoves = {
                new Move(Board.F3, Board.D4, Board.KNIGHT),
                new Move(Board.F3, Board.E5, Board.PAWN),
                new Move(Board.F3, Board.H4, Board.EMPTY)
            };
            List<Move> actualMoves = new List<Move>();
            Bitboard attacks = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES)
                & Bitboards.KnightBitboards[Board.F3];
            int square = Board.F3;

            board.GenerateMovesFromBitboard(actualMoves, attacks, square);
            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());

            // TODO: ADD ADDITIONAL TESTS
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetAllStepPieceMoves()
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

            List<Move> actualMoves = new List<Move>();
            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);

            board.GetAllStepPieceMoves(actualMoves, Board.KNIGHT, Board.WHITE, destinationBitboard);
            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());

            // TODO: Add additional tests
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetSlidingPieceMoves_WithBishop()
        {
            Board board = Board.FromFEN("r1bqkb1r/pppp1ppp/2n2n2/1B2p3/P3P3/3P4/1PP2PPP/RNBQK1NR b KQkq - 0 4");
            Move[] expectedMoves =
            {
                new Move(Board.B5, Board.A6),
                new Move(Board.B5, Board.C6, Board.KNIGHT),
                new Move(Board.B5, Board.C4)
            };

            List<Move> actualMoves = new List<Move>();
            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);
            board.GetSlidingPieceMoves(
                actualMoves,
                Board.BISHOP,
                Board.B5,
                board.AllPiecesBitboard,
                destinationBitboard);
            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetSlidingPieceMoves_WithRook()
        {
            Board board = Board.FromFEN("rnbqkbn1/ppp1ppp1/6r1/2Rp3p/7P/8/PPPPPPP1/RNBQKBN1 w Qq d6 0 5");
            Move[] expectedMoves =
            {
                new Move(Board.C5, Board.B5),
                new Move(Board.C5, Board.A5),
                new Move(Board.C5, Board.C6),
                new Move(Board.C5, Board.C7, Board.PAWN),
                new Move(Board.C5, Board.D5, Board.PAWN),
                new Move(Board.C5, Board.C4),
                new Move(Board.C5, Board.C3)
            };

            List<Move> actualMoves = new List<Move>();
            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);
            board.GetSlidingPieceMoves(
                actualMoves,
                Board.ROOK,
                Board.C5,
                board.AllPiecesBitboard,
                destinationBitboard);
            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetSlidingPieceMoves_WithQueen()
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

            List<Move> actualMoves = new List<Move>();

            Bitboard destinationBitboard = ~board.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES);
            board.GetSlidingPieceMoves(
                actualMoves,
                Board.QUEEN,
                Board.D1,
                board.AllPiecesBitboard,
                destinationBitboard);
            TestUtils.TestArrayEquality(expectedMoves, actualMoves.ToArray());
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnPushMoves_WithOpeningPosition()
        {
            string fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            Move[] expectedWhiteMoves =
            {
                new Move(Board.A2, Board.A3),
                new Move(Board.A2, Board.A4),
                new Move(Board.B2, Board.B3),
                new Move(Board.B2, Board.B4),
                new Move(Board.C2, Board.C3),
                new Move(Board.C2, Board.C4),
                new Move(Board.D2, Board.D3),
                new Move(Board.D2, Board.D4),
                new Move(Board.E2, Board.E3),
                new Move(Board.E2, Board.E4),
                new Move(Board.F2, Board.F3),
                new Move(Board.F2, Board.F4),
                new Move(Board.G2, Board.G3),
                new Move(Board.G2, Board.G4),
                new Move(Board.H2, Board.H3),
                new Move(Board.H2, Board.H4),
            };
            TestUtils.TestPawnMoves(false, fen, expectedWhiteMoves, Board.WHITE);

            Move[] expectedBlackMoves =
            {
                new Move(Board.A7, Board.A6),
                new Move(Board.A7, Board.A5),
                new Move(Board.B7, Board.B6),
                new Move(Board.B7, Board.B5),
                new Move(Board.C7, Board.C6),
                new Move(Board.C7, Board.C5),
                new Move(Board.D7, Board.D6),
                new Move(Board.D7, Board.D5),
                new Move(Board.E7, Board.E6),
                new Move(Board.E7, Board.E5),
                new Move(Board.F7, Board.F6),
                new Move(Board.F7, Board.F5),
                new Move(Board.G7, Board.G6),
                new Move(Board.G7, Board.G5),
                new Move(Board.H7, Board.H6),
                new Move(Board.H7, Board.H5),
            };
            TestUtils.TestPawnMoves(false, fen, expectedBlackMoves, Board.BLACK);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnPushMoves_ForWhite_WithBlockedPieces()
        {
            string fen = "7k/8/8/2pPp3/2P1P3/6pP/P1P3P1/7K w - - 0 1";
            Move[] expectedMoves =
            {
                new Move(Board.A2, Board.A3),
                new Move(Board.A2, Board.A4),
                new Move(Board.C2, Board.C3),
                new Move(Board.D5, Board.D6),
                new Move(Board.H3, Board.H4)
            };
            TestUtils.TestPawnMoves(false, fen, expectedMoves, Board.WHITE);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnPushMoves_ForBlack_WithBlockedPieces()
        {
            string fen = "8/3ppp2/3P4/p3P2p/P2K4/8/4k3/8 b - - 0 1";
            Move[] expectedMoves =
            {
                new Move(Board.E7, Board.E6),
                new Move(Board.F7, Board.F6),
                new Move(Board.F7, Board.F5),
                new Move(Board.H5, Board.H4)
            };
            TestUtils.TestPawnMoves(false, fen, expectedMoves, Board.BLACK);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnPushMoves_ForPromotions()
        {
            string fen = "5r2/1P3P2/8/4k1K1/8/8/4p2p/7R w - - 0 1";
            Move[] expectedWhiteMoves =
            {
                new Move(Board.B7, Board.B8, Board.EMPTY, Board.QUEEN),
                new Move(Board.B7, Board.B8, Board.EMPTY, Board.ROOK),
                new Move(Board.B7, Board.B8, Board.EMPTY, Board.BISHOP),
                new Move(Board.B7, Board.B8, Board.EMPTY, Board.KNIGHT)
            };
            TestUtils.TestPawnMoves(false, fen, expectedWhiteMoves, Board.WHITE);

            Move[] expectedBlackMoves =
            {
                new Move(Board.E2, Board.E1, Board.EMPTY, Board.QUEEN),
                new Move(Board.E2, Board.E1, Board.EMPTY, Board.ROOK),
                new Move(Board.E2, Board.E1, Board.EMPTY, Board.BISHOP),
                new Move(Board.E2, Board.E1, Board.EMPTY, Board.KNIGHT)
            };
            TestUtils.TestPawnMoves(false, fen, expectedBlackMoves, Board.BLACK);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnCaptureMoves_ForWhite_WithoutPromotion()
        {
            string fen = "2bqkbnr/r1p1pp2/p1n3pp/1p1p1P1P/P3P3/2N5/1PPP2P1/R1BQKBNR w KQk - 1 8";
            Move[] expectedMoves =
            {
                new Move(Board.A4, Board.B5, Board.PAWN),
                new Move(Board.E4, Board.D5, Board.PAWN),
                new Move(Board.F5, Board.G6, Board.PAWN),
                new Move(Board.H5, Board.G6, Board.PAWN)
            };
            TestUtils.TestPawnMoves(true, fen, expectedMoves, Board.WHITE);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnCaptureMoves_ForBlack_WithoutPromotion()
        {
            string fen = "rnbqkbnr/2pp1p1p/1p2p3/p2N2p1/1P3P1P/8/P1PPP1P1/R1BQKBNR b KQkq - 1 5";
            Move[] expectedMoves =
            {
                new Move(Board.A5, Board.B4, Board.PAWN),
                new Move(Board.E6, Board.D5, Board.KNIGHT),
                new Move(Board.G5, Board.F4, Board.PAWN),
                new Move(Board.G5, Board.H4, Board.PAWN)
            };
            TestUtils.TestPawnMoves(true, fen, expectedMoves, Board.BLACK);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnCaptureMoves_White_EnPassent()
        {
            string fen = "rnbqkbnr/1pp1pppp/8/2PpP3/p7/8/PP1P1PPP/RNBQKBNR w KQkq d6 0 5";
            Move[] expectedMoves =
            {
                new Move(Board.C5, Board.D6, true, false),
                new Move(Board.E5, Board.D6, true, false)
            };
            TestUtils.TestPawnMoves(true, fen, expectedMoves, Board.WHITE);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnCaptureMoves_Black_EnPassent()
        {
            string fen = "rn1qkbnr/pbp1p1pp/8/8/3pPp2/8/1PPP1PPP/RNBQKBNR b KQkq e3 0 6";
            Move[] expectedMoves =
            {
                new Move(Board.D4, Board.E3, true, false),
                new Move(Board.F4, Board.E3, true, false)
            };
            TestUtils.TestPawnMoves(true, fen, expectedMoves, Board.BLACK);
        }

        [TestMethod]
        [TestCategory("Move Generation")]
        public void GetPawnCaptureMoves_WithPromotion()
        {
            string fen = "3r1n2/4P3/8/K7/8/2k5/6p1/5R1R w - - 0 1";
            Move[] expectedWhiteMoves =
            {
                new Move(Board.E7, Board.D8, Board.ROOK,Board.QUEEN),
                new Move(Board.E7, Board.D8, Board.ROOK,Board.ROOK),
                new Move(Board.E7, Board.D8, Board.ROOK,Board.BISHOP),
                new Move(Board.E7, Board.D8, Board.ROOK,Board.KNIGHT),
                new Move(Board.E7, Board.F8, Board.KNIGHT,Board.QUEEN),
                new Move(Board.E7, Board.F8, Board.KNIGHT,Board.ROOK),
                new Move(Board.E7, Board.F8, Board.KNIGHT,Board.BISHOP),
                new Move(Board.E7, Board.F8, Board.KNIGHT,Board.KNIGHT)
            };
            TestUtils.TestPawnMoves(true, fen, expectedWhiteMoves, Board.WHITE);

            Move[] expectedBlackMoves =
            {
                new Move(Board.G2, Board.F1, Board.ROOK,Board.QUEEN),
                new Move(Board.G2, Board.F1, Board.ROOK,Board.ROOK),
                new Move(Board.G2, Board.F1, Board.ROOK,Board.BISHOP),
                new Move(Board.G2, Board.F1, Board.ROOK,Board.KNIGHT),
                new Move(Board.G2, Board.H1, Board.ROOK,Board.QUEEN),
                new Move(Board.G2, Board.H1, Board.ROOK,Board.ROOK),
                new Move(Board.G2, Board.H1, Board.ROOK,Board.BISHOP),
                new Move(Board.G2, Board.H1, Board.ROOK,Board.KNIGHT)
            };
            TestUtils.TestPawnMoves(true, fen, expectedBlackMoves, Board.BLACK);
        }
    }
}