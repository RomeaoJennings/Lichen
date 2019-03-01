using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    public class RepetitionTable
    {
        private readonly Dictionary<ulong, byte> dictionary = new Dictionary<ulong, byte>(100000);
        private long cntr = 0;
        public int AddPosition(ulong key)
        {
         //   Debug.Write($"Adding Position: {key} - Count: ");
            cntr++;
            byte value;
            if (dictionary.TryGetValue(key, out value))
            {
                value++;
                dictionary[key] = value;
                //Debug.WriteLine($"{value} - Total Nodes: {cntr}");
                return value;
            }
            else
            {
                dictionary[key] = 1;
                //Debug.WriteLine($"1 - Total Nodes: {cntr}");
                return 1;
            }
        }

        public int GetCount(ulong key)
        {
            byte value = 0;
            dictionary.TryGetValue(key, out value);
            return value;
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public void RemovePosition(ulong key)
        {
            byte value = 0;
            cntr--;
            if (dictionary.TryGetValue(key, out value))
            {
                if (value == 1)
                    dictionary.Remove(key);
                else
                    dictionary[key] = --value;
            }
           // Debug.WriteLine($"Removing Key: {key} - Count: {value} - TOTAL: {cntr}");
        }
    }
}
