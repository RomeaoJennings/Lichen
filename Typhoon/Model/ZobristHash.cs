using System;

namespace Typhoon.Model
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
                    hash ^= PieceHashes[Board.WHITE][Board.PAWN][i + Board.H2];
                    hash ^= PieceHashes[Board.BLACK][Board.PAWN][i + Board.H7];
                }

                // Iterate once for each color
                for (int color = 0; color < 2; color++)
                {
                    hash ^= PieceHashes[color][Board.ROOK][Board.H8 * color + Board.A1];
                    hash ^= PieceHashes[color][Board.KNIGHT][Board.H8 * color + Board.B1];
                    hash ^= PieceHashes[color][Board.BISHOP][Board.H8 * color + Board.C1];
                    hash ^= PieceHashes[color][Board.QUEEN][Board.H8 * color + Board.D1];
                    hash ^= PieceHashes[color][Board.KING][Board.H8 * color + Board.E1];
                    hash ^= PieceHashes[color][Board.BISHOP][Board.H8 * color + Board.F1];
                    hash ^= PieceHashes[color][Board.KNIGHT][Board.H8 * color + Board.G1];
                    hash ^= PieceHashes[color][Board.ROOK][Board.H8 * color + Board.H1];
                }

                return hash;
            }
        }
    }
}
