namespace AuditMonitor
{
    partial class FormGraphics
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
            this.ucGraphic = new GraphicLibrary.UserControl1();
            this.SuspendLayout();
            // 
            // ucGraphic
            // 
            this.ucGraphic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucGraphic.Location = new System.Drawing.Point(0, 0);
            this.ucGraphic.MinimumSize = new System.Drawing.Size(800, 300);
            this.ucGraphic.Name = "ucGraphic";
            this.ucGraphic.Size = new System.Drawing.Size(1024, 410);
            this.ucGraphic.TabIndex = 0;
            // 
            // FormGraphics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1024, 410);
            this.Controls.Add(this.ucGraphic);
            this.MinimumSize = new System.Drawing.Size(550, 280);
            this.Name = "FormGraphics";
            this.Text = "FormGraphics";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormGraphics_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private GraphicLibrary.UserControl1 ucGraphic;
    }
}