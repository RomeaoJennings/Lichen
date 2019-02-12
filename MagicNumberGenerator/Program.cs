using System;
using System.IO;

using Typhoon.Model;

namespace MagicNumberGenerator
{
    using Bitboard = UInt64;
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GenerateReducedMagic(1, true));
        }

        static Bitboard GenerateReducedMagic(int square, bool bishop)
        {
            Bitboard occBB = Bitboards.RookPremasks[square];
            if (bishop)
                occBB = Bitboards.BishopPremasks[square];
            Bitboard[] permutations = MagicBitboardFactory.GeneratePermutations(occBB);
            Bitboard[] moves = MagicBitboardFactory.GenerateMovesFromPermutations(
                square, 
                permutations, 
                bishop ? MagicBitboardFactory.BishopOffsets : MagicBitboardFactory.RookOffsets);
            return MagicBitboardFactory.GenerateMagicNumber(permutations, moves, 1);
        }

        static int[] GenerateShifts(Bitboard[] occupancyBBs)
        {
            int[] result = new int[64];
            for (int i = 0; i < 64; i++)
            {
                result[i] = 64 - Bitboards.CountBits(occupancyBBs[i]);
            }
            return result;
        }

        static void GenerateMagics(string piece, Bitboard[] occupancyBBs, int[] offsets, string filename)
        {

            Bitboard[] result = new Bitboard[64];

            DateTime start = DateTime.Now;
            for (int i = 0; i < 64; i++)
            {
                Console.Write($"Generating {piece} magic for square {i}...");
                var permutations = MagicBitboardFactory.GeneratePermutations(occupancyBBs[i]);
                var moves = MagicBitboardFactory.GenerateMovesFromPermutations(i, permutations, offsets);
                result[i] = MagicBitboardFactory.GenerateMagicNumber(permutations, moves);
                Console.WriteLine("Done");
            }
            Console.WriteLine($"Total Elapsed Time: {(DateTime.Now - start).ToString()}");
            if (File.Exists(filename))
                File.Delete(filename);
            MagicBitboardFactory.WriteMagicsToFile(filename, result);
        }
    }
}
