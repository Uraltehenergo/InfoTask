namespace AuditMonitor
{
    partial class FormModuleInit
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
            this.labStep2 = new System.Windows.Forms.Label();
            this.labStepComplited = new System.Windows.Forms.Label();
            this.labStepNum = new System.Windows.Forms.Label();
            this.labStep3 = new System.Windows.Forms.Label();
            this.labStep1 = new System.Windows.Forms.Label();
            this.frModuleInit = new System.Windows.Forms.GroupBox();
            this.labChacksum = new System.Windows.Forms.Label();
            this.labIntegrationTime = new System.Windows.Forms.Label();
            this.cbIntegrationTime = new System.Windows.Forms.ComboBox();
            this.labDataFormat = new System.Windows.Forms.Label();
            this.cbDataFormat = new System.Windows.Forms.ComboBox();
            this.cbChecksum = new System.Windows.Forms.ComboBox();
            this.cbBaudRate = new System.Windows.Forms.ComboBox();
            this.cbInputRange = new System.Windows.Forms.ComboBox();
            this.labBaudRate = new System.Windows.Forms.Label();
            this.labInputRange = new System.Windows.Forms.Label();
            this.tbAddress = new System.Windows.Forms.TextBox();
            this.tbAddress10 = new System.Windows.Forms.TextBox();
            this.labAddress = new System.Windows.Forms.Label();
            this.labStep4 = new System.Windows.Forms.Label();
            this.butNext = new System.Windows.Forms.Button();
            this.tbCommand = new System.Windows.Forms.TextBox();
            this.frModuleInit.SuspendLayout();
            this.SuspendLayout();
            // 
            // labStep2
            // 
            this.labStep2.AutoSize = true;
            this.labStep2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labStep2.Location = new System.Drawing.Point(64, 9);
            this.labStep2.Name = "labStep2";
            this.labStep2.Size = new System.Drawing.Size(319, 16);
            this.labStep2.TabIndex = 1;
            this.labStep2.Text = "Введите параметры модуля и нажмите \"Далее\".";
            this.labStep2.Visible = false;
            // 
            // labStepComplited
            // 
            this.labStepComplited.AutoSize = true;
            this.labStepComplited.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labStepComplited.Location = new System.Drawing.Point(64, 9);
            this.labStepComplited.MaximumSize = new System.Drawing.Size(302, 0);
            this.labStepComplited.Name = "labStepComplited";
            this.labStepComplited.Size = new System.Drawing.Size(302, 16);
            this.labStepComplited.TabIndex = 6;
            this.labStepComplited.Text = "Инициализация модуля успешно завершена!";
            this.labStepComplited.Visible = false;
            // 
            // labStepNum
            // 
            this.labStepNum.AutoSize = true;
            this.labStepNum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labStepNum.Location = new System.Drawing.Point(12, 9);
            this.labStepNum.Name = "labStepNum";
            this.labStepNum.Size = new System.Drawing.Size(46, 16);
            this.labStepNum.TabIndex = 8;
            this.labStepNum.Text = "Шаг 1.";
            // 
            // labStep3
            // 
            this.labStep3.AutoSize = true;
            this.labStep3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labStep3.Location = new System.Drawing.Point(64, 9);
            this.labStep3.Name = "labStep3";
            this.labStep3.Size = new System.Drawing.Size(202, 16);
            this.labStep3.TabIndex = 9;
            this.labStep3.Text = "Подождите несколько секунд";
            this.labStep3.Visible = false;
            // 
            // labStep1
            // 
            this.labStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labStep1.Location = new System.Drawing.Point(64, 9);
            this.labStep1.Name = "labStep1";
            this.labStep1.Size = new System.Drawing.Size(336, 51);
            this.labStep1.TabIndex = 24;
            this.labStep1.Text = "Переключите инициализируемый модуль в состояние \'Init\' и включите его в сеть. Отк" +
    "лючите другие модули. Затем нажмите \"Далее\".";
            // 
            // frModuleInit
            // 
            this.frModuleInit.Controls.Add(this.labChacksum);
            this.frModuleInit.Controls.Add(this.labIntegrationTime);
            this.frModuleInit.Controls.Add(this.cbIntegrationTime);
            this.frModuleInit.Controls.Add(this.labDataFormat);
            this.frModuleInit.Controls.Add(this.cbDataFormat);
            this.frModuleInit.Controls.Add(this.cbChecksum);
            this.frModuleInit.Controls.Add(this.cbBaudRate);
            this.frModuleInit.Controls.Add(this.cbInputRange);
            this.frModuleInit.Controls.Add(this.labBaudRate);
            this.frModuleInit.Controls.Add(this.labInputRange);
            this.frModuleInit.Controls.Add(this.tbAddress);
            this.frModuleInit.Controls.Add(this.tbAddress10);
            this.frModuleInit.Controls.Add(this.labAddress);
            this.frModuleInit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.frModuleInit.Location = new System.Drawing.Point(67, 32);
            this.frModuleInit.Name = "frModuleInit";
            this.frModuleInit.Size = new System.Drawing.Size(270, 200);
            this.frModuleInit.TabIndex = 25;
            this.frModuleInit.TabStop = false;
            this.frModuleInit.Text = "Параметры модуля";
            this.frModuleInit.Visible = false;
            // 
            // labChacksum
            // 
            this.labChacksum.AutoSize = true;
            this.labChacksum.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labChacksum.Location = new System.Drawing.Point(7, 172);
            this.labChacksum.Name = "labChacksum";
            this.labChacksum.Size = new System.Drawing.Size(140, 16);
            this.labChacksum.TabIndex = 36;
            this.labChacksum.Text = "Контрольная сумма:";
            // 
            // labIntegrationTime
            // 
            this.labIntegrationTime.AutoSize = true;
            this.labIntegrationTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labIntegrationTime.Location = new System.Drawing.Point(15, 142);
            this.labIntegrationTime.Name = "labIntegrationTime";
            this.labIntegrationTime.Size = new System.Drawing.Size(132, 16);
            this.labIntegrationTime.TabIndex = 35;
            this.labIntegrationTime.Text = "Время интеграции:";
            // 
            // cbIntegrationTime
            // 
            this.cbIntegrationTime.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIntegrationTime.Enabled = false;
            this.cbIntegrationTime.FormattingEnabled = true;
            this.cbIntegrationTime.Items.AddRange(new object[] {
            "50 ms",
            "60 ms"});
            this.cbIntegrationTime.Location = new System.Drawing.Point(152, 139);
            this.cbIntegrationTime.Name = "cbIntegrationTime";
            this.cbIntegrationTime.Size = new System.Drawing.Size(112, 24);
            this.cbIntegrationTime.Sorted = true;
            this.cbIntegrationTime.TabIndex = 34;
            // 
            // labDataFormat
            // 
            this.labDataFormat.AutoSize = true;
            this.labDataFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labDataFormat.Location = new System.Drawing.Point(34, 112);
            this.labDataFormat.Name = "labDataFormat";
            this.labDataFormat.Size = new System.Drawing.Size(112, 16);
            this.labDataFormat.TabIndex = 33;
            this.labDataFormat.Text = "Формат данных:";
            // 
            // cbDataFormat
            // 
            this.cbDataFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDataFormat.Enabled = false;
            this.cbDataFormat.FormattingEnabled = true;
            this.cbDataFormat.Items.AddRange(new object[] {
            "00     Eng units",
            "01     % of FSR",
            "10     TCH"});
            this.cbDataFormat.Location = new System.Drawing.Point(152, 109);
            this.cbDataFormat.Name = "cbDataFormat";
            this.cbDataFormat.Size = new System.Drawing.Size(112, 24);
            this.cbDataFormat.Sorted = true;
            this.cbDataFormat.TabIndex = 32;
            // 
            // cbChecksum
            // 
            this.cbChecksum.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.cbChecksum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChecksum.Enabled = false;
            this.cbChecksum.FormattingEnabled = true;
            this.cbChecksum.Items.AddRange(new object[] {
            "Disabled",
            "Enabled"});
            this.cbChecksum.Location = new System.Drawing.Point(152, 169);
            this.cbChecksum.Name = "cbChecksum";
            this.cbChecksum.Size = new System.Drawing.Size(112, 24);
            this.cbChecksum.Sorted = true;
            this.cbChecksum.TabIndex = 31;
            // 
            // cbBaudRate
            // 
            this.cbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBaudRate.Enabled = false;
            this.cbBaudRate.FormattingEnabled = true;
            this.cbBaudRate.Items.AddRange(new object[] {
            "03     1200 bps",
            "04     2400 bps",
            "05     4800 bps",
            "06     9600 bps",
            "07     19.2 kbps",
            "08     38.4 kbps"});
            this.cbBaudRate.Location = new System.Drawing.Point(152, 79);
            this.cbBaudRate.Name = "cbBaudRate";
            this.cbBaudRate.Size = new System.Drawing.Size(112, 24);
            this.cbBaudRate.Sorted = true;
            this.cbBaudRate.TabIndex = 30;
            // 
            // cbInputRange
            // 
            this.cbInputRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputRange.Enabled = false;
            this.cbInputRange.FormattingEnabled = true;
            this.cbInputRange.Items.AddRange(new object[] {
            "00",
            "02     ± 100 mV",
            "03     ± 500 mV",
            "04     ± 1 V",
            "05     ± 2.5 V",
            "07     +4~20mA",
            "08     ± 10 V",
            "09     ± 5 V",
            "0D     ± 20 mA",
            "0E     Type J",
            "0F     Type K",
            "10     Type T",
            "11     Type E",
            "12     Type R",
            "13     Type S",
            "14     Type B",
            "FF"});
            this.cbInputRange.Location = new System.Drawing.Point(152, 49);
            this.cbInputRange.Name = "cbInputRange";
            this.cbInputRange.Size = new System.Drawing.Size(112, 24);
            this.cbInputRange.Sorted = true;
            this.cbInputRange.TabIndex = 29;
            // 
            // labBaudRate
            // 
            this.labBaudRate.AutoSize = true;
            this.labBaudRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labBaudRate.Location = new System.Drawing.Point(7, 82);
            this.labBaudRate.Name = "labBaudRate";
            this.labBaudRate.Size = new System.Drawing.Size(139, 16);
            this.labBaudRate.TabIndex = 28;
            this.labBaudRate.Text = "Скорость передачи:";
            // 
            // labInputRange
            // 
            this.labInputRange.AutoSize = true;
            this.labInputRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labInputRange.Location = new System.Drawing.Point(14, 52);
            this.labInputRange.Name = "labInputRange";
            this.labInputRange.Size = new System.Drawing.Size(133, 16);
            this.labInputRange.TabIndex = 27;
            this.labInputRange.Text = "Входной диапазон:";
            // 
            // tbAddress
            // 
            this.tbAddress.Location = new System.Drawing.Point(211, 21);
            this.tbAddress.Name = "tbAddress";
            this.tbAddress.Size = new System.Drawing.Size(53, 22);
            this.tbAddress.TabIndex = 1;
            this.tbAddress.Validating += new System.ComponentModel.CancelEventHandler(this.tbAddress_Validating);
            // 
            // tbAddress10
            // 
            this.tbAddress10.Location = new System.Drawing.Point(152, 21);
            this.tbAddress10.Name = "tbAddress10";
            this.tbAddress10.Size = new System.Drawing.Size(53, 22);
            this.tbAddress10.TabIndex = 0;
            this.tbAddress10.Validating += new System.ComponentModel.CancelEventHandler(this.tbAddress10_Validating);
            // 
            // labAddress
            // 
            this.labAddress.AutoSize = true;
            this.labAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labAddress.Location = new System.Drawing.Point(44, 24);
            this.labAddress.Name = "labAddress";
            this.labAddress.Size = new System.Drawing.Size(102, 16);
            this.labAddress.TabIndex = 24;
            this.labAddress.Text = "Адрес модуля:";
            // 
            // labStep4
            // 
            this.labStep4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labStep4.Location = new System.Drawing.Point(65, 9);
            this.labStep4.Name = "labStep4";
            this.labStep4.Size = new System.Drawing.Size(336, 51);
            this.labStep4.TabIndex = 28;
            this.labStep4.Text = "Выключите питание модуля. Переключите модуль в состояниение \'Normal\'. Включите мо" +
    "дуль и нажмите \"Далее\".";
            this.labStep4.Visible = false;
            // 
            // butNext
            // 
            this.butNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butNext.Location = new System.Drawing.Point(229, 238);
            this.butNext.Name = "butNext";
            this.butNext.Size = new System.Drawing.Size(108, 34);
            this.butNext.TabIndex = 26;
            this.butNext.Text = "Далее";
            this.butNext.UseVisualStyleBackColor = true;
            this.butNext.Click += new System.EventHandler(this.butNext_Click);
            // 
            // tbCommand
            // 
            this.tbCommand.BackColor = System.Drawing.SystemColors.Window;
            this.tbCommand.Location = new System.Drawing.Point(68, 246);
            this.tbCommand.Name = "tbCommand";
            this.tbCommand.ReadOnly = true;
            this.tbCommand.Size = new System.Drawing.Size(146, 20);
            this.tbCommand.TabIndex = 2;
            this.tbCommand.Visible = false;
            // 
            // FormModuleInit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 279);
            this.Controls.Add(this.labStep4);
            this.Controls.Add(this.labStep2);
            this.Controls.Add(this.frModuleInit);
            this.Controls.Add(this.tbCommand);
            this.Controls.Add(this.butNext);
            this.Controls.Add(this.labStep1);
            this.Controls.Add(this.labStep3);
            this.Controls.Add(this.labStepNum);
            this.Controls.Add(this.labStepComplited);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormModuleInit";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Инициализация модуля";
            this.Validating += new System.ComponentModel.CancelEventHandler(this.tbAddress10_Validating);
            this.frModuleInit.ResumeLayout(false);
            this.frModuleInit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labStep2;
        private System.Windows.Forms.Label labStepComplited;
        private System.Windows.Forms.Label labStepNum;
        private System.Windows.Forms.Label labStep3;
        private System.Windows.Forms.Label labStep1;
        private System.Windows.Forms.GroupBox frModuleInit;
        private System.Windows.Forms.Label labChacksum;
        private System.Windows.Forms.Label labIntegrationTime;
        private System.Windows.Forms.ComboBox cbIntegrationTime;
        private System.Windows.Forms.Label labDataFormat;
        private System.Windows.Forms.ComboBox cbDataFormat;
        private System.Windows.Forms.ComboBox cbChecksum;
        private System.Windows.Forms.ComboBox cbBaudRate;
        private System.Windows.Forms.ComboBox cbInputRange;
        private System.Windows.Forms.Label labBaudRate;
        private System.Windows.Forms.Label labInputRange;
        private System.Windows.Forms.TextBox tbAddress;
        private System.Windows.Forms.TextBox tbAddress10;
        private System.Windows.Forms.Label labAddress;
        private System.Windows.Forms.Button butNext;
        private System.Windows.Forms.TextBox tbCommand;
        private System.Windows.Forms.Label labStep4;
    }
}