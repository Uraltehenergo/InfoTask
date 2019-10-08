namespace ReporterCommon
{
    partial class FormSaveReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSaveReport));
            this.PeriodBegin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PeriodEnd = new System.Windows.Forms.TextBox();
            this.IntervalName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ButOk = new System.Windows.Forms.Button();
            this.ButCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.PeriodBegin.Location = new System.Drawing.Point(163, 11);
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.ReadOnly = true;
            this.PeriodBegin.Size = new System.Drawing.Size(154, 22);
            this.PeriodBegin.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(99, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Отчет от";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(139, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "до";
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.PeriodEnd.Location = new System.Drawing.Point(163, 35);
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.ReadOnly = true;
            this.PeriodEnd.Size = new System.Drawing.Size(154, 22);
            this.PeriodEnd.TabIndex = 3;
            // 
            // IntervalName
            // 
            this.IntervalName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IntervalName.Location = new System.Drawing.Point(2, 84);
            this.IntervalName.Name = "IntervalName";
            this.IntervalName.Size = new System.Drawing.Size(451, 22);
            this.IntervalName.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(-1, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(217, 24);
            this.label3.TabIndex = 5;
            this.label3.Text = "Сохранить в журнал с именем";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ButOk
            // 
            this.ButOk.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.ButOk.Location = new System.Drawing.Point(101, 121);
            this.ButOk.Name = "ButOk";
            this.ButOk.Size = new System.Drawing.Size(115, 42);
            this.ButOk.TabIndex = 10;
            this.ButOk.Text = "Сохранить в журнал";
            this.ButOk.UseVisualStyleBackColor = true;
            this.ButOk.Click += new System.EventHandler(this.ButOk_Click);
            // 
            // ButCancel
            // 
            this.ButCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.ButCancel.Location = new System.Drawing.Point(233, 121);
            this.ButCancel.Name = "ButCancel";
            this.ButCancel.Size = new System.Drawing.Size(115, 42);
            this.ButCancel.TabIndex = 15;
            this.ButCancel.Text = "Отмена";
            this.ButCancel.UseVisualStyleBackColor = true;
            this.ButCancel.Click += new System.EventHandler(this.ButCancel_Click);
            // 
            // FormSaveReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 169);
            this.ControlBox = false;
            this.Controls.Add(this.IntervalName);
            this.Controls.Add(this.ButCancel);
            this.Controls.Add(this.ButOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PeriodEnd);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PeriodBegin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormSaveReport";
            this.ShowInTaskbar = false;
            this.Text = "Сохранение отчета в журнал";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormSaveReport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PeriodBegin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox PeriodEnd;
        private System.Windows.Forms.TextBox IntervalName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button ButOk;
        private System.Windows.Forms.Button ButCancel;
    }
}