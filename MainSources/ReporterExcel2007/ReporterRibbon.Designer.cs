namespace ReporterExcel2007
{
    partial class ReporterRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public ReporterRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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
            this.InfoTask = this.Factory.CreateRibbonTab();
            this.groupForming = this.Factory.CreateRibbonGroup();
            this.groupArchive = this.Factory.CreateRibbonGroup();
            this.groupSetup = this.Factory.CreateRibbonGroup();
            this.groupEdit = this.Factory.CreateRibbonGroup();
            this.groupInfo = this.Factory.CreateRibbonGroup();
            this.butFormReport = this.Factory.CreateRibbonButton();
            this.butFormLiveReport = this.Factory.CreateRibbonButton();
            this.ButVideo = this.Factory.CreateRibbonButton();
            this.butSaveReport = this.Factory.CreateRibbonButton();
            this.butLoadReport = this.Factory.CreateRibbonButton();
            this.butAbsoluteEdit = this.Factory.CreateRibbonButton();
            this.buttonSetup = this.Factory.CreateRibbonButton();
            this.buttonNewReport = this.Factory.CreateRibbonButton();
            this.ButCopyServerReport = this.Factory.CreateRibbonButton();
            this.butAddLink = this.Factory.CreateRibbonButton();
            this.ButShapeLibray = this.Factory.CreateRibbonButton();
            this.butUpdate = this.Factory.CreateRibbonButton();
            this.ButClearCells = this.Factory.CreateRibbonButton();
            this.butDeleteLinks = this.Factory.CreateRibbonButton();
            this.Help = this.Factory.CreateRibbonButton();
            this.InfoTask.SuspendLayout();
            this.groupForming.SuspendLayout();
            this.groupArchive.SuspendLayout();
            this.groupSetup.SuspendLayout();
            this.groupEdit.SuspendLayout();
            this.groupInfo.SuspendLayout();
            // 
            // InfoTask
            // 
            this.InfoTask.Groups.Add(this.groupForming);
            this.InfoTask.Groups.Add(this.groupArchive);
            this.InfoTask.Groups.Add(this.groupSetup);
            this.InfoTask.Groups.Add(this.groupEdit);
            this.InfoTask.Groups.Add(this.groupInfo);
            this.InfoTask.Label = "Отчеты InfoTask";
            this.InfoTask.Name = "InfoTask";
            // 
            // groupForming
            // 
            this.groupForming.Items.Add(this.butFormReport);
            this.groupForming.Items.Add(this.butFormLiveReport);
            this.groupForming.Items.Add(this.ButVideo);
            this.groupForming.Label = "Формирование";
            this.groupForming.Name = "groupForming";
            // 
            // groupArchive
            // 
            this.groupArchive.Items.Add(this.butSaveReport);
            this.groupArchive.Items.Add(this.butLoadReport);
            this.groupArchive.Items.Add(this.butAbsoluteEdit);
            this.groupArchive.Label = "Архивы";
            this.groupArchive.Name = "groupArchive";
            // 
            // groupSetup
            // 
            this.groupSetup.Items.Add(this.buttonSetup);
            this.groupSetup.Items.Add(this.buttonNewReport);
            this.groupSetup.Items.Add(this.ButCopyServerReport);
            this.groupSetup.Label = "Создание и настройка";
            this.groupSetup.Name = "groupSetup";
            // 
            // groupEdit
            // 
            this.groupEdit.Items.Add(this.butAddLink);
            this.groupEdit.Items.Add(this.ButShapeLibray);
            this.groupEdit.Items.Add(this.butUpdate);
            this.groupEdit.Items.Add(this.ButClearCells);
            this.groupEdit.Items.Add(this.butDeleteLinks);
            this.groupEdit.Label = "Редактирование бланка отчета";
            this.groupEdit.Name = "groupEdit";
            // 
            // groupInfo
            // 
            this.groupInfo.Items.Add(this.Help);
            this.groupInfo.Label = "Справка";
            this.groupInfo.Name = "groupInfo";
            // 
            // butFormReport
            // 
            this.butFormReport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.butFormReport.Label = "Формирование отчета";
            this.butFormReport.Name = "butFormReport";
            this.butFormReport.OfficeImageId = "ReviewShareWorkbook";
            this.butFormReport.ShowImage = true;
            this.butFormReport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butFormReport_Click);
            // 
            // butFormLiveReport
            // 
            this.butFormLiveReport.Label = "Динамический отчет";
            this.butFormLiveReport.Name = "butFormLiveReport";
            this.butFormLiveReport.OfficeImageId = "ViewSlideShowView";
            this.butFormLiveReport.ShowImage = true;
            this.butFormLiveReport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butFormLiveReport_Click);
            // 
            // ButVideo
            // 
            this.ButVideo.Label = "Видеоклип";
            this.ButVideo.Name = "ButVideo";
            this.ButVideo.OfficeImageId = "SlideMasterMediaPlaceholderInsert";
            this.ButVideo.ShowImage = true;
            this.ButVideo.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButVideo_Click);
            // 
            // butSaveReport
            // 
            this.butSaveReport.Label = "Сохранить отчет в журнал";
            this.butSaveReport.Name = "butSaveReport";
            this.butSaveReport.OfficeImageId = "ExportSharePointList";
            this.butSaveReport.ShowImage = true;
            this.butSaveReport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butSaveReport_Click);
            // 
            // butLoadReport
            // 
            this.butLoadReport.Label = "Загрузить отчет из журнала";
            this.butLoadReport.Name = "butLoadReport";
            this.butLoadReport.OfficeImageId = "ImportSharePointList";
            this.butLoadReport.ShowImage = true;
            this.butLoadReport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butLoadReport_Click);
            // 
            // butAbsoluteEdit
            // 
            this.butAbsoluteEdit.ImageName = "butAbsoluteEdit";
            this.butAbsoluteEdit.Label = "Редактировать Абс. значения";
            this.butAbsoluteEdit.Name = "butAbsoluteEdit";
            this.butAbsoluteEdit.OfficeImageId = "WordArtEditTextClassic";
            this.butAbsoluteEdit.ShowImage = true;
            this.butAbsoluteEdit.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butAbsoluteEdit_Click);
            // 
            // buttonSetup
            // 
            this.buttonSetup.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonSetup.Label = "Настройка отчета";
            this.buttonSetup.Name = "buttonSetup";
            this.buttonSetup.OfficeImageId = "ControlsGallery";
            this.buttonSetup.ShowImage = true;
            this.buttonSetup.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonSetup_Click);
            // 
            // buttonNewReport
            // 
            this.buttonNewReport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.buttonNewReport.Label = "Создать бланк отчета";
            this.buttonNewReport.Name = "buttonNewReport";
            this.buttonNewReport.OfficeImageId = "SlideNew";
            this.buttonNewReport.ShowImage = true;
            this.buttonNewReport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.buttonNewReport_Click);
            // 
            // ButCopyServerReport
            // 
            this.ButCopyServerReport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.ButCopyServerReport.Label = "Обновить бланк из контрольного";
            this.ButCopyServerReport.Name = "ButCopyServerReport";
            this.ButCopyServerReport.OfficeImageId = "PublishToPdfOrEdoc";
            this.ButCopyServerReport.ShowImage = true;
            this.ButCopyServerReport.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButCopyServerReport_Click);
            // 
            // butAddLink
            // 
            this.butAddLink.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.butAddLink.Label = "Редактирование ссылок";
            this.butAddLink.Name = "butAddLink";
            this.butAddLink.OfficeImageId = "AdpDiagramAddRelatedTables";
            this.butAddLink.ShowImage = true;
            this.butAddLink.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butAddLink_Click);
            // 
            // ButShapeLibray
            // 
            this.ButShapeLibray.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.ButShapeLibray.Label = "Свойства фигуры";
            this.ButShapeLibray.Name = "ButShapeLibray";
            this.ButShapeLibray.OfficeImageId = "WhatIfAnalysisMenu";
            this.ButShapeLibray.ShowImage = true;
            this.ButShapeLibray.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButShapeLibray_Click);
            // 
            // butUpdate
            // 
            this.butUpdate.Label = "Обновить ссылки";
            this.butUpdate.Name = "butUpdate";
            this.butUpdate.OfficeImageId = "RecurrenceEdit";
            this.butUpdate.ShowImage = true;
            this.butUpdate.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butUpdate_Click);
            // 
            // ButClearCells
            // 
            this.ButClearCells.Label = "Очистить ячейки";
            this.ButClearCells.Name = "ButClearCells";
            this.ButClearCells.OfficeImageId = "InkEraseMode";
            this.ButClearCells.ShowImage = true;
            this.ButClearCells.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.ButClearCells_Click);
            // 
            // butDeleteLinks
            // 
            this.butDeleteLinks.Label = "Удаление ссылок";
            this.butDeleteLinks.Name = "butDeleteLinks";
            this.butDeleteLinks.OfficeImageId = "CellsDelete";
            this.butDeleteLinks.ShowImage = true;
            this.butDeleteLinks.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.butDeleteLinks_Click);
            // 
            // Help
            // 
            this.Help.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.Help.Label = "Справка";
            this.Help.Name = "Help";
            this.Help.OfficeImageId = "TentativeAcceptInvitation";
            this.Help.ShowImage = true;
            this.Help.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Help_Click);
            // 
            // ReporterRibbon
            // 
            this.Name = "ReporterRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.InfoTask);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.ReporterRibbon_Load);
            this.InfoTask.ResumeLayout(false);
            this.InfoTask.PerformLayout();
            this.groupForming.ResumeLayout(false);
            this.groupForming.PerformLayout();
            this.groupArchive.ResumeLayout(false);
            this.groupArchive.PerformLayout();
            this.groupSetup.ResumeLayout(false);
            this.groupSetup.PerformLayout();
            this.groupEdit.ResumeLayout(false);
            this.groupEdit.PerformLayout();
            this.groupInfo.ResumeLayout(false);
            this.groupInfo.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab InfoTask;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupForming;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butFormReport;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupSetup;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupEdit;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonNewReport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton buttonSetup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butAddLink;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butDeleteLinks;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butUpdate;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Help;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butAbsoluteEdit;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ButCopyServerReport;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupArchive;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butSaveReport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butLoadReport;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup groupInfo;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton butFormLiveReport;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ButVideo;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ButShapeLibray;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton ButClearCells;
    }

    partial class ThisRibbonCollection
    {
        internal ReporterRibbon ReporterRibbon
        {
            get { return this.GetRibbon<ReporterRibbon>(); }
        }
    }
}
