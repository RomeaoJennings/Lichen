namespace UiComponents
{
    partial class PromotionDialog
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
            this.btnQueen = new System.Windows.Forms.Button();
            this.btnRook = new System.Windows.Forms.Button();
            this.btnBishop = new System.Windows.Forms.Button();
            this.btnKnight = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnQueen
            // 
            this.btnQueen.Location = new System.Drawing.Point(19, 17);
            this.btnQueen.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.btnQueen.Name = "btnQueen";
            this.btnQueen.Size = new System.Drawing.Size(48, 48);
            this.btnQueen.TabIndex = 0;
            this.btnQueen.Text = "q";
            this.btnQueen.UseVisualStyleBackColor = true;
            this.btnQueen.Click += new System.EventHandler(this.Promotion_Click);
            // 
            // btnRook
            // 
            this.btnRook.Location = new System.Drawing.Point(87, 17);
            this.btnRook.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.btnRook.Name = "btnRook";
            this.btnRook.Size = new System.Drawing.Size(48, 48);
            this.btnRook.TabIndex = 1;
            this.btnRook.Text = "r";
            this.btnRook.UseVisualStyleBackColor = true;
            this.btnRook.Click += new System.EventHandler(this.Promotion_Click);
            // 
            // btnBishop
            // 
            this.btnBishop.Location = new System.Drawing.Point(155, 17);
            this.btnBishop.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.btnBishop.Name = "btnBishop";
            this.btnBishop.Size = new System.Drawing.Size(48, 48);
            this.btnBishop.TabIndex = 2;
            this.btnBishop.Text = "b";
            this.btnBishop.UseVisualStyleBackColor = true;
            this.btnBishop.Click += new System.EventHandler(this.Promotion_Click);
            // 
            // btnKnight
            // 
            this.btnKnight.Location = new System.Drawing.Point(223, 17);
            this.btnKnight.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.btnKnight.Name = "btnKnight";
            this.btnKnight.Size = new System.Drawing.Size(48, 48);
            this.btnKnight.TabIndex = 3;
            this.btnKnight.Text = "n";
            this.btnKnight.UseVisualStyleBackColor = true;
            this.btnKnight.Click += new System.EventHandler(this.Promotion_Click);
            // 
            // PromotionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(19F, 37F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 82);
            this.ControlBox = false;
            this.Controls.Add(this.btnKnight);
            this.Controls.Add(this.btnBishop);
            this.Controls.Add(this.btnRook);
            this.Controls.Add(this.btnQueen);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PromotionDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Promotion Type...";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnQueen;
        private System.Windows.Forms.Button btnRook;
        private System.Windows.Forms.Button btnBishop;
        private System.Windows.Forms.Button btnKnight;
    }
}