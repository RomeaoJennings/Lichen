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
        public int HalfMoveClock;

        public BoardState(
            CastleRights castleRights, 
            Bitboard enPassentBitboard,
            Move move, 
            ulong zobrist, 
            int halfMoveClock)
        {
            CastleRights = castleRights;
            EnPassentBitboard = enPassentBitboard;
            Move = move;
            Zobrist = zobrist;
            HalfMoveClock = halfMoveClock;
        }

        public BoardState(Move move, Position board)
        {
            CastleRights = board.CastleRights;
            EnPassentBitboard = board.EnPassentBitboard;
            Move = move;
            Zobrist = board.Zobrist;
            HalfMoveClock = board.HalfMoveClock;
        }
    }
}
