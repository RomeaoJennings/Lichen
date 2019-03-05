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

        private static Position position;

        static void Main(string[] args)
        {
            position = new Position();
            MainUCILoop();
        }

        static void MainUCILoop()
        {
            Console.WriteLine("Typhoon v. 1.0.0 by Romeao Jennings");
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
                    case "position":
                        LoadPosition(line);
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

        static void LoadPosition(string command)
        {
            string[] elements = command.Split(' ');
            int moves = command.IndexOf("moves");

            // The position command can either provide "fen" or "startpos" for the base board, followed by 
            // zero or more moves.
            if (elements[1] == "startpos")
            {
                position = new Position();
            }
            else if (elements[1] == "fen")
            {
                string fenStr;
                const int positionFenStrLength = 13;
                if (moves == -1)
                {
                    fenStr = command.Substring(positionFenStrLength);
                }
                else
                {
                    fenStr = command.Substring(positionFenStrLength, moves - positionFenStrLength);
                }
                position = Position.FromFen(fenStr);
            }
            // process move list, if it exists
            if (moves != -1)
            {
                string[] moveList = command.Substring(moves + 6).Split(' ');
                position.ApplyUciMoveList(moveList);
            }
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
