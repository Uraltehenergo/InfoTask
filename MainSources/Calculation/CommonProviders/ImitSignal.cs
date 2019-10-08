using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    internal class ImitSignal : ProviderSignal
    {
        //Мгновенные значения сигнала из таблицы
        private readonly List<Moment> _moments = new List<Moment>();

        //Подгрузка свойств имитируемого юнита из таблиц
        public ImitSignal(IRecordRead rec, Imitator imitator) : base("", rec.GetString("FullCode"), DataType.Variant, imitator)
        {
            Imitator = imitator;
            ImitId = rec.GetInt("ImitId");
            ImitType = rec.GetInt("ImitType");
            _intervalLength = rec.GetInt("IntervalLength");
            if (_intervalLength < 0) _intervalLength = 0;
            _intervalSpan = new TimeSpan(0, 0, _intervalLength);
            _signalValue = rec.GetDouble("SignalValue");
            _signalNd = rec.GetInt("SignalNd");
        }

        //Присвоение сигналу типа данных
        public void SetDataType(DataType type)
        {
            DataType = type;
        }

        //Ссылка на провайдер
        public Imitator Imitator { get; private set; }
        //Id в таблице SignalsBehavior файла имитационных данных
        public int ImitId { get; private set; }
        //Длина интервала сигнала
        private readonly int _intervalLength;
        private readonly TimeSpan _intervalSpan;
        //Тип формирования значений (1-константа, 2-последовательность значений)
        public int ImitType { get; private set; }
        //True, если константа
        public bool IsConstant { get { return ImitType == 1 || _intervalLength == 0; } }
        //Значение константы
        private readonly double _signalValue;
        //Недостоверность констанды
        private readonly int _signalNd;

        //Чтение мгновенных значений из таблицы
        public void ReadMoments(IRecordRead rec)
        {
            while (!rec.EOF && rec.GetInt("ImitId") == ImitId)
            {
                if (!IsConstant)
                {
                    var mv = new Moment(DataType, rec.GetDouble("Value"), rec.GetTime("Time"), rec.GetInt("Nd"));
                    var last = _moments.Count == 0 ? null : _moments[_moments.Count - 1];
                    double rtime = rec.GetDouble("RelativeTime");
                    if ((last == null || last.Time < mv.Time) && rtime >= 0 && rtime < _intervalLength)
                        _moments.Add(mv);
                }
                rec.Read();    
            }
        }
        
        //Создать выходной список значений
        public void MakeMoments(DateTime begin, DateTime end)
        {
            if (IsConstant) Value = new SingleValue(new Moment(DataType, _signalValue, begin, _signalNd));
            else
            {
                DateTime zero = begin;
                if (Imitator.ImitMode == ImitMode.FromDay) zero = begin.Date;
                if (Imitator.ImitMode == ImitMode.FromHour) zero = begin.Date.AddHours(begin.Subtract(begin.Date).Hours);
                while (zero.Add(_intervalSpan) <= begin)
                    zero = zero.Add(_intervalSpan);
                var mlist = new List<Moment>();
                Value = new SingleValue(mlist);
                int n = _moments.Count;
                if (n == 0) return;
                TimeSpan span = zero.Subtract(Imitator.ZeroTime);
                int i = 0;
                while (i < n && _moments[i].Time.Add(span) <= begin) i++;
                if (i > 0) mlist.Add(_moments[i-1].Clone(begin));
                while (i < n && _moments[i].Time.Add(span) < end)
                {
                    mlist.Add(_moments[i].Clone(_moments[i].Time.Add(span)));
                    if (++i == n)
                    {
                        i = 0;
                        span = span.Add(_intervalSpan);
                    }
                }
            }
        }
    }
}
