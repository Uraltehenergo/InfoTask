using System;
using System.Threading;
using BaseLibrary;

namespace Calculation
{
    //Базовый класс для разовых и периодических расчетов
    public abstract class BaseCalc
    {
        //Поток контроллера расчета
        public ThreadCalc ThreadCalc { get; protected set; }
        //Поток запуска расчета 
        protected Thread Task { get; set; }

        //Начало, конец и длина текущего периода обработки
        public abstract DateTime PeriodBegin { get; }
        public abstract DateTime PeriodEnd { get; }
        public abstract double PeriodLength { get; }

        

        //Расчетные функции
        #region
        //Список всех функций
        public DicS<Fun> FunsDic { get; private set; }
        //Класс вычисления значений функций
        public Funs Funs { get; private set; }

        //Чтение списка функций из SprFunctions и SprFunctionsTypes
        protected void ReadFunctions()
        {
            FunsDic = new DicS<Fun>();
            const string stSql = "SELECT Functions.Name, Functions.Synonym, Functions.Code, Functions.CodeType, FunctionsOverloads.* " +
                                          "FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId WHERE Functions.NotLoad =False ORDER BY Functions.Name;";
            using (var rec = new ReaderAdo(General.GeneralDir + "General.accdb", stSql))
                while (rec.Read())
                    new Fun(rec, this);
        }
        #endregion
    }
}