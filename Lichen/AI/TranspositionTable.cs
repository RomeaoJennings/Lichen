using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lichen.Model;

namespace Lichen.AI
{
    public class TranspositionTable
    {
        private TableEntry[] entries;
        private readonly ulong count;
        private ulong filledEntries;

        public TranspositionTable(ulong numEntries = 10000000) 
        {
            entries = new TableEntry[numEntries];
            count = numEntries;
            filledEntries = 0;
        }

        public bool GetEntry(ulong key, out TableEntry entry)
        {
            entry = entries[key % count];
            return entry != null && key == entry.Zobrist;
        }

        public void AddEntry(ulong key, int score, NodeType nodeType, int depth, Move bestMove)
        {
            ulong index = key % count;
            if (entries[index] == null)
            {
                filledEntries++;

            }
            else if (entries[index].Depth > depth) // If existing entry with higher depth, do not replace.
            {
                
                return;
            }
            entries[index] = new TableEntry(key, score, nodeType, depth, bestMove);
        }

        public int TableUsage {
            get
            {
                return (int)((filledEntries * 10000UL) / count);
            }
        }

        public void Clear()
        {
            entries = new TableEntry[count];
            filledEntries = 0;
        }        
    }
}
