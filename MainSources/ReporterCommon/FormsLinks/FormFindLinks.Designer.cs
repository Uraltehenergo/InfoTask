namespace ReporterCommon
{
    partial class FormFindLinks
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFindLinks));
            this.Cells = new System.Windows.Forms.DataGridView();
            this.Page = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CellAdr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Field = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LinkType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.Code = new System.Windows.Forms.TextBox();
            this.Project = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FirstName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.LastName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.Cells)).BeginInit();
            this.SuspendLayout();
            // 
            // Cells
            // 
            this.Cells.AllowUserToAddRows = false;
            this.Cells.AllowUserToDeleteRows = false;
            this.Cells.AllowUserToOrderColumns = true;
            this.Cells.AllowUserToResizeRows = false;
            this.Cells.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Cells.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.Cells.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Cells.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Page,
            this.Cell,
            this.CellAdr,
            this.Field,
            this.LinkType});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.Cells.DefaultCellStyle = dataGridViewCellStyle2;
            this.Cells.Location = new System.Drawing.Point(0, 54);
            this.Cells.Name = "Cells";
            this.Cells.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Cells.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.Cells.RowHeadersVisible = false;
            this.Cells.RowHeadersWidth = 15;
            this.Cells.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.Cells.Size = new System.Drawing.Size(675, 208);
            this.Cells.TabIndex = 0;
            this.Cells.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.Cells_CellDoubleClick);
            this.Cells.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Cells_MouseDoubleClick);
            // 
            // Page
            // 
            this.Page.HeaderText = "Лист";
            this.Page.Name = "Page";
            this.Page.ReadOnly = true;
            this.Page.Width = 150;
            // 
            // Cell
            // 
            this.Cell.HeaderText = "Ячейка";
            this.Cell.Name = "Cell";
            this.Cell.ReadOnly = true;
            this.Cell.Width = 80;
            // 
            // CellAdr
            // 
            this.CellAdr.HeaderText = "Column1";
            this.CellAdr.Name = "CellAdr";
            this.CellAdr.ReadOnly = true;
            this.CellAdr.Visible = false;
            // 
            // Field
            // 
            this.Field.HeaderText = "Свойство";
            this.Field.Name = "Field";
            this.Field.ReadOnly = true;
            this.Field.Width = 180;
            // 
            // LinkType
            // 
            this.LinkType.HeaderText = "Тип ссылки";
            this.LinkType.Name = "LinkType";
            this.LinkType.ReadOnly = true;
            this.LinkType.Width = 240;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Код";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Code
            // 
            this.Code.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Code.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Code.Location = new System.Drawing.Point(41, 2);
            this.Code.Name = "Code";
            this.Code.ReadOnly = true;
            this.Code.Size = new System.Drawing.Size(310, 22);
            this.Code.TabIndex = 2;
            // 
            // Project
            // 
            this.Project.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Project.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Project.Location = new System.Drawing.Point(425, 2);
            this.Project.Name = "Project";
            this.Project.ReadOnly = true;
            this.Project.Size = new System.Drawing.Size(250, 22);
            this.Project.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(360, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Проект";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FirstName
            // 
            this.FirstName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FirstName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FirstName.Location = new System.Drawing.Point(41, 26);
            this.FirstName.Name = "FirstName";
            this.FirstName.ReadOnly = true;
            this.FirstName.Size = new System.Drawing.Size(382, 22);
            this.FirstName.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(1, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 16);
            this.label3.TabIndex = 5;
            this.label3.Text = "Имя";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // LastName
            // 
            this.LastName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LastName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LastName.Location = new System.Drawing.Point(425, 26);
            this.LastName.Name = "LastName";
            this.LastName.ReadOnly = true;
            this.LastName.Size = new System.Drawing.Size(250, 22);
            this.LastName.TabIndex = 7;
            // 
            // FormFindLinks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 263);
            this.Controls.Add(this.LastName);
            this.Controls.Add(this.Project);
            this.Controls.Add(this.FirstName);
            this.Controls.Add(this.Code);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Cells);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormFindLinks";
            this.Text = "Привязанные к параметру ячейки";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormFindLinksWin_FormClosing);
            this.Load += new System.EventHandler(this.FormFindLinksWin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Cells)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView Cells;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox Code;
        public System.Windows.Forms.TextBox Project;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox FirstName;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox LastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Page;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cell;
        private System.Windows.Forms.DataGridViewTextBoxColumn CellAdr;
        private System.Windows.Forms.DataGridViewTextBoxColumn Field;
        private System.Windows.Forms.DataGridViewTextBoxColumn LinkType;
    }
}