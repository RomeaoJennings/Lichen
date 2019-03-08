using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lichen.AI
{
    public class SearchCompletedEventArgs : EventArgs
    {
        public int Ply { get; private set; }
        public int Score { get; private set; }
        public PvNode PrincipalVariation { get; private set; }
        public long NodesPerSecond { get; private set; }
        public long Nodes { get; private set; }
        public int HashFull { get; private set; }
        public SearchCompletedEventArgs(int ply, int score, PvNode principalVariation, long nodes, long nps, int hashFullPercent)
        {
            Ply = ply;
            Score = score;
            PrincipalVariation = principalVariation;
            Nodes = nodes;
            NodesPerSecond = nps;
            HashFull = hashFullPercent;
        }
    }
}
