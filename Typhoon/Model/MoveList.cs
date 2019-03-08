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
        public bool SwapMove(Move move, int destinationIndex, int startIndex)
        {
            for (int i=startIndex;i<count;i++)
            {
                if (moves[i] == move)
                {
                    moves[i] = moves[destinationIndex];
                    moves[destinationIndex] = move;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(Move? pvMove, Move? hashMove)
        {
            int cntr = 0;
            if (pvMove != null && SwapMove((Move)pvMove, cntr, cntr))
            {
                cntr++;
            }
            if (hashMove != null && SwapMove((Move)hashMove, cntr, cntr))
            {
                cntr++;
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
