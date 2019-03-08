using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lichen.Model;

namespace LichenTests
{
    using Bitboard = UInt64;

    public class MagicBitboardFactoryTests
    {
        [TestClass]
        public class RookOccupancyBitboards
        {
            [TestMethod]
            [TestCategory("Magics")]
            public void CalculatesCorrectly()
            {
                var bitboards = MagicBitboardFactory.GenerateRookOccupancyBitboards();
                Assert.AreEqual(0x8080808080807EUL, bitboards[7]);
                Assert.AreEqual(0x404047A040400UL, bitboards[26]);
                Assert.AreEqual(0x17E0101010100UL, bitboards[40]);
                Assert.AreEqual(0x6E10101010101000UL, bitboards[60]);
            }
        }

        [TestClass]
        public class BishopOccupancyBitboards
        {
            [TestMethod]
            [TestCategory("Magics")]
            public void CaclulatesCorrectly()
            {
                Bitboard[] bitboards = MagicBitboardFactory.GenerateBishopOccupancyBitboards();

                Assert.AreEqual(0x40201008040200UL, bitboards[0]);
                Assert.AreEqual(0x40201008040200UL, bitboards[63]);
                Assert.AreEqual(0x28440200000000UL, bitboards[60]);
                Assert.AreEqual(0x40221400142200UL, bitboards[27]);
                Assert.AreEqual(0x4020100A0000UL, bitboards[10]);
            }
        }

        [TestClass]
        public class GenerateOffsets
        {
            [TestMethod]
            [TestCategory("Magics")]
            public void CalculatesCorrectly()
            {
                int[] expected = { 0, 2, 6, 8, 9, 13, 16 };
                int[] actual = MagicBitboardFactory.GenerateOffsets(0x12345UL);
                TestUtils.TestArrayEquality(expected, actual);

                expected = new int[] { 32, 37, 40, 41, 46 };
                actual = MagicBitboardFactory.GenerateOffsets(0x432100000000UL);
                TestUtils.TestArrayEquality(expected, actual);
            }
        }

        [TestClass]
        public class GeneratePermutations
        {
            [TestMethod]
            [TestCategory("Magics")]
            public void CalculatesCorrectly()
            {
                Bitboard[] expected = { 0, 1, 2, 3, 0x0000008000000000UL, 0x0000008000000001UL, 0x0000008000000002UL, 0x0000008000000003UL };
                Bitboard[] actual = MagicBitboardFactory.GeneratePermutations(0x8000000003UL);
                TestUtils.TestArrayEquality(expected, actual);
            }
        }
    }
}
