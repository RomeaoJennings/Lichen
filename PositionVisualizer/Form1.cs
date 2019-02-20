using System.Windows.Forms;

namespace UserInterface
{
    public partial class Form1 : Form
    {
        ChessBoardController cbc;
        public Form1()
        {
            InitializeComponent();
            cbc = new ChessBoardController(boardView, new Typhoon.Model.Board());
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            cbc.UndoMove();
        }
    }
}
