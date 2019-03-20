using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lichen.Model;

namespace Lichen.AI
{
    class TimeController
    {
        private Dictionary<string, int> dictionary = new Dictionary<string, int>();

        public static TimeController FromUciCommand(string uciString)
        {
            TimeController controller = new TimeController();
            string[] elements = uciString.Split(' ');
            

            for (int i=1;i<elements.Length;i+=2)
            {
                controller.dictionary.Add(elements[i], int.Parse(elements[i + 1]));
            }
            return controller;
        }

        public int GetTimeToSearch(Position p)
        {
            string[] incrementKeys = { "winc", "binc" };
            string[] timeKeys = { "wtime", "btime" };
            int playerToMove = p.PlayerToMove;
            int moveNumber = p.FullMoveNumber;

            int time = 0;
            dictionary.TryGetValue(incrementKeys[playerToMove], out time);
            if (dictionary.ContainsKey(timeKeys[playerToMove]))
            {
                int mainTime = dictionary[timeKeys[playerToMove]];
                time += mainTime / Math.Max(15, 50 - moveNumber);
                if (time > mainTime - 50)
                {
                    time = mainTime - 50;
                }
            }
            return time;
        }
    }
}
