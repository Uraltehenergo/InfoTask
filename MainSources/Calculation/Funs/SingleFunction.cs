using System;
using BaseLibrary;
using CommonTypes;
using ValueType = CommonTypes.ValueType;

namespace Calculation
{
    //Функция над ISingleValue
    public abstract class SingleBaseFunction : GeneralFunction
    {
        //Вычисление значения функции
        public override IVal Calculate(IVal[] par)
        {
            var spar = new ISingleVal[par.Length];
            var ctype = ValueType.Moments;
            int firstInd = -1;
            for (int i = 0; i < par.Length; i++)
            {
                if (par[i] is ISingleVal) 
                    spar[i] = (ISingleVal) par[i];
                else if (firstInd == -1)
                    firstInd = i;
                switch (par[i].ValueType)
                {
                    case ValueType.DicInt:
                        ctype = ctype == ValueType.DicString || ctype == ValueType.List ? ValueType.Void : ValueType.DicInt;
                        break;
                    case ValueType.DicString:
                        ctype = ctype == ValueType.DicInt || ctype == ValueType.List ? ValueType.Void : ValueType.DicString;
                        break;
                    case ValueType.List:
                        ctype = ctype == ValueType.DicString || ctype == ValueType.DicInt ? ValueType.Void : ValueType.List;
                        break;
                }
            }
            
            switch (ctype)
            {
                case ValueType.Moments: //Без массивов
                    return CalculateSingle(spar);

                case ValueType.List:
                    var resList = new MomDicList(DataType);
                    for (int k = 0; k < ((MomDicList) par[firstInd]).List.Count; k++)
                    {
                        bool hasInd = true;
                        for(int i = 0; i < par.Length; i++)
                        {
                            var dicList = par[i] as MomDicList;
                            if (dicList != null)
                            {
                                if (dicList.List.Count > k)
                                    spar[i] = dicList.List[k];
                                else hasInd = false;
                            }
                        }
                        if (hasInd)
                            resList.List.Add(CalculateSingle(spar));
                    }
                    return resList;

                case ValueType.DicInt:
                    var resDicInt = new MomDicInt(DataType);
                    foreach (var k in ((MomDicInt)par[firstInd]).Dic.Keys)
                    {
                        bool hasInd = true;
                        for (int i = 0; i < par.Length; i++)
                        {
                            var dicInt = par[i] as MomDicInt;
                            if (dicInt != null)
                            {
                                if (dicInt.Dic.ContainsKey(k))
                                    spar[i] = dicInt.Dic[k];
                                else hasInd = false;
                            }
                        }
                        if (hasInd)
                            resDicInt.Dic.Add(k, CalculateSingle(spar));
                    }
                    return resDicInt;

                case ValueType.DicString:
                    var resDicString = new MomDicString(DataType);
                    foreach (var k in ((MomDicString)par[firstInd]).Dic.Keys)
                    {
                        bool hasInd = true;
                        for (int i = 0; i < par.Length; i++)
                        {
                            var dicString = par[i] as MomDicString;
                            if (dicString != null)
                            {
                                if (dicString.Dic.ContainsKey(k))
                                    spar[i] = dicString.Dic[k];
                                else hasInd = false;
                            }
                        }
                        if (hasInd)
                            resDicString.Dic.Add(k, CalculateSingle(spar));
                    }
                    return resDicString;
            }
            return new MomList(DataType.Value);
        }

        //Отдельные вычисления для каждого индекса массива
        public abstract ISingleVal CalculateSingle(ISingleVal[] par);
    }

    //---------------------------------------------------------------------------------------------------------
    //Функция над списками мгновенных значений
    public abstract class SingleFunction : SingleBaseFunction
    {
        protected SingleFunction() {}

        protected SingleFunction(SingleDelegate deleg)
        {
            SingleCalculate = deleg;
        }

        //Делегат для вычисления и его экземпляр
        public delegate ISingleVal SingleDelegate(ISingleVal[] par);
        protected SingleDelegate SingleCalculate { get; private set; }

        //Инициализирует конкретную функцию расчета
        protected override void CreateDelegate(string funName)
        {
            SingleCalculate = (SingleDelegate)Delegate.CreateDelegate(typeof(SingleDelegate), this, funName);
        }

        public override ISingleVal CalculateSingle(ISingleVal[] par)
        {
            return SingleCalculate(par);
        }
    }
}