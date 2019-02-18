using System;

namespace Typhoon.Model
{
    public static class ZobristHash
    {
        private static readonly Random random = new Random(8675309);

        // Indexed by color, piece, square
        private static readonly ulong[][][] pieceHashes = InitPieces();

        private static readonly ulong whiteToMoveHash = random.NextUlong();

        private static readonly ulong[] enPassentFile = InitEnPassentHashes();

        private static readonly ulong castleWhiteKingHash = random.NextUlong();
        private static readonly ulong castleWhiteQueenHash = random.NextUlong();
        private static readonly ulong castleBlackKingHash = random.NextUlong();
        private static readonly ulong castleBlackQueenHash = random.NextUlong();

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
                        result[color][piece][square] = random.NextUlong();
                    }
                }
            }
            return result;
        }

        private static ulong[] InitEnPassentHashes()
        {
            ulong[] result = new ulong[8];
            for (int i = 0; i < 8; i++)
            {
                result[i] = random.NextUlong();
            }
            return result;
        }

        private static ulong NextUlong(this Random rand)
        {
            ulong upperBits = (ulong)random.Next();
            upperBits <<= 32;
            ulong lowerBits = (ulong)random.Next();
            return upperBits | lowerBits;
        }

        public static ulong NewGameHash
        {
            get
            {
                ulong hash = whiteToMoveHash;

                hash ^= castleBlackKingHash;
                hash ^= castleBlackQueenHash;
                hash ^= castleWhiteKingHash;
                hash ^= castleWhiteQueenHash;

                // Iterate through pawns
                for (int i = 0; i < 8; i++)
                {
                    hash ^= pieceHashes[Board.WHITE][Board.PAWN][i + Board.H2];
                    hash ^= pieceHashes[Board.BLACK][Board.PAWN][i + Board.H7];
                }

                // Iterate once for each color
                for (int color = 0; color < 2; color++)
                {
                    hash ^= pieceHashes[color][Board.ROOK][Board.H8 * color + Board.A1];
                    hash ^= pieceHashes[color][Board.KNIGHT][Board.H8 * color + Board.B1];
                    hash ^= pieceHashes[color][Board.BISHOP][Board.H8 * color + Board.C1];
                    hash ^= pieceHashes[color][Board.QUEEN][Board.H8 * color + Board.D1];
                    hash ^= pieceHashes[color][Board.KING][Board.H8 * color + Board.E1];
                    hash ^= pieceHashes[color][Board.BISHOP][Board.H8 * color + Board.F1];
                    hash ^= pieceHashes[color][Board.KNIGHT][Board.H8 * color + Board.G1];
                    hash ^= pieceHashes[color][Board.ROOK][Board.H8 * color + Board.H1];
                }

                return hash;
            }
        }
    }
}
