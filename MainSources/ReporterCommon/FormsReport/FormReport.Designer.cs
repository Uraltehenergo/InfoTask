namespace ReporterCommon
{
    partial class FormReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReport));
            this.label1 = new System.Windows.Forms.Label();
            this.panelSources = new System.Windows.Forms.Panel();
            this.ButArchivesRanges = new System.Windows.Forms.Button();
            this.SourcesEnd = new System.Windows.Forms.TextBox();
            this.SourcesBegin = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.butSourcesTime = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.progressReport = new System.Windows.Forms.ProgressBar();
            this.butFormReport = new System.Windows.Forms.Button();
            this.panelPeriod = new System.Windows.Forms.Panel();
            this.PeriodBegin = new System.Windows.Forms.TextBox();
            this.PeriodEnd = new System.Windows.Forms.TextBox();
            this.PeriodLength = new System.Windows.Forms.TextBox();
            this.butNowInterval = new System.Windows.Forms.Button();
            this.butNextInterval = new System.Windows.Forms.Button();
            this.butPreviousInterval = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.PeriodEndPicker = new System.Windows.Forms.DateTimePicker();
            this.PeriodBeginPicker = new System.Windows.Forms.DateTimePicker();
            this.IntervalName = new System.Windows.Forms.TextBox();
            this.SaveToArchive = new System.Windows.Forms.CheckBox();
            this.FillPages = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.butBreak = new System.Windows.Forms.Button();
            this.butSaveHandInput = new System.Windows.Forms.Button();
            this.panelSources.SuspendLayout();
            this.panelPeriod.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(5, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 35);
            this.label1.TabIndex = 0;
            this.label1.Text = "Исходные данные";
            // 
            // panelSources
            // 
            this.panelSources.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSources.Controls.Add(this.ButArchivesRanges);
            this.panelSources.Controls.Add(this.SourcesEnd);
            this.panelSources.Controls.Add(this.SourcesBegin);
            this.panelSources.Controls.Add(this.label2);
            this.panelSources.Controls.Add(this.butSourcesTime);
            this.panelSources.Controls.Add(this.label3);
            this.panelSources.Controls.Add(this.label1);
            this.panelSources.Location = new System.Drawing.Point(1, 1);
            this.panelSources.Name = "panelSources";
            this.panelSources.Size = new System.Drawing.Size(428, 51);
            this.panelSources.TabIndex = 0;
            // 
            // ButArchivesRanges
            // 
            this.ButArchivesRanges.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButArchivesRanges.Location = new System.Drawing.Point(320, 2);
            this.ButArchivesRanges.Name = "ButArchivesRanges";
            this.ButArchivesRanges.Size = new System.Drawing.Size(103, 46);
            this.ButArchivesRanges.TabIndex = 10;
            this.ButArchivesRanges.Text = "Подробнее>>";
            this.ButArchivesRanges.UseVisualStyleBackColor = true;
            this.ButArchivesRanges.Click += new System.EventHandler(this.ButArchivesRanges_Click);
            // 
            // SourcesEnd
            // 
            this.SourcesEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SourcesEnd.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SourcesEnd.Location = new System.Drawing.Point(100, 26);
            this.SourcesEnd.Name = "SourcesEnd";
            this.SourcesEnd.ReadOnly = true;
            this.SourcesEnd.Size = new System.Drawing.Size(134, 22);
            this.SourcesEnd.TabIndex = 3;
            this.SourcesEnd.Text = "01.01.0001 00:00:00";
            this.SourcesEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SourcesBegin
            // 
            this.SourcesBegin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SourcesBegin.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SourcesBegin.Location = new System.Drawing.Point(100, 3);
            this.SourcesBegin.Name = "SourcesBegin";
            this.SourcesBegin.ReadOnly = true;
            this.SourcesBegin.Size = new System.Drawing.Size(134, 22);
            this.SourcesBegin.TabIndex = 1;
            this.SourcesBegin.Text = "01.01.0001 00:00:00";
            this.SourcesBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(78, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "от";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butSourcesTime
            // 
            this.butSourcesTime.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butSourcesTime.Location = new System.Drawing.Point(236, 2);
            this.butSourcesTime.Name = "butSourcesTime";
            this.butSourcesTime.Size = new System.Drawing.Size(82, 46);
            this.butSourcesTime.TabIndex = 5;
            this.butSourcesTime.Text = "Обновить";
            this.butSourcesTime.UseVisualStyleBackColor = true;
            this.butSourcesTime.Click += new System.EventHandler(this.butSourcesTime_Click);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(76, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "до";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressReport
            // 
            this.progressReport.Location = new System.Drawing.Point(1, 158);
            this.progressReport.MarqueeAnimationSpeed = 50;
            this.progressReport.Name = "progressReport";
            this.progressReport.Size = new System.Drawing.Size(428, 23);
            this.progressReport.Step = 1;
            this.progressReport.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressReport.TabIndex = 15;
            // 
            // butFormReport
            // 
            this.butFormReport.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butFormReport.ForeColor = System.Drawing.SystemColors.WindowText;
            this.butFormReport.Location = new System.Drawing.Point(166, 185);
            this.butFormReport.Name = "butFormReport";
            this.butFormReport.Size = new System.Drawing.Size(137, 44);
            this.butFormReport.TabIndex = 75;
            this.butFormReport.Text = "Заполнить отчет";
            this.butFormReport.UseVisualStyleBackColor = true;
            this.butFormReport.Click += new System.EventHandler(this.butFormReport_Click);
            // 
            // panelPeriod
            // 
            this.panelPeriod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPeriod.Controls.Add(this.PeriodBegin);
            this.panelPeriod.Controls.Add(this.PeriodEnd);
            this.panelPeriod.Controls.Add(this.PeriodLength);
            this.panelPeriod.Controls.Add(this.butNowInterval);
            this.panelPeriod.Controls.Add(this.butNextInterval);
            this.panelPeriod.Controls.Add(this.butPreviousInterval);
            this.panelPeriod.Controls.Add(this.label6);
            this.panelPeriod.Controls.Add(this.label9);
            this.panelPeriod.Controls.Add(this.label5);
            this.panelPeriod.Controls.Add(this.PeriodEndPicker);
            this.panelPeriod.Controls.Add(this.PeriodBeginPicker);
            this.panelPeriod.Location = new System.Drawing.Point(1, 51);
            this.panelPeriod.Name = "panelPeriod";
            this.panelPeriod.Size = new System.Drawing.Size(428, 50);
            this.panelPeriod.TabIndex = 12;
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodBegin.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodBegin.Location = new System.Drawing.Point(100, 1);
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.Size = new System.Drawing.Size(134, 22);
            this.PeriodBegin.TabIndex = 15;
            this.PeriodBegin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PeriodBegin.Validated += new System.EventHandler(this.PeriodBegin_Validated);
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodEnd.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodEnd.Location = new System.Drawing.Point(100, 24);
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.Size = new System.Drawing.Size(134, 22);
            this.PeriodEnd.TabIndex = 20;
            this.PeriodEnd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.PeriodEnd.Validated += new System.EventHandler(this.PeriodEnd_Validated);
            // 
            // PeriodLength
            // 
            this.PeriodLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PeriodLength.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PeriodLength.Location = new System.Drawing.Point(320, 2);
            this.PeriodLength.Name = "PeriodLength";
            this.PeriodLength.ReadOnly = true;
            this.PeriodLength.Size = new System.Drawing.Size(103, 22);
            this.PeriodLength.TabIndex = 50;
            this.PeriodLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // butNowInterval
            // 
            this.butNowInterval.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butNowInterval.Location = new System.Drawing.Point(251, 24);
            this.butNowInterval.Name = "butNowInterval";
            this.butNowInterval.Size = new System.Drawing.Size(67, 23);
            this.butNowInterval.TabIndex = 35;
            this.butNowInterval.Text = "Сейчас";
            this.butNowInterval.UseVisualStyleBackColor = true;
            this.butNowInterval.Click += new System.EventHandler(this.butNowInterval_Click);
            // 
            // butNextInterval
            // 
            this.butNextInterval.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butNextInterval.Location = new System.Drawing.Point(369, 24);
            this.butNextInterval.Name = "butNextInterval";
            this.butNextInterval.Size = new System.Drawing.Size(54, 23);
            this.butNextInterval.TabIndex = 45;
            this.butNextInterval.Text = "След.";
            this.butNextInterval.UseVisualStyleBackColor = true;
            this.butNextInterval.Click += new System.EventHandler(this.butNextInterval_Click);
            // 
            // butPreviousInterval
            // 
            this.butPreviousInterval.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butPreviousInterval.Location = new System.Drawing.Point(319, 24);
            this.butPreviousInterval.Name = "butPreviousInterval";
            this.butPreviousInterval.Size = new System.Drawing.Size(49, 23);
            this.butPreviousInterval.TabIndex = 40;
            this.butPreviousInterval.Text = "Пред.";
            this.butPreviousInterval.UseVisualStyleBackColor = true;
            this.butPreviousInterval.Click += new System.EventHandler(this.butPreviousInterval_Click);
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(76, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 16);
            this.label6.TabIndex = 22;
            this.label6.Text = "до";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(77, 4);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(22, 16);
            this.label9.TabIndex = 26;
            this.label9.Text = "от";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(5, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 37);
            this.label5.TabIndex = 21;
            this.label5.Text = "Период отчета";
            // 
            // PeriodEndPicker
            // 
            this.PeriodEndPicker.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PeriodEndPicker.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.PeriodEndPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PeriodEndPicker.Location = new System.Drawing.Point(233, 24);
            this.PeriodEndPicker.MaxDate = new System.DateTime(2200, 12, 31, 0, 0, 0, 0);
            this.PeriodEndPicker.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.PeriodEndPicker.Name = "PeriodEndPicker";
            this.PeriodEndPicker.Size = new System.Drawing.Size(17, 22);
            this.PeriodEndPicker.TabIndex = 30;
            this.PeriodEndPicker.ValueChanged += new System.EventHandler(this.PeriodEndPicker_ValueChanged);
            // 
            // PeriodBeginPicker
            // 
            this.PeriodBeginPicker.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.PeriodBeginPicker.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold);
            this.PeriodBeginPicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PeriodBeginPicker.Location = new System.Drawing.Point(233, 1);
            this.PeriodBeginPicker.MaxDate = new System.DateTime(2200, 12, 31, 0, 0, 0, 0);
            this.PeriodBeginPicker.MinDate = new System.DateTime(1980, 1, 1, 0, 0, 0, 0);
            this.PeriodBeginPicker.Name = "PeriodBeginPicker";
            this.PeriodBeginPicker.Size = new System.Drawing.Size(17, 22);
            this.PeriodBeginPicker.TabIndex = 25;
            this.PeriodBeginPicker.ValueChanged += new System.EventHandler(this.PeriodBeginPicker_ValueChanged);
            // 
            // IntervalName
            // 
            this.IntervalName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.IntervalName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IntervalName.Location = new System.Drawing.Point(216, 105);
            this.IntervalName.Name = "IntervalName";
            this.IntervalName.Size = new System.Drawing.Size(213, 22);
            this.IntervalName.TabIndex = 60;
            // 
            // SaveToArchive
            // 
            this.SaveToArchive.AutoSize = true;
            this.SaveToArchive.Font = new System.Drawing.Font("Arial", 9.75F);
            this.SaveToArchive.Location = new System.Drawing.Point(8, 106);
            this.SaveToArchive.Name = "SaveToArchive";
            this.SaveToArchive.Size = new System.Drawing.Size(208, 20);
            this.SaveToArchive.TabIndex = 55;
            this.SaveToArchive.Text = "Сохранить в журнал по имени";
            this.SaveToArchive.UseVisualStyleBackColor = true;
            // 
            // FillPages
            // 
            this.FillPages.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FillPages.FormattingEnabled = true;
            this.FillPages.Items.AddRange(new object[] {
            "Все листы",
            "Текущий лист"});
            this.FillPages.Location = new System.Drawing.Point(81, 130);
            this.FillPages.Name = "FillPages";
            this.FillPages.Size = new System.Drawing.Size(130, 24);
            this.FillPages.TabIndex = 65;
            this.FillPages.Text = "Все листы";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(5, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 20);
            this.label7.TabIndex = 22;
            this.label7.Text = "Заполнять";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butBreak
            // 
            this.butBreak.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butBreak.ForeColor = System.Drawing.SystemColors.WindowText;
            this.butBreak.Location = new System.Drawing.Point(306, 185);
            this.butBreak.Name = "butBreak";
            this.butBreak.Size = new System.Drawing.Size(82, 44);
            this.butBreak.TabIndex = 80;
            this.butBreak.Text = "Прервать";
            this.butBreak.UseVisualStyleBackColor = true;
            this.butBreak.Visible = false;
            this.butBreak.Click += new System.EventHandler(this.butBreak_Click);
            // 
            // butSaveHandInput
            // 
            this.butSaveHandInput.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butSaveHandInput.ForeColor = System.Drawing.SystemColors.WindowText;
            this.butSaveHandInput.Location = new System.Drawing.Point(53, 185);
            this.butSaveHandInput.Name = "butSaveHandInput";
            this.butSaveHandInput.Size = new System.Drawing.Size(110, 44);
            this.butSaveHandInput.TabIndex = 70;
            this.butSaveHandInput.Text = "Сохранить ручной ввод";
            this.butSaveHandInput.UseVisualStyleBackColor = true;
            this.butSaveHandInput.Click += new System.EventHandler(this.ButSaveHandInput_Click);
            // 
            // FormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 233);
            this.Controls.Add(this.butSaveHandInput);
            this.Controls.Add(this.butBreak);
            this.Controls.Add(this.IntervalName);
            this.Controls.Add(this.FillPages);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panelPeriod);
            this.Controls.Add(this.panelSources);
            this.Controls.Add(this.progressReport);
            this.Controls.Add(this.butFormReport);
            this.Controls.Add(this.SaveToArchive);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReport";
            this.ShowInTaskbar = false;
            this.Text = "Формирование отчета";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormReportWin_FormClosed);
            this.Load += new System.EventHandler(this.FormReportWin_Load);
            this.panelSources.ResumeLayout(false);
            this.panelSources.PerformLayout();
            this.panelPeriod.ResumeLayout(false);
            this.panelPeriod.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelSources;
        private System.Windows.Forms.TextBox SourcesEnd;
        private System.Windows.Forms.TextBox SourcesBegin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button butSourcesTime;
        private System.Windows.Forms.Button butFormReport;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.ProgressBar progressReport;
        private System.Windows.Forms.Button ButArchivesRanges;
        private System.Windows.Forms.Panel panelPeriod;
        private System.Windows.Forms.TextBox PeriodLength;
        private System.Windows.Forms.Button butNowInterval;
        public System.Windows.Forms.TextBox PeriodEnd;
        public System.Windows.Forms.TextBox PeriodBegin;
        private System.Windows.Forms.Button butNextInterval;
        private System.Windows.Forms.Button butPreviousInterval;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox IntervalName;
        private System.Windows.Forms.CheckBox SaveToArchive;
        private System.Windows.Forms.ComboBox FillPages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button butBreak;
        private System.Windows.Forms.Button butSaveHandInput;
        private System.Windows.Forms.DateTimePicker PeriodBeginPicker;
        private System.Windows.Forms.DateTimePicker PeriodEndPicker;
    }
}