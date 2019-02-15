using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitboardVisualizer
{


    public partial class BitboardGrid : UserControl
    {
        public BitboardGrid()
        {
            InitializeComponent();
        }
        private UInt64 _bitboard;
        public UInt64 Bitboard
        {
            get
            {
                return _bitboard;
            }
            set
            {
                _bitboard = value;
                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            float gridSize = Math.Min(Width, Height);
            
            float cellSize = gridSize / 8.1F;
            float textSize = cellSize * 0.3F;
            Font font = new Font("Arial", textSize);
            float top = 0;
            float left = 0;
            for (int i = 63; i >= 0; i--)
            {
                bool bitSet = 0 != ((1UL << i) & _bitboard);
                Brush brush = bitSet ? Brushes.Gray : Brushes.White;
                e.Graphics.FillRectangle(brush, left, top, cellSize, cellSize);
                e.Graphics.DrawRectangle(Pens.Black, left, top, cellSize, cellSize);
                var size = e.Graphics.MeasureString(i.ToString(), font);
                e.Graphics.DrawString(i.ToString(), font, Brushes.Blue, left + ((cellSize - size.Width) / 2F), top + ((cellSize - size.Height) / 2F));
                left += cellSize;
                if (i % 8 == 0)
                {
                    top += cellSize;
                    left = 0;
                }
            }

        }
    }
}
