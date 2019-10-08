using System.Collections.Generic;
using BaseLibrary;

namespace Calculation
{
    //Одна таблица проекта
    public class Tabl
    {
        //Номер
        public int Num { get; private set; }
        //Количество полей в таблице и подтаблице
        public int FieldsCount { get; set; }
        public int SubFieldsCount { get; set; }
        //Параметры из подтаблицы, словари по кодам, номерам и Id
        private readonly Dictionary<string, TablParam> _paramsCode;
        public Dictionary<string, TablParam> ParamsCode { get { return _paramsCode; } }
        private readonly Dictionary<int, TablParam> _paramsNum;
        public Dictionary<int, TablParam> ParamsNum { get { return _paramsNum; } }
        private readonly Dictionary<int, TablParam> _paramsId;
        public Dictionary<int, TablParam> ParamsId { get { return _paramsId; } }

        public Tabl(int number)
        {
            Num = number;
            _paramsCode = new Dictionary<string, TablParam>();
            _paramsNum = new Dictionary<int, TablParam>();
            _paramsId = new Dictionary<int, TablParam>();
        }
    }

    //---------------------------------------------------------------------------------------

    //Одна запись из таблицы или подтаблицы проекта (Tabl или SubTabl)
    public class TablParam
    {
        //Код
        public string Code { get; private set; }
        //Номер
        public int Num { get; private set; }
        //Имя
        public string Name { get; private set; }
        //Id в таблице 
        public int Id { get; private set; }
        
        //Значения (Val_0, Val_1) и т.д.
        public string[] Values { get; private set; }
        //Параметры из подтаблицы, словари по кодам, номерам и Id
        private Dictionary<string, TablParam> _paramsCode;
        public Dictionary<string, TablParam> ParamsCode { get { return _paramsCode; }}
        private Dictionary<int, TablParam> _paramsNum;
        public Dictionary<int, TablParam> ParamsNum { get { return _paramsNum; } }

        //Загрузка параметра из рекордсета таблицы
        public void ParamFromRec(Tabl tabl, ReaderAdo rec)
        {
            Code = rec.GetString("Code");
            Num = rec.GetInt("Num");
            Name = rec.GetString("Name");
            Id = rec.GetInt("Id");
            Values = new string[tabl.FieldsCount];
            for (int i = 0; i < tabl.FieldsCount; i++)
                Values[i] = rec.GetString("Val_" + i) ?? "";
            if (Code != null && !tabl.ParamsCode.ContainsKey(Code))
                tabl.ParamsCode.Add(Code, this);
            if (!tabl.ParamsNum.ContainsKey(Num))
                tabl.ParamsNum.Add(Num, this);
            tabl.ParamsId.Add(Id, this);
            _paramsCode = new Dictionary<string, TablParam>();
            _paramsNum = new Dictionary<int, TablParam>();
        }

        //Загрузка подпараметра из рекордсета подтаблицы
        public void SubParamFromRec(Tabl tabl, ReaderAdo rec)
        {
            int pid = rec.GetInt("Id");
            if (tabl.ParamsId.ContainsKey(pid))
            {
                var parent = tabl.ParamsId[pid];
                Code = rec.GetString("SubCode");
                Num = rec.GetInt("SubNum");
                Name = rec.GetString("SubName");
                Id = rec.GetInt("SubId");
                Values = new string[tabl.SubFieldsCount];
                for (int i = 0; i < tabl.SubFieldsCount; i++)
                    Values[i] = rec.GetString("SubVal_" + i) ?? "";
                if (Code != null && !parent.ParamsCode.ContainsKey(Code))
                    parent.ParamsCode.Add(Code, this);
                if (!parent.ParamsNum.ContainsKey(Num))
                    parent.ParamsNum.Add(Num, this);
            }
        }
    }
}