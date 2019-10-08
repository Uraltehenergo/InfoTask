using System.Collections.Generic;
using BaseLibrary;

namespace Tablik
{
    //Параметр функции
    internal class FunParam
    {
        public FunParam(CalcType calcType, string d = null)
        {
            CalcType = calcType;
            Default = d;
        }

        //Тип данных, встроенный тип или просто указание, что расчетный параметр или переменнная
        public CalcType CalcType { get; private set; }
        //Значение по умолчанию или null
        public string Default { get; private set; }
        //True, если участвует в формировании типа результата
        public bool FormResult { get; set; }
    }

    //---------------------------------------------------------------------

    //Одна перегрузка функции
    internal class FunOverload
    {
        //Параметры
        private readonly List<FunParam> _inputs = new List<FunParam>();
        public List<FunParam> Inputs { get { return _inputs; } }
        //Дополнительные параметры
        private readonly List<FunParam> _inputsMore = new List<FunParam>();
        public List<FunParam> InputsMore { get { return _inputsMore; } }
        //Тип результата формируется из типов параметрв
        public bool IsCombined { get; private set; }
        //Тип результата (если не Combined)
        public CalcType Result { get; private set; }

        //Владелец 
        public FunClass Owner { get; private set; }
        //Номер в списке перегрузок
        public int Number { get; private set; }

        private CalcType TypeFromString(string s)
        {
            if (s == "void") return new CalcType(ClassType.Void);
            if (s == "var") return new CalcType(ClassType.Var);
            return new CalcType(s.ToDataType());
        }

        //Конструктор, на входе рекордсет с таблицей FunctionsOverloads
        public FunOverload(IRecordRead reco, FunClass funClass)
        {
            Owner = funClass;
            Number = reco.GetInt("RunNumber");
            IsCombined = reco.GetBool("IsCombined");
            for (int i = 1; i <= 9; ++i)
            {
                var t = reco.GetString("Operand" + i);
                if (t.IsEmpty()) break;
                Inputs.Add(new FunParam(TypeFromString(t), reco.GetString("Default" + i)));
            }
            for (int i = 1; i <= 2; ++i)
            {
                var t = reco.GetString("More" + i);
                if (t.IsEmpty()) break;
                InputsMore.Add(new FunParam(TypeFromString(t)));
            }

            var s = reco.GetString("Result");
            if (!IsCombined) Result = TypeFromString(s);
            else
            {
                var p = s.Split('+');
                foreach (string c in p)
                {
                    if (c == "M1") InputsMore[0].FormResult = true;
                    else if (c == "M2") InputsMore[1].FormResult = true;
                    else Inputs[int.Parse(c)-1].FormResult = true;
                }
            }
        }
    }

    //---------------------------------------------------------------------
    //Встроенная функция
    internal class FunClass
    {
        //Код для записи в польскую запись
        public string Name { get; private set; }
        //Английский синоним
        public string Synonym { get; private set; }
        //Код для записи в польскую запись, маленькими буквами
        public string Code { get; private set; }
        //Тип (scalar, list, array, operator)
        public string Type { get; private set; }
        //Перегрузки
        private readonly List<FunOverload> _overloads = new List<FunOverload>();
        public List<FunOverload> Overloads { get { return _overloads; } }

        //Конструктор, на входе рекордсет с таблицей Functions
        public FunClass(IRecordRead rec)
        {
            Name = rec.GetString("Name");
            Synonym = rec.GetString("Synonym");
            Code = rec.GetString("Code") ?? (Synonym ?? Name).ToLower();
            Type = rec.GetString("CodeType");
        }
    }
}

