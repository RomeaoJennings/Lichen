using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.AI
{
    public class SearchCompletedEventArgs : EventArgs
    {
        public int Ply { get; private set; }
        public int Score { get; private set; }
        public PvNode PrincipalVariation { get; private set; }
        public int NodesPerSecond { get; private set; }
        public int Nodes { get; private set; }

        public SearchCompletedEventArgs(int ply, int score, PvNode principalVariation, int nodes, int nps)
        {
            Ply = ply;
            Score = score;
            PrincipalVariation = principalVariation;
            Nodes = nodes;
            NodesPerSecond = nps;
        }
    }
}
