using UiComponents;

namespace UserInterface
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.boardView = new UiComponents.ChessBoardView();
            this.btnUndo = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioMaxPly = new System.Windows.Forms.RadioButton();
            this.radioMaxTime = new System.Windows.Forms.RadioButton();
            this.txtTime = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // boardView
            // 
            this.boardView.DarkSquares = System.Drawing.Color.LightSeaGreen;
            this.boardView.HighlightColor = this.boardView.HighlightColor;
            this.boardView.LightSquares = System.Drawing.Color.White;
            this.boardView.Location = new System.Drawing.Point(24, 36);
            this.boardView.Name = "boardView";
            this.boardView.Size = new System.Drawing.Size(373, 402);
            this.boardView.TabIndex = 0;
            // 
            // btnUndo
            // 
            this.btnUndo.Location = new System.Drawing.Point(591, 382);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(186, 32);
            this.btnUndo.TabIndex = 1;
            this.btnUndo.Text = "Undo Move";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(424, 382);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(161, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "GetBestMove";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(167, 28);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(108, 20);
            this.numericUpDown1.TabIndex = 3;
            this.numericUpDown1.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(424, 59);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(353, 160);
            this.listBox1.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTime);
            this.groupBox1.Controls.Add(this.radioMaxTime);
            this.groupBox1.Controls.Add(this.radioMaxPly);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Location = new System.Drawing.Point(424, 252);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Limit";
            // 
            // radioMaxPly
            // 
            this.radioMaxPly.AutoSize = true;
            this.radioMaxPly.Checked = true;
            this.radioMaxPly.Location = new System.Drawing.Point(65, 28);
            this.radioMaxPly.Name = "radioMaxPly";
            this.radioMaxPly.Size = new System.Drawing.Size(74, 17);
            this.radioMaxPly.TabIndex = 0;
            this.radioMaxPly.TabStop = true;
            this.radioMaxPly.Text = "Max Ply:   ";
            this.radioMaxPly.UseVisualStyleBackColor = true;
            // 
            // radioMaxTime
            // 
            this.radioMaxTime.AutoSize = true;
            this.radioMaxTime.Location = new System.Drawing.Point(65, 61);
            this.radioMaxTime.Name = "radioMaxTime";
            this.radioMaxTime.Size = new System.Drawing.Size(96, 17);
            this.radioMaxTime.TabIndex = 4;
            this.radioMaxTime.Text = "Max Time (ms):";
            this.radioMaxTime.UseVisualStyleBackColor = true;
            // 
            // txtTime
            // 
            this.txtTime.Location = new System.Drawing.Point(167, 60);
            this.txtTime.Name = "txtTime";
            this.txtTime.Size = new System.Drawing.Size(108, 20);
            this.txtTime.TabIndex = 5;
            this.txtTime.Text = "5000";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnUndo);
            this.Controls.Add(this.boardView);
            this.Name = "Form1";
            this.Text = "Lichen Chess";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ChessBoardView boardView;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtTime;
        private System.Windows.Forms.RadioButton radioMaxTime;
        private System.Windows.Forms.RadioButton radioMaxPly;
    }
}

