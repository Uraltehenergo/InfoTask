using System.Collections.Generic;
using System.Linq;
using BaseLibrary;

namespace Tablik
{
    internal class CalcType
    {
        public CalcType(ClassType type)
        {
            ClassType = type;
        }

        public CalcType(DataType dataType, DataType indexType = DataType.Value)
        {
            ClassType = ClassType.Single;
            DataType = dataType;
            _indexType = indexType;
        }

        public CalcType(DataType dataType, string text)
        {
            ClassType = ClassType.Single;
            DataType = dataType;
            Text = text;
        }

        public CalcType(Var v)
        {
            ClassType = ClassType.Var;
            Var = v;
        }

        public CalcType(Signal sig)
        {
            ClassType = ClassType.Single;
            Signal = sig;
            DataType = sig.DataType;
        }

        //Тип объекта
        public ClassType ClassType { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Тип индекса массива: Value - не массив, Integer или String
        private DataType _indexType = DataType.Value;
        public DataType IndexType
        { get { return _indexType; } set { _indexType = value; } }
        //Текстовое значение константы, если определено
        public string Text { get; private set; }

        //Сигнал
        public Signal Signal { get; private set; }
        //Допустимые сигналы с  указанием типов данных, для передачи объекта в параметр-функцию
        private readonly DicS<DataType> _inputSignals = new DicS<DataType>();
        public DicS<DataType> InputSignals
        {
            get { return _inputSignals; }
            set
            {
                _inputSignals.Clear();
                if (value != null)
                    foreach (var sig in value.Dic)
                        _inputSignals.Add(sig.Key, sig.Value);
            }
        }

        //Цепочка порождающих параметров, входом может быть только последний
        private readonly List<CalcParam> _parentParams = new List<CalcParam>();
        public List<CalcParam> ParentParams
        {
            get { return _parentParams; }
            set
            {
                _parentParams.Clear();
                if (value != null)
                    foreach (var cp in value)
                        _parentParams.Add(cp);
            }
        }
        //Цепочка порождающих параметров, без входом может быть только последний
        private readonly List<CalcParam> _arrayParentParams = new List<CalcParam>();
        public List<CalcParam> ArrayParentParams
        {
            get { return _arrayParentParams; }
            set
            {
                _arrayParentParams.Clear();
                if (value != null)
                    foreach (var cp in value)
                        _arrayParentParams.Add(cp);
            }
        }

        //Последний в цепочке порождающих параметров
        public CalcParam ParentParam
        {
            get
            {
                if (ParentParams.Count == 0) return null;
                return ParentParams.Last();    
            }
        }

        //Переменная
        public Var Var { get; private set; }

        //Копия себя 
        public CalcType Clone()
        {
            CalcType ct;
            switch (ClassType)
            {
                case ClassType.Single:
                    ct = new CalcType(DataType, Text) {IndexType = IndexType};
                    break;
                case ClassType.Var:
                    ct = new CalcType(Var);
                    break;
                default:
                    ct = new CalcType(ClassType);
                    break;
            }
            ct.InputSignals = InputSignals;
            ct.ParentParams = ParentParams;
            ct.ArrayParentParams = ArrayParentParams;
            return ct;
        }
        
        //Сравнивает два типа данных без учета ParentParam
        public bool Equals(CalcType type)
        {
            if (ClassType != type.ClassType) return false;
            if (IndexType != type.IndexType) return false;
            if (ClassType == ClassType.Single)
            {
                if (DataType != type.DataType) return false;
                if (Text != null && type.Text == null) return false;
                if (Text == null && type.Text != null) return false;
                bool e = true;
                foreach (var sig in type.InputSignals.Dic)
                    e &= InputSignals.Get(sig.Key, DataType.Error) == sig.Value;
                foreach (var sig in InputSignals.Dic)
                    e &= type.InputSignals.Get(sig.Key, DataType.Error) == sig.Value;
                return e;
            }
            return true;
        }

        //True - если данное значение подходит под заданный CommonType, useParam - использовать при сравнении ParentParam
        public bool LessOrEquals(CalcType type, bool useParam = false)
        {
            if (useParam && type.ParentParam != null)
                foreach (var par in ParentParams)
                    if (par == type.ParentParam) return true;
            if (ClassType == ClassType.Calc)
                return ParentParam.CalcType.LessOrEquals(type);
            if (ClassType == ClassType.Undef) return true;
            if (type.ClassType == ClassType.Undef && ClassType == ClassType.Error) return false;
            if (ClassType == ClassType.Error) return true;
            if (type.ClassType == ClassType.Max) return true;
            if (ClassType != type.ClassType) return false;
            if (IndexType != type.IndexType) return false;
            if (Equals(type)) return true;
            if (ClassType == ClassType.Single)
            {
                if (!DataType.LessOrEquals(type.DataType)) return false;
                if (Text == null && type.Text != null) return false;
                if (type.InputSignals.Count > 0)
                {
                    bool e = true;
                    if (Signal != null)
                        foreach (var sig in type.InputSignals.Dic)
                            e &= Signal.ObjectSignal.Signals.ContainsKey(sig.Key) && Signal.ObjectSignal.Signals[sig.Key].DataType.LessOrEquals(sig.Value);
                    else foreach (var sig in type.InputSignals.Dic)
                        e &= InputSignals[sig.Key].LessOrEquals(sig.Value);
                    return e;
                }
                return true;
            }
            return false;
        }

        //Формирует тип данных к которому сводятся оба типа, useParam - использовать при сравнении ParentParam
        public CalcType Add(CalcType type, bool useParam = false)
        {
            CalcType ct;
            if (LessOrEquals(type)) ct = type.Clone();
            else if (type.LessOrEquals(this)) ct = Clone();
            else if (ClassType == ClassType.Single && type.ClassType == ClassType.Single && IndexType == type.IndexType)
            {
                var dt = DataType.Add(type.DataType);
                if (dt == DataType.Error) return new CalcType(ClassType.Error);
                return new CalcType(dt, IndexType);
            }
            else ct = new CalcType(ClassType.Max);
            ct.ParentParams = !useParam ? null : type.ParentParams;
            ct.ArrayParentParams = !useParam ? null : type.ArrayParentParams;
            if (ClassType != ClassType.Undef)
            {
                ct.Text = null;
                ct.Signal = null;
                ct.InputSignals.Clear();
            }
            return ct;
        }

        //Возвращает тип данных, получаемый из текущего взятием индекса массива типа ind
        public CalcType GetIndex(CalcType ind)
        {
            if (ind.ClassType != ClassType.Single || !ind.DataType.LessOrEquals(IndexType))
                return new CalcType(ClassType.Max);
            if (ind.DataType == DataType.Value) return this;
            if (ClassType == ClassType.Single)
                return new CalcType(DataType) {ArrayParentParams = ParentParams };
            return new CalcType(ClassType.Max);
        }

        //Возвращает тип данных, получаемый из текущего добавление индексированности по типу ind
        public CalcType AddIndex(CalcType ind)
        {
            if (ind.ClassType != ClassType.Single || !ind.DataType.LessOrEquals(DataType.String))
                return new CalcType(ClassType.Max);
            if (ind.DataType == DataType.Value) return this;
            if (IndexType != DataType.Value)
                return new CalcType(ClassType.Max);
            if (ClassType == ClassType.Single)
            {
                var idt = ind.DataType.LessOrEquals(DataType.Integer) ? DataType.Integer : DataType.String;
                return new CalcType(DataType, idt) { ArrayParentParams = ParentParams, ParentParams = null };
            }
            return new CalcType(ClassType.Max);
        }
    }
}