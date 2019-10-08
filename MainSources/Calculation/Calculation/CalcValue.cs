using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    public class CalcValue
    {
        public CalcValue()
        {
            Type = CalcValueType.Void;
        }

        public CalcValue(SingleValue value, CalcUnit signal = null)
        {
            Type = CalcValueType.Single;
            SingleValue = value;
            Signal = signal;
            Error = value.Error;
        }

        public CalcValue(SortedDictionary<int, VarRun> intArray)
        {
            Type = CalcValueType.IntArray;
            IntArray = intArray;
        }

        public CalcValue(SortedDictionary<string, VarRun> stringArray)
        {
            Type = CalcValueType.StringArray;
            StringArray = stringArray;
        }

        public CalcValue(VarRun v)
        {
            Type = CalcValueType.Var;
            VarRun = v;
        }

        //Тип самого значения
        public CalcValueType Type { get; private set; }
        //Значение
        public SingleValue SingleValue { get; set; }
        //Набор мгновенных значений массива с целыми индексами
        public SortedDictionary<int, VarRun> IntArray { get; set; }
        //Набор мгновенных значений массива со строковыми индексами
        public SortedDictionary<string, VarRun> StringArray { get; set; }
        //Ссылка на переменную
        public VarRun VarRun { get; set; }
        //Порождающий расчетный параметр
        public CalcParamRun ParentParam { get; set; }
        //Порождающий сигнал
        public CalcUnit Signal { get; set; }
        //Ошибка вычислений
        public ErrorCalc Error { get; set; }

        //Итоговая ошибка для сохранения в Result
        public string LastError
        {
            get
            {
                if (Error != null) return Error.Text;
                if (SingleValue != null) return SingleValue.LastError;
                return null;
            }
        }

        //Преобразует значение в другой тип данных, если это возможно
        public CalcValue ChangeDataType(DataType dt)
        {
            if (Type != CalcValueType.Single) 
                return this;
            if (SingleValue.DataType == DataType.Value || SingleValue.DataType == DataType.Segments || dt.LessOrEquals(SingleValue.DataType))
                return this;
            return new CalcValue(SingleValue.ChangeDataType(dt));
        }

        //Неглубокое копирование, все свойства копируются как ссылки, parent - приписываемый владелец значения
        public CalcValue LinkClone(CalcParamRun parent = null)
        {
            CalcValue cv = null;
            switch (Type)
            {
                case CalcValueType.Var:
                    cv = new CalcValue(VarRun);
                    break;
                case CalcValueType.Single:
                    cv = new CalcValue(SingleValue);
                    break;
                case CalcValueType.IntArray:
                    cv = new CalcValue(IntArray);
                    break;
                case CalcValueType.StringArray:
                    cv = new CalcValue(StringArray);
                    break;
                case CalcValueType.Void:
                    cv = new CalcValue();
                    break;
            }
            if (cv != null)
            {
                cv.Error = Error;
                cv.ParentParam = parent;
            }
            return cv;
        }

        //Глубокое копирование, если erradr != null, то всем ошибкам приписывается в цепочку новый расчетный параметр
        public CalcValue Clone(string erradr = null)
        {
            var er = Error == null ? null : new ErrorCalc(erradr, Error);
            switch (Type)
            {
                case CalcValueType.Var:
                    return new CalcValue(VarRun);
                case CalcValueType.Single:
                    var sv = new SingleValue{Error = er};
                    if (SingleValue.Type == SingleType.Moment)
                        sv.Moment = SingleValue.Moment.Clone(erradr);
                    if (SingleValue.Type == SingleType.List)
                        sv.Moments = SingleValue.Moments.Select(m => m.Clone(erradr)).ToList();
                    if (SingleValue.Type == SingleType.Segments)
                        sv.Segments = SingleValue.Segments.Select(seg => seg.Clone()).ToList();
                    return new CalcValue(sv){ParentParam = ParentParam};
                case CalcValueType.IntArray:
                    var iarr = new SortedDictionary<int, VarRun>();
                    foreach (var i in IntArray.Keys)
                        iarr.Add(i, new VarRun(IntArray[i].CalcValue.Clone(erradr)));
                    return new CalcValue(iarr){ParentParam = ParentParam};
                case CalcValueType.StringArray:
                    var sarr = new SortedDictionary<string, VarRun>();
                    foreach (var s in StringArray.Keys)
                        sarr.Add(s, new VarRun(StringArray[s].CalcValue.Clone(erradr)));
                    return new CalcValue(sarr){ParentParam = ParentParam};
            }
            return new CalcValue();
        }
    }
}