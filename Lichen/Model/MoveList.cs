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
        public void Sort(Move? hashMove, Move k0, Move k1)
        {
            int cntr = 0;
            if (hashMove != null)
            {
                for (int i = cntr; i < count; i++)
                {
                    if (moves[i] == hashMove)
                    {
                        moves[i] = moves[cntr];
                        moves[cntr++] = (Move)hashMove;
                        break;
                    }
                }
            }
            for (int i = cntr; i < count; i++)
            {
                if (moves[i] == k0)
                {
                    moves[i] = moves[cntr];
                    moves[cntr++] = k0;
                    break;
                }
            }
            for (int i = cntr; i < count; i++)
            {
                if (moves[i] == k1)
                {
                    moves[i] = moves[cntr];
                    moves[cntr] = k1;
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
