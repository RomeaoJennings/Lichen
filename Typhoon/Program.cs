using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Typhoon.AI;

namespace Typhoon
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
