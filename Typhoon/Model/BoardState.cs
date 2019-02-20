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
        public CastleRights CastleRights { get; private set; }
        public Bitboard EnPassentBitboard { get; private set; }
        public Move Move { get; private set; }

        public BoardState(CastleRights castleRights, Bitboard enPassentBitboard, Move move)
        {
            CastleRights = castleRights;
            EnPassentBitboard = enPassentBitboard;
            Move = move;
        }
    }
}
