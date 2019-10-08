namespace ReporterCommon
{
    partial class FormLiveReport
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLiveReport));
            this.PeriodLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SourcesLate = new System.Windows.Forms.TextBox();
            this.PeriodBegin = new System.Windows.Forms.TextBox();
            this.PeriodEnd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ButStart = new System.Windows.Forms.Button();
            this.ButStop = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.FillPages = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // PeriodLength
            // 
            this.PeriodLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodLength.Location = new System.Drawing.Point(234, 19);
            this.PeriodLength.Name = "PeriodLength";
            this.PeriodLength.Size = new System.Drawing.Size(61, 22);
            this.PeriodLength.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(20, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(208, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Длина интервала отчета (мин)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(20, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Задержка источника (мин)";
            // 
            // SourcesLate
            // 
            this.SourcesLate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SourcesLate.Location = new System.Drawing.Point(234, 47);
            this.SourcesLate.Name = "SourcesLate";
            this.SourcesLate.Size = new System.Drawing.Size(61, 22);
            this.SourcesLate.TabIndex = 2;
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodBegin.Location = new System.Drawing.Point(161, 115);
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.ReadOnly = true;
            this.PeriodBegin.Size = new System.Drawing.Size(134, 22);
            this.PeriodBegin.TabIndex = 27;
            this.PeriodBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodEnd.Location = new System.Drawing.Point(331, 115);
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.ReadOnly = true;
            this.PeriodEnd.Size = new System.Drawing.Size(134, 22);
            this.PeriodEnd.TabIndex = 28;
            this.PeriodEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(307, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 16);
            this.label6.TabIndex = 30;
            this.label6.Text = "до";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(121, 117);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 16);
            this.label9.TabIndex = 31;
            this.label9.Text = "от";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(20, 117);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(108, 20);
            this.label5.TabIndex = 29;
            this.label5.Text = "Текущий отчет";
            // 
            // ButStart
            // 
            this.ButStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButStart.Location = new System.Drawing.Point(94, 149);
            this.ButStart.Name = "ButStart";
            this.ButStart.Size = new System.Drawing.Size(122, 36);
            this.ButStart.TabIndex = 32;
            this.ButStart.Text = "Пуск";
            this.ButStart.UseVisualStyleBackColor = true;
            this.ButStart.Click += new System.EventHandler(this.ButStart_Click);
            // 
            // ButStop
            // 
            this.ButStop.Enabled = false;
            this.ButStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButStop.Location = new System.Drawing.Point(261, 149);
            this.ButStop.Name = "ButStop";
            this.ButStop.Size = new System.Drawing.Size(122, 36);
            this.ButStop.TabIndex = 33;
            this.ButStop.Text = "Стоп";
            this.ButStop.UseVisualStyleBackColor = true;
            this.ButStop.Click += new System.EventHandler(this.ButStop_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FillPages
            // 
            this.FillPages.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FillPages.FormattingEnabled = true;
            this.FillPages.Items.AddRange(new object[] {
            "Все листы",
            "Текущий лист"});
            this.FillPages.Location = new System.Drawing.Point(166, 75);
            this.FillPages.Name = "FillPages";
            this.FillPages.Size = new System.Drawing.Size(130, 24);
            this.FillPages.TabIndex = 67;
            this.FillPages.Text = "Все листы";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(90, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 20);
            this.label7.TabIndex = 66;
            this.label7.Text = "Заполнять";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FormLiveReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 195);
            this.Controls.Add(this.FillPages);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ButStop);
            this.Controls.Add(this.ButStart);
            this.Controls.Add(this.PeriodBegin);
            this.Controls.Add(this.PeriodEnd);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SourcesLate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PeriodLength);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormLiveReport";
            this.Text = "Динамический отчет";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormLiveReport_FormClosing);
            this.Load += new System.EventHandler(this.FormLiveReport_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PeriodLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox SourcesLate;
        public System.Windows.Forms.TextBox PeriodBegin;
        public System.Windows.Forms.TextBox PeriodEnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ButStart;
        private System.Windows.Forms.Button ButStop;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox FillPages;
        private System.Windows.Forms.Label label7;
    }
}