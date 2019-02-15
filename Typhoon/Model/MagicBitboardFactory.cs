using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    using Bitboard = UInt64;

    public static class MagicBitboardFactory
    {
        public const int ROOK = 2;
        public const int BISHOP = 3;

        public static readonly int[] RookOffsets = { -1, 1, 8, -8 };
        public static readonly int[] BishopOffsets = { 7, 9, -7, -9 };

        public static Bitboard[] GenerateRookOccupancyBitboards()
        {
            Bitboard[] result = new Bitboard[Bitboards.NUM_SQUARES];
            Bitboard outerColsBitboard = ~(Bitboards.ColumnBitboards[0] | Bitboards.ColumnBitboards[7]);
            Bitboard outerRowsBitboard = ~(Bitboards.RowBitboards[0] | Bitboards.RowBitboards[63]);

            for (int square = 0; square < Bitboards.NUM_SQUARES; square++)
            {
                Bitboard rowBitboard = outerColsBitboard & Bitboards.RowBitboards[square];
                Bitboard colBitboard = outerRowsBitboard & Bitboards.ColumnBitboards[square];
                result[square] = (rowBitboard | colBitboard) & ~Bitboards.SquareBitboards[square];
            }
            return result;
        }

        public static Bitboard[] GenerateBishopOccupancyBitboards()
        {
            Bitboard[] result = new Bitboard[Bitboards.NUM_SQUARES];
            Bitboard notBorder = 0x7E7E7E7E7E7E00UL;
            for (int i = 0; i < Bitboards.NUM_SQUARES; i++)
            {
                result[i] = (Bitboards.FrontDiagonalBitboards[i] ^ Bitboards.BackDiagonalBitboards[i]) & notBorder;
            }
            return result;
        }

        public static Bitboard[] GeneratePermutations(Bitboard occupancyBitboard)
        {
            int numBits = Bitboards.CountBits(occupancyBitboard);
            int numPossibilities = (int)Math.Pow(2, numBits);
            int[] offsets = GenerateOffsets(occupancyBitboard);

            Bitboard[] result = new Bitboard[numPossibilities];

            for (int i = 0; i < numPossibilities; i++)
            {
                for (int bit = 0; bit < numBits; bit++)
                {
                    bool bitIsSet = (i & (1 << bit)) != 0;
                    if (bitIsSet)
                        result[i] |= Bitboards.SquareBitboards[offsets[bit]];
                }
            }

            return result;
        }

        public static int[] GenerateOffsets(Bitboard bitboard)
        {
            int[] result = new int[Bitboards.CountBits(bitboard)];
            int cnt = 0;
            while (bitboard != 0)
            {
                result[cnt++] = Bitboards.BitScanForward(bitboard);
                Bitboards.PopLsb(ref bitboard);
            }
            return result;
        }

        public static Bitboard GenerateMoveFromPermutation(int square, Bitboard permutation, int[] offsets)
        {
            Bitboard result = 0;
            foreach (int offset in offsets)
            {
                int curr = square;
                int next = curr + offset;
                bool notBlocked = true;
                while (notBlocked && next >= 0 && next < 64 && Bitboards.SquareDistance[curr, next] == 1)
                {
                    result |= Bitboards.SquareBitboards[next];
                    if ((permutation & Bitboards.SquareBitboards[next]) != 0)
                    {
                        notBlocked = false;
                    }
                    curr = next;
                    next += offset;
                }
            }
            return result;
        }

        public static Bitboard[] GenerateMovesFromPermutations(int square, Bitboard[] permutations, int[] offsets)
        {
            Bitboard[] result = new Bitboard[permutations.Length];
            for (int i = 0; i < permutations.Length; i++)
            {
                result[i] = GenerateMoveFromPermutation(square, permutations[i], offsets);
            }
            return result;
        }

        public static Bitboard GenerateMagicNumber(
            Bitboard[] permutations,
            Bitboard[] moves,
            int reduceTableSize = 0)
        {
            Random random = new Random();
            int numBits = (int)Math.Log(permutations.Length, 2) - reduceTableSize;
            int shiftVal = 64 - numBits;
            Bitboard magic = 0;
            bool failed;
            do
            {
                failed = false;
                Bitboard[] database = new Bitboard[(int)Math.Pow(2, numBits)];
                bool[] touched = new bool[database.Length];
                magic = ((Bitboard)random.Next() << 32) + (Bitboard)random.Next();
                Bitboard magic2 = ((Bitboard)random.Next() << 32) + (Bitboard)random.Next();
                magic &= magic2;
                for (int i = 0; i < permutations.Length; i++)
                {
                    Bitboard test = magic * permutations[i];
                    test >>= shiftVal;
                    int index = (int)test;
                    if (touched[index] && (database[index] != moves[i]))
                    {
                        failed = true;
                        break;
                    }
                    touched[index] = true;
                    database[index] = moves[i];
                }
            } while (failed);
            return magic;
        }


        public static Bitboard[][] InitMagics(Bitboard[] occupancyBitboards, Bitboard[] magics, int[] shifts, int[] offsets)
        {
            Bitboard[][] result = new Bitboard[64][];
            for (int square = 0; square < 64; square++)
            {
                Bitboard[] permutations = MagicBitboardFactory.GeneratePermutations(occupancyBitboards[square]);
                Bitboard[] moves = GenerateMovesFromPermutations(square, permutations, offsets);
                result[square] = new Bitboard[(int)Math.Pow(2, 64 - shifts[square])];
                for (int i = 0; i < moves.Length; i++)
                {
                    var index = (permutations[i] * magics[square]) >> shifts[square];
                    result[square][index] = moves[i];
                }
            }
            return result;
        }

        public static Bitboard[] GenerateMagicNumbers(Bitboard[] occupancyBitboards, int[] offsets)
        {
            Bitboard[] result = new Bitboard[64];
            for (int i = 0; i < 64; i++)
            {
                Bitboard[] permutations = GeneratePermutations(occupancyBitboards[i]);
                Bitboard[] moves = GenerateMovesFromPermutations(i, permutations, offsets);
                result[i] = GenerateMagicNumber(permutations, moves);
            }
            return result;
        }

        public static void WriteMagicsToFile(string filename, Bitboard[] magics)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 64; i++)
            {
                string magicStr = $"{magics[i]:X}";
                magicStr = magicStr.PadLeft(16, '0');
                sb.Append($"0x{magicStr}UL, ");
                if (i % 8 == 3 || i % 8 == 7)
                    sb.Append("\r\n");
            }
            File.WriteAllText(filename, sb.ToString());
        }
    }
}
