using System;

namespace Typhoon.Model
{
    public static class ZobristHash
    {
        private static readonly Random random = new Random(8675309);

        // Indexed by color, piece, square
        private static readonly ulong[][][] pieces = InitPieces();



        private static ulong[][][] InitPieces()
        {
            ulong[][][] result = new ulong[2][][];

            for (int color = 0; color < 2; color++)
            {
                result[color] = new ulong[6][];
                for (int piece = 0; piece < 6; piece++)
                {
                    result[color][piece] = new ulong[64];
                    for (int square = 0; square < 64; square++)
                    {
                        ulong upperBits = (ulong)random.Next();
                        upperBits <<= 32;
|= (ulong)random.Next();
                        result[color][piece][square] = upperBits;
                    }
                }
            }
            return result;
        }
    }
}
