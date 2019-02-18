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

        public CastleInfo(
            int kingOrigin,
            int kingDestination,
            int rookOrigin,
            int rookDestination,
            Bitboard kingBitboard,
            Bitboard rookBitboard)
        {
            KingOrigin = kingOrigin;
            KingDestination = kingDestination;
            RookOrigin = rookOrigin;
            RookDestination = rookDestination;
            KingBitboard = kingBitboard;
            RookBitboard = rookBitboard;
        }
    }
}
