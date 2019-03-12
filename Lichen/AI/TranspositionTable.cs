using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private ushort searchNum;

        public TranspositionTable(ulong numEntries = 10000000) 
        {
            entries = new TableEntry[numEntries];
            count = numEntries;
            filledEntries = 0;
            searchNum = 0;
        }

        public void NewSearch()
        {
            searchNum++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool GetEntry(ulong key, out TableEntry entry)
        {
            entry = entries[key % count];
            return entry != null && key == entry.Key;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntry(TableEntry entry)
        {
            ulong index = entry.Key % count;
            if (entries[index] == null)
            {
                filledEntries++;
            }
            else if (entries[index].SearchNumber == searchNum && entries[index].Depth > entry.Depth) // If existing entry with higher depth from same search, do not replace.
            {
                return;
            }
            entry.SearchNumber = searchNum;
            entries[index] = entry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntry(ulong key, int score, NodeType nodeType, int depth, Move bestMove)
        {
            AddEntry(new TableEntry(key, score, nodeType, depth, bestMove));
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
            searchNum = 0;
        }        
    }
}
