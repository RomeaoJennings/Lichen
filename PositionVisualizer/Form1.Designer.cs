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
            this.boardView = new UserInterface.ChessBoardView();
            this.SuspendLayout();
            // 
            // boardView
            // 
            this.boardView.DarkSquares = System.Drawing.Color.LightSeaGreen;
            this.boardView.HighlightColor = boardView.HighlightColor;
            this.boardView.LightSquares = System.Drawing.Color.White;
            this.boardView.Location = new System.Drawing.Point(24, 36);
            this.boardView.Name = "boardView";
            this.boardView.Size = new System.Drawing.Size(373, 402);
            this.boardView.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.boardView);
            this.Name = "Form1";
            this.Text = "Typhoon Chess";
            this.ResumeLayout(false);

        }

        #endregion

        private ChessBoardView boardView;
    }
}

