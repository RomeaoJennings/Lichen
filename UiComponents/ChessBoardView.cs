using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace UiComponents
{
    public partial class ChessBoardView : UserControl
    {

        public event EventHandler<SquareClickedEventArgs> SquareClicked;


        private Color lightSquares = Color.White;
        private Color darkSquares = Color.LightSeaGreen;
        private Color highlight = Color.FromArgb(200, Color.Orange);

        private const int EMPTY = -1;
        private const int WHITE = 0;
        private const int BLACK = 6;

        private const int KING = 0;
        private const int QUEEN = 1;
        private const int ROOK = 2;
        private const int BISHOP = 3;
        private const int KNIGHT = 4;
        private const int PAWN = 5;

        private readonly int[] squares = new int[64];
        private readonly bool[] highlightedSquares = new bool[64];

        private const string fontMapping = "kqrbnplwtvmo";

        public Color LightSquares
        {
            get { return lightSquares; }
            set
            {
                lightSquares = value;
                Invalidate();
            }
        }

        public Color DarkSquares
        {
            get { return darkSquares; }
            set
            {
                darkSquares = value;
                Invalidate();
            }
        }

        public Color HighlightColor
        {
            get
            {
                return highlight;
            }
            set
            {
                highlight = value;
                Invalidate();
            }
        }

        public void ResetSquares()
        {
            for (int i = 0; i < 64; i++)
            {
                squares[i] = EMPTY;
            }
        }


        public ChessBoardView()
        {
            ResetSquares();
            InitializeComponent();
        }


        private void OnSquareClicked(int square)
        {
            if (SquareClicked != null)
            {
                SquareClicked(this, new SquareClickedEventArgs(square));
            }

        }

        public void ResetHighlights()
        {
            for (int i = 0; i < 64; i++)
            {
                highlightedSquares[i] = false;
            }
            Invalidate();
        }

        public void ResetHighlight(int square)
        {
            highlightedSquares[square] = false;
            Invalidate();
        }
        public void SetHighlight(int square)
        {
            highlightedSquares[square] = true;
            Invalidate();
        }

        public int GetPieceType(int square)
        {
            int piece = squares[square];
            return piece > PAWN ? piece - PAWN : piece;
        }

        public int GetPieceColor(int square)
        {
            return squares[square] > PAWN ? BLACK : WHITE;
        }

        public void SetPiece(int square, int color, int pieceType)
        {
            Debug.Assert(square >= 0 && square < 64);
            Debug.Assert(color == 0 || color == 1);
            Debug.Assert(pieceType >= EMPTY && pieceType <= PAWN);
            if (pieceType == EMPTY)
            {
                squares[square] = EMPTY;
            }
            else
            {
                squares[square] = color * 6 + pieceType;
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            float squareSize = Math.Min(Width, Height) / 8.3F;
            float gridSize = squareSize * 8;
            float left = (Width - gridSize) / 2;
            float top = (Height - gridSize) / 2;
            float curr = left;

            Font font = new Font(ChessFonts.Magnetic, squareSize * .6F);

            Brush[] brushes = { new SolidBrush(lightSquares), new SolidBrush(darkSquares) };
            int brushIndex = 0;
            for (int i = 63; i >= 0; --i)
            {


                g.FillRectangle(brushes[brushIndex], curr, top, squareSize, squareSize);



                if (squares[i] != EMPTY)
                {
                    int square = squares[i];
                    string pieceMask = fontMapping[square].ToString();
                    if (square < 6)
                    {
                        pieceMask = fontMapping[square + 6].ToString();
                    }
                    string piece = fontMapping[square].ToString();


                    var size = g.MeasureString(piece, font);
                    float pieceLeft = (3 + squareSize - size.Width) / 2;
                    float pieceTop = (5 + squareSize - size.Height) / 2;

                    g.DrawString(pieceMask, font, Brushes.White, curr + pieceLeft, top + pieceTop);
                    g.DrawString(piece, font, Brushes.Black, curr + pieceLeft, top + pieceTop);
                }
                if (highlightedSquares[i])
                {
                    g.FillRectangle(new SolidBrush(highlight), curr, top, squareSize, squareSize);
                }
                g.DrawRectangle(Pens.Black, curr, top, squareSize, squareSize);
                if (i % 8 == 0)
                {
                    top += squareSize;
                    curr = left;
                }
                else
                {
                    curr += squareSize;
                    brushIndex = (brushIndex + 1) % 2;
                }
            }
        }

        private void ChessBoardView_MouseClick(object sender, MouseEventArgs e)
        {
            float squareSize = Math.Min(Width, Height) / 8.3F;
            float gridSize = squareSize * 8;
            float left = (Width - gridSize) / 2;
            float top = (Height - gridSize) / 2;

            int x = e.X;
            int y = e.Y;

            if (x < left ||
                x > left + gridSize ||
                y < top ||
                y > top + gridSize)
            {
                return;
            }

            int column = 7 - (int)((x - left) / squareSize);
            int row = 7 - (int)((y - top) / squareSize);
            OnSquareClicked(row * 8 + column);
        }
    }
}
