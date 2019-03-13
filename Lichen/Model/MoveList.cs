using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lichen.Model
{
    public class MoveList
    {
        private readonly int[] pieceValues = { 0, 900, 500, 350, 325, 100 };
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
        public void SwapMove(Move? move, ref int index)
        {
            if (move != null)
            {
                for (int i = index; i < count; i++)
                {
                    if (moves[i] == move)
                    {
                        moves[i] = moves[index];
                        moves[index++] = (Move)move;
                        return;
                    }
                }
            }
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(IComparer<Move> comparer, Move? hashMove, Move? killer0, Move? killer1)
        {
            int index = 0;
            SwapMove(hashMove, ref index);
            SwapMove(killer0, ref index);
            SwapMove(killer1, ref index);

            Array.Sort(moves, index, count - index, comparer);
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
