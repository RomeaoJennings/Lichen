using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    [TestClass]
    public class BoardTests
    {
        Board b1 = new Board();
        Board b2 = new Board();
        Board differentCastleRights = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq - 0 1");
        Board differentHalfMoveCounter = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 5 1");
        Board differentFullMoveNum = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq - 0 2");
        Board differentEnPassent = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQq e5 0 1");

        [TestMethod]
        [TestCategory("Board")]
        public void Test_EqualityOperators()
        {

            Assert.AreEqual(true, b1 == b2);
            Assert.AreEqual(false, b1 != b2);
            Assert.AreEqual(true, b1.Equals(b2));

            Assert.AreNotEqual(true, b1 == differentCastleRights);
            Assert.AreNotEqual(true, b1 == differentHalfMoveCounter);
            Assert.AreNotEqual(true, b1 == differentFullMoveNum);
            Assert.AreNotEqual(true, b1 == differentEnPassent);
        }

        [TestMethod]
        [TestCategory("Board")]
        public void Test_GetHashCode_MatchesEquality()
        {
            Assert.AreEqual(b1.GetHashCode(), b2.GetHashCode());

            Assert.AreNotEqual(b1.GetHashCode(), differentCastleRights.GetHashCode());
            Assert.AreNotEqual(b1.GetHashCode(), differentFullMoveNum.GetHashCode());
            Assert.AreNotEqual(b1.GetHashCode(), differentHalfMoveCounter.GetHashCode());
            Assert.AreNotEqual(b1.GetHashCode(), differentEnPassent.GetHashCode());
        }

        [TestMethod]
        [TestCategory("Board")]
        public void Test_FromFEN()
        {
            Board b = new Board();
            Board startingPos = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            Board sicilian = Board.FromFEN("rnbqkbnr/pp1ppppp/8/2p5/4P3/5N2/PPPP1PPP/RNBQKB1R b KQkq - 1 2");

            for (int color = 0; color < 2; color++)
            {
                for (int piece = 0; piece < 7; piece++)
                {
                    Assert.AreEqual(b.GetPieceBitboard(color, piece), startingPos.GetPieceBitboard(color, piece));
                }
            }
            TestUtils.TestArrayEquality(b.GetPieceSquares(), startingPos.GetPieceSquares());
            Assert.AreEqual(b.FullMoveNumber, startingPos.FullMoveNumber);
            Assert.AreEqual(b.HalfMoveClock, startingPos.HalfMoveClock);
            Assert.AreEqual(b.EnPassentBitboard, startingPos.EnPassentBitboard);

            Assert.AreEqual(0x8UL, sicilian.GetPieceBitboard(Board.WHITE, Board.KING));
            Assert.AreEqual(0x10UL, sicilian.GetPieceBitboard(Board.WHITE, Board.QUEEN));
            Assert.AreEqual(0x81UL, sicilian.GetPieceBitboard(Board.WHITE, Board.ROOK));
            Assert.AreEqual(0x24UL, sicilian.GetPieceBitboard(Board.WHITE, Board.BISHOP));
            Assert.AreEqual(0x40040UL, sicilian.GetPieceBitboard(Board.WHITE, Board.KNIGHT));
            Assert.AreEqual(0x800F700UL, sicilian.GetPieceBitboard(Board.WHITE, Board.PAWN));
            Assert.AreEqual(0x804F7FDUL, sicilian.GetPieceBitboard(Board.WHITE, Board.ALL_PIECES));

            Assert.AreEqual(0x800000000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.KING));
            Assert.AreEqual(0x1000000000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.QUEEN));
            Assert.AreEqual(0x8100000000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.ROOK));
            Assert.AreEqual(0x2400000000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.BISHOP));
            Assert.AreEqual(0x4200000000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.KNIGHT));
            Assert.AreEqual(0xDF002000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.PAWN));
            Assert.AreEqual(0xFFDF002000000000UL, sicilian.GetPieceBitboard(Board.BLACK, Board.ALL_PIECES));

            Assert.AreEqual(1, sicilian.HalfMoveClock);
            Assert.AreEqual(2, sicilian.FullMoveNumber);
            Assert.AreEqual(0UL, sicilian.EnPassentBitboard);

            int[] squares = sicilian.GetPieceSquares();
            Assert.AreEqual(squares[37], Board.PAWN);
            Assert.AreEqual(squares[53], Board.EMPTY);
            Assert.AreEqual(squares[18], Board.KNIGHT);
            Assert.AreEqual(squares[1], Board.EMPTY);
        }
    }
}
