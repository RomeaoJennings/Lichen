using System;
using System.Drawing;
using System.Windows.Forms;

namespace UiComponents
{
    public partial class PromotionDialog : Form
    {
        public int PromotionType { get; set; }

        public PromotionDialog()
        {
            InitializeComponent();
            Font font = new Font(ChessFonts.Magnetic, btnQueen.Font.Size);
            btnQueen.Font = font;
            btnRook.Font = font;
            btnBishop.Font = font;
            btnKnight.Font = font;
        }

        private void Promotion_Click(object sender, EventArgs e)
        {
            if (sender == btnQueen)
            {
                PromotionType = 1;
            }
            else if (sender == btnRook)
            {
                PromotionType = 2;
            }
            else if (sender == btnBishop)
            {
                PromotionType = 3;
            }
            else
            {
                PromotionType = 4;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
