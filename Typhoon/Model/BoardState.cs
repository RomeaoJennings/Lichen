using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    using Bitboard = UInt64;

    public struct BoardState
    {
        public CastleRights CastleRights;
        public Bitboard EnPassentBitboard;
        public Move Move;
        public ulong Zobrist;

        public BoardState(CastleRights castleRights, Bitboard enPassentBitboard, Move move, ulong zobrist)
        {
            CastleRights = castleRights;
            EnPassentBitboard = enPassentBitboard;
            Move = move;
            Zobrist = zobrist;
        }
    }
}
