namespace ReporterCommon
{
    partial class ControlLinkProps
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.PartNumberPanel = new System.Windows.Forms.Panel();
            this.PartEndNumber = new System.Windows.Forms.TextBox();
            this.PartBeginNumber = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.PartByNumber = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.CellLinkType = new System.Windows.Forms.ComboBox();
            this.PartByTime = new System.Windows.Forms.CheckBox();
            this.LengthPanel = new System.Windows.Forms.Panel();
            this.LengthMinute = new System.Windows.Forms.TextBox();
            this.LengthHour = new System.Windows.Forms.TextBox();
            this.LengthDay = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.PartPanel = new System.Windows.Forms.Panel();
            this.PartEndMinute = new System.Windows.Forms.TextBox();
            this.PartBeginMinute = new System.Windows.Forms.TextBox();
            this.PartEndHour = new System.Windows.Forms.TextBox();
            this.PartBeginHour = new System.Windows.Forms.TextBox();
            this.PartEndDay = new System.Windows.Forms.TextBox();
            this.PartBeginDay = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.DistancePanel = new System.Windows.Forms.Panel();
            this.ValuesOrder = new System.Windows.Forms.ComboBox();
            this.ValueDistanceY = new System.Windows.Forms.TextBox();
            this.ValueDistanceX = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SkipFirstCell = new System.Windows.Forms.CheckBox();
            this.AllowEdit = new System.Windows.Forms.CheckBox();
            this.LinkTypePanel = new System.Windows.Forms.Panel();
            this.PartNumberPanel.SuspendLayout();
            this.LengthPanel.SuspendLayout();
            this.PartPanel.SuspendLayout();
            this.DistancePanel.SuspendLayout();
            this.LinkTypePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label2.Location = new System.Drawing.Point(2, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "Тип ссылки";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // PartNumberPanel
            // 
            this.PartNumberPanel.Controls.Add(this.PartEndNumber);
            this.PartNumberPanel.Controls.Add(this.PartBeginNumber);
            this.PartNumberPanel.Controls.Add(this.label21);
            this.PartNumberPanel.Controls.Add(this.label20);
            this.PartNumberPanel.Controls.Add(this.PartByNumber);
            this.PartNumberPanel.Location = new System.Drawing.Point(355, 0);
            this.PartNumberPanel.Name = "PartNumberPanel";
            this.PartNumberPanel.Size = new System.Drawing.Size(98, 65);
            this.PartNumberPanel.TabIndex = 39;
            // 
            // PartEndNumber
            // 
            this.PartEndNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartEndNumber.Enabled = false;
            this.PartEndNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartEndNumber.Location = new System.Drawing.Point(42, 40);
            this.PartEndNumber.Name = "PartEndNumber";
            this.PartEndNumber.ReadOnly = true;
            this.PartEndNumber.Size = new System.Drawing.Size(35, 22);
            this.PartEndNumber.TabIndex = 75;
            // 
            // PartBeginNumber
            // 
            this.PartBeginNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartBeginNumber.Enabled = false;
            this.PartBeginNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartBeginNumber.Location = new System.Drawing.Point(42, 19);
            this.PartBeginNumber.Name = "PartBeginNumber";
            this.PartBeginNumber.ReadOnly = true;
            this.PartBeginNumber.Size = new System.Drawing.Size(35, 22);
            this.PartBeginNumber.TabIndex = 72;
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label21.Location = new System.Drawing.Point(17, 21);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 16);
            this.label21.TabIndex = 32;
            this.label21.Text = "от";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label20.Location = new System.Drawing.Point(15, 42);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(24, 16);
            this.label20.TabIndex = 33;
            this.label20.Text = "до";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // PartByNumber
            // 
            this.PartByNumber.AutoSize = true;
            this.PartByNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartByNumber.Location = new System.Drawing.Point(1, 1);
            this.PartByNumber.Name = "PartByNumber";
            this.PartByNumber.Size = new System.Drawing.Size(97, 20);
            this.PartByNumber.TabIndex = 70;
            this.PartByNumber.Text = "По номеру";
            this.PartByNumber.UseVisualStyleBackColor = true;
            this.PartByNumber.CheckedChanged += new System.EventHandler(this.PartByNumber_CheckedChanged);
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label11.Location = new System.Drawing.Point(2, 2);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(171, 16);
            this.label11.TabIndex = 15;
            this.label11.Text = "Выделить часть периода";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CellLinkType
            // 
            this.CellLinkType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CellLinkType.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.CellLinkType.FormattingEnabled = true;
            this.CellLinkType.Items.AddRange(new object[] {
            "Итоговое значение",
            "Комбинированное значение",
            "Абсолютное значение",
            "Абсолютное с редактированием",
            "Равномерный список значений",
            "Список мгновенных значений",
            "Абсолютное комбинированное",
            "Список абсолютных значений",
            "Ручной ввод",
            "Сохранение в архив",
            "Системная ссылка"});
            this.CellLinkType.Location = new System.Drawing.Point(90, 4);
            this.CellLinkType.Name = "CellLinkType";
            this.CellLinkType.Size = new System.Drawing.Size(268, 24);
            this.CellLinkType.TabIndex = 5;
            this.CellLinkType.SelectedIndexChanged += new System.EventHandler(this.CellLinkType_SelectedIndexChanged);
            // 
            // PartByTime
            // 
            this.PartByTime.AutoSize = true;
            this.PartByTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartByTime.Location = new System.Drawing.Point(215, 1);
            this.PartByTime.Name = "PartByTime";
            this.PartByTime.Size = new System.Drawing.Size(105, 20);
            this.PartByTime.TabIndex = 50;
            this.PartByTime.Text = "По времени";
            this.PartByTime.UseVisualStyleBackColor = true;
            this.PartByTime.CheckedChanged += new System.EventHandler(this.PartByTime_CheckedChanged);
            // 
            // LengthPanel
            // 
            this.LengthPanel.Controls.Add(this.LengthMinute);
            this.LengthPanel.Controls.Add(this.LengthHour);
            this.LengthPanel.Controls.Add(this.LengthDay);
            this.LengthPanel.Controls.Add(this.label1);
            this.LengthPanel.Controls.Add(this.label8);
            this.LengthPanel.Controls.Add(this.label6);
            this.LengthPanel.Controls.Add(this.label7);
            this.LengthPanel.Location = new System.Drawing.Point(3, 95);
            this.LengthPanel.Name = "LengthPanel";
            this.LengthPanel.Size = new System.Drawing.Size(453, 30);
            this.LengthPanel.TabIndex = 23;
            // 
            // LengthMinute
            // 
            this.LengthMinute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LengthMinute.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LengthMinute.Location = new System.Drawing.Point(402, 4);
            this.LengthMinute.Name = "LengthMinute";
            this.LengthMinute.Size = new System.Drawing.Size(35, 22);
            this.LengthMinute.TabIndex = 45;
            // 
            // LengthHour
            // 
            this.LengthHour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LengthHour.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LengthHour.Location = new System.Drawing.Point(307, 4);
            this.LengthHour.Name = "LengthHour";
            this.LengthHour.Size = new System.Drawing.Size(35, 22);
            this.LengthHour.TabIndex = 40;
            // 
            // LengthDay
            // 
            this.LengthDay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LengthDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LengthDay.Location = new System.Drawing.Point(226, 4);
            this.LengthDay.Name = "LengthDay";
            this.LengthDay.Size = new System.Drawing.Size(35, 22);
            this.LengthDay.TabIndex = 35;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(180, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 26;
            this.label1.Text = "сутки";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label8.Location = new System.Drawing.Point(344, 6);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 16);
            this.label8.TabIndex = 9;
            this.label8.Text = "минуты";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(265, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 1;
            this.label6.Text = "часы";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(3, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(175, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "Длина одного интервала";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // PartPanel
            // 
            this.PartPanel.Controls.Add(this.PartEndMinute);
            this.PartPanel.Controls.Add(this.PartBeginMinute);
            this.PartPanel.Controls.Add(this.PartEndHour);
            this.PartPanel.Controls.Add(this.PartBeginHour);
            this.PartPanel.Controls.Add(this.PartEndDay);
            this.PartPanel.Controls.Add(this.PartBeginDay);
            this.PartPanel.Controls.Add(this.PartNumberPanel);
            this.PartPanel.Controls.Add(this.label16);
            this.PartPanel.Controls.Add(this.label17);
            this.PartPanel.Controls.Add(this.label14);
            this.PartPanel.Controls.Add(this.label15);
            this.PartPanel.Controls.Add(this.label13);
            this.PartPanel.Controls.Add(this.label12);
            this.PartPanel.Controls.Add(this.label11);
            this.PartPanel.Controls.Add(this.PartByTime);
            this.PartPanel.Location = new System.Drawing.Point(3, 124);
            this.PartPanel.Name = "PartPanel";
            this.PartPanel.Size = new System.Drawing.Size(453, 62);
            this.PartPanel.TabIndex = 24;
            // 
            // PartEndMinute
            // 
            this.PartEndMinute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartEndMinute.Enabled = false;
            this.PartEndMinute.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartEndMinute.Location = new System.Drawing.Point(320, 40);
            this.PartEndMinute.Name = "PartEndMinute";
            this.PartEndMinute.ReadOnly = true;
            this.PartEndMinute.Size = new System.Drawing.Size(35, 22);
            this.PartEndMinute.TabIndex = 65;
            // 
            // PartBeginMinute
            // 
            this.PartBeginMinute.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartBeginMinute.Enabled = false;
            this.PartBeginMinute.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartBeginMinute.Location = new System.Drawing.Point(320, 19);
            this.PartBeginMinute.Name = "PartBeginMinute";
            this.PartBeginMinute.ReadOnly = true;
            this.PartBeginMinute.Size = new System.Drawing.Size(35, 22);
            this.PartBeginMinute.TabIndex = 57;
            // 
            // PartEndHour
            // 
            this.PartEndHour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartEndHour.Enabled = false;
            this.PartEndHour.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartEndHour.Location = new System.Drawing.Point(225, 40);
            this.PartEndHour.Name = "PartEndHour";
            this.PartEndHour.ReadOnly = true;
            this.PartEndHour.Size = new System.Drawing.Size(35, 22);
            this.PartEndHour.TabIndex = 62;
            // 
            // PartBeginHour
            // 
            this.PartBeginHour.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartBeginHour.Enabled = false;
            this.PartBeginHour.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartBeginHour.Location = new System.Drawing.Point(225, 19);
            this.PartBeginHour.Name = "PartBeginHour";
            this.PartBeginHour.ReadOnly = true;
            this.PartBeginHour.Size = new System.Drawing.Size(35, 22);
            this.PartBeginHour.TabIndex = 55;
            // 
            // PartEndDay
            // 
            this.PartEndDay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartEndDay.Enabled = false;
            this.PartEndDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartEndDay.Location = new System.Drawing.Point(147, 40);
            this.PartEndDay.Name = "PartEndDay";
            this.PartEndDay.ReadOnly = true;
            this.PartEndDay.Size = new System.Drawing.Size(35, 22);
            this.PartEndDay.TabIndex = 60;
            // 
            // PartBeginDay
            // 
            this.PartBeginDay.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PartBeginDay.Enabled = false;
            this.PartBeginDay.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PartBeginDay.Location = new System.Drawing.Point(147, 19);
            this.PartBeginDay.Name = "PartBeginDay";
            this.PartBeginDay.ReadOnly = true;
            this.PartBeginDay.Size = new System.Drawing.Size(35, 22);
            this.PartBeginDay.TabIndex = 52;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label16.Location = new System.Drawing.Point(262, 42);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(57, 16);
            this.label16.TabIndex = 25;
            this.label16.Text = "минуты";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label17.Location = new System.Drawing.Point(262, 21);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(57, 16);
            this.label17.TabIndex = 24;
            this.label17.Text = "минуты";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label14.Location = new System.Drawing.Point(184, 42);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(41, 17);
            this.label14.TabIndex = 21;
            this.label14.Text = "часы";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label15.Location = new System.Drawing.Point(184, 18);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(41, 21);
            this.label15.TabIndex = 20;
            this.label15.Text = "часы";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label13.Location = new System.Drawing.Point(10, 42);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(135, 16);
            this.label13.TabIndex = 17;
            this.label13.Text = "Сдвиг конца:  сутки";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label12.Location = new System.Drawing.Point(1, 21);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(144, 16);
            this.label12.TabIndex = 16;
            this.label12.Text = "Сдвиг начала:  сутки";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DistancePanel
            // 
            this.DistancePanel.Controls.Add(this.ValuesOrder);
            this.DistancePanel.Controls.Add(this.ValueDistanceY);
            this.DistancePanel.Controls.Add(this.ValueDistanceX);
            this.DistancePanel.Controls.Add(this.label22);
            this.DistancePanel.Controls.Add(this.label5);
            this.DistancePanel.Controls.Add(this.label4);
            this.DistancePanel.Controls.Add(this.SkipFirstCell);
            this.DistancePanel.Location = new System.Drawing.Point(3, 33);
            this.DistancePanel.Name = "DistancePanel";
            this.DistancePanel.Size = new System.Drawing.Size(453, 63);
            this.DistancePanel.TabIndex = 21;
            // 
            // ValuesOrder
            // 
            this.ValuesOrder.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ValuesOrder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ValuesOrder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ValuesOrder.FormattingEnabled = true;
            this.ValuesOrder.Items.AddRange(new object[] {
            "по возрастанию времени",
            "по убыванию времени"});
            this.ValuesOrder.Location = new System.Drawing.Point(91, 31);
            this.ValuesOrder.Name = "ValuesOrder";
            this.ValuesOrder.Size = new System.Drawing.Size(194, 24);
            this.ValuesOrder.TabIndex = 25;
            // 
            // ValueDistanceY
            // 
            this.ValueDistanceY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ValueDistanceY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ValueDistanceY.Location = new System.Drawing.Point(330, 3);
            this.ValueDistanceY.Name = "ValueDistanceY";
            this.ValueDistanceY.Size = new System.Drawing.Size(35, 22);
            this.ValueDistanceY.TabIndex = 20;
            // 
            // ValueDistanceX
            // 
            this.ValueDistanceX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ValueDistanceX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ValueDistanceX.Location = new System.Drawing.Point(250, 3);
            this.ValueDistanceX.Name = "ValueDistanceX";
            this.ValueDistanceX.Size = new System.Drawing.Size(35, 22);
            this.ValueDistanceX.TabIndex = 15;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label22.Location = new System.Drawing.Point(2, 34);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(87, 16);
            this.label22.TabIndex = 9;
            this.label22.Text = "Сортировка";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(290, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 16);
            this.label5.TabIndex = 1;
            this.label5.Text = "по Y:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(1, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(246, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Расстояние между значениями по X:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SkipFirstCell
            // 
            this.SkipFirstCell.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SkipFirstCell.Location = new System.Drawing.Point(296, 24);
            this.SkipFirstCell.Name = "SkipFirstCell";
            this.SkipFirstCell.Size = new System.Drawing.Size(149, 36);
            this.SkipFirstCell.TabIndex = 30;
            this.SkipFirstCell.Text = "Не заполнять ячейку со ссылкой";
            this.SkipFirstCell.UseVisualStyleBackColor = true;
            // 
            // AllowEdit
            // 
            this.AllowEdit.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AllowEdit.Location = new System.Drawing.Point(362, -1);
            this.AllowEdit.Name = "AllowEdit";
            this.AllowEdit.Size = new System.Drawing.Size(94, 37);
            this.AllowEdit.TabIndex = 10;
            this.AllowEdit.Text = "Разрешить правку";
            this.AllowEdit.UseVisualStyleBackColor = true;
            // 
            // LinkTypePanel
            // 
            this.LinkTypePanel.Controls.Add(this.CellLinkType);
            this.LinkTypePanel.Controls.Add(this.label2);
            this.LinkTypePanel.Controls.Add(this.AllowEdit);
            this.LinkTypePanel.Location = new System.Drawing.Point(3, 0);
            this.LinkTypePanel.Name = "LinkTypePanel";
            this.LinkTypePanel.Size = new System.Drawing.Size(453, 34);
            this.LinkTypePanel.TabIndex = 26;
            // 
            // ControlLinkProps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LengthPanel);
            this.Controls.Add(this.LinkTypePanel);
            this.Controls.Add(this.PartPanel);
            this.Controls.Add(this.DistancePanel);
            this.Name = "ControlLinkProps";
            this.Size = new System.Drawing.Size(458, 189);
            this.Load += new System.EventHandler(this.ControlLinkProps_Load);
            this.PartNumberPanel.ResumeLayout(false);
            this.PartNumberPanel.PerformLayout();
            this.LengthPanel.ResumeLayout(false);
            this.LengthPanel.PerformLayout();
            this.PartPanel.ResumeLayout(false);
            this.PartPanel.PerformLayout();
            this.DistancePanel.ResumeLayout(false);
            this.DistancePanel.PerformLayout();
            this.LinkTypePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel PartNumberPanel;
        private System.Windows.Forms.TextBox PartEndNumber;
        private System.Windows.Forms.TextBox PartBeginNumber;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.CheckBox PartByNumber;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox PartByTime;
        private System.Windows.Forms.Panel LengthPanel;
        private System.Windows.Forms.TextBox LengthMinute;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox LengthHour;
        private System.Windows.Forms.TextBox LengthDay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel PartPanel;
        private System.Windows.Forms.TextBox PartEndMinute;
        private System.Windows.Forms.TextBox PartBeginMinute;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox PartEndHour;
        private System.Windows.Forms.TextBox PartBeginHour;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox PartEndDay;
        private System.Windows.Forms.TextBox PartBeginDay;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel DistancePanel;
        private System.Windows.Forms.ComboBox ValuesOrder;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox ValueDistanceY;
        private System.Windows.Forms.TextBox ValueDistanceX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel LinkTypePanel;
        public System.Windows.Forms.ComboBox CellLinkType;
        public System.Windows.Forms.CheckBox AllowEdit;
        private System.Windows.Forms.CheckBox SkipFirstCell;

    }
}
