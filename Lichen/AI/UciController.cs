using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lichen.Model;

namespace Lichen.AI
{
    public class UciController
    {
        private Position position;
        private readonly BaseSearch search;

        const string engineName = "Lichen v. 1.0.0";
        const string author = "Romeao Jennings";

        public UciController()
        {
            position = new Position();
            search = new BaseSearch();
            search.IterationCompleted += SendIterationInfo;
            search.SearchCompleted += BestMove;

        }

        public void UciMainLoop()
        {
            Console.WriteLine($"{engineName} by {author}");
            bool quit = false;
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
                    case "go":
                        if (elements[1] == "perft")
                        {
                            if (elements.Length > 2)
                            {
                                GoPerft(elements[2]);
                            }
                            else
                            {
                                Console.WriteLine("Bad Perft command.  Must provide depth.");
                            }
                        }
                        else
                        {
                            DoSearch(elements);
                        }
                        break;
                    case "isready":
                        IsReady();
                        break;
                    case "quit":
                        quit = true;
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {elements[0]}");
                        break;
                }
            } while (!quit);
        }

        private void GoPerft(string depthStr)
        {
            int depth;
            if (int.TryParse(depthStr, out depth) && depth > 0)
            {
                position.Perft(depth);
            }
            else
            {
                Console.WriteLine("Invalid Perft depth.");
            }
        }

        private void IsReady()
        {
            Console.WriteLine("readyok");
        }

        private void DoSearch(string[] elements)
        {
            int maxPly;
            if (elements.Length > 2 && elements[1] == "depth" && int.TryParse(elements[2], out maxPly) && maxPly > 0)
            {
                search.IterativeDeepening(maxPly, position);
            }
            else
            {
                search.IterativeDeepening(9, position);
            }
        }

        private void BestMove(object sender, SearchCompletedEventArgs e)
        {
            Console.WriteLine($"bestmove {e.PrincipalVariation[0]}");
        }

        private void SendIterationInfo(object sender, SearchCompletedEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            message.Append("info depth ");
            message.Append(e.Ply);
            message.Append(" score ");
            if (e.IsMateScore)
            {
                message.Append("mate ");
            }
            else
            {
                message.Append("cp ");
            }
            message.Append(e.Score);
            message.Append(" nodes ");
            message.Append(e.Nodes);
            message.Append(" nps ");
            message.Append(e.NodesPerSecond);
            message.Append(" hashfull ");
            message.Append(e.HashFull);
            message.Append(" pv");
            foreach (Move move in e.PrincipalVariation)
            {
                message.Append(" ");
                message.Append(move);
            }
            Console.WriteLine(message.ToString());
        }

        private void LoadPosition(string command)
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

        private void DoUciInit()
        {
            Console.WriteLine($"id name {engineName}");
            Console.WriteLine($"id author {author}");
            SendOptions();
            Console.WriteLine("uciok");
        }

        private void SendOptions()
        {
            // TODO: Add available engine options.
        }

        private void DisplayPosition(Position position)
        {
            Console.WriteLine(position.Print());
            Console.WriteLine($"FEN: {position.ToFen()}");
            Console.WriteLine($"Zobrist Hash: {position.Zobrist}");
        }
    }
}
