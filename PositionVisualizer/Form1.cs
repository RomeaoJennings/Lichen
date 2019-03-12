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
            button1.Enabled = false;
            button1.Text = "Thinking . . .";
            
            listBox1.Items.Clear();
            Thread thread = new Thread(new ThreadStart(DoSearch));
            thread.Start();

        }

        private void DoSearch()
        {
            BaseSearch search = new BaseSearch();
            search.IterationCompleted += Search_IterationCompleted;
            search.SearchCompleted += Search_SearchCompleted;
            search.IterativeDeepening((int)numericUpDown1.Value, position);
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
            for (int i=e.PrincipalVariation.Length - 1; i > 0; i--)
            {
                sb.Append("  ");
                sb.Append(e.PrincipalVariation[i]);
            }
            Invoke(new UpdatePvDelegate(AddPv), sb.ToString());
        }

        private void AddPv(string pv)
        {
            listBox1.Items.Add(pv);
        }
    }
}
