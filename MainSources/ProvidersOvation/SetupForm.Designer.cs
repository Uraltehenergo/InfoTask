namespace Provider
{
    partial class SetupForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.label1 = new System.Windows.Forms.Label();
            this.DataSource = new System.Windows.Forms.TextBox();
            this.butCheck = new System.Windows.Forms.Button();
            this.butOK = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.IsOriginal = new System.Windows.Forms.RadioButton();
            this.IsCopy = new System.Windows.Forms.RadioButton();
            this.DatabaseFile = new System.Windows.Forms.TextBox();
            this.butOpenDatabase = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(2, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(307, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Источник данных Historian ПТК \"Овация\"";
            // 
            // DataSource
            // 
            this.DataSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DataSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DataSource.Location = new System.Drawing.Point(2, 63);
            this.DataSource.Name = "DataSource";
            this.DataSource.Size = new System.Drawing.Size(379, 22);
            this.DataSource.TabIndex = 1;
            // 
            // butCheck
            // 
            this.butCheck.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCheck.Location = new System.Drawing.Point(192, 87);
            this.butCheck.Name = "butCheck";
            this.butCheck.Size = new System.Drawing.Size(189, 32);
            this.butCheck.TabIndex = 3;
            this.butCheck.Text = "Проверка соединения";
            this.butCheck.UseVisualStyleBackColor = true;
            this.butCheck.Click += new System.EventHandler(this.butCheck_Click);
            // 
            // butOK
            // 
            this.butOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butOK.Location = new System.Drawing.Point(67, 215);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(108, 32);
            this.butOK.TabIndex = 4;
            this.butOK.Text = "OK";
            this.butOK.UseVisualStyleBackColor = true;
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            // 
            // butCancel
            // 
            this.butCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCancel.Location = new System.Drawing.Point(211, 215);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(108, 32);
            this.butCancel.TabIndex = 5;
            this.butCancel.Text = "Отмена";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // IsOriginal
            // 
            this.IsOriginal.AutoSize = true;
            this.IsOriginal.Checked = true;
            this.IsOriginal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IsOriginal.Location = new System.Drawing.Point(12, 41);
            this.IsOriginal.Name = "IsOriginal";
            this.IsOriginal.Size = new System.Drawing.Size(307, 20);
            this.IsOriginal.TabIndex = 6;
            this.IsOriginal.TabStop = true;
            this.IsOriginal.Text = "Архив Historian (нужно указать DROP)";
            this.IsOriginal.UseVisualStyleBackColor = true;
            // 
            // IsCopy
            // 
            this.IsCopy.AutoSize = true;
            this.IsCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.IsCopy.Location = new System.Drawing.Point(11, 132);
            this.IsCopy.Name = "IsCopy";
            this.IsCopy.Size = new System.Drawing.Size(179, 20);
            this.IsCopy.TabIndex = 7;
            this.IsCopy.TabStop = true;
            this.IsCopy.Text = "Клон архива (.accdb)";
            this.IsCopy.UseVisualStyleBackColor = true;
            // 
            // DatabaseFile
            // 
            this.DatabaseFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DatabaseFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DatabaseFile.Location = new System.Drawing.Point(2, 155);
            this.DatabaseFile.Multiline = true;
            this.DatabaseFile.Name = "DatabaseFile";
            this.DatabaseFile.Size = new System.Drawing.Size(342, 38);
            this.DatabaseFile.TabIndex = 8;
            // 
            // butOpenDatabase
            // 
            this.butOpenDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butOpenDatabase.ImageKey = "folder_database.png";
            this.butOpenDatabase.ImageList = this.imageList1;
            this.butOpenDatabase.Location = new System.Drawing.Point(343, 155);
            this.butOpenDatabase.Name = "butOpenDatabase";
            this.butOpenDatabase.Size = new System.Drawing.Size(38, 38);
            this.butOpenDatabase.TabIndex = 41;
            this.butOpenDatabase.UseVisualStyleBackColor = true;
            this.butOpenDatabase.Click += new System.EventHandler(this.butOpenDatabase_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder_database.png");
            this.imageList1.Images.SetKeyName(1, "add.png");
            this.imageList1.Images.SetKeyName(2, "folder.png");
            // 
            // SetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 256);
            this.ControlBox = false;
            this.Controls.Add(this.butOpenDatabase);
            this.Controls.Add(this.DatabaseFile);
            this.Controls.Add(this.IsCopy);
            this.Controls.Add(this.IsOriginal);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.butCheck);
            this.Controls.Add(this.DataSource);
            this.Controls.Add(this.label1);
            this.Name = "SetupForm";
            this.Text = "Настройка источника";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SetupForm_FormClosing);
            this.Load += new System.EventHandler(this.SetupForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DataSource;
        private System.Windows.Forms.Button butCheck;
        private System.Windows.Forms.Button butOK;
        private System.Windows.Forms.Button butCancel;
        private System.Windows.Forms.RadioButton IsOriginal;
        private System.Windows.Forms.RadioButton IsCopy;
        private System.Windows.Forms.TextBox DatabaseFile;
        private System.Windows.Forms.Button butOpenDatabase;
        private System.Windows.Forms.ImageList imageList1;
    }
}