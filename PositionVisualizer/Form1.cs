using System.Windows.Forms;
using Typhoon.Model;
using Typhoon.Search;

namespace UserInterface
{
    public partial class Form1 : Form
    {
        ChessBoardController cbc;
        Position position = new Position();
        public Form1()
        {
            InitializeComponent();
            cbc = new ChessBoardController(boardView, position);
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            cbc.UndoMove();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            Search search = new Search();
            MessageBox.Show(search.IterativeDeepening((int)numericUpDown1.Value, position).ToString());
        }
    }
}
