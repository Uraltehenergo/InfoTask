namespace ControllerMonitor
{
    partial class ErrorsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorsForm));
            this.DataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataColumn8 = new System.Data.DataColumn();
            this.Errors = new System.Windows.Forms.DataGridView();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeriodBegin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PeriodEnd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Params = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Command = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Context = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ThreadId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorsTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.BindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.ButUpdate = new System.Windows.Forms.Button();
            this.ButDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Errors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorsTableBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // DataSet1
            // 
            this.DataSet1.DataSetName = "NewDataSet";
            this.DataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7,
            this.dataColumn8});
            this.dataTable1.TableName = "ErrorsTable";
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "Время";
            this.dataColumn1.ColumnName = "Time";
            this.dataColumn1.DataType = typeof(System.DateTime);
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "Ошибка";
            this.dataColumn2.ColumnName = "Description";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Начало";
            this.dataColumn3.ColumnName = "PeriodBegin";
            this.dataColumn3.DataType = typeof(System.DateTime);
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "Конец";
            this.dataColumn4.ColumnName = "PeriodEnd";
            this.dataColumn4.DataType = typeof(System.DateTime);
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "Параметры";
            this.dataColumn5.ColumnName = "Params";
            // 
            // dataColumn6
            // 
            this.dataColumn6.Caption = "Команда";
            this.dataColumn6.ColumnName = "Command";
            // 
            // dataColumn7
            // 
            this.dataColumn7.Caption = "Контекст";
            this.dataColumn7.ColumnName = "Context";
            // 
            // dataColumn8
            // 
            this.dataColumn8.Caption = "Поток";
            this.dataColumn8.ColumnName = "ThreadId";
            this.dataColumn8.DataType = typeof(int);
            // 
            // Errors
            // 
            this.Errors.AllowUserToAddRows = false;
            this.Errors.AllowUserToDeleteRows = false;
            this.Errors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Errors.AutoGenerateColumns = false;
            this.Errors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Errors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Description,
            this.PeriodBegin,
            this.PeriodEnd,
            this.Params,
            this.Command,
            this.Context,
            this.ThreadId});
            this.Errors.DataSource = this.errorsTableBindingSource;
            this.Errors.Location = new System.Drawing.Point(1, 32);
            this.Errors.Name = "Errors";
            this.Errors.ReadOnly = true;
            this.Errors.RowHeadersWidth = 10;
            this.Errors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.Errors.Size = new System.Drawing.Size(1315, 586);
            this.Errors.TabIndex = 0;
            // 
            // Time
            // 
            this.Time.DataPropertyName = "Time";
            this.Time.HeaderText = "Время";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Width = 110;
            // 
            // Description
            // 
            this.Description.DataPropertyName = "Description";
            this.Description.HeaderText = "Описание";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.Width = 380;
            // 
            // PeriodBegin
            // 
            this.PeriodBegin.DataPropertyName = "PeriodBegin";
            this.PeriodBegin.HeaderText = "Начало";
            this.PeriodBegin.Name = "PeriodBegin";
            this.PeriodBegin.ReadOnly = true;
            this.PeriodBegin.Width = 110;
            // 
            // PeriodEnd
            // 
            this.PeriodEnd.DataPropertyName = "PeriodEnd";
            this.PeriodEnd.HeaderText = "Конец";
            this.PeriodEnd.Name = "PeriodEnd";
            this.PeriodEnd.ReadOnly = true;
            this.PeriodEnd.Width = 110;
            // 
            // Params
            // 
            this.Params.DataPropertyName = "Params";
            this.Params.HeaderText = "Параметры";
            this.Params.Name = "Params";
            this.Params.ReadOnly = true;
            this.Params.Width = 200;
            // 
            // Command
            // 
            this.Command.DataPropertyName = "Command";
            this.Command.HeaderText = "Команда";
            this.Command.Name = "Command";
            this.Command.ReadOnly = true;
            this.Command.Width = 190;
            // 
            // Context
            // 
            this.Context.DataPropertyName = "Context";
            this.Context.HeaderText = "Контекст";
            this.Context.Name = "Context";
            this.Context.ReadOnly = true;
            this.Context.Width = 140;
            // 
            // ThreadId
            // 
            this.ThreadId.DataPropertyName = "ThreadId";
            this.ThreadId.HeaderText = "Поток";
            this.ThreadId.Name = "ThreadId";
            this.ThreadId.ReadOnly = true;
            this.ThreadId.Width = 50;
            // 
            // errorsTableBindingSource
            // 
            this.errorsTableBindingSource.DataMember = "ErrorsTable";
            this.errorsTableBindingSource.DataSource = this.BindingSource1;
            // 
            // BindingSource1
            // 
            this.BindingSource1.DataSource = this.DataSet1;
            this.BindingSource1.Position = 0;
            // 
            // ButUpdate
            // 
            this.ButUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButUpdate.Location = new System.Drawing.Point(1, 2);
            this.ButUpdate.Name = "ButUpdate";
            this.ButUpdate.Size = new System.Drawing.Size(161, 29);
            this.ButUpdate.TabIndex = 1;
            this.ButUpdate.Text = "Обновить";
            this.ButUpdate.UseVisualStyleBackColor = true;
            this.ButUpdate.Click += new System.EventHandler(this.ButUpdate_Click);
            // 
            // ButDelete
            // 
            this.ButDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ButDelete.Location = new System.Drawing.Point(1145, 2);
            this.ButDelete.Name = "ButDelete";
            this.ButDelete.Size = new System.Drawing.Size(171, 29);
            this.ButDelete.TabIndex = 2;
            this.ButDelete.Text = "Удалить все ошибки";
            this.ButDelete.UseVisualStyleBackColor = true;
            this.ButDelete.Click += new System.EventHandler(this.ButDelete_Click);
            // 
            // ErrorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 619);
            this.Controls.Add(this.ButDelete);
            this.Controls.Add(this.ButUpdate);
            this.Controls.Add(this.Errors);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ErrorsForm";
            this.Text = "Ошибки";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.DataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Errors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorsTableBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Data.DataSet DataSet1;
        private System.Windows.Forms.BindingSource BindingSource1;
        private System.Windows.Forms.DataGridView Errors;
        private System.Windows.Forms.Button ButUpdate;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Data.DataColumn dataColumn8;
        private System.Windows.Forms.BindingSource errorsTableBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodBegin;
        private System.Windows.Forms.DataGridViewTextBoxColumn PeriodEnd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Params;
        private System.Windows.Forms.DataGridViewTextBoxColumn Command;
        private System.Windows.Forms.DataGridViewTextBoxColumn Context;
        private System.Windows.Forms.DataGridViewTextBoxColumn ThreadId;
        private System.Windows.Forms.Button ButDelete;
    }
}