using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lichen.Model;

namespace Lichen.AI
{
    public class PvTable
    {
        private Dictionary<ulong, PvTableEntry> dictionary;
        private readonly int count;

        public PvTable(int size = 10000)
        {
            dictionary = new Dictionary<ulong, PvTableEntry>(size);
            count = size;
        }

        public bool GetEntry(ulong key, out PvTableEntry entry)
        {
            return dictionary.TryGetValue(key, out entry);
        }

        public void AddEntry(ulong key, Move move, int score, int depth)
        {
            PvTableEntry entry;
            if (GetEntry(key, out entry))
            {
                if (entry.Depth > depth)
                {
                    Console.WriteLine("Did not overrite entry");
                    return;
                }
                else
                    Console.WriteLine("Overriting existing entry");
            }
            dictionary[key] = new PvTableEntry(move, score, depth);
        }

        public void Clear()
        {
            dictionary = new Dictionary<ulong, PvTableEntry>(count);
        }

    }
}
