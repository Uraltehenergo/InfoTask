namespace ReporterCommon
{
    partial class FormAbsoluteEdit
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbsoluteEdit));
            this.panel1 = new System.Windows.Forms.Panel();
            this.Value = new System.Windows.Forms.TextBox();
            this.Time = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.TimePicker = new System.Windows.Forms.DateTimePicker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.OldValue = new System.Windows.Forms.TextBox();
            this.OldTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.butChangeValue = new System.Windows.Forms.Button();
            this.Props = new System.Windows.Forms.DataGridView();
            this.PropName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PropValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Props)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.Value);
            this.panel1.Controls.Add(this.Time);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.TimePicker);
            this.panel1.Location = new System.Drawing.Point(2, 192);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(340, 58);
            this.panel1.TabIndex = 16;
            // 
            // Value
            // 
            this.Value.BackColor = System.Drawing.SystemColors.Window;
            this.Value.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Value.Location = new System.Drawing.Point(166, 4);
            this.Value.Name = "Value";
            this.Value.Size = new System.Drawing.Size(151, 22);
            this.Value.TabIndex = 15;
            this.Value.Text = "****";
            // 
            // Time
            // 
            this.Time.BackColor = System.Drawing.SystemColors.Window;
            this.Time.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Time.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Time.Location = new System.Drawing.Point(166, 28);
            this.Time.Name = "Time";
            this.Time.Size = new System.Drawing.Size(151, 22);
            this.Time.TabIndex = 20;
            this.Time.Text = "00.00.0000 00:00:00";
            this.Time.Visible = false;
            this.Time.Validated += new System.EventHandler(this.Time_Validated);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(6, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 18);
            this.label5.TabIndex = 16;
            this.label5.Text = "Новое";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(80, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 16);
            this.label8.TabIndex = 19;
            this.label8.Text = "Значение:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label9.Location = new System.Drawing.Point(107, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 16);
            this.label9.TabIndex = 17;
            this.label9.Text = "Время:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label9.Visible = false;
            // 
            // TimePicker
            // 
            this.TimePicker.CustomFormat = "dd.MM.yyyy hh:mm:ss";
            this.TimePicker.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.TimePicker.Location = new System.Drawing.Point(314, 28);
            this.TimePicker.Name = "TimePicker";
            this.TimePicker.Size = new System.Drawing.Size(17, 22);
            this.TimePicker.TabIndex = 25;
            this.TimePicker.Visible = false;
            this.TimePicker.ValueChanged += new System.EventHandler(this.TimePicker_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.OldValue);
            this.panel2.Controls.Add(this.OldTime);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Location = new System.Drawing.Point(2, 136);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(340, 54);
            this.panel2.TabIndex = 17;
            // 
            // OldValue
            // 
            this.OldValue.BackColor = System.Drawing.SystemColors.Menu;
            this.OldValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OldValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OldValue.Location = new System.Drawing.Point(166, 6);
            this.OldValue.Name = "OldValue";
            this.OldValue.ReadOnly = true;
            this.OldValue.Size = new System.Drawing.Size(151, 22);
            this.OldValue.TabIndex = 5;
            this.OldValue.Text = "****";
            // 
            // OldTime
            // 
            this.OldTime.BackColor = System.Drawing.SystemColors.Menu;
            this.OldTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.OldTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.OldTime.Location = new System.Drawing.Point(166, 27);
            this.OldTime.Name = "OldTime";
            this.OldTime.ReadOnly = true;
            this.OldTime.Size = new System.Drawing.Size(151, 22);
            this.OldTime.TabIndex = 10;
            this.OldTime.Text = "00.00.0000 00:00:00";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(0, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 22);
            this.label4.TabIndex = 12;
            this.label4.Text = "Прежнее";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(79, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 16);
            this.label7.TabIndex = 15;
            this.label7.Text = "Значение:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(106, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "Время:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // butChangeValue
            // 
            this.butChangeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butChangeValue.Location = new System.Drawing.Point(56, 253);
            this.butChangeValue.Name = "butChangeValue";
            this.butChangeValue.Size = new System.Drawing.Size(242, 37);
            this.butChangeValue.TabIndex = 30;
            this.butChangeValue.Text = "Установить новое значение";
            this.butChangeValue.UseVisualStyleBackColor = true;
            this.butChangeValue.Click += new System.EventHandler(this.butChangeValue_Click);
            // 
            // Props
            // 
            this.Props.AllowUserToAddRows = false;
            this.Props.AllowUserToDeleteRows = false;
            this.Props.AllowUserToResizeColumns = false;
            this.Props.AllowUserToResizeRows = false;
            this.Props.BackgroundColor = System.Drawing.SystemColors.Control;
            this.Props.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Props.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Props.ColumnHeadersVisible = false;
            this.Props.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PropName,
            this.PropValue});
            this.Props.GridColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Props.Location = new System.Drawing.Point(2, 1);
            this.Props.Name = "Props";
            this.Props.ReadOnly = true;
            this.Props.RowHeadersVisible = false;
            this.Props.Size = new System.Drawing.Size(340, 133);
            this.Props.TabIndex = 0;
            // 
            // PropName
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PropName.DefaultCellStyle = dataGridViewCellStyle1;
            this.PropName.Frozen = true;
            this.PropName.HeaderText = "Column1";
            this.PropName.Name = "PropName";
            this.PropName.ReadOnly = true;
            this.PropName.Width = 59;
            // 
            // PropValue
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PropValue.DefaultCellStyle = dataGridViewCellStyle2;
            this.PropValue.Frozen = true;
            this.PropValue.HeaderText = "Column1";
            this.PropValue.Name = "PropValue";
            this.PropValue.ReadOnly = true;
            this.PropValue.Width = 280;
            // 
            // FormAbsoluteEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 291);
            this.Controls.Add(this.Props);
            this.Controls.Add(this.butChangeValue);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormAbsoluteEdit";
            this.Text = "Правка абсолютных значений";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormAbsoluteEdit_FormClosed);
            this.Load += new System.EventHandler(this.FormAbsoluteEdit_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Props)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox Value;
        public System.Windows.Forms.TextBox Time;
        public System.Windows.Forms.TextBox OldValue;
        public System.Windows.Forms.TextBox OldTime;
        private System.Windows.Forms.Button butChangeValue;
        private System.Windows.Forms.DataGridView Props;
        private System.Windows.Forms.DateTimePicker TimePicker;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropName;
        private System.Windows.Forms.DataGridViewTextBoxColumn PropValue;
    }
}