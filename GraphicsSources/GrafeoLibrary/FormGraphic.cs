using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using BaseLibrary;

namespace GrafeoLibrary
{
    public partial class FormGraphic : Form
    {
        private DataGridViewGrafeo _dataGridViewGrafeo;

        public FormGraphic()
        {
            InitializeComponent();

            _dataGridViewGrafeo = new DataGridViewGrafeo(dataGridView);
        }

        private void InitControls()
        {
            try
            {
                //butViewPeriodToMin.Visible = false;   //убрано в конструктор
                //butViewPeriodToMax.Left = 42;         //убрано в конструктор
                
                //Наклейки на скрывающие кнопки
                //labRightPanel.Image = Properties.Resources.rightl.ToBitmap();   //убрано в конструктор
                //labBottomPanel.Image = Properties.Resources.downl.ToBitmap();   //убрано в конструктор

                //Для красивого отображение форм
                Application.EnableVisualStyles();

                //заполняем комбобоксы
                cbViewPeriodType.Items.AddRange(new[] { "Сут.", "Час.", "Мин.", "Сек." });
                cbDynPeriodType.Items.AddRange(new[] { "Час.", "Мин.", "Сек." });
                for (var i = 1; i < 11; i++) cbViewPeriod.Items.Add(i.ToString());
                cbViewPeriod.Items.AddRange(new[] { "12", "15", "20", "24", "30", "45", "60" });
                for (var i = 1; i < 11; i++) cbDynPeriod.Items.Add(i.ToString());
                for (var i = 1; i < 6; i++) cbLineWidth.Items.Add(i.ToString());
                cbViewPeriodStep.Items.AddRange(new[] { "3%", "5%", "10%", "25%", "50%" });

                //принтеры
                //a InitPrinters();

                //Легенда отключена
                chartMain.Legends.Last().Enabled = false;

                //Кнопки скрывания приделываем
                //Controls.Add(labBottomPanel);   //убрано в конструктор
                //labBottomPanel.BringToFront();  //убрано в конструктор
                labBottomPanel_Move(null, null);
                //Controls.Add(labRightPanel);    //убрано в конструктор
                //labRightPanel.BringToFront();   //убрано в конструктор
                labRightPanel_Move(null, null);

                //Приделываем полосы прокрутки
                //splitContainerV.Panel1.Controls.Add(_hscrollb);         //убрано в конструктор
                //splitContainerV.Panel1.Controls.Add(_buttonScaleDrop);  //убрано в конструктор
                //a splitContainerV.Panel1.Resize += panelChart_Resize;

                //a _hscrollb.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                //a _hscrollb.Location = new Point(_buttonScaleDrop.Width, splitContainerV.Panel1.Size.Height - 20);
                //a _hscrollb.Width = chartMain.Size.Width - _buttonScaleDrop.Width;
                //a //_hscrollb.Visible = false;
                //a _hscrollb.Value = 0;
                //a _hscrollb.Maximum = 1000;
                //a _hscrollb.Minimum = 0;
                //a _hscrollb.LargeChange = 1001;
                //a _hscrollb.BringToFront();
                //a _hscrollb.Scroll += HScrollerScroll;
                //a _hscrollb.MouseEnter += HScroller1;
                //a _hscrollb.MouseLeave += HScroller2;

                //a //_hscrollb.MouseDoubleClick += ScrollDoubleHit;

                //a _buttonScaleDrop.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
                //a _buttonScaleDrop.Location = new Point(0, splitContainerV.Panel1.Size.Height - 20);
                //a //_buttonScaleDrop.Visible = false;
                //a _buttonScaleDrop.FlatStyle = FlatStyle.Popup;
                //a _buttonScaleDrop.Height = 17;
                //a //_buttonScaleDrop.Text = "max";
                //a _buttonScaleDrop.TextAlign = ContentAlignment.MiddleCenter;
                //a _buttonScaleDrop.Image = Properties.Resources.X_Full;
                //a _buttonScaleDrop.ImageAlign = ContentAlignment.MiddleCenter;
                //a _buttonScaleDrop.Font = new Font("Microsoft Sans Serif", 6F, FontStyle.Bold, GraphicsUnit.Point, ((204)));
                //a _buttonScaleDrop.BringToFront();
                //a _buttonScaleDrop.Click += butViewPeriodToMax_Click;

                //a //MouseWheel += WheelZooming;

                //Поле с номером текущего графика
                labCur.ForeColor = Color.White;
                labCur.TextAlign = ContentAlignment.MiddleCenter;
                //labCur1.ForeColor = Color.White;
                //labCur1.TextAlign = ContentAlignment.MiddleCenter;
                //labTimeCur.ForeColor = Color.White;
                //labTimeCur.TextAlign = ContentAlignment.MiddleCenter;
                //labCtrlCur.ForeColor = Color.White;
                //labCtrlCur.TextAlign = ContentAlignment.MiddleCenter;
                labVizirL.ForeColor = Color.White;
                labVizirL.TextAlign = ContentAlignment.MiddleCenter;
                labVizirR.ForeColor = Color.White;
                labVizirR.TextAlign = ContentAlignment.MiddleCenter;

                //~ _chartTypeMode = GraphicTypeMode.Empty;

                butCurMinMaxApply.Enabled = false;
                tbCurMin.Enabled = false;
                tbCurMax.Enabled = false;
                //button8.Enabled = false;
                //button9.Enabled = false;
                rbDynStorageFrom.Enabled = false;
                rbDynShift.Enabled = false;

                //~ butViewPeriodDec.Enabled = false;
                //~ butViewPeriodInc.Enabled = false;
                //~ butViewPeriodToMin.Enabled = false;
                //~ butViewPeriodToMax.Enabled = false;

                panelChart.AutoScroll = true;

                contextMenuStrip.ShowImageMargin = false;
                contextMenuStrip.MaximumSize = new Size(40, 0);

                //Устанавливаем некоторые значения по умолчанию
                //~ cbViewPeriod.Text = "7";
                //~ cbViewPeriodType.Text = "Мин.";

                //~ _timerFactor1 = 1;
                //~ _timerFactor2 = 1000;

                //a PrintDocInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        //////private void label1_Click(object sender, EventArgs e)
        //////{
        //////    MessageBox.Show(Size + "\n" + splitContainerH.Size + 
        //////                           "\n" + splitContainerH.Panel1.Size + 
        //////                           "\n" + splitContainerH.Panel2.Size + 
        //////                           "\n" + splitContainerH.SplitterDistance);
        //////}

        //private void InitControlEvents()
        //{
        //    Shown += Form_BlockAppear; //Блокировка отображения формы, если не задан интервал времени

        //    //запрещаем ввод не чисел в текстовые поля
        //    cbViewPeriod.KeyPress += CheckInputPositive;
        //    cbDynPeriod.KeyPress += CheckInputPositive;
        //    tbCurMin.KeyPress += CheckInputReal;
        //    tbCurMax.KeyPress += CheckInputReal;
        //    cbLineWidth.KeyPress += CheckInputInt;
        //    cbViewPeriodStep.KeyPress += CheckInputPositive;
        //    //~ cbViewPeriodStep.KeyPress += ComboBox1KeyPress; //Сделано в конструктаре
        //    //~ cbViewPeriodStep.Leave += ComboBox1Leave;       //Сделано в конструктаре

        //    //Режим отображения "Сдвиг"
        //    //~ rbDynShift.Click += SetScalePosition1;

        //    //~ butViewPeriodDec.Click += WheelZooming;
        //    //~ butViewPeriodInc.Click += WheelZooming;
        //    //~ butViewPeriodDec.MouseDown += ButtonHold;
        //    //~ butViewPeriodDec.MouseUp += ButtonRelease;
        //    //~ butViewPeriodInc.MouseDown += ButtonHold;
        //    //~ butViewPeriodInc.MouseUp += ButtonRelease;

        //    butVizirNextCur.Click += VizirNextCur;
        //    butVizirPrevCur.Click += VizirPrevCur;
        //    butVizirPrevAll.Click += VizirPrevAll;
        //    butVizirNextAll.Click += VizirNextAll;
        //    //~ butVizirPrevAll.MouseDown += ButtonHold;
        //    //~ butVizirPrevAll.MouseUp += ButtonRelease;
        //    //~ butVizirNextAll.MouseDown += ButtonHold;
        //    //~ butVizirNextAll.MouseUp += ButtonRelease;
        //    //~ butVizirNextCur.MouseDown += ButtonHold;
        //    //~ butVizirNextCur.MouseUp += ButtonRelease;
        //    //~ butVizirPrevCur.MouseDown += ButtonHold;
        //    //~ butVizirPrevCur.MouseUp += ButtonRelease;

        //    butCurMinMaxApply.MouseHover += ButtonsHover;
        //    butViewPeriodApply.MouseHover += ButtonsHover;
        //    butViewPeriodDec.MouseHover += ButtonsHover;
        //    butViewPeriodInc.MouseHover += ButtonsHover;
        //    butViewPeriodToMin.MouseHover += ButtonsHover;
        //    butViewPeriodToMax.MouseHover += ButtonsHover;
        //    butLineWidthCur.MouseHover += ButtonsHover;
        //    butLineWidthAll.MouseHover += ButtonsHover;
        //    butVizirNextCur.MouseHover += ButtonsHover;
        //    butVizirPrevCur.MouseHover += ButtonsHover;
        //    butVizirPrevAll.MouseHover += ButtonsHover;
        //    butVizirNextAll.MouseHover += ButtonsHover;

        //    butUndo.MouseHover += ButtonsHover;
        //    butRedo.MouseHover += ButtonsHover;

        //    //"Обнуляет" кнопки путешествия по времени
        //    butVizirNextCur.LostFocus += VizirLabelsColorReset;
        //    butVizirPrevCur.LostFocus += VizirLabelsColorReset;
        //    butVizirNextAll.LostFocus += VizirLabelsColorReset;
        //    butVizirPrevAll.LostFocus += VizirLabelsColorReset;
        //    chartMain.MouseDown += VizirLabelsColorReset;

        //    //Метод разлепляет графики, если мы перетащили один из скрепленных на область построения,
        //    //а также в случае выделения правой кнопкой мыши осуществляет масштабирование
        //    //~ chartMain.MouseUp += Chart1MouseUp; //Перенесено в конструктор

        //    //при отображении визира появляется данные с его местоположением
        //    //chartMain.CursorPositionChanged += CursorPositionView;
        //    //chartMain.CursorPositionChanged += CursorPositionGridView;
        //    chartMain.CursorPositionChanged += CursorPositionChanged; 
        //    //Синхронизация курсоров аналоговых и дискретных
        //    chartMain.CursorPositionChanging += CursorSynch;
        //    chartMain.CursorPositionChanged += CursorSynchFinal;

        //    Closing += GarbageCollection; //Чистка мусора при закрытии формы
        //}

        //private void SetDynPeriod(double dynPeriod, bool isDynShift = true)
        //{
        //    if (dynPeriod > 0)
        //    {
        //        DynPeriod = dynPeriod;
        //        IsDynShift = isDynShift;

        //        if (DynPeriod < 60)
        //        {
        //            //~ _timerFactor1 = m;
        //            //~ _timerFactor2 = 1000;
        //            cbDynPeriod.Text = DynPeriod.ToString();
        //            cbDynPeriodType.Text = "Сек.";
        //        }
        //        else
        //        {
        //            //~ _timerFactor1 = m/60;
        //            //~ _timerFactor2 = 1000*60;
        //            cbDynPeriod.Text = (DynPeriod/60).ToString();
        //            cbDynPeriodType.Text = "Мин.";
        //        }

        //        //timer1.Enabled = true;
        //    }
        //    else
        //    {
        //        cbDynPeriodType.Visible = false;
        //        cbDynPeriod.Visible = false;
        //        labDynPeriod.Visible = false;
        //        butDynShiftOnOff.Visible = false;
        //        butDynClear.Visible = false;
        //        rbDynStorageFrom.Visible = false;
        //        rbDynShift.Visible = false;
        //        dateTimePicker.Visible = false;
        //        butDynStorageFromApply.Visible = false;
        //        labViewPeriod.Location = new Point(labViewPeriod.Location.X, labViewPeriod.Location.Y - 89);
        //        cbViewPeriodType.Location = new Point(cbViewPeriodType.Location.X, cbViewPeriodType.Location.Y - 89);
        //        cbViewPeriod.Location = new Point(cbViewPeriod.Location.X, cbViewPeriod.Location.Y - 89);
        //        rbDynShift.Location = new Point(rbDynShift.Location.X, rbDynShift.Location.Y - 89);
        //        butViewPeriodApply.Top -= 89;
        //        butViewPeriodDec.Top = butViewPeriodApply.Top + 25;
        //        butViewPeriodInc.Top = butViewPeriodApply.Top + 25;
        //        butViewPeriodToMin.Top = butViewPeriodDec.Top + 25;
        //        butViewPeriodToMax.Top = butViewPeriodDec.Top + 25;
        //        cbViewPeriodStep.Top = butViewPeriodApply.Top + 25;
        //        rbDynStorageFrom.Text = "Полное";
        //        rbDynShift.Text = "Частичное";
        //        tabControl.Size = new Size(tabControl.Size.Width, Math.Max(
        //            gbVizir.Bottom,
        //            gbCtrl.Bottom) + 29);
        //        gbPeriod.Height = butViewPeriodToMin.Location.Y + 30;
        //        gbVizir.Location = new Point(2, gbPeriod.Height + 2);
        //        //labTimeCurTxt.Location = new Point(labTimeCurTxt.Location.X,
        //        //                                   gbVizir.Location.Y + gbVizir.Height + 5);
        //        //labTimeCur.Location = new Point(labTimeCur.Location.X, labTimeCurTxt.Location.Y - 3);

        //        _fillScaleViewPerCentage = 1;

        //        if (dataGridView.Columns.Contains("Последнее")) dataGridView.Columns.Remove("Последнее");
        //    }
        //}

    #region Controls
        private void labBottomPanel_Click(object sender, EventArgs e)
        {
            splitContainerV.Panel2Collapsed = !splitContainerV.Panel2Collapsed;
            labBottomPanel_Move(sender, e);
            labBottomPanel.Image = (splitContainerV.Panel2Collapsed ? Properties.Resources.upl : Properties.Resources.downl).ToBitmap();
        }

        private void labBottomPanel_Move(object sender, EventArgs e)
        {
            //labBottomPanel.Location = new Point(labBottomPanel.Location.X, splitContainerV.Panel1.Height + 5);
            labBottomPanel.Top = splitContainerV.Panel1.Height + 5;
        }

        private void labPanel_MouseEnter(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = Color.White;
        }

        private void labPanel_MouseLeave(object sender, EventArgs e)
        {
            ((Label)sender).BackColor = SystemColors.GradientInactiveCaption; //Color.LightSeaGreen;
        }

        private void labRightPanel_Click(object sender, EventArgs e)
        {
            splitContainerH.Panel2Collapsed = !splitContainerH.Panel2Collapsed;
            labRightPanel_Move(sender, e);
            labRightPanel.Image = (splitContainerH.Panel2Collapsed ? Properties.Resources.leftl : Properties.Resources.rightl).ToBitmap();
        }
        
        private void labRightPanel_Move(object sender, EventArgs e)
        {
            //labRightPanel.Location = new Point(splitContainerH.Panel1.Width + 5, labRightPanel.Location.Y);
            labRightPanel.Left = splitContainerH.Panel1.Width + 5;
        }

        private void splitContainerV_SplitterMoved(object sender, SplitterEventArgs e)
        {
            labBottomPanel_Move(sender, e);
        }
    #endregion Controls
    }
}