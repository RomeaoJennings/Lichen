using System;
using System.Collections.Generic;
using Typhoon.Model;
using UiComponents;

namespace UserInterface
{
    using Bitboard = UInt64;

    public class ChessBoardController
    {

        private readonly ChessBoardView view;
        private readonly Board model;

        private int selectedSquare = -1;
        private readonly Stack<BoardState> previousStates = new Stack<BoardState>();

        public ChessBoardController(ChessBoardView theView, Board theModel)
        {
            view = theView;
            view.SquareClicked += HandleClickedSquare;
            model = theModel;
            SetAllSquares();
        }

        public void UndoMove()
        {
            if (previousStates.Count > 0)
            {
                model.UndoMove(previousStates.Pop());
                view.ResetHighlights();
                SetAllSquares();
            }
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
                if (mv.DestinationSquare() == square)
                {
                    view.ResetHighlights();
                    BoardState bs = new BoardState(model.CastleRights, model.EnPassentBitboard, mv);
                    previousStates.Push(bs);
                    if (mv.PromotionType() != Board.EMPTY)
                    {
                        PromotionDialog pd = new PromotionDialog();
                        pd.ShowDialog();
                        model.DoMove(new Move(mv.OriginSquare(), mv.DestinationSquare(), mv.CapturePiece(), pd.PromotionType));
                    }
                    else
                    {
                        model.DoMove(mv);
                    }

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
                view.SetHighlight(m.DestinationSquare());
            }
        }
        private List<Move> GetMovesByOrigin(int square)
        {
            Bitboard pinnedBitboard = model.GetPinnedPiecesBitboard();
            return model.GetAllMoves().FindAll(m => model.IsLegalMove(m,pinnedBitboard) && m.OriginSquare() == square);
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
