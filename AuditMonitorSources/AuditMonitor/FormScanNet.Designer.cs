namespace AuditMonitor
{
    partial class FormScanNet
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
            this.gbPort = new System.Windows.Forms.GroupBox();
            this.cbPort = new System.Windows.Forms.ComboBox();
            this.gpScan = new System.Windows.Forms.GroupBox();
            this.labCurModuleAddress = new System.Windows.Forms.Label();
            this.tbCurModuleAddress = new System.Windows.Forms.TextBox();
            this.labLastModuleAddress = new System.Windows.Forms.Label();
            this.labFirstModuleAddress = new System.Windows.Forms.Label();
            this.tbLastModuleAddress = new System.Windows.Forms.TextBox();
            this.tbFirstModuleAddress = new System.Windows.Forms.TextBox();
            this.butScanNet = new System.Windows.Forms.Button();
            this.butLoadConfigFromObjectsPTK = new System.Windows.Forms.Button();
            this.gbPort.SuspendLayout();
            this.gpScan.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPort
            // 
            this.gbPort.Controls.Add(this.cbPort);
            this.gbPort.Location = new System.Drawing.Point(12, 12);
            this.gbPort.Name = "gbPort";
            this.gbPort.Size = new System.Drawing.Size(100, 76);
            this.gbPort.TabIndex = 0;
            this.gbPort.TabStop = false;
            this.gbPort.Text = "Порт";
            // 
            // cbPort
            // 
            this.cbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPort.FormattingEnabled = true;
            this.cbPort.Location = new System.Drawing.Point(6, 34);
            this.cbPort.Name = "cbPort";
            this.cbPort.Size = new System.Drawing.Size(88, 21);
            this.cbPort.TabIndex = 0;
            this.cbPort.SelectionChangeCommitted += new System.EventHandler(this.cbPort_SelectionChangeCommitted);
            // 
            // gpScan
            // 
            this.gpScan.Controls.Add(this.labCurModuleAddress);
            this.gpScan.Controls.Add(this.tbCurModuleAddress);
            this.gpScan.Controls.Add(this.labLastModuleAddress);
            this.gpScan.Controls.Add(this.labFirstModuleAddress);
            this.gpScan.Controls.Add(this.tbLastModuleAddress);
            this.gpScan.Controls.Add(this.tbFirstModuleAddress);
            this.gpScan.Location = new System.Drawing.Point(118, 12);
            this.gpScan.Name = "gpScan";
            this.gpScan.Size = new System.Drawing.Size(284, 76);
            this.gpScan.TabIndex = 1;
            this.gpScan.TabStop = false;
            this.gpScan.Text = "Сканирование";
            // 
            // labCurModuleAddress
            // 
            this.labCurModuleAddress.AutoSize = true;
            this.labCurModuleAddress.Location = new System.Drawing.Point(152, 37);
            this.labCurModuleAddress.Name = "labCurModuleAddress";
            this.labCurModuleAddress.Size = new System.Drawing.Size(62, 13);
            this.labCurModuleAddress.TabIndex = 5;
            this.labCurModuleAddress.Text = "Тек. адрес";
            this.labCurModuleAddress.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbCurModuleAddress
            // 
            this.tbCurModuleAddress.BackColor = System.Drawing.SystemColors.Window;
            this.tbCurModuleAddress.Location = new System.Drawing.Point(220, 34);
            this.tbCurModuleAddress.Name = "tbCurModuleAddress";
            this.tbCurModuleAddress.ReadOnly = true;
            this.tbCurModuleAddress.Size = new System.Drawing.Size(58, 20);
            this.tbCurModuleAddress.TabIndex = 4;
            // 
            // labLastModuleAddress
            // 
            this.labLastModuleAddress.AutoSize = true;
            this.labLastModuleAddress.Location = new System.Drawing.Point(6, 48);
            this.labLastModuleAddress.Name = "labLastModuleAddress";
            this.labLastModuleAddress.Size = new System.Drawing.Size(62, 13);
            this.labLastModuleAddress.TabIndex = 3;
            this.labLastModuleAddress.Text = "Кон. адрес";
            // 
            // labFirstModuleAddress
            // 
            this.labFirstModuleAddress.AutoSize = true;
            this.labFirstModuleAddress.Location = new System.Drawing.Point(6, 22);
            this.labFirstModuleAddress.Name = "labFirstModuleAddress";
            this.labFirstModuleAddress.Size = new System.Drawing.Size(62, 13);
            this.labFirstModuleAddress.TabIndex = 2;
            this.labFirstModuleAddress.Text = "Нач. адрес";
            // 
            // tbLastModuleAddress
            // 
            this.tbLastModuleAddress.Location = new System.Drawing.Point(74, 45);
            this.tbLastModuleAddress.Name = "tbLastModuleAddress";
            this.tbLastModuleAddress.Size = new System.Drawing.Size(58, 20);
            this.tbLastModuleAddress.TabIndex = 1;
            this.tbLastModuleAddress.Validating += new System.ComponentModel.CancelEventHandler(this.tbLastModuleAddress_Validating);
            // 
            // tbFirstModuleAddress
            // 
            this.tbFirstModuleAddress.Location = new System.Drawing.Point(74, 19);
            this.tbFirstModuleAddress.Name = "tbFirstModuleAddress";
            this.tbFirstModuleAddress.Size = new System.Drawing.Size(58, 20);
            this.tbFirstModuleAddress.TabIndex = 0;
            this.tbFirstModuleAddress.Validating += new System.ComponentModel.CancelEventHandler(this.tbFirstModuleAddress_Validating);
            // 
            // butScanNet
            // 
            this.butScanNet.Location = new System.Drawing.Point(118, 94);
            this.butScanNet.Name = "butScanNet";
            this.butScanNet.Size = new System.Drawing.Size(188, 40);
            this.butScanNet.TabIndex = 2;
            this.butScanNet.Text = "Сканировать сеть";
            this.butScanNet.UseVisualStyleBackColor = true;
            this.butScanNet.Click += new System.EventHandler(this.butScanNet_Click);
            // 
            // butLoadConfigFromObjectsPTK
            // 
            this.butLoadConfigFromObjectsPTK.Location = new System.Drawing.Point(118, 140);
            this.butLoadConfigFromObjectsPTK.Name = "butLoadConfigFromObjectsPTK";
            this.butLoadConfigFromObjectsPTK.Size = new System.Drawing.Size(188, 40);
            this.butLoadConfigFromObjectsPTK.TabIndex = 3;
            this.butLoadConfigFromObjectsPTK.Text = "Загрузить конфигурацию каналов";
            this.butLoadConfigFromObjectsPTK.UseVisualStyleBackColor = true;
            this.butLoadConfigFromObjectsPTK.Click += new System.EventHandler(this.butLoadConfigFromObjectsPTK_Click);
            // 
            // FormScanNet
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Sound;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 186);
            this.Controls.Add(this.butLoadConfigFromObjectsPTK);
            this.Controls.Add(this.butScanNet);
            this.Controls.Add(this.gpScan);
            this.Controls.Add(this.gbPort);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormScanNet";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Сканирование сети";
            this.gbPort.ResumeLayout(false);
            this.gpScan.ResumeLayout(false);
            this.gpScan.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPort;
        private System.Windows.Forms.ComboBox cbPort;
        private System.Windows.Forms.GroupBox gpScan;
        private System.Windows.Forms.Label labFirstModuleAddress;
        private System.Windows.Forms.TextBox tbLastModuleAddress;
        private System.Windows.Forms.TextBox tbFirstModuleAddress;
        private System.Windows.Forms.Label labCurModuleAddress;
        private System.Windows.Forms.TextBox tbCurModuleAddress;
        private System.Windows.Forms.Label labLastModuleAddress;
        private System.Windows.Forms.Button butScanNet;
        private System.Windows.Forms.Button butLoadConfigFromObjectsPTK;
    }
}