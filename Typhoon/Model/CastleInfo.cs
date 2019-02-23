using System;

namespace Typhoon.Model
{
    using Bitboard = UInt64;

    public struct CastleInfo
    {
        public readonly int KingOrigin;
        public readonly int KingDestination;
        public readonly int RookOrigin;
        public readonly int RookDestination;
        public readonly Bitboard KingBitboard;
        public readonly Bitboard RookBitboard;
        public readonly ulong KingZobrist;
        public readonly ulong RookZobrist;

        public CastleInfo(
            int kingOrigin,
            int kingDestination,
            int rookOrigin,
            int rookDestination,
            int color)
        {
            KingOrigin = kingOrigin;
            KingDestination = kingDestination;
            RookOrigin = rookOrigin;
            RookDestination = rookDestination;
            KingBitboard = Bitboards.SquareBitboards[kingOrigin] | Bitboards.SquareBitboards[kingDestination];
            RookBitboard = Bitboards.SquareBitboards[rookOrigin] | Bitboards.SquareBitboards[rookDestination];

            KingZobrist = ZobristHash.PieceHashes[color][Board.KING][kingOrigin] ^
                ZobristHash.PieceHashes[color][Board.KING][kingDestination];
            RookZobrist = ZobristHash.PieceHashes[color][Board.ROOK][rookOrigin] ^
                ZobristHash.PieceHashes[color][Board.ROOK][rookDestination];
        }
    }
}
