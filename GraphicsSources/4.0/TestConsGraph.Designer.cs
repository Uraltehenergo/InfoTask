namespace _4._0
{
    partial class TestConsGraph
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
            this.cGraphControl1 = new ConsGraphLibrary.CGraphControl();
            this.SuspendLayout();
            // 
            // cGraphControl1
            // 
            this.cGraphControl1.Location = new System.Drawing.Point(12, 12);
            this.cGraphControl1.Name = "cGraphControl1";
            this.cGraphControl1.Size = new System.Drawing.Size(798, 472);
            this.cGraphControl1.TabIndex = 0;
            // 
            // TestConsGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 496);
            this.Controls.Add(this.cGraphControl1);
            this.Name = "TestConsGraph";
            this.Text = "TestConsGraph";
            this.ResumeLayout(false);

        }

        #endregion

        private ConsGraphLibrary.CGraphControl cGraphControl1;
    }
}