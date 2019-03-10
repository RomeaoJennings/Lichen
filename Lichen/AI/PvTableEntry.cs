

using Lichen.Model;

namespace Lichen.AI
{
    public struct PvTableEntry
    {
        public int Score;
        public int Depth;
        public Move Move;
       

        public PvTableEntry(Move move, int score, int depth)
        {
            Score = score;
            Move = move;
            Depth = depth;
        }
    }
}
