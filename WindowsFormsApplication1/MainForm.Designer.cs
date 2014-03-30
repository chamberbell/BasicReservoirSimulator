namespace WindowsFormsApplication1
{
    partial class MainForm
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
            this.txGridBlocks = new System.Windows.Forms.TextBox();
            this.lbGridBlocks = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txGridBlocks
            // 
            this.txGridBlocks.Location = new System.Drawing.Point(102, 51);
            this.txGridBlocks.Name = "txGridBlocks";
            this.txGridBlocks.Size = new System.Drawing.Size(61, 20);
            this.txGridBlocks.TabIndex = 0;
            // 
            // lbGridBlocks
            // 
            this.lbGridBlocks.AutoSize = true;
            this.lbGridBlocks.Location = new System.Drawing.Point(35, 54);
            this.lbGridBlocks.Name = "lbGridBlocks";
            this.lbGridBlocks.Size = new System.Drawing.Size(61, 13);
            this.lbGridBlocks.TabIndex = 1;
            this.lbGridBlocks.Text = "Grid Blocks";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 346);
            this.Controls.Add(this.lbGridBlocks);
            this.Controls.Add(this.txGridBlocks);
            this.Name = "Form1";
            this.Text = "Reservoir Simulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txGridBlocks;
        private System.Windows.Forms.Label lbGridBlocks;
    }
}

