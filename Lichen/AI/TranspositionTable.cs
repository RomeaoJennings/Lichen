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
        private TableEntry[] replaceDeeper;
        private TableEntry[] replaceAlways;

        private readonly ulong count;
        private ulong filledEntries;
        private ushort searchNum;

        public TranspositionTable(ulong numEntries = 2000000) 
        {
            numEntries += numEntries % 2; // Ensure an even number.
            count = numEntries / 2;
            replaceDeeper = new TableEntry[count];
            replaceAlways = new TableEntry[count];
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
            ulong index = key % count;
            entry = replaceDeeper[index];
            if (entry != null && key == entry.Key)
            {
                return true;
            }
            entry = replaceAlways[index];
            if (entry != null && key == entry.Key)
            {
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntry(TableEntry entry)
        {
            ulong index = entry.Key % count;
            entry.SearchNumber = searchNum;

            if (replaceDeeper[index] == null)
            {
                filledEntries++;
                replaceDeeper[index] = entry;
                return;
            }
            // Replace old entries and ones that are shallower than the new entry
            if (replaceDeeper[index].SearchNumber < searchNum || replaceDeeper[index].Depth < entry.Depth)
            {
                replaceDeeper[index] = entry;
                return;
            }
            // If we get here, we are replacing the always replace bucket.
            if (replaceAlways[index] == null)
            {
                filledEntries++;
            }
            replaceAlways[index] = entry;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddEntry(ulong key, int score, NodeType nodeType, int depth, Move bestMove)
        {
            AddEntry(new TableEntry(key, score, nodeType, depth, bestMove));
        }

        public int TableUsage {
            get
            {
                return (int)((filledEntries * 500UL) / count);
            }
        }

        public void Clear()
        {
            replaceDeeper = new TableEntry[count];
            replaceAlways = new TableEntry[count];
            filledEntries = 0;
            searchNum = 0;
        }        
    }
}
