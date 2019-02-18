using System.Windows.Forms;

namespace UserInterface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ChessBoardController cbc = new ChessBoardController(boardView, new Typhoon.Model.Board());
        }
    }
}
