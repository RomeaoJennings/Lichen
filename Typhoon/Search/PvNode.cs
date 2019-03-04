using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typhoon.Model;

namespace Typhoon.Search
{
    public class PvNode
    {
        public PvNode Next;
        public Move Move;

        public PvNode(Move move, PvNode next)
        {
            Next = next;
            Move = move;
        }

        public PvNode(Move move)
        {
            Move = move;
            Next = null;
        }

        public PvNode()
        {
            Move = new Move();
            Next = null;
        }
    }
}
