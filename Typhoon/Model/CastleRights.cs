using System;
using System.Text;

namespace Lichen.Model
{
    public struct CastleRights
    {
        public readonly bool WhiteKing;
        public readonly bool WhiteQueen;
        public readonly bool BlackKing;
        public readonly bool BlackQueen;

        public static CastleRights All { get { return new CastleRights(true, true, true, true); } }

        public CastleRights(bool whiteKing, bool whiteQueen, bool blackKing, bool blackQueen)
        {
            WhiteKing = whiteKing;
            WhiteQueen = whiteQueen;
            BlackKing = blackKing;
            BlackQueen = blackQueen;
        }

        public CastleRights(CastleRights copy)
        {
            WhiteKing = copy.WhiteKing;
            WhiteQueen = copy.WhiteQueen;
            BlackKing = copy.BlackKing;
            BlackQueen = copy.BlackQueen;
        }

        public static CastleRights FromFen(string fen)
        {
            bool whiteKing = fen.Contains("K");
            bool whiteQueen = fen.Contains("Q");
            bool blackKing = fen.Contains("k");
            bool blackQueen = fen.Contains("q");
            return new CastleRights(whiteKing, whiteQueen, blackKing, blackQueen);
        }

        public string ToFen()
        {
            StringBuilder sb = new StringBuilder();
            if (WhiteKing)
                sb.Append("K");
            if (WhiteQueen)
                sb.Append("Q");
            if (BlackKing)
                sb.Append("k");
            if (BlackQueen)
                sb.Append("q");
            if (sb.Length > 0)
            {
                sb.Append(" ");
                return sb.ToString();
            }
            else
            {
                return "- ";
            }
        }

        public static bool operator ==(CastleRights c1, CastleRights c2)
        {
            return c1.WhiteKing == c2.WhiteKing &&
                c1.WhiteQueen == c2.WhiteQueen &&
                c1.BlackKing == c2.BlackKing &&
                c1.BlackQueen == c2.BlackQueen;
        }

        public static bool operator !=(CastleRights c1, CastleRights c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(object obj)
        {
            if (obj is CastleRights)
            {
                return (CastleRights)obj == this;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return
                (Convert.ToInt32(WhiteKing) + 1 << 28) |
                (Convert.ToInt32(WhiteQueen) + 1 << 20) |
                (Convert.ToInt32(BlackKing) + 1 << 12) |
                (Convert.ToInt32(BlackQueen) + 1 << 4);
        }
    }
}
