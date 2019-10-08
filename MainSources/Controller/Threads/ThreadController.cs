using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using BaseLibrary;
using Calculation;
using CommonTypes;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace Controller
{
    //Поток расчетов созданный из контроллера
    public class ThreadController : ThreadCalc
    {
        public ThreadController(IRecordRead rec)
        {
            using (var c = Start())
            {
                Id = rec.GetInt("ThreadId");
                ThreadName = Id.ToString();
                ApplicationType = ApplicationType.Controller;
                OpenHistory(General.ControllerDir + @"History\History" + Id + ".accdb", General.HistryTemplateFile, true);
                PrepareResultFile();
                using (StartAtom(Atom.OpenThread))
                {
                    try
                    {
                        AddEvent("Чтение списка функций");
                        ReadFunctions();
                        AddEvent("Чтение свойств потока");
                        Comment = rec.GetString("Comment");
                        IsPeriodic = rec.GetBool("IsPeriodic");
                        CalcType = IsPeriodic ? "Периодический" : "Разовый";
                        PeriodLength = rec.GetDouble("PeriodLength", 15);
                        SourcesLate = rec.GetDouble("SourcesLate", 2);
                        RushWaitingTime = rec.GetDouble("RushWaitingTime");
                        TimeAfterError = rec.GetDouble("TimeAfterError", 5);
                        IsImit = rec.GetBool("IsImit");
                        ImitModeStr = rec.GetString("ImitMode");
                        using (var sys = new SysTabl(ResultFile))
                        {
                            PeriodBegin = sys.SubValue("PeriodInfo", "PeriodBegin").ToDateTime();
                            PeriodEnd = sys.SubValue("PeriodInfo", "PeriodEnd").ToDateTime();
                            CalcName = sys.SubValue("PeriodInfo", "CalcName");
                            StartMoment = sys.SubValue("PeriodInfo", "StartMoment").ToDateTime();
                            StartTime = sys.SubValue("PeriodInfo", "StartTime").ToDateTime();
                            StopTime = sys.SubValue("PeriodInfo", "StopTime").ToDateTime();
                            IsStopTime = sys.SubValue("PeriodInfo", "IsStopTime") == "True";
                            _lastErrorText = sys.SubValue("LastErrorInfo", "ErrorText");
                            _lastErrorTime = sys.SubValue("LastErrorInfo", "ErrorPeriodBegin").ToDateTime();
                            _lastErrorPos = 0;
                            UpdateTablo();
                        }
                        _state = State.Stopped;
                    }
                    catch (Exception ex)
                    {
                        AddError("Ошибка загрузки потока", ex);
                    }
                }
                StartAtom(Atom.ReadSetup, ReadSetup);
                IsClosed = false;
                if (c.IsError) Different.MessageError("Ошибка загрузки данных потока расчета. " + c.ErrorMessage());
            }
        }

        //Ссылка на приложение
        public App App { get; set; }

        //Окно управления потоком
        public ThreadWindow ThreadWindow { get; set; }
        //Окно настроек
        public SetupWindow SetupWindow { get; set; }
        //Окно ручного ввода
        public HandInputWindow HandInputWindow { get; set; }
        //Окно сообщений
        public ErrorsListWindow ErrorsListWindow { get; set; }

        private delegate void Delegate();
        //Обновление интерфейса окна потока по состоянию
        public void UpdateState()
        {
            var w = ThreadWindow;
            w.butStart.Content = (State == State.Calc || State == State.FinishWaiting || State == State.GetTime || State == State.Waiting) ? "Стоп" : "Пуск";
            w.butStart.IsEnabled = State != State.AbsoluteEdit && State != State.HandInput && State != State.Setup && State != State.ErrorsList;
            w.butBreak.Visibility = State == State.FinishCalc ? Visibility.Visible : Visibility.Hidden;
            w.butUpdateSources.IsEnabled = State == State.Stopped || State == State.Waiting || State == State.HandInput || State == State.AbsoluteEdit;
            w.periodBegin.IsReadOnly = State != State.Stopped;
            w.periodEnd.IsReadOnly = State != State.Stopped || IsPeriodic;
            w.periodEnd.IsEnabled = !IsPeriodic;
            w.periodEnd.BorderBrush = new SolidColorBrush(IsPeriodic ? Colors.White : Colors.Black);
            w.CalcName.IsReadOnly = State != State.Stopped;
            w.gridPeriodic.Visibility = IsPeriodic ? Visibility.Visible : Visibility.Hidden;
            if (IsPeriodic)
            {
                w.IsStopTime.IsEnabled = State == State.Stopped;
                w.StopTime.IsReadOnly = State != State.Stopped;
            }
            w.butSetup.IsEnabled = State == State.Stopped || State == State.Setup;
            w.butMessages.IsEnabled = State == State.Stopped || State == State.ErrorsList;
            w.butHandInput.IsEnabled = State == State.Stopped || State == State.HandInput;
            w.butAbsolute.IsEnabled = State == State.Stopped || State == State.AbsoluteEdit;
        }

        //Обновляет текст и внешний вид табло с ошибкой
        private void UpdateTablo()
        {
            var t = PeriodBegin.Subtract(_lastErrorTime);
            if (t.TotalSeconds < -1) TabloVisible = false;
            else
            {
                TabloVisible = t.TotalDays <= 3;
                TabloColor = _lastErrorPos > 0 ? Brushes.Salmon : Brushes.White;
                TabloText = (_lastErrorText ?? "") + (_lastErrorTime == Different.MinDate ? "" : (", расчет от " + _lastErrorTime + ""));
            }
        }

        //Обновление кнопок Пуск и Прервать
        public void UpdateStopPressed()
        {
            var w = ThreadWindow;
            w.butStart.Content = StopPressed ? "Стоп" : "Пуск";
        }

        //True, если нажата кнопка Стоп, но расчет еще не закончился
        private bool _stopPressed;
        private readonly object _stopPressedLock = new object();
        public bool StopPressed
        {
            get { lock (_stopPressedLock) return _stopPressed; }
            set
            {
                lock (_stopPressedLock)
                {
                    if (_stopPressed == value) return;
                    _stopPressed = value;
                }
                if (ThreadWindow != null)
                    ThreadWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Delegate(UpdateStopPressed));
            }
        }
        //Состояние интерфейса
        private State _state = State.Stopped;
        private readonly object _stateLock = new object();
        public override State State 
        { 
            get { lock (_stateLock) return _state;} 
            set
            {
                lock (_stateLock)
                {
                    if (_state == value) return;
                    _state = value;   
                }
                if (ThreadWindow != null)
                    ThreadWindow.Dispatcher.Invoke(DispatcherPriority.Normal, new Delegate(UpdateState));
            } 
        }

        //Возможная задержка источников в минутах
        private double _sourcesLate;
        public double SourcesLate
        {
            get { return _sourcesLate; }
            set
            {
                if (value == _sourcesLate) return;
                _sourcesLate = value;
                OnPropertyChanged("SourcesLate");
            }
        }
        //Перерыв между расчетами в режиме выравнивания
        private double _rushWaitingTime;
        public double RushWaitingTime
        {
            get { return _rushWaitingTime; }
            set
            {
                if (value == _rushWaitingTime) return;
                _rushWaitingTime = value;
                OnPropertyChanged("RushWaitingTime");
            }
        }

        //Сколько минут ждать перед повторным расчетом или повторным обновлением времени
        private double _timeAfterError;
        public double TimeAfterError
        {
            get { return _timeAfterError; }
            set
            {
                if (value == _timeAfterError) return;
                _timeAfterError = value;
                OnPropertyChanged("TimeAfterError");
            }
        }

        //Время запуска цикла расчета
        private DateTime? _startMoment;
        public DateTime? StartMoment
        {
            get { return _startMoment; }
            set
            {
                if (value == _startMoment) return;
                _startMoment = value;
                OnPropertyChanged("StartMoment");
            }
        }
        //Начало диапазона текущего цикла расчета
        private DateTime? _startTime;
        public DateTime? StartTime
        {
            get { return _startTime; }
            set
            {
                if (value == _startTime) return;
                _startTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        //True, если задан конец диапазона расчета
        private bool _isStopTime;
        public bool IsStopTime
        {
            get { return _isStopTime; }
            set
            {
                if (value == _isStopTime) return;
                _isStopTime = value;
                OnPropertyChanged("IsStopTime");
            }
        }

        //Характеристики последней ошибки
        private string _lastErrorText;
        private DateTime _lastErrorTime;
        //0 - ошибка была давно, 1 - в текущий период, 2 - в прошлый
        private int _lastErrorPos;

        //Характеристики табло вывода ошибок и последней ошибки
        private string _tabloText;
        public string TabloText
        {
            get { return _tabloText; }
            set
            {
                if (value == _tabloText) return;
                _tabloText = value;
                OnPropertyChanged("TabloText");
            }
        }
        private bool _tabloVisible = false;
        public bool TabloVisible
        {
            get { return _tabloVisible; }
            set
            {
                if (value == _tabloVisible) return;
                _tabloVisible = value;
                OnPropertyChanged("TabloVisible");
            }
        }
        private SolidColorBrush _tabloColor;
        public SolidColorBrush TabloColor
        {
            get { return _tabloColor; }
            set
            {
                if (value == _tabloColor) return;
                _tabloColor = value;
                OnPropertyChanged("TabloColor");
            }
        }

        //Список параметров для ручного ввода и для редактирования абсолютных значений
        private readonly ObservableCollection<GridInputParam> _gridInputParams = new ObservableCollection<GridInputParam>();
        public ObservableCollection<GridInputParam> GridInputParams { get { return _gridInputParams; } }
        //Список ошибок
        private readonly ObservableCollection<GridError> _gridErrors = new ObservableCollection<GridError>();
        public ObservableCollection<GridError> GridErrors { get { return _gridErrors; } }
        //Список ошибок был изменен
        public int GridErrorsCount { get; private set; }
        
        //Обработка нажатия кнопки Обновить
        public void RefreshTime()
        {
            if (State == State.Stopped)
                StartAtom(Atom.ReadTime, ReadTime);
            else if (State == State.Waiting)
                State = State.GetTime;
        }

        //Обработка нажатия кнопки Пуск / Стоп
        public void StartCalc()
        {
            switch (State)
            {
                case State.Stopped:
                    StartMoment = DateTime.Now;
                    StartTime = PeriodBegin;
                    Task = IsPeriodic ? new Thread(CalcPeriodic) : new Thread(CalcSingle);
                    Task.IsBackground = false;
                    Task.Priority = ThreadPriority.AboveNormal;
                    Task.Start();
                    return;
                case State.Calc:
                    State = State.FinishCalc;
                    return;
                case State.FinishCalc:
                    State = State.Calc;
                    return;
                case State.Waiting:
                case State.GetTime:
                    State = State.FinishWaiting; 
                    return;            
            }
        }
        
        //Соединение и SysTabl файла результатов
        private DaoDb _dbResilt;
        private SysTabl _sysResult;

        //Разовый расчет
        public void CalcSingle()
        {
            State = State.Calc;
            CalcMode = "Запущен";
            WriteArchiveType = IntervalType.Single;
            using (StartAtom(Atom.Run))
            {
                OpenResult();
                SavePeriod(false);
            }
            using (StartView(ViewAtom.Calc, true))
                if (Start(PrepareCalc, 0, 15) && State != State.FinishCalc && Projects.Values.Count(x => x.Otm) != 0)
                    Start(Cycle, 15);
            using (StartAtom(Atom.Stop))
                CloseResult();
            State = State.Stopped;
            CalcMode = "Остановлен";
        }
        
        //Периодический расчет
        public void CalcPeriodic()
        {
            State = State.Calc;
            CalcMode = "Запущен";
            WriteArchiveType = IntervalType.Periodic;
            using (StartAtom(Atom.Run)) //Запуск расчета
            {
                OpenResult();
                SavePeriod(true);
            }    
            //Подготовка расчета
            StartView(ViewAtom.PrepareCalc, PrepareCalc, true);
            if (Projects.Values.Count(x => x.Otm) != 0)
                while (State != State.FinishCalc && State != State.FinishWaiting) 
                {
                    while (General.Paused) Thread.Sleep(1000); //Отладочная пауза для измерения памяти
                    State = State.Waiting;
                    Start(Waiting);//Ожидание
                    if (State == State.FinishWaiting) break;
                    State = State.Calc;//Расчет
                    StartView(ViewAtom.Calc, Cycle, true);
                    //Следующий период расчета
                    PeriodBegin = PeriodBegin.AddMinutes(PeriodLength);
                    using (StartAtom(Atom.Next))
                        SavePeriod(false);
                    if (State == State.FinishCalc) break;
               
                    //Прерыватся, если достигнуто время остановки расчета
                    if (IsStopTime && StopTime != null && StopTime.Value.Subtract(PeriodBegin).TotalSeconds < 1)
                        break;
                    UpdateHistory(true);
                }
            using (StartAtom(Atom.Stop)) //Остановка расчета
                CloseResult();
            State = State.Stopped;
            CalcMode = "Остановлен";
        }

        //Открытие и закрытие SysTabl файла Result
        private void OpenResult()
        {
            try
            {
                if (_sysResult != null) _sysResult.Dispose();
                if (_dbResilt != null) _dbResilt.Dispose();
                _dbResilt = new DaoDb(ResultFile);
                _sysResult = new SysTabl(_dbResilt);
                _lastErrorPos = 0;
            }
            catch (Exception ex)
            {
                AddError("Ошибка при открытии SysTabl файла Result", ex);
            }
        }
        private void CloseResult()
        {
            if (_sysResult != null)
            {
                _sysResult.Dispose();
                _sysResult = null;
            }
            if (_dbResilt != null)
            {
                _dbResilt.Dispose();
                _dbResilt = null;
            }
        }

        //Сохранение периода расчета в ControllerData, saveStartStop = true - сохраняются также время запуска расчета, время останова и т.д.
        private void SavePeriod(bool saveStartStop)
        {
            try
            {
                _lastErrorPos = saveStartStop || _lastErrorPos != 1 ? 0 : 2;
                UpdateTablo();
                _sysResult.PutSubValue("PeriodInfo", "PeriodBegin", PeriodBegin.ToString());
                _sysResult.PutSubValue("PeriodInfo", "PeriodEnd", PeriodEnd.ToString());
                if (_lastErrorTime == PeriodBegin)
                {
                    _sysResult.PutSubValue("LastErrorInfo", "ErrorText", _lastErrorText);
                    _sysResult.PutSubValue("LastErrorInfo", "ErrorPeriodBegin", _lastErrorTime.ToString());
                }
                if (saveStartStop)
                {
                    _lastErrorPos = 0;
                    _sysResult.PutSubValue("PeriodInfo", "CalcName", CalcName);
                    _sysResult.PutSubValue("PeriodInfo", "StartMoment", StartMoment.ToString());
                    _sysResult.PutSubValue("PeriodInfo", "StartTime", StartTime.ToString());
                    _sysResult.PutSubValue("PeriodInfo", "StopTime", StopTime.ToString());
                    _sysResult.PutSubValue("PeriodInfo", "IsStopTime", IsStopTime ? "True" : "False");
                }
            }
            catch(Exception ex)
            {
                AddError("Ошибка записи в SysTabl файла результатов", ex);
            }
        }

        //Один цикл ожидания atom - ожидание или дополнительное, finish - время завершения ожидания
        private void Wait(DateTime finish, Atom atom)
        {
            var start = DateTime.Now;
            while (DateTime.Now < finish && State != State.FinishWaiting)
            {
                using (StartAtom(atom))
                    while (DateTime.Now < finish && State == State.Waiting)
                    {
                        Procent = 100 * (DateTime.Now.Subtract(start).TotalSeconds) / (Math.Max(1, finish.Subtract(start).TotalSeconds));
                        TimeSpan t = finish.Subtract(DateTime.Now);
                        IndicatorText = t.Minutes + ":" + (t.Seconds < 10 ? "0" : "") + t.Seconds;
                        Thread.Sleep(Math.Min(1000, Convert.ToInt32(t.TotalSeconds)));
                    }
                if (State == State.GetTime)
                {
                    State = State.Waiting;
                    StartAtom(Atom.ReadTime, ReadTime, Procent, Procent);
                }
                if (State == State.FinishWaiting)
                {
                    IndicatorText = "";
                    return;
                }
                IndicatorText = "";
            }
        }
        
        //Определяет, нужно ли ждать и ждет, если нужно
        private void Waiting()
        {
            DateTime start = DateTime.Now;
            FinishCycleTime = PeriodEnd.AddMinutes(SourcesLate);
            if (FinishCycleTime.Subtract(start).TotalSeconds < 1)
            {
                CalcMode = "Выравнивание";
                if (RushWaitingTime > 0)
                    Wait(DateTime.Now.AddMinutes(RushWaitingTime), Atom.Wait);
                if (State == State.FinishWaiting) return;
            }
            else
            {
                CalcMode = "Синхронный";
                using (StartView(ViewAtom.Waiting, false))
                    Wait(FinishCycleTime, Atom.Wait);
                if (State == State.FinishWaiting) return;
            }
            if ((!StartAtom(Atom.ReadTime, ReadTime) || FinishCycleTime.AddMinutes(-SourcesLate) > SourcesEnd))
            {
                if (State == State.FinishWaiting) return;
                State = State.Waiting;
                using (StartView(ViewAtom.ErrorWaiting, false))
                    Wait(start.AddMinutes(Math.Max(1, SourcesLate)), Atom.ErrorWait);
                if (State != State.FinishWaiting)
                    StartAtom(Atom.ReadTime, ReadTime);
            }
        }

        //Один цикл расчета
        public void Cycle()
        {
            UpdateTablo();
            StartProcess("Calc");
            try
            {
                double sc = Sources.Count * 30, pc = Projects.Count * 20, ac = Archive == null ? 0 : Projects.Count * 15, rc = Receivers.Count * 10;
                double n = 100.0 / (sc + pc + ac + rc), i = 0;
                Start(ReadSources, i, i += sc * n);
                if (Archive != null) StartAtom(Atom.UpdateAbsoluteEdit, UpdateAbsoluteEdit, i, i);
                Start(Calculate, i, i += pc * n);
                Start(WriteReceivers, i, i += rc * n);
                Start(WriteArchives, i, i + ac * n);
                ClearMemory();
            }
            finally {FinishProcess("Calc");}
        }

        //Обновляет, если надо ручной ввод и абсолютные значения из архива
        private void UpdateAbsoluteEdit()
        {
            try
            {
                foreach (var project in Projects.Values)
                    if (project.IsAbsolute && Archive.IsAbsoluteEdited(project.Code))
                        Start(project.ReadArchiveForPeriodic);
            }
            catch(Exception ex)
            {
                AddError("Ошибка при чтении из архива", ex, "Архив:" + Archive.Name);
            }
        }

        //Прерывание расчета
        public void BreakCalc()
        {
            Task.Abort();
            if (CommandLog != null)
                AddEvent("Выполнение прервано");
            while (Command != null && Command.Behaviour != CommandBehaviour.SubLog)
                Finish("Выполнение прервано", true);
            if (Command != null) Finish("Выполнение прервано", true);
            using (StartAtom(Atom.BreakCalc))
            {
                State = State.Stopped;
                CalcMode = "Остановлен";
                IndicatorProcent = 0;
                IndicatorText = "";
            }
        }

        //Удаление потока
        public void DeleteThread()
        {
            using (StartAtom(Atom.DeleteThread))
            {
                try
                {
                    DaoDb.Execute(General.ControllerFile, "DELETE * FROM Threads WHERE ThreadId=" + Id);
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при удалении потока", ex);
                }    
            }
            CloseThread();
        }

        //Загрузка параметров ручного ввода для редактирования
        public void LoadHandInput()
        {
            ClearLists();
            foreach (var pr in ProjectsList)
            {
                try
                {
                    using ( var rec = new ReaderAdo(pr.File, "SELECT * FROM CalcParams WHERE (CalcParamType Is Not Null) And (CalcOn=True) And (TaskOn=True) ORDER BY Task, Code"))
                        while (rec.Read())
                        {
                            var gip = new GridInputParam(pr.Code, rec, false);
                            if (!ProjectsForFilter.Contains(gip.Project)) ProjectsForFilter.Add(gip.Project);
                            if (!TasksForFilter.Contains(gip.Task)) TasksForFilter.Add(gip.Task);
                            if (!DataTypesForFilter.Contains(gip.DataType)) DataTypesForFilter.Add(gip.DataType);
                            GridInputParams.Add(gip);
                        }    
                }
                catch(Exception ex)
                {
                    AddError("Не найден или неправильный проект", ex, pr.Code);
                    MessageBox.Show("Не найден или неправильный проект", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //Загрузка абсолютных значений для редактирования
        public void LoadAbsolute()
        {
            ClearLists();
            foreach (var pr in ProjectsList)
            {
                using (StartAtom(Atom.LoadAbsolute, 0, 100, "Проект " + pr.Code, pr.Code))
                {
                    try
                    {
                        if (SysTabl.SubValueS(pr.File, "ArchiveOptions", "IsAbsolute") == "True")
                        {
                            //Чтение значений из архива
                            var apars = Archive.ReadAbsoluteEdit(pr.Code, false);
                            //Чтение параметров из проекта
                            using (var rec = new ReaderAdo(pr.File, "SELECT CalcParamsArchive.FullCode as Code, CalcParams.Task, CalcParams.Name, CalcSubParams.Name AS SubName, " +
                                               "CalcParamsArchive.Units, CalcParamsArchive.DataType, CalcParamsArchive.SuperProcessType, CalcParams.Comment " +
                                               "FROM CalcParams INNER JOIN (CalcSubParams RIGHT JOIN CalcParamsArchive ON CalcSubParams.CalcParamId = CalcParamsArchive.CalcSubParamId) ON CalcParams.CalcParamId = CalcParamsArchive.CalcParamId " +
                                               "WHERE (CalcParams.CalcOn=True) And (CalcParams.TaskOn=True) And ((CalcSubParams.CalcOn=True) Or (CalcSubParams.CalcOn Is Null)) " +
                                               "ORDER BY Task, FullCode;"))
                                while (rec.Read())
                                {
                                    var sp = rec.GetString("SuperProcessType").ToSuperProcess();
                                    if (sp.IsAbsolute())
                                    {
                                        var gip = new GridInputParam(pr.Code, rec, true);
                                        if (!ProjectsForFilter.Contains(gip.Project)) ProjectsForFilter.Add(gip.Project);
                                        if (!TasksForFilter.Contains(gip.Task)) TasksForFilter.Add(gip.Task);
                                        if (!DataTypesForFilter.Contains(gip.DataType)) DataTypesForFilter.Add(gip.DataType);
                                        if (apars.ContainsKey(gip.Code))
                                        {
                                            var hip = apars[gip.Code];
                                            gip.OldValue = hip.OldValue;
                                            gip.OldTime = hip.OldTime.ToString();
                                            gip.Value = hip.Value;
                                            gip.Time = hip.Time == Different.MinDate ? null : hip.Time.ToString();
                                        }
                                        GridInputParams.Add(gip);
                                    }
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddError("Ошибка загрузки абсолютных значений", ex);
                        MessageBox.Show("Ошибка загрузки абсолютных значений", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        //Очистка списка параметров и выпадающих списков
        private void ClearLists()
        {
            GridInputParams.Clear();
            ProjectsForFilter.Clear();
            ProjectsForFilter.Add("<Все>");
            TasksForFilter.Clear();
            TasksForFilter.Add("<Все>");
            DataTypesForFilter.Clear();
            DataTypesForFilter.Add("<Все>");
        }

        //Сохранение абсолютных значений после редактирования
        public void SaveAbsolute()
        {
            try
            {
                var list = new List<HandInputParam>();
                foreach (var gip in GridInputParams)
                    if (gip.ValueCorrect && gip.TimeCorrect)
                        list.Add(new HandInputParam(gip.Project, gip.Code, gip.Value, DateTime.Parse(gip.Time)));
                Archive.WriteAbsoluteEdit(list);
            }
            catch (Exception ex)
            {
                AddError("Ошибка сохранения абсолютных значений", ex);
                Different.MessageError("Ошибка сохранения абсолютных значений");
            }
            GridInputParams.Clear();
        }

        //Загрузка списка ошибок
        public void LoadErrorsList()
        {
            State = State.ErrorsList;
            GridErrors.Clear();
            if (ErrorsRec != null && ErrorsRec.HasRows())
            {
                ErrorsRec.MoveLast();
                while (!ErrorsRec.BOF)
                {
                    var ge = new GridError(ErrorsRec);
                    GridErrors.Add(ge);
                    ErrorsRec.MovePrevious();
                }
                GridErrorsCount = GridErrors.Count;
            }
            else GridErrorsCount = 0;
        }

        //Сохранение спписка ошибок
        public void CloseErrorsList()
        {
            if (GridErrorsCount != GridErrors.Count)
            {
                ErrorsRec.Dispose();
                HistoryDb.Execute("DELETE * FROM ErrorsList");
                ErrorsRec = new RecDao(HistoryDb, "SELECT * FROM ErrorsList ORDER BY Id");
                foreach (var ge in GridErrors)
                    ge.ToRecordset(ErrorsRec);
                ErrorsRec.MoveLast();
            }
            State = State.Stopped;
        }

        //Запускается при переходе на вкладку настроек
        public void BeginSetup()
        {
            State = State.Setup;
            StartAtom(Atom.Setup);
        }

        //Настройка провайдера
        public void SetupProvider(Provider provider)
        {
            try
            {
                provider.Inf = General.RunProvider(provider.Code, provider.Name, provider.Inf, this).Setup();
                AddEvent("Настройка провайдера", "Имя:" + provider.Name + ", Код:" + provider.Code + ", " + provider.Inf);
            }
            catch(Exception ex)
            {
                ex.MessageError("Не удалось настроить провайдер. Скорее всего указан недопустимый код провайдера.");
            }
        }

        //Добавить к потоку новый файл проекта
        public void AddProject()
        {
            var op = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = "accbd",
                Multiselect = false,
                Title = "Файл проекта",
                Filter = "Файлы MS Access (.accdb) | *.accdb"
            };
            op.ShowDialog();
            if (op.FileName.IsEmpty()) return;
            using (Start())
            {
                var proj = new Project(op.FileName, this);
                if (Command.IsError)
                {
                    if (Command.IsError && Projects.ContainsKey(proj.Code))
                        AddError("Проект уже есть в потоке", null, "Код=" + proj.Code);
                    Different.MessageError(Command.ErrorMessage());
                }
                else
                {
                    ProjectsList.Add(proj);
                    Projects.Add(proj.Code, proj);
                    AddEvent("Добавлен проект", proj.Code + ", " + proj.File);
                    proj.ReadProviders();
                    if (Command.IsError)
                        Different.MessageError(Command.ErrorMessage());
                }
                MakeProviders();
                MakeProjectString();   
            }
        }

        //Удаление проекта project из потока, isSetup = false - удаление при настройке другого потока
        public void DeleteProject(Project project)
        {
            AddEvent("Удаление проекта", project.Code);
            try
            {
                foreach (var p in project.Providers.Values)
                {
                    p.Projects.Remove(project.Code);
                    if (p.Projects.Count == 0)
                        ProvidersDic.Remove(p.Name);
                }
                Projects.Remove(project.Code);
                ProjectsList.Remove(project);    
            }
            catch(Exception ex)
            {
                AddError("Ошибка при удалении проекта", ex);
            }
            MakeProviders();
            MakeProjectString();
        }

        //Добавление в поток уже созданного проекта, isSetup = false - добавление при настройке другого потока
        public void PasteProject(Project project)
        {
            AddEvent("Добавление проекта", "Проект " + project.Code + " из потока " + project.ThreadCalc.Id);
            try
            {
                project.ThreadCalc = this;
                Projects.Add(project.Code, project);
                ProjectsList.Add(project);
                var list = project.Providers.Values.Select(p => p.Copy(this)).ToList();
                project.Providers.Clear();
                foreach (var p in list)
                {
                    if (ProvidersDic.ContainsKey(p.Name))
                        project.Providers.Add(p.Name, ProvidersDic[p.Name]); 
                    else
                    {
                        project.Providers.Add(p.Name, p);
                        ProvidersDic.Add(p.Name, p);
                    }
                    ProvidersDic[p.Name].Projects.Add(project.Code);
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при добавлении проекта", ex);
            }
            MakeProviders();
            MakeProjectString();
        }
        
        //Экспорт проекта в другой поток
        public void ExportProject(ThreadController thread, Project project)
        {
            DeleteProject(project);
            if (thread.State == State.Setup)
                thread.PasteProject(project);
            else
                using (thread.StartAtom(Atom.PasteProject, 0, 100, "Проект " + project.Code, project.Code))
                {
                    thread.PasteProject(project);
                    thread.SaveSetup();    
                }
        }

        //Загрузка настроек и проверка соединения с провайдерами
        public void CheckSetup()
        {
            Start(true);
            if (StartAtom(Atom.ReadSetup, ReadSetup))
                StartAtom(Atom.CheckProviders, CheckProviders);
            if (PeriodLength < 0 || SourcesLate < 0 || TimeAfterError < 0)
                AddError("Длина периода, возможная задержка источника и ожидание после ошибке не должны быть меньше 0");
            string mess = Finish().ErrorMessage();
            if (!mess.IsEmpty()) Different.MessageError(mess);
        }

        //Сохранение данных после настройки
        public void SaveSetup()
        {
            try
            {
                AddEvent("Сохранение настроек");
                using (var db = new DaoDb(General.ControllerFile))
                {
                    using (var rec = new RecDao(db, "SELECT * FROM Threads WHERE ThreadId=" + Id))
                    {
                        rec.Put("Comment", Comment);
                        rec.Put("IsPeriodic", IsPeriodic);
                        rec.Put("PeriodLength", PeriodLength);
                        rec.Put("SourcesLate", SourcesLate);
                        rec.Put("RushWaitingTime", RushWaitingTime);
                        rec.Put("TimeAfterError", TimeAfterError);
                        rec.Put("TimeChange", DateTime.Now.ToString());    
                        rec.Put("IsImit", IsImit);
                        rec.Put("ImitMode", ImitModeStr);
                    }

                    db.Execute("DELETE * FROM Projects WHERE ThreadId=" + Id);
                    using (var rec = new RecDao(db, "Projects"))
                        foreach (var pr in Projects.Values)
                            pr.ToRecordset(rec, true);
                    
                    db.Execute("DELETE * FROM Providers WHERE ThreadId=" + Id);
                    using (var rec = new RecDao(db, "Providers"))
                        foreach (var p in ProvidersDic.Values)
                            p.ToRecordset(rec, true);    
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при сохранении настроек", ex);
                Different.MessageError("Ошибка при сохранении настроек");
            }   
            Finish();//Setup
            StartAtom(Atom.ReadSetup, ReadSetup);
            State = State.Stopped;
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //ModelView
        //////////////////////////////////////////////////////////////////////////////////////////
        
        //Список проектов для выпадающего списка
        private readonly HashSet<string> _projectsForFilter = new HashSet<string>();
        public HashSet<string> ProjectsForFilter { get { return _projectsForFilter; } }
        //Список задач для выпадающего списка
        private readonly HashSet<string> _tasksForFilter = new HashSet<string>();
        public HashSet<string> TasksForFilter { get { return _tasksForFilter; } }
        //Список типов данных для выпадающего списка
        private readonly HashSet<string> _dataTypesForFilter = new HashSet<string>();
        public HashSet<string> DataTypesForFilter { get { return _dataTypesForFilter; } }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Логирование
        //////////////////////////////////////////////////////////////////////////////////////////

        //Добавляет Ошибку в сообщения
        protected override void MessageError(ErrorCommand er)
        {
            _lastErrorText = er.Text;
            _lastErrorTime = PeriodBegin;
            _lastErrorPos = 1;
            UpdateTablo();
        }

        //Отображает процент на индикаторе процесса
        protected override void ViewProcent(double procent)
        {
            IndicatorProcent = procent;
            IndicatorText = IndicatorText ?? Convert.ToInt32(procent) + " %";
        }

        //Запись ошибки в SQL-базу
        protected override void LogErrorsSpecial(ErrorCommand er, DateTime time, CommandLog command)
        {
            App.MonitorHistory.AddError(this, er, time, command);
        }
    }
}
