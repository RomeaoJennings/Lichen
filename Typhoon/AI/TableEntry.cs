
using Typhoon.Model;

namespace Typhoon.AI
{
    public class TableEntry
    {
        public ulong Zobrist;
        public int Score;
        public NodeType NodeType;
        public int Depth;
        public Move BestMove;

        public TableEntry(ulong key, int score, NodeType type, int depth, Move bestMove)
        {
            Zobrist = key;
            Score = score;
            NodeType = type;
            Depth = depth;
            BestMove = bestMove;
        }
    }
}
