namespace BaseLibrary
{
    //Комманда для отображения индикатора
    public class CommandProgress : Command
    {
        internal CommandProgress(Logger logger, Command parent, string context) 
            : base(logger, parent, context)
        {
        }

        //Величина
        private double _procent;
        public override double Procent
        {
            get { return _procent; }
            set
            {
                _procent = value;
                Logger.ViewProcent(value);
            }
        }

        //Завершение команды
        protected override void FinishCommand(string results)
        {
            Logger.FinishProgressCommand();
        }
    }
}