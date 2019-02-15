using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace BitboardVisualizer
{
    using Bitboard = UInt64;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void txtDecimal_Changed(object sender, EventArgs e)
        {
            if (txtDecimal.Text == "")
            {
                grid.Bitboard = 0;
                txtHex.Text = "";
                return;
            }
            Bitboard bb;
            if (Bitboard.TryParse(txtDecimal.Text, out bb))
            {
                grid.Bitboard = bb;
                txtHex.Text = $"{bb:X}";
            }


        }

        private void txtHex_TextChanged(object sender, EventArgs e)
        {
            if (txtHex.Text == "")
            {
                txtDecimal.Text = "";
                grid.Bitboard = 0;
                return;
            }
            ulong bb = 0;
            try
            {
                bb = Convert.ToUInt64(txtHex.Text, 16);
                txtDecimal.Text = bb.ToString();
                grid.Bitboard = bb;
                txtDecimal.BackColor = Color.White;
                txtHex.BackColor = Color.White;
            }
            catch
            {
                txtHex.BackColor = Color.Red;
            }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager reorigins = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label1 = new System.Windows.Forms.Label();
            this.txtDecimal = new System.Windows.Forms.TextBox();
            this.txtHex = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grid = new BitboardVisualizer.BitboardGrid();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label1.Location = new System.Drawing.Point(11, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Decimal:";
            // 
            // txtDecimal
            // 
            this.txtDecimal.Location = new System.Drawing.Point(65, 20);
            this.txtDecimal.Name = "txtDecimal";
            this.txtDecimal.Size = new System.Drawing.Size(240, 20);
            this.txtDecimal.TabIndex = 1;
            this.txtDecimal.Text = "0";
            this.txtDecimal.TextChanged += new System.EventHandler(this.txtDecimal_Changed);
            // 
            // txtHex
            // 
            this.txtHex.Location = new System.Drawing.Point(65, 46);
            this.txtHex.Name = "txtHex";
            this.txtHex.Size = new System.Drawing.Size(240, 20);
            this.txtHex.TabIndex = 2;
            this.txtHex.Text = "0";
            this.txtHex.TextChanged += new System.EventHandler(this.txtHex_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtHex);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtDecimal);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(318, 80);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bitboard Value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label2.Location = new System.Drawing.Point(30, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Hex:";
            // 
            // grid
            // 
            this.grid.Bitboard = ((ulong)(0ul));
            this.grid.Location = new System.Drawing.Point(12, 108);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(328, 321);
            this.grid.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 441);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(reorigins.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bitboard Visualizer";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }


    }
}
