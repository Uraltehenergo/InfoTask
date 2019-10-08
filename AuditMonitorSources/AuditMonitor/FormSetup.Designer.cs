namespace AuditMonitor
{
    partial class FormSetup
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
            this.butSendCommand = new System.Windows.Forms.Button();
            this.butProgramModules = new System.Windows.Forms.Button();
            this.butModulesConfig = new System.Windows.Forms.Button();
            this.gbGeneral = new System.Windows.Forms.GroupBox();
            this.tbModuleTimeOut = new System.Windows.Forms.TextBox();
            this.labModuleTimeOut = new System.Windows.Forms.Label();
            this.tbReadPeriod = new System.Windows.Forms.TextBox();
            this.labReadPeriod = new System.Windows.Forms.Label();
            this.gbArchive = new System.Windows.Forms.GroupBox();
            this.rbWriteArchive_ByRead = new System.Windows.Forms.RadioButton();
            this.rbWriteArchive_ByAperture = new System.Windows.Forms.RadioButton();
            this.gbFont = new System.Windows.Forms.GroupBox();
            this.labFont = new System.Windows.Forms.Label();
            this.butDefault = new System.Windows.Forms.Button();
            this.chbItalic = new System.Windows.Forms.CheckBox();
            this.chbBold = new System.Windows.Forms.CheckBox();
            this.cbFontSize = new System.Windows.Forms.ComboBox();
            this.labFontSize = new System.Windows.Forms.Label();
            this.cbFontName = new System.Windows.Forms.ComboBox();
            this.pButtons = new System.Windows.Forms.Panel();
            this.butInitVirtualNet = new System.Windows.Forms.Button();
            this.gbNetConfiguration = new System.Windows.Forms.GroupBox();
            this.gbSaveNetOnClose = new System.Windows.Forms.GroupBox();
            this.rbSaveNetOnClose_No = new System.Windows.Forms.RadioButton();
            this.rbSaveNetOnClose_OnRequest = new System.Windows.Forms.RadioButton();
            this.rbSaveNetOnClose_Yes = new System.Windows.Forms.RadioButton();
            this.butSaveNet = new System.Windows.Forms.Button();
            this.gbGeneral.SuspendLayout();
            this.gbArchive.SuspendLayout();
            this.gbFont.SuspendLayout();
            this.pButtons.SuspendLayout();
            this.gbNetConfiguration.SuspendLayout();
            this.gbSaveNetOnClose.SuspendLayout();
            this.SuspendLayout();
            // 
            // butSendCommand
            // 
            this.butSendCommand.Location = new System.Drawing.Point(154, 44);
            this.butSendCommand.Name = "butSendCommand";
            this.butSendCommand.Size = new System.Drawing.Size(145, 35);
            this.butSendCommand.TabIndex = 1;
            this.butSendCommand.Text = "Выполнить команду";
            this.butSendCommand.UseVisualStyleBackColor = true;
            this.butSendCommand.Click += new System.EventHandler(this.butSendCommand_Click);
            // 
            // butProgramModules
            // 
            this.butProgramModules.Location = new System.Drawing.Point(154, 3);
            this.butProgramModules.Name = "butProgramModules";
            this.butProgramModules.Size = new System.Drawing.Size(145, 35);
            this.butProgramModules.TabIndex = 2;
            this.butProgramModules.Text = "Программирование модулей";
            this.butProgramModules.UseVisualStyleBackColor = true;
            this.butProgramModules.Click += new System.EventHandler(this.butProgramModules_Click);
            // 
            // butModulesConfig
            // 
            this.butModulesConfig.Location = new System.Drawing.Point(3, 3);
            this.butModulesConfig.Name = "butModulesConfig";
            this.butModulesConfig.Size = new System.Drawing.Size(145, 35);
            this.butModulesConfig.TabIndex = 3;
            this.butModulesConfig.Text = "Первичная конфигурация модулей";
            this.butModulesConfig.UseVisualStyleBackColor = true;
            this.butModulesConfig.Click += new System.EventHandler(this.butModulesConfig_Click);
            // 
            // gbGeneral
            // 
            this.gbGeneral.Controls.Add(this.tbModuleTimeOut);
            this.gbGeneral.Controls.Add(this.labModuleTimeOut);
            this.gbGeneral.Controls.Add(this.tbReadPeriod);
            this.gbGeneral.Controls.Add(this.labReadPeriod);
            this.gbGeneral.Location = new System.Drawing.Point(12, 12);
            this.gbGeneral.Name = "gbGeneral";
            this.gbGeneral.Size = new System.Drawing.Size(453, 49);
            this.gbGeneral.TabIndex = 4;
            this.gbGeneral.TabStop = false;
            this.gbGeneral.Text = "Опрос";
            // 
            // tbModuleTimeOut
            // 
            this.tbModuleTimeOut.Location = new System.Drawing.Point(371, 19);
            this.tbModuleTimeOut.Name = "tbModuleTimeOut";
            this.tbModuleTimeOut.Size = new System.Drawing.Size(70, 20);
            this.tbModuleTimeOut.TabIndex = 7;
            this.tbModuleTimeOut.Validating += new System.ComponentModel.CancelEventHandler(this.tbModuleTimeOut_Validating);
            // 
            // labModuleTimeOut
            // 
            this.labModuleTimeOut.Location = new System.Drawing.Point(260, 14);
            this.labModuleTimeOut.Name = "labModuleTimeOut";
            this.labModuleTimeOut.Size = new System.Drawing.Size(105, 32);
            this.labModuleTimeOut.TabIndex = 6;
            this.labModuleTimeOut.Text = "Время ожидания ответа модуля (мс)";
            // 
            // tbReadPeriod
            // 
            this.tbReadPeriod.Location = new System.Drawing.Point(126, 19);
            this.tbReadPeriod.Name = "tbReadPeriod";
            this.tbReadPeriod.Size = new System.Drawing.Size(70, 20);
            this.tbReadPeriod.TabIndex = 4;
            this.tbReadPeriod.Validating += new System.ComponentModel.CancelEventHandler(this.tbReadPeriod_Validating);
            // 
            // labReadPeriod
            // 
            this.labReadPeriod.AutoSize = true;
            this.labReadPeriod.Location = new System.Drawing.Point(6, 22);
            this.labReadPeriod.Name = "labReadPeriod";
            this.labReadPeriod.Size = new System.Drawing.Size(114, 13);
            this.labReadPeriod.TabIndex = 5;
            this.labReadPeriod.Text = "Период опроса (сек) ";
            // 
            // gbArchive
            // 
            this.gbArchive.Controls.Add(this.rbWriteArchive_ByRead);
            this.gbArchive.Controls.Add(this.rbWriteArchive_ByAperture);
            this.gbArchive.Location = new System.Drawing.Point(12, 67);
            this.gbArchive.Name = "gbArchive";
            this.gbArchive.Size = new System.Drawing.Size(453, 66);
            this.gbArchive.TabIndex = 5;
            this.gbArchive.TabStop = false;
            this.gbArchive.Text = "Запись в архив";
            // 
            // rbWriteArchive_ByRead
            // 
            this.rbWriteArchive_ByRead.AutoSize = true;
            this.rbWriteArchive_ByRead.Location = new System.Drawing.Point(6, 42);
            this.rbWriteArchive_ByRead.Name = "rbWriteArchive_ByRead";
            this.rbWriteArchive_ByRead.Size = new System.Drawing.Size(127, 17);
            this.rbWriteArchive_ByRead.TabIndex = 2;
            this.rbWriteArchive_ByRead.Text = "При каждом опросе";
            this.rbWriteArchive_ByRead.UseVisualStyleBackColor = true;
            this.rbWriteArchive_ByRead.Click += new System.EventHandler(this.ArchiveWriteMode_Changed);
            // 
            // rbWriteArchive_ByAperture
            // 
            this.rbWriteArchive_ByAperture.AutoSize = true;
            this.rbWriteArchive_ByAperture.Checked = true;
            this.rbWriteArchive_ByAperture.Location = new System.Drawing.Point(6, 19);
            this.rbWriteArchive_ByAperture.Name = "rbWriteArchive_ByAperture";
            this.rbWriteArchive_ByAperture.Size = new System.Drawing.Size(121, 17);
            this.rbWriteArchive_ByAperture.TabIndex = 1;
            this.rbWriteArchive_ByAperture.TabStop = true;
            this.rbWriteArchive_ByAperture.Text = "С учётом апертуры";
            this.rbWriteArchive_ByAperture.UseVisualStyleBackColor = true;
            this.rbWriteArchive_ByAperture.Click += new System.EventHandler(this.ArchiveWriteMode_Changed);
            // 
            // gbFont
            // 
            this.gbFont.Controls.Add(this.labFont);
            this.gbFont.Controls.Add(this.butDefault);
            this.gbFont.Controls.Add(this.chbItalic);
            this.gbFont.Controls.Add(this.chbBold);
            this.gbFont.Controls.Add(this.cbFontSize);
            this.gbFont.Controls.Add(this.labFontSize);
            this.gbFont.Controls.Add(this.cbFontName);
            this.gbFont.Location = new System.Drawing.Point(12, 262);
            this.gbFont.Name = "gbFont";
            this.gbFont.Size = new System.Drawing.Size(453, 78);
            this.gbFont.TabIndex = 6;
            this.gbFont.TabStop = false;
            this.gbFont.Text = "Список сигналов";
            // 
            // labFont
            // 
            this.labFont.AutoSize = true;
            this.labFont.Location = new System.Drawing.Point(6, 22);
            this.labFont.Name = "labFont";
            this.labFont.Size = new System.Drawing.Size(41, 13);
            this.labFont.TabIndex = 7;
            this.labFont.Text = "Шрифт";
            // 
            // butDefault
            // 
            this.butDefault.Location = new System.Drawing.Point(178, 46);
            this.butDefault.Name = "butDefault";
            this.butDefault.Size = new System.Drawing.Size(92, 23);
            this.butDefault.TabIndex = 6;
            this.butDefault.Text = "По умолчанию";
            this.butDefault.UseVisualStyleBackColor = true;
            this.butDefault.Click += new System.EventHandler(this.butDefault_Click);
            // 
            // chbItalic
            // 
            this.chbItalic.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbItalic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chbItalic.Location = new System.Drawing.Point(418, 17);
            this.chbItalic.Name = "chbItalic";
            this.chbItalic.Size = new System.Drawing.Size(29, 24);
            this.chbItalic.TabIndex = 5;
            this.chbItalic.Text = "К";
            this.chbItalic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chbItalic.UseVisualStyleBackColor = true;
            this.chbItalic.Click += new System.EventHandler(this.chbItalic_Click);
            // 
            // chbBold
            // 
            this.chbBold.Appearance = System.Windows.Forms.Appearance.Button;
            this.chbBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.chbBold.Location = new System.Drawing.Point(383, 17);
            this.chbBold.Name = "chbBold";
            this.chbBold.Size = new System.Drawing.Size(29, 24);
            this.chbBold.TabIndex = 4;
            this.chbBold.Text = "Ж";
            this.chbBold.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chbBold.UseVisualStyleBackColor = true;
            this.chbBold.Click += new System.EventHandler(this.chbBold_Click);
            // 
            // cbFontSize
            // 
            this.cbFontSize.FormattingEnabled = true;
            this.cbFontSize.Location = new System.Drawing.Point(320, 19);
            this.cbFontSize.Name = "cbFontSize";
            this.cbFontSize.Size = new System.Drawing.Size(57, 21);
            this.cbFontSize.TabIndex = 3;
            this.cbFontSize.SelectedIndexChanged += new System.EventHandler(this.cbFontSize_SelectedIndexChanged);
            this.cbFontSize.Leave += new System.EventHandler(this.cbFontSize_Leave);
            // 
            // labFontSize
            // 
            this.labFontSize.AutoSize = true;
            this.labFontSize.Location = new System.Drawing.Point(268, 22);
            this.labFontSize.Name = "labFontSize";
            this.labFontSize.Size = new System.Drawing.Size(46, 13);
            this.labFontSize.TabIndex = 2;
            this.labFontSize.Text = "Размер";
            // 
            // cbFontName
            // 
            this.cbFontName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFontName.FormattingEnabled = true;
            this.cbFontName.Location = new System.Drawing.Point(53, 19);
            this.cbFontName.Name = "cbFontName";
            this.cbFontName.Size = new System.Drawing.Size(204, 21);
            this.cbFontName.TabIndex = 0;
            this.cbFontName.SelectionChangeCommitted += new System.EventHandler(this.cbFontName_SelectionChangeCommitted);
            // 
            // pButtons
            // 
            this.pButtons.Controls.Add(this.butInitVirtualNet);
            this.pButtons.Controls.Add(this.butSendCommand);
            this.pButtons.Controls.Add(this.butProgramModules);
            this.pButtons.Controls.Add(this.butModulesConfig);
            this.pButtons.Location = new System.Drawing.Point(12, 346);
            this.pButtons.Name = "pButtons";
            this.pButtons.Size = new System.Drawing.Size(453, 82);
            this.pButtons.TabIndex = 7;
            // 
            // butInitVirtualNet
            // 
            this.butInitVirtualNet.Location = new System.Drawing.Point(305, 3);
            this.butInitVirtualNet.Name = "butInitVirtualNet";
            this.butInitVirtualNet.Size = new System.Drawing.Size(145, 35);
            this.butInitVirtualNet.TabIndex = 4;
            this.butInitVirtualNet.Text = "Настройка виртуальной сети";
            this.butInitVirtualNet.UseVisualStyleBackColor = true;
            this.butInitVirtualNet.Click += new System.EventHandler(this.butInitVirtualNet_Click);
            // 
            // gbNetConfiguration
            // 
            this.gbNetConfiguration.Controls.Add(this.gbSaveNetOnClose);
            this.gbNetConfiguration.Controls.Add(this.butSaveNet);
            this.gbNetConfiguration.Location = new System.Drawing.Point(12, 139);
            this.gbNetConfiguration.Name = "gbNetConfiguration";
            this.gbNetConfiguration.Size = new System.Drawing.Size(453, 117);
            this.gbNetConfiguration.TabIndex = 8;
            this.gbNetConfiguration.TabStop = false;
            this.gbNetConfiguration.Text = "Конфигурация сети";
            // 
            // gbSaveNetOnClose
            // 
            this.gbSaveNetOnClose.Controls.Add(this.rbSaveNetOnClose_No);
            this.gbSaveNetOnClose.Controls.Add(this.rbSaveNetOnClose_OnRequest);
            this.gbSaveNetOnClose.Controls.Add(this.rbSaveNetOnClose_Yes);
            this.gbSaveNetOnClose.Location = new System.Drawing.Point(6, 19);
            this.gbSaveNetOnClose.Name = "gbSaveNetOnClose";
            this.gbSaveNetOnClose.Size = new System.Drawing.Size(316, 89);
            this.gbSaveNetOnClose.TabIndex = 5;
            this.gbSaveNetOnClose.TabStop = false;
            this.gbSaveNetOnClose.Text = "Сохранять конфигурацию сети при закрытии приложения";
            // 
            // rbSaveNetOnClose_No
            // 
            this.rbSaveNetOnClose_No.AutoSize = true;
            this.rbSaveNetOnClose_No.Location = new System.Drawing.Point(6, 65);
            this.rbSaveNetOnClose_No.Name = "rbSaveNetOnClose_No";
            this.rbSaveNetOnClose_No.Size = new System.Drawing.Size(42, 17);
            this.rbSaveNetOnClose_No.TabIndex = 2;
            this.rbSaveNetOnClose_No.Text = "нет";
            this.rbSaveNetOnClose_No.UseVisualStyleBackColor = true;
            this.rbSaveNetOnClose_No.Click += new System.EventHandler(this.SaveNetOnClose_Changed);
            // 
            // rbSaveNetOnClose_OnRequest
            // 
            this.rbSaveNetOnClose_OnRequest.AutoSize = true;
            this.rbSaveNetOnClose_OnRequest.Location = new System.Drawing.Point(6, 42);
            this.rbSaveNetOnClose_OnRequest.Name = "rbSaveNetOnClose_OnRequest";
            this.rbSaveNetOnClose_OnRequest.Size = new System.Drawing.Size(81, 17);
            this.rbSaveNetOnClose_OnRequest.TabIndex = 1;
            this.rbSaveNetOnClose_OnRequest.Text = "по запросу";
            this.rbSaveNetOnClose_OnRequest.UseVisualStyleBackColor = true;
            this.rbSaveNetOnClose_OnRequest.Click += new System.EventHandler(this.SaveNetOnClose_Changed);
            // 
            // rbSaveNetOnClose_Yes
            // 
            this.rbSaveNetOnClose_Yes.AutoSize = true;
            this.rbSaveNetOnClose_Yes.Checked = true;
            this.rbSaveNetOnClose_Yes.Location = new System.Drawing.Point(6, 19);
            this.rbSaveNetOnClose_Yes.Name = "rbSaveNetOnClose_Yes";
            this.rbSaveNetOnClose_Yes.Size = new System.Drawing.Size(37, 17);
            this.rbSaveNetOnClose_Yes.TabIndex = 0;
            this.rbSaveNetOnClose_Yes.TabStop = true;
            this.rbSaveNetOnClose_Yes.Text = "да";
            this.rbSaveNetOnClose_Yes.UseVisualStyleBackColor = true;
            this.rbSaveNetOnClose_Yes.Click += new System.EventHandler(this.SaveNetOnClose_Changed);
            // 
            // butSaveNet
            // 
            this.butSaveNet.Location = new System.Drawing.Point(328, 51);
            this.butSaveNet.Name = "butSaveNet";
            this.butSaveNet.Size = new System.Drawing.Size(119, 36);
            this.butSaveNet.TabIndex = 1;
            this.butSaveNet.Text = "Сохранить конфигурацию";
            this.butSaveNet.UseVisualStyleBackColor = true;
            this.butSaveNet.Click += new System.EventHandler(this.butSaveNet_Click);
            // 
            // FormSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 438);
            this.Controls.Add(this.gbNetConfiguration);
            this.Controls.Add(this.pButtons);
            this.Controls.Add(this.gbFont);
            this.Controls.Add(this.gbArchive);
            this.Controls.Add(this.gbGeneral);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSetup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSetup_FormClosing);
            this.gbGeneral.ResumeLayout(false);
            this.gbGeneral.PerformLayout();
            this.gbArchive.ResumeLayout(false);
            this.gbArchive.PerformLayout();
            this.gbFont.ResumeLayout(false);
            this.gbFont.PerformLayout();
            this.pButtons.ResumeLayout(false);
            this.gbNetConfiguration.ResumeLayout(false);
            this.gbSaveNetOnClose.ResumeLayout(false);
            this.gbSaveNetOnClose.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button butSendCommand;
        private System.Windows.Forms.Button butProgramModules;
        private System.Windows.Forms.Button butModulesConfig;
        private System.Windows.Forms.GroupBox gbGeneral;
        private System.Windows.Forms.TextBox tbModuleTimeOut;
        private System.Windows.Forms.Label labModuleTimeOut;
        private System.Windows.Forms.TextBox tbReadPeriod;
        private System.Windows.Forms.Label labReadPeriod;
        private System.Windows.Forms.GroupBox gbArchive;
        private System.Windows.Forms.GroupBox gbFont;
        private System.Windows.Forms.ComboBox cbFontSize;
        private System.Windows.Forms.Label labFontSize;
        private System.Windows.Forms.ComboBox cbFontName;
        private System.Windows.Forms.CheckBox chbBold;
        private System.Windows.Forms.CheckBox chbItalic;
        private System.Windows.Forms.Button butDefault;
        private System.Windows.Forms.Panel pButtons;
        private System.Windows.Forms.Button butInitVirtualNet;
        private System.Windows.Forms.Label labFont;
        private System.Windows.Forms.GroupBox gbNetConfiguration;
        private System.Windows.Forms.Button butSaveNet;
        private System.Windows.Forms.GroupBox gbSaveNetOnClose;
        private System.Windows.Forms.RadioButton rbSaveNetOnClose_No;
        private System.Windows.Forms.RadioButton rbSaveNetOnClose_OnRequest;
        private System.Windows.Forms.RadioButton rbSaveNetOnClose_Yes;
        private System.Windows.Forms.RadioButton rbWriteArchive_ByRead;
        private System.Windows.Forms.RadioButton rbWriteArchive_ByAperture;
    }
}