namespace ReporterCommon
{
    partial class FormVideo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVideo));
            this.label3 = new System.Windows.Forms.Label();
            this.PeriodLength = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.BetweenPeriods = new System.Windows.Forms.TextBox();
            this.PeriodBegin = new System.Windows.Forms.TextBox();
            this.PeriodEnd = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ButStart = new System.Windows.Forms.Button();
            this.ButStop = new System.Windows.Forms.Button();
            this.ButPause = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.FillPages = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.VideoBegin = new System.Windows.Forms.TextBox();
            this.VideoEnd = new System.Windows.Forms.TextBox();
            this.PeriodEndPicker = new System.Windows.Forms.DateTimePicker();
            this.PeriodBeginPicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SourcesEnd = new System.Windows.Forms.TextBox();
            this.ButArchivesRanges = new System.Windows.Forms.Button();
            this.butSourcesTime = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SourcesBegin = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(40, 110);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(208, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Длина интервала отчета (мин)";
            // 
            // PeriodLength
            // 
            this.PeriodLength.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodLength.Location = new System.Drawing.Point(251, 107);
            this.PeriodLength.Name = "PeriodLength";
            this.PeriodLength.Size = new System.Drawing.Size(61, 22);
            this.PeriodLength.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(8, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(240, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "Ожидание следущего периода (сек)";
            // 
            // BetweenPeriods
            // 
            this.BetweenPeriods.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.BetweenPeriods.Location = new System.Drawing.Point(251, 132);
            this.BetweenPeriods.Name = "BetweenPeriods";
            this.BetweenPeriods.Size = new System.Drawing.Size(61, 22);
            this.BetweenPeriods.TabIndex = 8;
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodBegin.Location = new System.Drawing.Point(127, 187);
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.ReadOnly = true;
            this.PeriodBegin.Size = new System.Drawing.Size(127, 22);
            this.PeriodBegin.TabIndex = 32;
            this.PeriodBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodEnd.Location = new System.Drawing.Point(281, 187);
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.ReadOnly = true;
            this.PeriodEnd.Size = new System.Drawing.Size(134, 22);
            this.PeriodEnd.TabIndex = 33;
            this.PeriodEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(257, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 16);
            this.label6.TabIndex = 35;
            this.label6.Text = "до";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(3, 189);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 20);
            this.label5.TabIndex = 34;
            this.label5.Text = "Текущий отчет от";
            // 
            // ButStart
            // 
            this.ButStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButStart.Location = new System.Drawing.Point(56, 215);
            this.ButStart.Name = "ButStart";
            this.ButStart.Size = new System.Drawing.Size(99, 36);
            this.ButStart.TabIndex = 37;
            this.ButStart.Text = "Пуск";
            this.ButStart.UseVisualStyleBackColor = true;
            this.ButStart.Click += new System.EventHandler(this.ButStart_Click);
            // 
            // ButStop
            // 
            this.ButStop.Enabled = false;
            this.ButStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButStop.Location = new System.Drawing.Point(260, 215);
            this.ButStop.Name = "ButStop";
            this.ButStop.Size = new System.Drawing.Size(102, 36);
            this.ButStop.TabIndex = 38;
            this.ButStop.Text = "Стоп";
            this.ButStop.UseVisualStyleBackColor = true;
            this.ButStop.Click += new System.EventHandler(this.ButStop_Click);
            // 
            // ButPause
            // 
            this.ButPause.Enabled = false;
            this.ButPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButPause.Location = new System.Drawing.Point(160, 215);
            this.ButPause.Name = "ButPause";
            this.ButPause.Size = new System.Drawing.Size(95, 36);
            this.ButPause.TabIndex = 39;
            this.ButPause.Text = "Пауза";
            this.ButPause.UseVisualStyleBackColor = true;
            this.ButPause.Click += new System.EventHandler(this.ButPause_Click);
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
            this.FillPages.Location = new System.Drawing.Point(182, 157);
            this.FillPages.Name = "FillPages";
            this.FillPages.Size = new System.Drawing.Size(130, 24);
            this.FillPages.TabIndex = 69;
            this.FillPages.Text = "Все листы";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(106, 158);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 20);
            this.label7.TabIndex = 68;
            this.label7.Text = "Заполнять";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.VideoBegin);
            this.panel1.Controls.Add(this.VideoEnd);
            this.panel1.Controls.Add(this.PeriodEndPicker);
            this.panel1.Controls.Add(this.PeriodBeginPicker);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(2, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(416, 54);
            this.panel1.TabIndex = 70;
            // 
            // VideoBegin
            // 
            this.VideoBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.VideoBegin.Location = new System.Drawing.Point(98, 4);
            this.VideoBegin.Name = "VideoBegin";
            this.VideoBegin.Size = new System.Drawing.Size(150, 22);
            this.VideoBegin.TabIndex = 6;
            // 
            // VideoEnd
            // 
            this.VideoEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.VideoEnd.Location = new System.Drawing.Point(98, 27);
            this.VideoEnd.Name = "VideoEnd";
            this.VideoEnd.Size = new System.Drawing.Size(150, 22);
            this.VideoEnd.TabIndex = 8;
            // 
            // PeriodEndPicker
            // 
            this.PeriodEndPicker.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PeriodEndPicker.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.PeriodEndPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PeriodEndPicker.Location = new System.Drawing.Point(249, 27);
            this.PeriodEndPicker.MaxDate = new System.DateTime(2200, 12, 31, 0, 0, 0, 0);
            this.PeriodEndPicker.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.PeriodEndPicker.Name = "PeriodEndPicker";
            this.PeriodEndPicker.Size = new System.Drawing.Size(17, 22);
            this.PeriodEndPicker.TabIndex = 31;
            this.PeriodEndPicker.ValueChanged += new System.EventHandler(this.PeriodEndPicker_ValueChanged);
            // 
            // PeriodBeginPicker
            // 
            this.PeriodBeginPicker.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PeriodBeginPicker.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.PeriodBeginPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PeriodBeginPicker.Location = new System.Drawing.Point(249, 4);
            this.PeriodBeginPicker.MaxDate = new System.DateTime(2200, 12, 31, 0, 0, 0, 0);
            this.PeriodBeginPicker.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.PeriodBeginPicker.Name = "PeriodBeginPicker";
            this.PeriodBeginPicker.Size = new System.Drawing.Size(17, 22);
            this.PeriodBeginPicker.TabIndex = 26;
            this.PeriodBeginPicker.ValueChanged += new System.EventHandler(this.PeriodBeginPicker_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(69, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "до";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Период от";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.SourcesEnd);
            this.panel2.Controls.Add(this.ButArchivesRanges);
            this.panel2.Controls.Add(this.butSourcesTime);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.SourcesBegin);
            this.panel2.Location = new System.Drawing.Point(2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(416, 49);
            this.panel2.TabIndex = 71;
            // 
            // SourcesEnd
            // 
            this.SourcesEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SourcesEnd.Location = new System.Drawing.Point(97, 24);
            this.SourcesEnd.Name = "SourcesEnd";
            this.SourcesEnd.ReadOnly = true;
            this.SourcesEnd.Size = new System.Drawing.Size(150, 22);
            this.SourcesEnd.TabIndex = 8;
            // 
            // ButArchivesRanges
            // 
            this.ButArchivesRanges.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButArchivesRanges.Location = new System.Drawing.Point(322, 1);
            this.ButArchivesRanges.Name = "ButArchivesRanges";
            this.ButArchivesRanges.Size = new System.Drawing.Size(91, 46);
            this.ButArchivesRanges.TabIndex = 11;
            this.ButArchivesRanges.Text = "Подробнее";
            this.ButArchivesRanges.UseVisualStyleBackColor = true;
            this.ButArchivesRanges.Click += new System.EventHandler(this.ButArchivesRanges_Click);
            // 
            // butSourcesTime
            // 
            this.butSourcesTime.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butSourcesTime.Location = new System.Drawing.Point(247, 1);
            this.butSourcesTime.Name = "butSourcesTime";
            this.butSourcesTime.Size = new System.Drawing.Size(76, 46);
            this.butSourcesTime.TabIndex = 10;
            this.butSourcesTime.Text = "Обновить";
            this.butSourcesTime.UseVisualStyleBackColor = true;
            this.butSourcesTime.Click += new System.EventHandler(this.butSourcesTime_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(67, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(26, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "до";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label10.Location = new System.Drawing.Point(8, 5);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(85, 16);
            this.label10.TabIndex = 7;
            this.label10.Text = "Данные от";
            // 
            // SourcesBegin
            // 
            this.SourcesBegin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SourcesBegin.Location = new System.Drawing.Point(97, 2);
            this.SourcesBegin.Name = "SourcesBegin";
            this.SourcesBegin.ReadOnly = true;
            this.SourcesBegin.Size = new System.Drawing.Size(150, 22);
            this.SourcesBegin.TabIndex = 6;
            // 
            // FormVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 253);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.FillPages);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ButPause);
            this.Controls.Add(this.ButStop);
            this.Controls.Add(this.ButStart);
            this.Controls.Add(this.PeriodBegin);
            this.Controls.Add(this.PeriodEnd);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BetweenPeriods);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.PeriodLength);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormVideo";
            this.Text = "Видеоклип";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormVideo_FormClosing);
            this.Load += new System.EventHandler(this.FormVideo_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PeriodLength;
        private System.Windows.Forms.TextBox BetweenPeriods;
        public System.Windows.Forms.TextBox PeriodBegin;
        public System.Windows.Forms.TextBox PeriodEnd;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button ButStart;
        private System.Windows.Forms.Button ButStop;
        private System.Windows.Forms.Button ButPause;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox FillPages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox VideoEnd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox VideoBegin;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox SourcesEnd;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox SourcesBegin;
        private System.Windows.Forms.DateTimePicker PeriodBeginPicker;
        private System.Windows.Forms.DateTimePicker PeriodEndPicker;
        private System.Windows.Forms.Button butSourcesTime;
        private System.Windows.Forms.Button ButArchivesRanges;
    }
}