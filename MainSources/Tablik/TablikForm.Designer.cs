namespace Tablik
{
    partial class TablikForm
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
            this.Indicator = new System.Windows.Forms.ProgressBar();
            this.Operation = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Indicator
            // 
            this.Indicator.Location = new System.Drawing.Point(0, 24);
            this.Indicator.Name = "Indicator";
            this.Indicator.Size = new System.Drawing.Size(300, 23);
            this.Indicator.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.Indicator.TabIndex = 1;
            // 
            // Operation
            // 
            this.Operation.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Operation.Location = new System.Drawing.Point(0, 1);
            this.Operation.Name = "Operation";
            this.Operation.Size = new System.Drawing.Size(300, 20);
            this.Operation.TabIndex = 2;
            this.Operation.Text = "Компилятор";
            this.Operation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TablikForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(299, 78);
            this.ControlBox = false;
            this.Controls.Add(this.Operation);
            this.Controls.Add(this.Indicator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TablikForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Компилятор Tablik";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar Indicator;
        private System.Windows.Forms.Label Operation;
    }
}