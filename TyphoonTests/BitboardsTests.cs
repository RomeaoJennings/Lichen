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
                Assert.AreEqual((byte)0, Bitboards.SquareDistance[Position.H1, Position.H1]);
                Assert.AreEqual((byte)7, Bitboards.SquareDistance[Position.H1, Position.A1]);
                Assert.AreEqual((byte)7, Bitboards.SquareDistance[Position.A7, Position.H1]);
                Assert.AreEqual((byte)2, Bitboards.SquareDistance[Position.D5, Position.E3]);
            }
        }

        [TestClass]
        public class RowBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0xFF0000UL, Bitboards.RowBitboards[Position.A3]);
                Assert.AreEqual(0xFF00000000UL, Bitboards.RowBitboards[Position.H5]);
                Assert.AreEqual(0xFFUL, Bitboards.RowBitboards[Position.H1]);
            }
        }

        [TestClass]
        public class ColumnBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x1010101010101010UL, Bitboards.ColumnBitboards[Position.D8]);
                Assert.AreEqual(0x8080808080808080UL, Bitboards.ColumnBitboards[Position.A1]);
                Assert.AreEqual(0x101010101010101UL, Bitboards.ColumnBitboards[Position.H1]);
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
                    Bitboards.DiagonalBitboards[Bitboards.FORWARD, Position.D1]);

                Assert.AreEqual(
                    0x102040810UL,
                    Bitboards.DiagonalBitboards[Bitboards.FORWARD, Position.H5]);
            }

            [TestMethod]
            [TestCategory("Bitboards")]
            public void BackwardBitboardsArePopulatedCorrectly()
            {
                Assert.AreEqual(
                    0x4020100804020100UL,
                    Bitboards.DiagonalBitboards[Bitboards.BACKWARD, Position.C7]);

                Assert.AreEqual(
                    0x8040201008040201UL,
                    Bitboards.DiagonalBitboards[Bitboards.BACKWARD, Position.H1]);
            }
        }

        [TestClass]
        public class LineBitboards
        {
            [TestMethod]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(Bitboards.RowBitboards[Position.A1], Bitboards.LineBitboards[Position.A1, Position.C1]);
                Assert.AreEqual(Bitboards.RowBitboards[Position.H5], Bitboards.LineBitboards[Position.C5, Position.A5]);

                Assert.AreEqual(Bitboards.ColumnBitboards[Position.A1], Bitboards.LineBitboards[Position.A1, Position.A5]);
                Assert.AreEqual(Bitboards.ColumnBitboards[Position.F8], Bitboards.LineBitboards[Position.F6, Position.F3]);

                Assert.AreEqual(
                    Bitboards.DiagonalBitboards[Bitboards.FORWARD, Position.A1],
                    Bitboards.LineBitboards[Position.B2, Position.C3]);
                Assert.AreEqual(
                    Bitboards.DiagonalBitboards[Bitboards.BACKWARD, Position.F3],
                    Bitboards.LineBitboards[Position.G2, Position.D5]);

                Assert.AreEqual(0UL, Bitboards.LineBitboards[Position.A1, Position.C2]);
            }

            [TestMethod]
            public void InversesEqualEachOther()
            {
                Random r = new Random();
                for (int i = 0; i < 100; i++)
                {
                    int one = r.Next(64);
                    int two = r.Next(64);
                    Assert.AreEqual(Bitboards.LineBitboards[one, two], Bitboards.LineBitboards[two, one]);
                }
            }
        }

        [TestClass]
        public class BetweenBitboards
        {
            [TestMethod]
            public void CalculatesCorrectly() {
                Assert.AreEqual(0UL, Bitboards.BetweenBitboards[Position.A1, Position.C4]);
                Assert.AreEqual(0UL, Bitboards.BetweenBitboards[Position.H7, Position.H6]);

                Assert.AreEqual(0x102000UL, Bitboards.BetweenBitboards[Position.B1, Position.E4]);
                Assert.AreEqual(0x4020100800UL, Bitboards.BetweenBitboards[Position.A6, Position.F1]);
                Assert.AreEqual(0x1EUL, Bitboards.BetweenBitboards[Position.C1, Position.H1]);
                Assert.AreEqual(0x4040404040000UL, Bitboards.BetweenBitboards[Position.F8, Position.F2]);
            }

            [TestMethod]
            public void InversesEqualEachOther()
            {
                Random r = new Random();
                for (int i = 0; i < 100; i++)
                {
                    int one = r.Next(64);
                    int two = r.Next(64);
                    Assert.AreEqual(
                        Bitboards.BetweenBitboards[one, two],
                        Bitboards.BetweenBitboards[two, one]);
                }
                
            }
        }

        [TestClass]
        public class KingBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x40C0000000000000UL, Bitboards.KingBitboards[Position.A8]);
                Assert.AreEqual(0x302UL, Bitboards.KingBitboards[Position.H1]);
                Assert.AreEqual(0x70507000000000UL, Bitboards.KingBitboards[Position.C6]);
            }
        }

        [TestClass]
        public class KnightBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x28440044280000UL, Bitboards.KnightBitboards[Position.D5]);
                Assert.AreEqual(0x4400442800000000UL, Bitboards.KnightBitboards[Position.D7]);
                Assert.AreEqual(0x20400000000000UL, Bitboards.KnightBitboards[Position.A8]);
                Assert.AreEqual(0x204000402UL, Bitboards.KnightBitboards[Position.H3]);
            }
        }

        [TestClass]
        public class PawnBitboards
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void IsPopulatedCorrectly()
            {
                Assert.AreEqual(0x4000000000UL, Bitboards.PawnBitboards[Bitboards.WHITE, Position.A4]);
                Assert.AreEqual(0x400000UL, Bitboards.PawnBitboards[Bitboards.BLACK, Position.A4]);
                Assert.AreEqual(0x2800000000UL, Bitboards.PawnBitboards[Bitboards.WHITE, Position.D4]);
                Assert.AreEqual(0x280000UL, Bitboards.PawnBitboards[Bitboards.BLACK, Position.D4]);
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
                int square = Position.D3;
                Assert.AreEqual(0x824428002804UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0x2103A24FEA64C6D8UL;
                square = Position.A1;
                Assert.AreEqual(0x4000UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0x356A829C31CABB6AUL;
                square = Position.D5;
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
                int square = Position.G1;
                Assert.AreEqual(0x202020DUL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0xC5C100D915991UL;
                square = Position.B4;
                Assert.AreEqual(0x4040B8404000UL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));

                occupancyBitboard = 0;
                square = Position.E8;
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
                Assert.AreEqual(Position.C1, Bitboards.BitScanForward(0x3789237839373920UL));
                Assert.AreEqual(Position.H5, Bitboards.BitScanForward(0x3578900000000UL));
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
                Assert.AreEqual(Position.A1, Bitboards.GetSquareFromName("a1"));
                Assert.AreEqual(Position.A7, Bitboards.GetSquareFromName("a7"));
                Assert.AreEqual(Position.H8, Bitboards.GetSquareFromName("h8"));
                Assert.AreEqual(Position.G3, Bitboards.GetSquareFromName("g3"));
            }
        }

        [TestClass]
        public class GetNameFromSquare
        {
            [TestMethod]
            [TestCategory("Bitboards")]
            public void CalculatesCorrectly()
            {
                Assert.AreEqual("a8", Bitboards.GetNameFromSquare(Position.A8));
                Assert.AreEqual("d4", Bitboards.GetNameFromSquare(Position.D4));
                Assert.AreEqual("g6", Bitboards.GetNameFromSquare(Position.G6));
            }
        }
    }
}