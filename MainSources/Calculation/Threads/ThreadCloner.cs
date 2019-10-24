using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Поток клонирования
    public class ThreadCloner : Logger
    {
        //commCode - код коммуникатора
        public ThreadCloner(string commCode, bool showIndicator)
        {
            General.Initialize();
            _commCode = commCode;
            ThreadName = commCode;
            _showIndicator = showIndicator;
            string itd = General.InfoTaskDir;
            OpenHistory(itd + @"\Controller\History\History" + commCode + ".accdb", itd + @"\General\HistoryTemplate.accdb");
            _isClosed = false;
        }

        //Код коммуникатора 
        private readonly string _commCode;
        //Показывать индикатор
        private readonly bool _showIndicator;

        //Форма с индикатором
        private IndicatorForm _form;
        //Состояние данных потока, true - если не загружен или закрыт
        private bool _isClosed;

        //Сообщение об ошибке
        public string ErrMessage { get; private set; }

        //Комманда, закрытие потока
        public string Close()
        {
            if (_isClosed) return "Поток уже был закрыт";
            using (Start(true))
            {
                try
                {
                    if (_source != null) //ban 22.01.2019
                        _source.Dispose();
                    UpdateHistory(false);
                    CloseHistory();
                    _isClosed = true;
                }
                catch (Exception ex)
                {
                    AddError("Ошибка при закрытии файла истории", ex);
                }
                return ErrMessage = Command.ErrorMessage();
            }
        }

        //Настройки создания клона, индивидуальные для каждого источника
        private string _cloneInf;
        //Ссылка на источник
        private ISource _source;
        //Начало и конец периода клона
        private DateTime _timeBegin;
        private DateTime _timeEnd;
        //Диапазон источника
        public DateTime SourceBegin { get; private set; }
        public DateTime SourceEnd { get; private set; }

        //Создание источника и получение диапазона, на входе код, имя и настройки провайдера
        public void RunSource(string code, string name, string inf)
        {
            Start(true);
            try
            {
                if (_source == null || _source.Code != code || _source.Name != name || _source.Inf != inf)
                    _source = (ISource) General.RunProvider(code, name, inf, this);
                
                var t = _source.GetTime();
                SourceBegin = t.Begin;
                SourceEnd = t.End;
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с источником", ex);
            }
            ErrMessage = Finish().ErrorMessage();
        }

        //Чтение списка сигналов из клона
        private void PrepareForClone(string cloneFile)
        {
            try
            {
                AddEvent("Чтение свойств клона");
                if (!DaoDb.Check(cloneFile, new[] { "Objects", "Signals", "MomentsValues" }))
                    AddError("Недопустимый файл клона");
                else using (var db = new DaoDb(cloneFile))
                {
                    using (var sys = new SysTabl(db))
                    {
                        if (_form != null)
                            _form.CalcName.Text = sys.Value("CloneDescription");
                        _cloneInf = sys.Value("CloneInf");
                        RunSource(sys.SubValue("Source", "ProviderCode"), sys.SubValue("Source", "ProviderName"), sys.SubValue("Source", "ProviderInf"));
                    }

                    AddEvent("Чтение списка сигналов клона");
                    _source.ClearSignals();
                    int n = 0;
                    using (var rec = new RecDao(db, "SELECT SignalId, FullCode, DataType, Inf FROM Signals WHERE ConstValue Is Null"))
                        while (rec.Read())
                        {
                            _source.AddSignal(rec.GetString("Inf"), rec.GetString("FullCode"), rec.GetString("DataType").ToDataType(), rec.GetInt("SignalId"));
                            n++;
                        }
                    AddEvent("Сигналы клона прочитаны", n + " сигналов");

                    //db.Execute("DELETE * FROM MomentsValues");
                    //db.Execute("DELETE * FROM Intervals");
                    using (var rec = new RecDao(db, "Intervals"))
                    {
                        rec.AddNew();
                        rec.Put("TimeBegin", _timeBegin);
                        rec.Put("TimeEnd", _timeEnd);
                    }
                }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при подготовке клона", ex);
            }
        }

        //Показать форму индикатора
        private void OpenIndicatorForm()
        {
            if (_showIndicator)
            {
                Application.EnableVisualStyles();
                _form = new IndicatorForm();
                var screen = Screen.PrimaryScreen;
                _form.Location = new Point(screen.WorkingArea.Width - _form.Width - 1, screen.WorkingArea.Height - _form.Height - 2);
                _form.Show();
                _form.Text = _commCode;
                _form.PeriodBegin.Text = _timeBegin.ToString();
                _form.PeriodEnd.Text = _timeEnd.ToString();
                _form.CalcName.Text = "";
                _form.Refresh();
                Thread.Sleep(100);
            }
        }

        //Скрыть форму индикатора
        private void CloseIndicatorForm()
        {
            if (_form != null)
            {
                try { _form.Hide(); }
                catch { }
                _form.Dispose();
                _form = null;
            }
        }

        //Запуск комманды для лога и индикатора
        private CommandLog StartAtom(double start, double finish, string name, string pars = "")
        {
            _form.CurrentOperation.Text = name;
            return StartLog(start, finish, name, pars, _source == null ? "" : _source.Context, _source == null ? "" : _source.Code, true);
        }

        //Создание клона от timeBegin до timeEnd, cloneFile - файл клона
        public string MakeClone(string cloneFile, DateTime timeBegin, DateTime timeEnd)
        {
            if (_isClosed) return "Поток уже был закрыт";
            if (timeBegin > timeEnd) return "Начало интервала расчета не должно быть позже его окончания";
            Start(true);
            try
            {
                _timeBegin = timeBegin;
                _timeEnd = timeEnd;
                OpenIndicatorForm();
                using (StartAtom(0, 10, "Подготовка клона"))
                    PrepareForClone(cloneFile);
                if (!Command.IsError)
                    using (StartAtom(10, 100, "Получение клона архива", timeBegin + " - " + timeEnd))
                    {
                        _source.Prepare();
                        _source.MakeClone(timeBegin, timeEnd, cloneFile, _cloneInf);
                    }
            }
            catch (Exception ex)
            {
                AddError("Ошибка при формировании клона", ex);
            }
            finally
            {
                CloseIndicatorForm();
            }
            return ErrMessage = Finish().ErrorMessage();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        //Логирование
        //////////////////////////////////////////////////////////////////////////////////////////
        protected override void FinishLogCommand() { }

        protected override void FinishSubLogCommand() { }

        protected override void FinishProgressCommand() { }

        protected override void MessageError(ErrorCommand er) { }

        protected override void ViewProcent(double procent)
        {
            if (_form != null)
            {
                _form.Procent.Value = Convert.ToInt32(procent);
                _form.Refresh();
            }
        }
    }
}
