﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Typhoon.Model;

namespace TyphoonTests
{
    using Bitboard = UInt64;

    [TestClass]
    [TestCategory("Magics")]
    public class MagicBitboardFactoryTests
    {
        private void TestArrayEquality<T>(T[] expected, T[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length, "Arrays are not the same length");
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i], $"Error at index: {i}");
        }

        [TestMethod]
        [TestCategory("Magics")]
        public void Test_RookOccupancyBitboards()
        {
            var bitboards = MagicBitboardFactory.GenerateRookOccupancyBitboards();
            Assert.AreEqual(0x8080808080807EUL, bitboards[7]);
            Assert.AreEqual(0x404047A040400UL, bitboards[26]);
            Assert.AreEqual(0x17E0101010100UL, bitboards[40]);
            Assert.AreEqual(0x6E10101010101000UL, bitboards[60]);
        }

        [TestMethod]
        [TestCategory("Magics")]
        public void Test_BishopOccupancyBitboards()
        {
            Bitboard[] bitboards = MagicBitboardFactory.GenerateBishopOccupancyBitboards();

            Assert.AreEqual(0x40201008040200UL, bitboards[0]);
            Assert.AreEqual(0x40201008040200UL, bitboards[63]);
            Assert.AreEqual(0x28440200000000UL, bitboards[60]);
            Assert.AreEqual(0x40221400142200UL, bitboards[27]);
            Assert.AreEqual(0x4020100A0000UL, bitboards[10]);
        }

        [TestMethod]
        [TestCategory("Magics")]
        public void Test_GenerateOffsets()
        {
            int[] expected = { 0, 2, 6, 8, 9, 13, 16 };
            int[] actual = MagicBitboardFactory.GenerateOffsets(0x12345UL);
            TestArrayEquality(expected, actual);

            expected = new int[] { 32, 37, 40, 41, 46 };
            actual = MagicBitboardFactory.GenerateOffsets(0x432100000000UL);
            TestArrayEquality(expected, actual);
        }

        [TestMethod]
        [TestCategory("Magics")]
        public void Test_GeneratePermutations()
        {
            Bitboard[] expected = { 0, 1, 2, 3, 0x0000008000000000UL, 0x0000008000000001UL, 0x0000008000000002UL, 0x0000008000000003UL };
            Bitboard[] actual = MagicBitboardFactory.GeneratePermutations(0x8000000003UL);
            TestArrayEquality(expected, actual);
        }
    }
}
