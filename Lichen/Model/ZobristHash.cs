using System;

namespace Lichen.Model
{
    public static class ZobristHash
    {
        private static readonly Random random = new Random(8675309);

        // Indexed by color, piece, square
        public static readonly ulong[][][] PieceHashes = InitPieces();

        public static readonly ulong WhiteToMoveHash = random.NextUlong();

        public static readonly ulong[] EnPassentSquareHashes = InitEnPassentHashes();

        public static readonly ulong CastleWhiteKingHash = random.NextUlong();
        public static readonly ulong CastleWhiteQueenHash = random.NextUlong();
        public static readonly ulong CastleBlackKingHash = random.NextUlong();
        public static readonly ulong CastleBlackQueenHash = random.NextUlong();

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
            ulong[] result = new ulong[64];
            for (int i = 0; i < 64; i++)
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
                ulong hash = WhiteToMoveHash;

                hash ^= CastleBlackKingHash;
                hash ^= CastleBlackQueenHash;
                hash ^= CastleWhiteKingHash;
                hash ^= CastleWhiteQueenHash;

                // Iterate through pawns
                for (int i = 0; i < 8; i++)
                {
                    hash ^= PieceHashes[Position.WHITE][Position.PAWN][i + Position.H2];
                    hash ^= PieceHashes[Position.BLACK][Position.PAWN][i + Position.H7];
                }

                // Iterate once for each color
                for (int color = 0; color < 2; color++)
                {
                    hash ^= PieceHashes[color][Position.ROOK][Position.H8 * color + Position.A1];
                    hash ^= PieceHashes[color][Position.KNIGHT][Position.H8 * color + Position.B1];
                    hash ^= PieceHashes[color][Position.BISHOP][Position.H8 * color + Position.C1];
                    hash ^= PieceHashes[color][Position.QUEEN][Position.H8 * color + Position.D1];
                    hash ^= PieceHashes[color][Position.KING][Position.H8 * color + Position.E1];
                    hash ^= PieceHashes[color][Position.BISHOP][Position.H8 * color + Position.F1];
                    hash ^= PieceHashes[color][Position.KNIGHT][Position.H8 * color + Position.G1];
                    hash ^= PieceHashes[color][Position.ROOK][Position.H8 * color + Position.H1];
                }

                return hash;
            }
        }
    }
}
