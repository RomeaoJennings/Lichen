using System;
using System.Collections.Generic;
using Typhoon.Model;

namespace UserInterface
{
    using Bitboard = UInt64;

    public class ChessBoardController
    {

        private readonly ChessBoardView view;
        private readonly Board model;

        private int selectedSquare = -1;

        public ChessBoardController(ChessBoardView theView, Board theModel)
        {
            view = theView;
            view.SquareClicked += HandleClickedSquare;
            model = theModel;
            SetAllSquares();
        }

        private void HandleClickedSquare(object sender, SquareClickedEventArgs e)
        {
            int square = e.Square;
            if (selectedSquare == square)
            {
                selectedSquare = -1;
                view.ResetHighlights();
                return;
            }
            // If no square is clicked, see if the clicked square piece has available moves
            else if (selectedSquare == -1)
            {
                List<Move> moves = GetMovesByOrigin(square);
                if (moves.Count == 0)
                {
                    return;
                }
                selectedSquare = square;
                HighlightDestinations(moves);
                view.SetHighlight(square);
            }

            // Check of clicked square completes a move
            foreach (var mv in GetMovesByOrigin(selectedSquare))
            {
                if (mv.DestinationSquare == square)
                {
                    view.ResetHighlights();
                    model.DoMove(mv);
                    SetAllSquares();
                    selectedSquare = -1;
                    return;
                }
            }

            var m = GetMovesByOrigin(square);
            if (m.Count != 0)
            {
                view.ResetHighlights();
                selectedSquare = square;
                view.SetHighlight(square);
                HighlightDestinations(m);
            }
        }
        private void HighlightDestinations(List<Move> moves)
        {
            foreach (var m in moves)
            {
                view.SetHighlight(m.DestinationSquare);
            }
        }
        private List<Move> GetMovesByOrigin(int square)
        {
            return model.GetMoves().FindAll(m => m.OriginSquare == square);
        }

        public void SetAllSquares()
        {
            view.ResetSquares();
            for (int color = 0; color < 2; color++)
            {
                for (int piece = 0; piece < 6; piece++)
                {
                    Bitboard b = model.GetPieceBitboard(color, piece);
                    while (b != 0)
                    {
                        int square = b.BitScanForward();
                        Bitboards.PopLsb(ref b);
                        view.SetPiece(square, color, piece);
                    }
                }
            }
        }
    }
}
