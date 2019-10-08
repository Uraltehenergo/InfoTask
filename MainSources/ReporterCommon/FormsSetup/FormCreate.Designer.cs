namespace ReporterCommon
{
    partial class FormCreate
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreate));
            this.label1 = new System.Windows.Forms.Label();
            this.CodeReport = new System.Windows.Forms.TextBox();
            this.NameReport = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DescriptionReport = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.FileReport = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.butFileReport = new System.Windows.Forms.Button();
            this.butOK = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.label1.Location = new System.Drawing.Point(49, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Код";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CodeReport
            // 
            this.CodeReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CodeReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CodeReport.Location = new System.Drawing.Point(90, 6);
            this.CodeReport.Name = "CodeReport";
            this.CodeReport.Size = new System.Drawing.Size(356, 24);
            this.CodeReport.TabIndex = 5;
            this.CodeReport.TextChanged += new System.EventHandler(this.CodeReport_TextChanged);
            // 
            // NameReport
            // 
            this.NameReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NameReport.Location = new System.Drawing.Point(90, 36);
            this.NameReport.Name = "NameReport";
            this.NameReport.Size = new System.Drawing.Size(467, 24);
            this.NameReport.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(47, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Имя";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DescriptionReport
            // 
            this.DescriptionReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DescriptionReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DescriptionReport.Location = new System.Drawing.Point(90, 66);
            this.DescriptionReport.Multiline = true;
            this.DescriptionReport.Name = "DescriptionReport";
            this.DescriptionReport.Size = new System.Drawing.Size(467, 42);
            this.DescriptionReport.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(4, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Описание";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FileReport
            // 
            this.FileReport.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FileReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FileReport.Location = new System.Drawing.Point(90, 114);
            this.FileReport.Multiline = true;
            this.FileReport.Name = "FileReport";
            this.FileReport.Size = new System.Drawing.Size(397, 42);
            this.FileReport.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(38, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 18);
            this.label4.TabIndex = 6;
            this.label4.Text = "Файл";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butFileReport
            // 
            this.butFileReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butFileReport.Location = new System.Drawing.Point(487, 114);
            this.butFileReport.Name = "butFileReport";
            this.butFileReport.Size = new System.Drawing.Size(70, 42);
            this.butFileReport.TabIndex = 25;
            this.butFileReport.Text = "Обзор";
            this.butFileReport.UseVisualStyleBackColor = true;
            this.butFileReport.Click += new System.EventHandler(this.butFileReport_Click);
            // 
            // butOK
            // 
            this.butOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butOK.Location = new System.Drawing.Point(124, 169);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(118, 39);
            this.butOK.TabIndex = 30;
            this.butOK.Text = "OK";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCancel.Location = new System.Drawing.Point(311, 169);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(118, 39);
            this.butCancel.TabIndex = 35;
            this.butCancel.Text = "Отмена";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // FormCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 216);
            this.Controls.Add(this.butFileReport);
            this.Controls.Add(this.FileReport);
            this.Controls.Add(this.DescriptionReport);
            this.Controls.Add(this.NameReport);
            this.Controls.Add(this.CodeReport);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormCreate";
            this.Text = "Создание бланка отчета";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCreateWin_FormClosed);
            this.Load += new System.EventHandler(this.FormCreateWin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox CodeReport;
        private System.Windows.Forms.TextBox NameReport;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox DescriptionReport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox FileReport;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button butFileReport;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Button butCancel;
    }
}