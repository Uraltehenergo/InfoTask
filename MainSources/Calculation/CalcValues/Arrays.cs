using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Словарь расчетных значений со строковыми ключами
    public class MomDicString : CalcVal
    {
        public MomDicString(DataType dataType, ErrMom err = null) : base(err)
        {
            _dataType = dataType;
        }

        //Словарь с данными
        private readonly DicS<ISingleVal> _dic = new DicS<ISingleVal>();
        public DicS<ISingleVal> Dic { get { return _dic; } }

        public override ValueType ValueType
        {
            get { return ValueType.DicString;}
        }

        private readonly DataType _dataType;
        public override DataType DataType { get { return _dataType; } }
    }

    //---------------------------------------------------------------------------------------------------
    //Словарь расчетных значений со числовыми ключами
    public class MomDicInt : CalcVal
    {
        public MomDicInt(DataType dataType, ErrMom err = null) : base(err)
        {
            _dataType = dataType;
        }

        //Словарь с данными
        private readonly DicI<ISingleVal> _dic = new DicI<ISingleVal>();
        public DicI<ISingleVal> Dic { get { return _dic; } }

        public override ValueType ValueType
        {
            get { return ValueType.DicInt; }
        }

        private readonly DataType _dataType;
        public override DataType DataType { get { return _dataType; } }
    }

    //---------------------------------------------------------------------------------------------------
    //Список расчетных значений
    public class MomDicList : CalcVal
    {
        public MomDicList(DataType dataType, ErrMom err = null) : base(err)
        {
            _dataType = dataType;
        }

        //Словарь с данными
        private readonly List<ISingleVal> _list = new List<ISingleVal>();
        public List<ISingleVal> List { get { return _list; } }

        public override ValueType ValueType
        {
            get { return ValueType.List; }
        }

        private readonly DataType _dataType;
        public override DataType DataType { get { return _dataType; } }
    }
}