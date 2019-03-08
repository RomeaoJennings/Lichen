using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lichen.AI;

namespace Lichen
{
    static class Program
    { 
        static void Main(string[] args)
        {
            UciController controller = new UciController();
            controller.UciMainLoop();
        }
    }
}
