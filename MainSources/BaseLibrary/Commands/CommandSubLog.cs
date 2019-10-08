using System;

namespace BaseLibrary
{
    //Комманда для записи в SubHistory
    public class CommandSubLog : CommandLogBase
    {
        internal CommandSubLog(Logger logger, Command parent, double start, double finish, string name, DateTime periodBegin, DateTime periodEnd, string mode, string context)
            : base(logger, parent, start, finish, name, context)
        {
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            _mode = mode;
        }

        internal CommandSubLog(Logger logger, Command parent, string name, DateTime periodBegin, DateTime periodEnd, string mode, string context)
            : base(logger, parent, name, context)
        {
            PeriodBegin = periodBegin;
            PeriodEnd = periodEnd;
            _mode = mode;
        }

        //Начло, конец и режим текущего расчета
        public DateTime PeriodBegin { get; private set; }
        public DateTime PeriodEnd { get; private set; }
        private readonly string _mode;

        //Добавление в лог записи о команде
        protected override void BeginLog()
        {
            var hist = Logger.SubHistory;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.AddNew();
                    hist.Put("Command", Name);
                    hist.Put("Time", StartTime);
                    hist.Put("Status", Status);
                    hist.Put("PeriodBegin", PeriodBegin);
                    hist.Put("PeriodEnd", PeriodEnd);
                    hist.Put("Mode", _mode);
                    HistoryId = hist.GetInt("SubHistoryId");
                });
        }

        //Завершение команды
        protected override void FinishCommand(string results)
        {
            var hist = Logger.SubHistory;
            Logger.RunHistoryOperation(hist, () =>
                {
                    hist.MoveLast();
                    hist.Put("Status", Status);
                    hist.Put("ProcessLength", FromStart);
                });
            Logger.FinishSubLogCommand();
        }
    }
}