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
        private Node[] table;
        private readonly int capacity;
        private readonly int shift;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RepetitionTable()
        {
            capacity = (int)Math.Pow(2, 10);
            shift = 64 - 10;
            table = new Node[capacity];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            table = new Node[capacity];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AddPosition(ulong key)
        {
            int index = (int)(key >> shift);
            if (table[index] == null)
            {
                table[index] = new Node(key);
                return 1;
            }
            else
            {
                Node curr = table[index];
                if (curr.Key == key)
                    return ++curr.Count;
                while (curr.Next != null)
                {
                    if (curr.Next.Key == key)
                    {
                        return ++curr.Next.Count;
                    }
                    curr = curr.Next;
                }
                curr.Next = new Node(key);
                return 1;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemovePosition(ulong key)
        {
            int index = (int)(key >> shift);
            Node curr = table[index];
            if (curr == null)
                return;
            else if (curr.Key == key)
            {
                if (curr.Count > 1)
                {
                    curr.Count--;
                    return;
                }
                else
                {
                    table[index] = curr.Next;
                    return;
                }
            }
            Node next = curr.Next;
            while (next != null)
            {
                if (next.Key == key)
                {
                    if (next.Count > 1)
                    {
                        next.Count--;
                        return;
                    }
                    else
                    {
                        curr.Next = next.Next;
                        return;
                    }
                }
                curr = curr.Next;
                next = next.Next;   
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetCount(ulong key)
        {
            int index = (int)(key >> shift);
            Node curr = table[index];
            while (curr != null)
            {
                if (curr.Key == key)
                    return curr.Count;
                curr = curr.Next;
            }
            return 0;
        }

        private class Node
        {
            public ulong Key;
            public Node Next;
            public int Count;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Node(ulong key)
            {
                Key = key;
                Count = 1;
            }

        }
    }
}
