using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Typhoon.Model;

namespace Typhoon
{
    static class Program
    {
        private const string engineName = "Typhoon 1.0.0";
        private const string author = "Romeao Jennings";

        static void Main(string[] args)
        {
            MainUCILoop();
        }

        static void MainUCILoop()
        {
            Console.WriteLine("Typhoon v. 1.0.0 by Romeao Jennings");
            Position position = new Position();
            do
            {
                string line = Console.ReadLine();
                string[] elements = line.Split(' ');
                if (elements.Length == 0)
                    continue;
                switch (elements[0])
                {
                    case "uci":
                        DoUciInit();
                        break;
                    case "d":
                        DisplayPosition(position);
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {elements[0]}");
                        break;
                }
            } while (true);
        }

        static void DoUciInit()
        {
            Console.WriteLine($"id name {engineName}");
            Console.WriteLine($"id author {author}");
            SendOptions();
            Console.WriteLine("uciok");
        }

        static void SendOptions()
        {
            // TODO: Add available engine options.
        }

        static void DisplayPosition(Position position)
        {
            Console.WriteLine(position.Print());
            Console.WriteLine($"FEN: {position.ToFen()}");
            Console.WriteLine($"Zobrist Hash: {position.Zobrist}");
        }
    }
}
