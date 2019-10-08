using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Функция над мгновенными значениями 
    public abstract class MomFunction : SingleBaseFunction
    {
        protected MomFunction() {}

        protected MomFunction(MomDelegate deleg)
        {
            MomCalculate = deleg;
        }

        //Делегат для вычисления и его экземпляр
        //На входе par - массив мгновенных значений, сpar - логический массив, 1 если время вычисляемого значения соответствует моменту из исходного списка
        //Возвращает true, если в результат добавляется новое значение
        public delegate bool MomDelegate(IMom[] par, bool[] cpar, MomEdit res);
        public MomDelegate MomCalculate { get; set; }

        //Инициализирует конкретную функцию расчета
        protected override void CreateDelegate(string funName)
        {
            MomCalculate = (MomDelegate)Delegate.CreateDelegate(typeof(MomDelegate), this, funName);
        }

        //Вычисление значения функции
        public override ISingleVal CalculateSingle(ISingleVal[] par)
        {
            var mpar = new IMom[par.Length];
            var cpar = new bool[par.Length];
            for (int i = 0; i < par.Length; i++) 
                cpar[i] = false;
            
            var lists = new List<MList>();
            bool isMom = true;
            for (int i = 0; i < par.Length; i++)
            {
                var mom = par[i] as Mom;
                if (mom != null) mpar[i] = mom;
                else
                {
                    isMom = false;
                    var moms = ((MomList) par[i]).Moments;
                    if (moms.Count == 0 && this is ScalarFunction)
                        return new MomList(DataType);
                    lists.Add(new MList(moms, i));
                }
            }
            if (isMom)//Одно значение
            {
                var mres = new MomEdit(DataType, MaxError(mpar));
                MomCalculate(mpar, cpar, mres);
                return mres.ToMom();
            }
            //Список значений
            var rlist = new MomList(DataType);
            bool e = true;
            while (e)
            {
                e = false;
                DateTime ctime = Different.MaxDate;
                foreach (var list in lists)
                    if (list.NextTime < ctime) ctime = list.NextTime;
                if (ctime < Different.MaxDate)
                {
                    e = true;
                    for (int i = 0; i < cpar.Length; i++) 
                        cpar[i] = false;
                    foreach (var list in lists)
                    {
                        if (list.NextTime == ctime)
                        {
                            list.Pos++;
                            cpar[list.Num] = true;
                        }
                        mpar[list.Num] = ((MomList)par[list.Num]).Interpolation(CalcParam.Interpolation, list.Pos, ctime);
                    }
                    var temp = new MomEdit(DataType, ctime, MinError(mpar));
                    if (MomCalculate(mpar, cpar, temp))
                        rlist.AddMomClone(temp);
                }
            }
            return rlist;
        } 

        //Вычисляет суммарную ошибку мгновенных значений
        protected ErrMom MaxError(IMom[] par)
        {
            ErrMom err = null;
            foreach (var mom in par)
                err = err.Add(mom.Error);
            return err;
        }
        //Вычисляет минимальную ошибку мгновенных значений
        protected ErrMom MinError(IMom[] par)
        {
            if (par.Length == 0) return null;
            foreach (var mom in par)
                if (mom.Error == null) return null;
            return par[0].Error;
        }

        //Добавляет к значению res мгновенную ошибку с текстом text
        protected void AddError(MomEdit res, string text)
        {
            res.MakeDefaultValue();
            res.AddError(CalcParam.Project.ErrMomFactory.MakeError(CalcParam, text));
        }
    }

    //------------------------------------------------------------------------------------------
    //Вспомогательный класс для хранения одного списка значений
    internal class MList
    {
        public MList(ReadOnlyCollection<Mom> list, int num)
        {
            List = list;
            Num = num;
            Pos = -1;
        }

        //Список значений
        public ReadOnlyCollection<Mom> List { get; private set; }
        //Номер в списке исходных параметров
        public int Num { get; private set; }

        //Текущее обрабатываемый индекс
        public int Pos { get; set; }
        //Время следующего индекса
        public DateTime NextTime
        {
            get 
            { 
                if (Pos + 1 >= List.Count) 
                    return DateTime.MaxValue;
                return List[Pos + 1].Time;
            }
        }
    }

    //------------------------------------------------------------------------------------------
    //Cкалярная функция (вызывается без передачи булевского массива используемых индексов (cpar))
    public abstract class ScalarFunction : MomFunction
    {
        //Делегат для вычисления и его экземпляр
        private delegate void ScalarDelegate(IMom[] par, MomEdit res);
        private ScalarDelegate _scalarCalculate;

        //Инициализирует конкретную функцию расчета
        protected override void CreateDelegate(string funName)
        {
            _scalarCalculate = (ScalarDelegate)Delegate.CreateDelegate(typeof(ScalarDelegate), this, funName);
            MomCalculate = MomFun;
        }

        private bool MomFun(IMom[] par, bool[] cpar, MomEdit res)
        {
            _scalarCalculate(par, res);
            return true;
        }
    }
}