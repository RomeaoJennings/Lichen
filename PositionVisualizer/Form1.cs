using System.Windows.Forms;
using Lichen.Model;
using Lichen.AI;
using System.Text;
using System.Threading;

namespace UserInterface
{
    delegate void UpdatePvDelegate(string s);
    delegate void SearchDoneDelegate();

    public partial class Form1 : Form
    {
        ChessBoardController cbc;
        Position position = new Position();
        Search search;

        public Form1()
        {
            search = new Search();
            search.IterationCompleted += Search_IterationCompleted;
            search.SearchCompleted += Search_SearchCompleted;

            InitializeComponent();
            cbc = new ChessBoardController(boardView, position);
        }

        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            cbc.UndoMove();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            button1.Enabled = false;
            button1.Text = "Thinking . . .";
            
            listBox1.Items.Clear();
            Thread thread = new Thread(new ThreadStart(DoSearch));
            thread.Start();

        }

        private void DoSearch()
        {
            Position p = new Position(position);
            if (radioMaxPly.Checked)
            {
                int ply = (int)numericUpDown1.Value;
                search.IterativeDeepening(ply, 0, p);
            }
            else
            {
                int time;
                if (int.TryParse(txtTime.Text, out time) && time > 0)
                {
                    search.IterativeDeepening(200, time, p);
                }
                else
                {
                    MessageBox.Show("Invalid time setting.");
                }
            }
        }

        private void Search_SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            Invoke(new SearchDoneDelegate(SearchComplete));
        }

        private void SearchComplete()
        {
            button1.Text = "Get Best Move";
            button1.Enabled = true;

        }

        private void Search_IterationCompleted(object sender, SearchCompletedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"Ply: {e.Ply}\t");
            sb.Append($"Val: {e.Score}\t");
            foreach (Move move in e.PrincipalVariation)
            {
                sb.Append("  ");
                sb.Append(move);
            }
            Invoke(new UpdatePvDelegate(AddPv), sb.ToString());
        }

        private void AddPv(string pv)
        {
            listBox1.Items.Add(pv);
        }
    }
}
