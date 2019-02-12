using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    public struct Move
    {
        public int SourceSquare;
        public int DestinationSquare;

        public Move(int source, int destination)
        {
            SourceSquare = source;
            DestinationSquare = destination;
        }
    }
}
