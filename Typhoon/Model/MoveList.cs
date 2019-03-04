using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Typhoon.Model
{
    public class MoveList
    {
        private Move[] moves;
        private int count;
        public int Count { get { return count; } }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public MoveList()
        {
            moves = new Move[40];
            count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Move Get(int index)
        {
            return moves[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SwapPvNode(Move pvMove)
        {
            for (int i = 0; i < count; i++)
            {
                if (moves[i] == pvMove)
                {
                    moves[i] = moves[0];
                    moves[0] = pvMove;
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Move move)
        {
            if (Count == moves.Length)
                ResizeArray();
            moves[count++] = move;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResizeArray()
        {
            int len = moves.Length;
            Move[] newArray = new Move[len + 40];
            Array.Copy(moves, newArray, len);
            moves = newArray;
        }

        public Move[] ToArray()
        {
            Move[] array = new Move[Count];
            Array.Copy(moves, array, Count);
            return array;
        }
    }
}
