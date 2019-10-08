namespace ControllerMonitor
{
    partial class ThreadsForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThreadsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Threads = new System.Windows.Forms.DataGridView();
            this.threadsTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.BindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.DataSet1 = new System.Data.DataSet();
            this.ThreadsTable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.ButThread = new System.Windows.Forms.Button();
            this.ButSetup = new System.Windows.Forms.Button();
            this.ButKvit = new System.Windows.Forms.Button();
            this.ButUpdate = new System.Windows.Forms.Button();
            this.ErrorTablo = new System.Windows.Forms.Label();
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.ThreadId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.Threads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadsTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ThreadsTable)).BeginInit();
            this.SuspendLayout();
            // 
            // Threads
            // 
            this.Threads.AllowUserToAddRows = false;
            this.Threads.AllowUserToDeleteRows = false;
            this.Threads.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Threads.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Threads.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Threads.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Threads.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ThreadId,
            this.LastTime,
            this.LastText});
            this.Threads.DataSource = this.threadsTableBindingSource;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Threads.DefaultCellStyle = dataGridViewCellStyle3;
            this.Threads.Location = new System.Drawing.Point(1, 59);
            this.Threads.MultiSelect = false;
            this.Threads.Name = "Threads";
            this.Threads.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Threads.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.Threads.RowHeadersWidth = 20;
            this.Threads.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Threads.Size = new System.Drawing.Size(689, 212);
            this.Threads.TabIndex = 0;
            this.Threads.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Threads_CellContentDoubleClick);
            // 
            // threadsTableBindingSource
            // 
            this.threadsTableBindingSource.DataMember = "ThreadsTable";
            this.threadsTableBindingSource.DataSource = this.BindingSource1;
            // 
            // BindingSource1
            // 
            this.BindingSource1.DataSource = this.DataSet1;
            this.BindingSource1.Position = 0;
            // 
            // DataSet1
            // 
            this.DataSet1.DataSetName = "NewDataSet";
            this.DataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.ThreadsTable});
            // 
            // ThreadsTable
            // 
            this.ThreadsTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3});
            this.ThreadsTable.TableName = "ThreadsTable";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Поток";
            this.dataColumn1.ColumnName = "ThreadId";
            this.dataColumn1.DataType = typeof(int);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Время ошибки";
            this.dataColumn2.ColumnName = "LastTime";
            this.dataColumn2.DataType = typeof(System.DateTime);
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Ошибка";
            this.dataColumn3.ColumnName = "LastText";
            // 
            // ButThread
            // 
            this.ButThread.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButThread.Location = new System.Drawing.Point(282, 2);
            this.ButThread.Name = "ButThread";
            this.ButThread.Size = new System.Drawing.Size(143, 28);
            this.ButThread.TabIndex = 1;
            this.ButThread.Text = "События потока";
            this.ButThread.UseVisualStyleBackColor = true;
            this.ButThread.Click += new System.EventHandler(this.ButThread_Click);
            // 
            // ButSetup
            // 
            this.ButSetup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButSetup.Location = new System.Drawing.Point(427, 2);
            this.ButSetup.Name = "ButSetup";
            this.ButSetup.Size = new System.Drawing.Size(144, 28);
            this.ButSetup.TabIndex = 2;
            this.ButSetup.Text = "Настройка истории";
            this.ButSetup.UseVisualStyleBackColor = true;
            this.ButSetup.Click += new System.EventHandler(this.ButSetup_Click);
            // 
            // ButKvit
            // 
            this.ButKvit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButKvit.Location = new System.Drawing.Point(141, 2);
            this.ButKvit.Name = "ButKvit";
            this.ButKvit.Size = new System.Drawing.Size(139, 28);
            this.ButKvit.TabIndex = 3;
            this.ButKvit.Text = "Квитировать";
            this.ButKvit.UseVisualStyleBackColor = true;
            this.ButKvit.Click += new System.EventHandler(this.ButKvit_Click);
            // 
            // ButUpdate
            // 
            this.ButUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButUpdate.Location = new System.Drawing.Point(0, 2);
            this.ButUpdate.Name = "ButUpdate";
            this.ButUpdate.Size = new System.Drawing.Size(139, 28);
            this.ButUpdate.TabIndex = 4;
            this.ButUpdate.Text = "Обновить";
            this.ButUpdate.UseVisualStyleBackColor = true;
            this.ButUpdate.Click += new System.EventHandler(this.ButUpdate_Click);
            // 
            // ErrorTablo
            // 
            this.ErrorTablo.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ErrorTablo.ForeColor = System.Drawing.Color.Red;
            this.ErrorTablo.Location = new System.Drawing.Point(-2, 33);
            this.ErrorTablo.Name = "ErrorTablo";
            this.ErrorTablo.Size = new System.Drawing.Size(690, 24);
            this.ErrorTablo.TabIndex = 5;
            this.ErrorTablo.Text = "Ошибка";
            this.ErrorTablo.Click += new System.EventHandler(this.ErrorTablo_Click);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            this.NotifyIcon.BalloonTipText = "Ошибка";
            this.NotifyIcon.BalloonTipTitle = "InfoTask";
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "История монитора InfoTask";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(573, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 28);
            this.button1.TabIndex = 6;
            this.button1.Text = "Выход";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ButExit_Click);
            // 
            // ThreadId
            // 
            this.ThreadId.DataPropertyName = "ThreadId";
            this.ThreadId.HeaderText = "Поток";
            this.ThreadId.Name = "ThreadId";
            this.ThreadId.ReadOnly = true;
            this.ThreadId.Width = 60;
            // 
            // LastTime
            // 
            this.LastTime.DataPropertyName = "LastTime";
            dataGridViewCellStyle2.Format = "dd.MM.yyyy HH:mm:ss";
            dataGridViewCellStyle2.NullValue = null;
            this.LastTime.DefaultCellStyle = dataGridViewCellStyle2;
            this.LastTime.HeaderText = "Время";
            this.LastTime.Name = "LastTime";
            this.LastTime.ReadOnly = true;
            this.LastTime.Width = 130;
            // 
            // LastText
            // 
            this.LastText.DataPropertyName = "LastText";
            this.LastText.HeaderText = "Сообщение";
            this.LastText.Name = "LastText";
            this.LastText.ReadOnly = true;
            this.LastText.Width = 470;
            // 
            // ThreadsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 272);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ErrorTablo);
            this.Controls.Add(this.ButUpdate);
            this.Controls.Add(this.ButKvit);
            this.Controls.Add(this.ButSetup);
            this.Controls.Add(this.ButThread);
            this.Controls.Add(this.Threads);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ThreadsForm";
            this.ShowInTaskbar = false;
            this.Text = "Мониторинг ошибок расчета";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ThreadsForm_FormClosing);
            this.Load += new System.EventHandler(this.ThreadsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Threads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.threadsTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ThreadsTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ButThread;
        private System.Windows.Forms.Button ButSetup;
        internal System.Windows.Forms.DataGridView Threads;
        private System.Windows.Forms.BindingSource BindingSource1;
        private System.Data.DataTable ThreadsTable;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        internal System.Data.DataSet DataSet1;
        private System.Windows.Forms.Button ButKvit;
        private System.Windows.Forms.Button ButUpdate;
        internal System.Windows.Forms.Label ErrorTablo;
        private System.Windows.Forms.BindingSource threadsTableBindingSource;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        internal System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn ThreadId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastText;
    }
}

