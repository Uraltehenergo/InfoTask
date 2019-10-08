using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;
using Point = System.Drawing.Point;

namespace Calculation
{
    //Интерфейс потока приложения
    [ServiceContract]
    public interface IThreadApp
    {
        //Задание Id потока (id) и чтение настроек, showIndicator - отображать окно с индикатором
        [OperationContract]
        string Open(int id, bool showIndicator);
        //Возвращает Id потока
        [OperationContract]
        int ThreadId();
        //Закрытие потока
        [OperationContract]
        string Close();
        //Загрузка настроек и проверка соединения с провайдерами
        [OperationContract]
        string LoadSetup();
        //Расчет, timeBegin - начало, timeEnd - конец, intervalName - имя
        //writeArchiveType - Single, Named, NamedAdd, NamedAddParams, Periodic, periodLength - длина интервала в минутах
        [OperationContract]
        string Calc(DateTime timeBegin, DateTime timeEnd, string imitMode = "NoImit", string intervalName = "", string writeArchiveType = "Single", int periodLength = 15); 
        //Расчет + запалнение ведомости анализатора
        [OperationContract]
        string AnalyzerCalc(DateTime timeBegin, DateTime timeEnd, string vedFile, string task = "");
        //Задание параметров расчета
        [OperationContract]
        void SetCalcOperations(bool readSources, bool writeArchives, bool writeReceivers);
        //Задание параметров отладочного сохранения
        [OperationContract]
        void SetDebugOperations(bool saveSignals, bool saveParams, bool saveMethods, bool saveVariables, bool saveReceiversSignals, bool saveValues);
        //Интервал последнего расчета
        [OperationContract]
        int IntervalId();
        //Получение времени источников
        [OperationContract]
        string GetSourcesTime();
        //Время источников 
        [OperationContract]
        TimeInterval SourcesTime();
        //Строка содержащая времена всех источников в формате Имя Провайдера=Начало периода-Конец периода;
        [OperationContract]
        string SourcesTimeString();
        //Текущее сообщение об ошибке потока
        [OperationContract]
        string ErrMessage();
    }

    //-------------------------------------------------------------------------------
    //Поток расчетов созданный из другого приложения
    public class ThreadApp : ThreadCalc, IThreadApp
    {
        //Комманда, открытие потока, showIndicator - отображать окно с индикатором
        public string Open(int id, bool showIndicator)
        {
            using (Start(true))
            {
                General.Initialize();
                ReadFunctions();
                Id = id;
                ThreadName = Id.ToString();
                ShowIndicator = showIndicator;
                OpenHistory(General.ControllerDir + @"History\History" + Id + ".accdb", General.HistryTemplateFile, true);
                PrepareResultFile();
                using (StartAtom(Atom.OpenThread))
                {
                    try
                    {
                        using (var rec = new RecDao(General.ControllerFile, "SELECT * FROM Threads WHERE ThreadId = " + Id))
                        {
                            ApplicationType = rec.GetString("ApplicationType").ToApplicationType();
                            Comment = rec.GetString("Comment");
                            IsImit = rec.GetBool("IsImit");
                            ImitModeStr = rec.GetString("ImitMode");
                        }
                    }
                    catch (Exception ex)
                    {
                        AddError("Ошибка открытия потока", ex);
                    }
                }
                if (!Command.IsError)
                    StartAtom(Atom.ReadSetup, ReadSetup);
                IsClosed = false;
                return _errMessage = Command.ErrorMessage();
            }
        }

        //Показывать индикатор
        public bool ShowIndicator { get; private set; }
        //Форма с индикатором
        public IndicatorForm Form { get; private set; }

        //Возвращает Id потока
        public int ThreadId()
        {
            return Id;
        }
        //Сообщение об ошибке
        private string _errMessage;
        public string ErrMessage()
        {
            return _errMessage;
        }

        //Комманда, закрытие потока
        public string Close()
        {
            if (IsClosed) return "Поток уже был закрыт";
            Start(true);
            CloseThread();
            return _errMessage = Finish().ErrorMessage();    
        }

        //Загрузка настроек и проверка соединения с провайдерами
        public string LoadSetup()
        {
            if (IsClosed) return "Поток уже был закрыт";
            Start(true);
            if (StartAtom(Atom.ReadSetup, ReadSetup))
                StartAtom(Atom.CheckProviders, CheckProviders);
            return _errMessage = Finish().ErrorMessage();
        }

        //Комманда, получение времени источников
        public string GetSourcesTime()
        {
            if (IsClosed) return "Поток уже был закрыт";
            Start(true);
            StartAtom(Atom.ReadTime, ReadTime);
            return _errMessage = Finish().ErrorMessage();
        }

        //Комманда, Время источников
        public TimeInterval SourcesTime()
        {
            return new TimeInterval(SourcesBegin, SourcesEnd);
        }

        //Комманда, расчет, timeBegin - начало, timeEnd - конец, intervalName - имя
        //writeArchiveType - Single, Named, NamedAdd, NamedAddParams, Periodic, periodLength - длина интервала в минутах
        public string Calc(DateTime timeBegin, DateTime timeEnd, string imitMode = "NoImit", string intervalName = "", string writeArchiveType = "Single", int periodLength = 15)
        {
            if (timeBegin > timeEnd) return "Начало интервала расчета не должно быть позже его окончания";
            ImitModeCalc = imitMode.ToImitMode();
            CalcName = intervalName;
            WriteArchiveType = writeArchiveType.ToIntervalType();
            IsPeriodic = WriteArchiveType == IntervalType.Periodic;
            PeriodLength = periodLength;
            if (!IsPeriodic) PeriodEnd = timeEnd;
            PeriodBegin = timeBegin;
            StopTime = timeEnd;
            IsWriteVed = false;
            return CalcCommand();
        }

        //Расчет с сохранением результатов в ведомость анализатора (РАС)
        public string AnalyzerCalc(DateTime timeBegin, DateTime timeEnd, string vedFile, string task = "")
        {
            if (timeBegin > timeEnd) return "Начало интервала расчета не должно быть позже его окончания";
            PeriodBegin = timeBegin;
            PeriodEnd = timeEnd;
            StopTime = timeEnd;
            IsWriteVed = true;
            VedTask = task;
            VedFile = vedFile;
            return CalcCommand();
        }

        private string CalcCommand()
        {
            if (IsClosed) return "Поток уже был закрыт";
            Start(true);
            try
            {
                IdInterval = 0;
                ShowIndicatorForm();
                if (Projects.Values.Count(x => x.Otm) == 0) return "";
                using (StartAtom(Atom.Run)) { }
                RunCalc();   
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке расчета", ex);
            }
            finally { HideIndicatorForm(); }
            return _errMessage = Finish().ErrorMessage();
        }

        //Показать форму индикатора
        private void ShowIndicatorForm()
        {
            if (ShowIndicator)
            {
                CalcMode = "Запущен";
                Application.EnableVisualStyles();
                Form = new IndicatorForm {ThreadApp = this};
                var screen = Screen.PrimaryScreen;
                Form.Location = new Point(screen.WorkingArea.Width - Form.Width - 1, screen.WorkingArea.Height - Form.Height - 2);
                Form.Show();
                Form.Text = "Поток " + Id + " (" + ProjectsString + ")";
                Form.CalcName.Text = CalcName;
                Form.Refresh();
                Thread.Sleep(100);
            }
        }

        //Спрятать форму индикатора
        private void HideIndicatorForm()
        {
            PeriodBegin = new DateTime();
            PeriodEnd = new DateTime();
            CalcName = null;
            if (Form != null) Form.Invoke(new FormDelegate(HideForm));
            CalcMode = "Остановлен";
        }

        private void RunCalc()
        {
            bool e;
            using (StartView(ViewAtom.Calc, true))
            {
                if (Form != null) Form.Invoke(new FormDelegate(UpdateFormTime));
                //Подготовка рачета
                Start(PrepareCalc, 0, 25);
                //Расчет
                if (!Start(Cycle, 25) && !IsPeriodic)
                    using (StartAtom(Atom.Stop)) { }
                e = Command.IsError;
            }

            //Периодический расчет
            if (IsPeriodic)
                while (!e && PeriodEnd.AddSeconds(1) < StopTime)
                {
                    PeriodBegin = PeriodBegin.AddMinutes(PeriodLength);
                    if (Form != null) Form.Invoke(new FormDelegate(UpdateFormTime));
                    using (StartAtom(Atom.Next)) { }
                    StartView(ViewAtom.Calc, Cycle, true);
                    e |= Command.IsError;
                }
        }

        //Обновление времени в форме индикатора
        private void UpdateFormTime()
        {
            Form.PeriodBegin.Text = PeriodBegin.ToString();
            Form.PeriodEnd.Text = PeriodEnd.ToString();
            Form.Refresh();
        }

        //Прерываение расчета
        public void BreakCalc()
        {
            Task.Abort();
            //if (CommandLog != null)
            //    AddEvent("Выполнение прервано");
            //while (Command != null && Command.Behaviour != CommandBehaviour.SubLog)
            //    Finish("Выполнение прервано", true);
            //if (Command != null) Finish("Выполнение прервано", true);
            try
            {
                using (StartAtom(Atom.BreakCalc))
                {
                    State = State.Stopped;
                    CalcMode = "Остановлен";
                    IndicatorProcent = 0;
                    IndicatorText = "";
                }
                if (Form != null) Form.Invoke(new FormDelegate(HideForm));
            }
            catch { }
        }
        private delegate void FormDelegate();
        private void HideForm()
        {
            try { Form.Hide(); }
            catch { }
            Form.Dispose();
            Form = null;      
        }


        //Состояние (растчет или остановлен)
        private State _state = State.Stopped;
        public override State State { get { return _state; } set { _state = value; } }

        //Запускает один полный расчет без чтения списка параметров
        private void Cycle()
        {
            double sc = IsReadSources ? Sources.Count * 30 + Imitators.Count * 15 : 0, 
                       pc = Projects.Count * 20, 
                       ac = IsWriteArchives ? Projects.Count * 15 : 0, 
                       rc = IsWriteReceivers ? Receivers.Count * 10 : 0,
                       vc = VedSaver != null ? 20 : 0;
            if (sc + pc + ac + rc > 1)
            {
                double n = 100.0 / (sc + pc + ac + rc + vc + 15), i = 0;
                if (IsReadSources)
                    IsSourcesRead = Start(ReadSources, i, i += sc*n);
                Start(Calculate, i, i += pc * n);
                if (IsWriteReceivers) Start(WriteReceivers, i, i += rc*n);
                if (IsWriteArchives) Start(WriteArchives, i, i += ac*n);
                if (IsWriteVed)
                    StartAtom(Atom.WriteVed, new VedSaver(ProjectsList.First(), VedFile, VedTask).SaveVed, i, i + vc * n);
                LastPeriodBegin = PeriodBegin;
                LastPeriodEnd = PeriodEnd;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Логирование
        //////////////////////////////////////////////////////////////////////////////////////////
        protected override void MessageError(ErrorCommand er) { }

        protected override void ViewProcent(double procent)
        {
            if (Form != null)
            {
                Form.Procent.Value = Convert.ToInt32(procent);
                Form.Refresh();
            }
        }
    }
}
