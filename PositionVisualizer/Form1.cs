using System.Windows.Forms;
using Typhoon.Model;

namespace UserInterface
{
    public partial class Form1 : Form
    {
        ChessBoardController cbc;
        public Form1()
        {
            InitializeComponent();
            cbc = new ChessBoardController(boardView, Position.FromFen("8/8/3p4/1Pp3kr/RK3p2/8/4P1P1/8 w - c6 0 3"));
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            cbc.UndoMove();
        }
    }
}
