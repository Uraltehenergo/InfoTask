namespace Calculation
{
    partial class IndicatorForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.PeriodBegin = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PeriodEnd = new System.Windows.Forms.Label();
            this.Procent = new System.Windows.Forms.ProgressBar();
            this.CurrentOperation = new System.Windows.Forms.Label();
            this.CalcName = new System.Windows.Forms.Label();
            this.ButBreak = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(-1, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "от";
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodBegin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.PeriodBegin.Location = new System.Drawing.Point(19, 19);
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.Size = new System.Drawing.Size(147, 16);
            this.PeriodBegin.TabIndex = 2;
            this.PeriodBegin.Text = "00.00.0000 00:00:00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(167, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "до";
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodEnd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.PeriodEnd.Location = new System.Drawing.Point(191, 19);
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.Size = new System.Drawing.Size(150, 16);
            this.PeriodEnd.TabIndex = 4;
            this.PeriodEnd.Text = "00.00.0000 00:00:00";
            // 
            // Procent
            // 
            this.Procent.Location = new System.Drawing.Point(0, 36);
            this.Procent.Name = "Procent";
            this.Procent.Size = new System.Drawing.Size(341, 23);
            this.Procent.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.Procent.TabIndex = 5;
            // 
            // CurrentOperation
            // 
            this.CurrentOperation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CurrentOperation.Location = new System.Drawing.Point(-1, 60);
            this.CurrentOperation.Name = "CurrentOperation";
            this.CurrentOperation.Size = new System.Drawing.Size(342, 20);
            this.CurrentOperation.TabIndex = 6;
            this.CurrentOperation.Text = "***";
            this.CurrentOperation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CalcName
            // 
            this.CalcName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CalcName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.CalcName.Location = new System.Drawing.Point(-1, 3);
            this.CalcName.Name = "CalcName";
            this.CalcName.Size = new System.Drawing.Size(342, 15);
            this.CalcName.TabIndex = 8;
            // 
            // ButBreak
            // 
            this.ButBreak.Location = new System.Drawing.Point(212, -1);
            this.ButBreak.Name = "ButBreak";
            this.ButBreak.Size = new System.Drawing.Size(71, 20);
            this.ButBreak.TabIndex = 9;
            this.ButBreak.Text = "Прервать";
            this.ButBreak.UseVisualStyleBackColor = true;
            this.ButBreak.Visible = false;
            this.ButBreak.Click += new System.EventHandler(this.ButBreak_Click);
            // 
            // IndicatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 113);
            this.ControlBox = false;
            this.Controls.Add(this.ButBreak);
            this.Controls.Add(this.CalcName);
            this.Controls.Add(this.CurrentOperation);
            this.Controls.Add(this.Procent);
            this.Controls.Add(this.PeriodEnd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PeriodBegin);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IndicatorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "IndicatorForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.IndicatorForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label PeriodBegin;
        public System.Windows.Forms.Label PeriodEnd;
        public System.Windows.Forms.ProgressBar Procent;
        public System.Windows.Forms.Label CurrentOperation;
        public System.Windows.Forms.Label CalcName;
        private System.Windows.Forms.Button ButBreak;
    }
}