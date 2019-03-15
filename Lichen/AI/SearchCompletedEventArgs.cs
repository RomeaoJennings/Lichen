using System;

using Lichen.Model;

namespace Lichen.AI
{
    public class SearchCompletedEventArgs : EventArgs
    {
        public int Ply { get; private set; }
        public int Score { get; private set; }
        public Move[] PrincipalVariation { get; private set; }
        public long NodesPerSecond { get; private set; }
        public long Nodes { get; private set; }
        public int HashFull { get; private set; }
        public bool IsMateScore { get; private set; }

        public SearchCompletedEventArgs(int ply, int score, bool isMate, Move[] principalVariation, long nodes, long nps, int hashFullPercent)
        {
            Ply = ply;
            Score = score;
            PrincipalVariation = principalVariation;
            Nodes = nodes;
            NodesPerSecond = nps;
            HashFull = hashFullPercent;
            IsMateScore = isMate;
        }
    }
}
