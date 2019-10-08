using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Одна перегрузка функции
    public class FunOverload
    {
        //Загрузка из рекордсета перегрузок функций
        public FunOverload(IRecordRead rec)
        {
            Class = rec.GetString("Class");
            Method = rec.GetString("Method");

            string name = rec.GetString("Name");
            string synonym = rec.GetString("Synonym");
            Code = rec.GetString("Code") ?? (synonym ?? name).ToLower();
            for (int i = 1; i < 10 && rec.GetString("Operand" + i) != null; i++)
                Code += rec.GetString("Operand" + i).ToDataType().ToLetter();
        }

        //Код функции с буквами типов данных параметров
        public string Code { get; private set; }
        //Имя класса и имя метода расчета значений
        public string Class { get; private set; }
        public string Method { get; private set; }
    }

    //---------------------------------------------------------------------------------------------------------

    //Базовый класс для всех функций
    public abstract class GeneralFunction
    {
        //Расчетный параметр
        protected CalcParam CalcParam { get; private set; }
        //Поток
        protected ThreadCalc Thread { get { return CalcParam.Project.ThreadCalc; } }
        //Тип данных результата
        protected DataType DataType { get; private set; }

        //Вычисление значения функции
        public abstract IVal Calculate(IVal[] par);

        //Запуск экземпляра конкретной функции
        //funCode - код перегрузки функции, dataType - тип данных возвращаемого значения, calc - параметр, при расчете которого вызывается функция
        public static GeneralFunction Create(string funCode, DataType dataType, CalcParam calc)
        {
            var type = Type.GetType("Calculation." + Funs[funCode].Class);
            var fun = (GeneralFunction)Activator.CreateInstance(type);
            fun.CreateDelegate(Funs[funCode].Method);
            fun.CalcParam = calc;
            fun.DataType = dataType;
            return fun;
        }

        //Инициализирует конкретную функцию расчета
        protected abstract void CreateDelegate(string funName);
        
        //Словарь перегрузок функций, ключи - коду с буквами типов параметров
        private static readonly DicS<FunOverload> _funs = new DicS<FunOverload>();
        protected static DicS<FunOverload> Funs { get { return _funs; } }

        //Загрузка списка функций
        public static void LoadFunctions()
        {
            const string stSql = "SELECT Functions.Class, Functions.Name, Functions.Synonym, Functions.Code, FunctionsOverloads.* " +
                                          "FROM Functions INNER JOIN FunctionsOverloads ON Functions.Id = FunctionsOverloads.FunctionId WHERE Functions.NotLoad =False ORDER BY Functions.Name;";
            using (var rec = new ReaderAdo(General.GeneralDir + "General.accdb", stSql))
                while (rec.Read())
                {
                    var f = new FunOverload(rec);
                    Funs.Add(f.Code, f);
                }
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //Функция над IVal
    public abstract class ValueFunction : GeneralFunction
    {
        //Делегат для вычисления и его экземпляр
        protected delegate IVal ValueDelegate(IVal[] par);
        protected ValueDelegate ValueCalculate { get; private set; }
        
        protected override void CreateDelegate(string funName)
        {
            ValueCalculate = (ValueDelegate)Delegate.CreateDelegate(typeof(ValueDelegate), this, funName);
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //Функции над параметрами и сигналами
    public abstract class ParamSignalFunction : ValueFunction
    {
        //Вычисление значения (предварительно выполняется получение значений переменных)
        public override IVal Calculate(IVal[] par)
        {
            var ppar = new IVal[par.Length];
            for (int i = 0; i < par.Length; i++)
                ppar[i] = par[i].Value;
            return ValueCalculate(ppar);
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //Функция над CalcValue
    public abstract class CalcFunction : ValueFunction
    {
        //Вычисление значения (предварительно выполняется получение расчетных значений)
        public override IVal Calculate(IVal[] par)
        {
            var ppar = new IVal[par.Length];
            for (int i = 0; i < par.Length; i++)
                ppar[i] = par[i].CalcValue;
            return ValueCalculate(ppar);
        }        
    }
}