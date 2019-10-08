using System;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ControllerClient
{
    //Чтение из источника отдельных значений по отдельным сигналам
    //Класс, вызываемый через Com
    //Все функции возвращают ошибку или ""
    public class SourceReader : Logger
    {
        public SourceReader()
        {
            General.Initialize();
        }

        //Ссылка на источник
        private ISource _source;
        //Состояние данных потока, true - если не загружен или закрыт
        private bool _isClosed = true;
        
        //Диапазон источника
        public DateTime SourceBegin { get; private set; }
        public DateTime SourceEnd { get; private set; }
        
        //Открытие и получение диапазона источника
        public string RunSource(string code, string name, string inf)
        {
            Start(true);
            try
            {
                if (_source == null || _source.Code != code || _source.Name != name || _source.Inf != inf)
                    _source = (ISource)General.RunProvider(code, name, inf, this);
                _isClosed = false;
                var t = _source.GetTime();
                SourceBegin = t.Begin;
                SourceEnd = t.End;
            }
            catch (Exception ex)
            {
                AddError("Ошибка соединения с источником", ex);
            }
            return Finish().ErrorMessage();
        }

        //Закрытие источника
        public string Close()
        {
            try
            {
                return ExternalFun(_source.Dispose);
            }
            finally
            {
                _isClosed = true;
                GC.Collect();
            }
        }

        //Очистка списка сигналов
        public string ClearSignals()
        {
            return ExternalFun(_source.ClearSignals);
        }

        //Добавить сигнал для чтения
        public string AddSignal(string code,  //Полный код сигнала
                                            string dataType,  //Тип данных
                                            string inf) //Информация для источника
        {
            return ExternalFun(() => _source.AddSignal(inf, code, dataType.ToDataType()));
        }

        //Время, за которое было произведено последнее считывание
        private DateTime _lastTime;

        //Прочитать значения из источника на указанное время
        //Если предыдущее считывание было недавно, значения считываются за период от предыдущего до заданного времени
        //Иначе считывется срез за заданное время
        public string ReadValues(DateTime time)
        {
            return ExternalFun(() =>
                {
                    _source.GetValues(time.Subtract(_lastTime).TotalMinutes < 1 ? _lastTime : time, time);
                    //_source.GetValues(time, time);
                    _lastTime = time;
                });
        }

        //Вернуть значение указанного сигнала, для разных типов сигналов разные функции
        public bool BooleanValue(string code)
        {
            return ReturnMoment(code).Boolean;
        }
        public double IntegerValue(string code)
        {
            return ReturnMoment(code).Integer;
        }
        public double RealValue(string code)
        {
            return ReturnMoment(code).Real;
        }
        public string StringValue(string code)
        {
            return ReturnMoment(code).String;
        }
        public int NdValue(string code)
        {
            return ReturnMoment(code).Nd;
        }
        
        public bool HasValue(string code)
        {
            var v = _source.Signals[code].Value;
            return v != null && v.Moments != null && v.Moments.Count != 0;
        }

        private Moment ReturnMoment(string code)
        {
            var v = _source.Signals[code].Value;
            return v == null ? new Moment(DataType.Value, null, 2) : v.LastMoment;
        }

        //Вызов функций
        private string ExternalFun(Action action)
        {
            if (_isClosed) return "Поток уже был закрыт";
            using (var c = Start())
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    AddError("Ошибка", ex);
                }
                return c.ErrorMessage();
            }
        }

        //Логирование
        protected override void FinishLogCommand() { }
        protected override void FinishSubLogCommand() { }
        protected override void FinishProgressCommand() { }
        protected override void MessageError(ErrorCommand er) { }
        protected override void ViewProcent(double procent) { }
    }
}