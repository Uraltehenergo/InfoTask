namespace ReporterCommon
{
    partial class FormArchivesRanges
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormArchivesRanges));
            this.butArchiveTime = new System.Windows.Forms.Button();
            this.RangesList = new System.Windows.Forms.DataGridView();
            this.Provider = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Interval = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeBegin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TimeEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.RangesList)).BeginInit();
            this.SuspendLayout();
            // 
            // butArchiveTime
            // 
            this.butArchiveTime.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butArchiveTime.Location = new System.Drawing.Point(429, 0);
            this.butArchiveTime.Name = "butArchiveTime";
            this.butArchiveTime.Size = new System.Drawing.Size(86, 26);
            this.butArchiveTime.TabIndex = 19;
            this.butArchiveTime.Text = "Обновить";
            this.butArchiveTime.UseVisualStyleBackColor = true;
            this.butArchiveTime.Click += new System.EventHandler(this.butArchiveTime_Click);
            // 
            // RangesList
            // 
            this.RangesList.AllowUserToAddRows = false;
            this.RangesList.AllowUserToDeleteRows = false;
            this.RangesList.AllowUserToResizeRows = false;
            this.RangesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RangesList.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.RangesList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RangesList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Provider,
            this.Interval,
            this.TimeBegin,
            this.TimeEnd});
            this.RangesList.Location = new System.Drawing.Point(1, 27);
            this.RangesList.Name = "RangesList";
            this.RangesList.RowHeadersVisible = false;
            this.RangesList.RowHeadersWidth = 20;
            this.RangesList.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.RangesList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.RangesList.Size = new System.Drawing.Size(515, 272);
            this.RangesList.TabIndex = 20;
            // 
            // Provider
            // 
            this.Provider.Frozen = true;
            this.Provider.HeaderText = "Архив или источник";
            this.Provider.Name = "Provider";
            this.Provider.ReadOnly = true;
            this.Provider.Width = 150;
            // 
            // Interval
            // 
            this.Interval.Frozen = true;
            this.Interval.HeaderText = "Интервал";
            this.Interval.Name = "Interval";
            this.Interval.ReadOnly = true;
            this.Interval.Width = 120;
            // 
            // TimeBegin
            // 
            this.TimeBegin.Frozen = true;
            this.TimeBegin.HeaderText = "Начало";
            this.TimeBegin.Name = "TimeBegin";
            this.TimeBegin.ReadOnly = true;
            this.TimeBegin.Width = 120;
            // 
            // TimeEnd
            // 
            this.TimeEnd.Frozen = true;
            this.TimeEnd.HeaderText = "Конец";
            this.TimeEnd.Name = "TimeEnd";
            this.TimeEnd.ReadOnly = true;
            this.TimeEnd.Width = 120;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(4, 5);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(186, 16);
            this.label8.TabIndex = 21;
            this.label8.Text = "Диапазоны исходных данных";
            // 
            // FormArchivesRanges
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 300);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.RangesList);
            this.Controls.Add(this.butArchiveTime);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormArchivesRanges";
            this.Text = "Архивы и источники";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormArchivesRanges_FormClosed);
            this.Load += new System.EventHandler(this.FormArchivesRanges_Load);
            ((System.ComponentModel.ISupportInitialize)(this.RangesList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button butArchiveTime;
        private System.Windows.Forms.DataGridView RangesList;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Provider;
        private System.Windows.Forms.DataGridViewTextBoxColumn Interval;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeBegin;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeEnd;
    }
}