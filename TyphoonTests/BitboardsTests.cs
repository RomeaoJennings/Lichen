using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;
namespace TyphoonTests
{
    using Bitboard = UInt64;

    [TestClass]
    public class BitboardsTests
    {
        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_SquareDistance()
        {
            Assert.AreEqual((byte)0, Bitboards.SquareDistance[0, 0]);
            Assert.AreEqual((byte)7, Bitboards.SquareDistance[0, 7]);
            Assert.AreEqual((byte)7, Bitboards.SquareDistance[55, 0]);
            Assert.AreEqual((byte)2, Bitboards.SquareDistance[36, 19]);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_RowBitboards()
        {
            Assert.AreEqual(0xFF0000UL, Bitboards.RowBitboards[23]);
            Assert.AreEqual(0xFF00000000UL, Bitboards.RowBitboards[32]);
            Assert.AreEqual(0xFFUL, Bitboards.RowBitboards[0]);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_ColumnBitboards()
        {
            Assert.AreEqual(0x1010101010101010UL, Bitboards.ColumnBitboards[60]);
            Assert.AreEqual(0x8080808080808080UL, Bitboards.ColumnBitboards[7]);
            Assert.AreEqual(0x101010101010101UL, Bitboards.ColumnBitboards[0]);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_FrontDiagonalBitboards()
        {
            Assert.AreEqual(0x102040810UL, Bitboards.FrontDiagonalBitboards[4]);
            Assert.AreEqual(0x102040810UL, Bitboards.FrontDiagonalBitboards[32]);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_KingBitboards()
        {
            Assert.AreEqual(Bitboards.KingBitboards[63], 0x40C0000000000000UL);
            Assert.AreEqual(Bitboards.KingBitboards[0], 0x302UL);
            Assert.AreEqual(Bitboards.KingBitboards[45], 0x70507000000000UL);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_KnightBitboards()
        {
            Assert.AreEqual(Bitboards.KnightBitboards[36], 0x28440044280000UL);
            Assert.AreEqual(Bitboards.KnightBitboards[52], 0x4400442800000000UL);
            Assert.AreEqual(Bitboards.KnightBitboards[63], 0x20400000000000UL);
            Assert.AreEqual(Bitboards.KnightBitboards[16], 0x204000402UL);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_PawnMoveBitboards()
        {
            Assert.AreEqual(Bitboards.PawnMoveBitboards[Bitboards.WHITE, 15], 0x80800000UL);
            Assert.AreEqual(Bitboards.PawnMoveBitboards[Bitboards.BLACK, 15], 0x80UL);
            Assert.AreEqual(Bitboards.PawnMoveBitboards[Bitboards.BLACK, 48], 0x10100000000UL);
            Assert.AreEqual(Bitboards.PawnMoveBitboards[Bitboards.WHITE, 48], 0x100000000000000UL);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_PawnAttkBitboards()
        {
            Assert.AreEqual(Bitboards.PawnAttkBitboards[Bitboards.WHITE, 31], 0x4000000000UL);
            Assert.AreEqual(Bitboards.PawnAttkBitboards[Bitboards.BLACK, 31], 0x400000UL);
            Assert.AreEqual(Bitboards.PawnAttkBitboards[Bitboards.WHITE, 28], 0x2800000000UL);
            Assert.AreEqual(Bitboards.PawnAttkBitboards[Bitboards.BLACK, 28], 0x280000UL);
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_GetBishopMoves()
        {
            Bitboard occupancyBitboard = 0x780B2201E661F5UL;
            int square = 20;
            Assert.AreEqual(0x824428002804UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));

            occupancyBitboard = 0x2103A24FEA64C6D8UL;
            square = 7;
            Assert.AreEqual(0x4000UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));

            occupancyBitboard = 0x356A829C31CABB6AUL;
            square = 36;
            Assert.AreEqual(0x244280028040200UL, Bitboards.GetBishopMoveBitboard(square, occupancyBitboard));
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_GetRookMoves()
        {
            Bitboard occupancyBitboard = 0x2C479CB67DA469UL;
            int square = 1;
            Assert.AreEqual(0x202020DUL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));

            occupancyBitboard = 0xC5C100D915991UL;
            square = 30;
            Assert.AreEqual(0x4040B8404000UL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));

            occupancyBitboard = 0;
            square = 59;
            Assert.AreEqual(0xF708080808080808UL, Bitboards.GetRookMoveBitboard(square, occupancyBitboard));
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_BitScanForward()
        {
            Assert.AreEqual(5, Bitboards.BitScanForward(0x3789237839373920UL));
            Assert.AreEqual(32, Bitboards.BitScanForward(0x3578900000000UL));
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_CountBits()
        {
            Assert.AreEqual(10, Bitboards.CountBits(0x3578900000000UL));
            Assert.AreEqual(0, Bitboards.CountBits(0));
            Assert.AreEqual(64, Bitboards.CountBits(0xFFFFFFFFFFFFFFFFUL));
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_GetSquareFromName()
        {
            Assert.AreEqual(7, Bitboards.GetSquareFromName("a1"));
            Assert.AreEqual(55, Bitboards.GetSquareFromName("a7"));
            Assert.AreEqual(56, Bitboards.GetSquareFromName("h8"));
            Assert.AreEqual(17, Bitboards.GetSquareFromName("g3"));
        }

        [TestMethod]
        [TestCategory("Bitboards")]
        public void Test_GetNameFromSquare()
        {
            Assert.AreEqual("a8", Bitboards.GetNameFromSquare(Board.A8));
            Assert.AreEqual("d4", Bitboards.GetNameFromSquare(Board.D4));
            Assert.AreEqual("g6", Bitboards.GetNameFromSquare(Board.G6));
        }
    }
}
