using System;

namespace Calculation
{
    //Класс разового расчета
    public class SingleCalc : BaseCalc
    {
        public SingleCalc(ThreadCalc thread)
        {
            ThreadCalc = thread;
            ReadFunctions();
        }

        //Начало периода расчета
        private DateTime _periodBegin;
        public override DateTime PeriodBegin { get { return _periodBegin; } }
        //Конец периода расчета
        private DateTime _periodEnd;
        public override DateTime PeriodEnd { get { return _periodEnd; } }
        //Длительность периода расчета в секундах
        public override double PeriodLength
        {
            get { return PeriodEnd.Subtract(PeriodBegin).TotalSeconds; }
        }

        //todo: Убрать
        //Имя расчета
        public string CalcName { get; private set; }

        //Инициализация при запуске расчета
        public void Run(DateTime periodBegin, DateTime periodEnd, string calcName)
        {
            _periodBegin = periodBegin;
            _periodEnd = periodEnd;
            CalcName = calcName;
        }
    }
}