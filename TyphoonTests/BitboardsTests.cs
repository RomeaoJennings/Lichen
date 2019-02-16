using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    using Bitboard = UInt64;

    public class BitboardsTests
    {
        [TestClass]
        public class SquareDistance
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual((byte)0, Bitboards.SquareDistance[Board.H1, Board.H1]);
                Assert.AreEqual((byte)7, Bitboards.SquareDistance[Board.H1, Board.A1]);
                Assert.AreEqual((byte)7, Bitboards.SquareDistance[Board.A7, Board.H1]);
                Assert.AreEqual((byte)2, Bitboards.SquareDistance[Board.D5, Board.E3]);
            }
        }

        [TestClass]
        public class RowBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0xFF0000UL, Bitboards.RowBitboards[Board.A3]);
                Assert.AreEqual(0xFF00000000UL, Bitboards.RowBitboards[Board.H5]);
                Assert.AreEqual(0xFFUL, Bitboards.RowBitboards[Board.H1]);
            }
        }

        [TestClass]
        public class ColumnBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x1010101010101010UL, Bitboards.ColumnBitboards[Board.D8]);
                Assert.AreEqual(0x8080808080808080UL, Bitboards.ColumnBitboards[Board.A1]);
                Assert.AreEqual(0x101010101010101UL, Bitboards.ColumnBitboards[Board.H1]);
            }
        }

        [TestClass]
        public class DiagonalBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void ForwardBitboardsArePopulatedCorrectly()
            {
                Assert.AreEqual(
                    0x102040810UL,
                    Bitboards.DiagonalBitboards[Bitboards.FORWARD, Board.D1]);

                Assert.AreEqual(
                    0x102040810UL,
                    Bitboards.DiagonalBitboards[Bitboards.FORWARD, Board.H5]);
            }

            [TestMethod]
            [TestCategory("Bitboards")]
            public void BackwardBitboardsArePopulatedCorrectly()
            {
                Assert.AreEqual(
                    0x4020100804020100UL,
                    Bitboards.DiagonalBitboards[Bitboards.BACKWARD, Board.C7]);

                Assert.AreEqual(
                    0x8040201008040201UL,
                    Bitboards.DiagonalBitboards[Bitboards.BACKWARD, Board.H1]);
            }
        }

        [TestClass]
        public class KingBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x40C0000000000000UL, Bitboards.KingBitboards[Board.A8]);
                Assert.AreEqual(0x302UL, Bitboards.KingBitboards[Board.H1]);
                Assert.AreEqual(0x70507000000000UL, Bitboards.KingBitboards[Board.C6]);
            }
        }

        [TestClass]
        public class KnightBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x28440044280000UL, Bitboards.KnightBitboards[Board.D5]);
                Assert.AreEqual(0x4400442800000000UL, Bitboards.KnightBitboards[Board.D7]);
                Assert.AreEqual(0x20400000000000UL, Bitboards.KnightBitboards[Board.A8]);
                Assert.AreEqual(0x204000402UL, Bitboards.KnightBitboards[Board.H3]);
            }
        }

        [TestClass]
        public class PawnBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x4000000000UL, Bitboards.PawnBitboards[Bitboards.WHITE, Board.A4]);
                Assert.AreEqual(0x400000UL, Bitboards.PawnBitboards[Bitboards.BLACK, Board.A4]);
                Assert.AreEqual(0x2800000000UL, Bitboards.PawnBitboards[Bitboards.WHITE, Board.D4]);
                Assert.AreEqual(0x280000UL, Bitboards.PawnBitboards[Bitboards.BLACK, Board.D4]);
            }
        }

        [TestClass]
        public class GetBithopMoveBitboard
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Bitboard occupancyBitboard = 0x780B2201E661F5UL;
                int square = Board.D3;
                Assert.AreEqual(0x824428002804UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0x2103A24FEA64C6D8UL;
                square = Board.A1;
                Assert.AreEqual(0x4000UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0x356A829C31CABB6AUL;
                square = Board.D5;
                Assert.AreEqual(0x244280028040200UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));
            }
        }
        [TestClass]
        public class GetRookMoveBitboard
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Bitboard occupancyBitboard = 0x2C479CB67DA469UL;
                int square = Board.G1;
                Assert.AreEqual(0x202020DUL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0xC5C100D915991UL;
                square = Board.B4;
                Assert.AreEqual(0x4040B8404000UL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0;
                square = Board.E8;
                Assert.AreEqual(0xF708080808080808UL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));
            }
        }

        [TestClass]
        public class BitScanForward
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Assert.AreEqual(Board.C1, Bitboards.BitScanForward(0x3789237839373920UL));
                Assert.AreEqual(Board.H5, Bitboards.BitScanForward(0x3578900000000UL));
            }
        }

        [TestClass]
        public class CountBits
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Assert.AreEqual(10, Bitboards.CountBits(0x3578900000000UL));
                Assert.AreEqual(0, Bitboards.CountBits(0));
                Assert.AreEqual(64, Bitboards.CountBits(0xFFFFFFFFFFFFFFFFUL));
            }
        }

        [TestClass]
        public class GetSquareFromName
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Assert.AreEqual(Board.A1, Bitboards.GetSquareFromName("a1"));
                Assert.AreEqual(Board.A7, Bitboards.GetSquareFromName("a7"));
                Assert.AreEqual(Board.H8, Bitboards.GetSquareFromName("h8"));
                Assert.AreEqual(Board.G3, Bitboards.GetSquareFromName("g3"));
            }
        }

        [TestClass]
        public class GetNameFromSquare
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Assert.AreEqual("a8", Bitboards.GetNameFromSquare(Board.A8));
                Assert.AreEqual("d4", Bitboards.GetNameFromSquare(Board.D4));
                Assert.AreEqual("g6", Bitboards.GetNameFromSquare(Board.G6));
            }
        }
    }
}