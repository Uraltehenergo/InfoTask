using System;

namespace Calculation
{
    public class PeriodicCalc : BaseCalc
    {
        public PeriodicCalc(ThreadCalc thread)
        {
            ThreadCalc = thread;
            ReadFunctions();
        }

        //Длина периода расчета в секундах
        private double _periodLength;
        public override double PeriodLength { get { return _periodLength; } }
        //Время окончания периодического расчета
        public DateTime? StopTime { get; private set; }

        //Начало и конец текущего периода расчета
        private DateTime _periodBegin;
        public override DateTime PeriodBegin { get { return _periodBegin; } }
        public override DateTime PeriodEnd
        {
            get { return _periodBegin.AddSeconds(_periodLength); }
        }

        //Инициализация при запуске расчета
        public void Run(DateTime startTime, double periodLength, DateTime? stopTime)
        {
            _periodLength = periodLength;
            _periodBegin = PeriodBegin;
            StopTime = stopTime;
        }

    }
}