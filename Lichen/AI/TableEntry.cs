
using Lichen.Model;

namespace Lichen.AI
{
    public class TableEntry
    {
        public ulong Key;
        public int Score;
        public NodeType NodeType;
        public int Depth;
        public Move BestMove;
        public ushort SearchNumber;

        public TableEntry(ulong key, int score, NodeType type, int depth, Move bestMove)
        {
            Key = key;
            Score = score;
            NodeType = type;
            Depth = depth;
            BestMove = bestMove;
        }

        public TableEntry(ulong key, int depth)
        {
            Key = key;
            Depth = depth;
            NodeType = NodeType.UpperBound;
        }
    }
}
