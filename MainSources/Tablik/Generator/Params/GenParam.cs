using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Tablik.Generator
{
    //Базовый класс для генерирующих параметров или подпараметров
    internal abstract class GenParamBase
    {
        protected GenParamBase(RecDao rec, ParsingCondition parsingCondition, ParsingGenerator parsingGenerator)
        {
            ErrMess = "";
            ParsingCondition = parsingCondition;
            _parsingGenerator = parsingGenerator;
            _code = MakeField(rec, "Code");
            _name = MakeField(rec, "Name");
            _units = MakeField(rec, "Units");
            _inputs = MakeField(rec, "Inputs");
            _userExpr1 = MakeField(rec, "UserExpr1");
            _userExpr2 = MakeField(rec, "UserExpr2");
            _interpolationType = MakeField(rec, "InterpolationType");
            _superProcessType = MakeField(rec, "SuperProcessType");
            _comment = MakeField(rec, "Comment");
            _min = MakeField(rec, "Min");
            _max = MakeField(rec, "Max");
            _tag = MakeField(rec, "Tag");
            _receiverCode = MakeField(rec, "ReceiverCode");
            _codeSignal = MakeField(rec, "CodeSignal");
        }

        //Создает простое или генерирующее поле
        protected NodeFormText MakeField(IRecordRead rec, string fieldName)
        {
            var s = rec.GetString(fieldName);
            if (s.IsEmpty() || !s.Contains("["))
                return new NodeText(s);
            var node = _parsingGenerator.Parse(s);
            if (node is NodeError)
            {
                var errMess = ((NodeError) node).ErrMess;
                ErrMess += errMess + " (" + fieldName + "); ";
                return new NodeText(errMess);
            }
            return (NodeFormText) node;
        }

        //Парсеры для условия и полей
        protected ParsingCondition ParsingCondition { get; private set; }
        private readonly ParsingGenerator _parsingGenerator;

        //Ошибка загрузки условий и полей генерации
        public string ErrMess { get; set; }

        //Поля таблицы
        private readonly NodeFormText _code;
        private readonly NodeFormText _name;
        private readonly NodeFormText _units;
        private readonly NodeFormText _inputs;
        private readonly NodeFormText _userExpr1;
        private readonly NodeFormText _userExpr2;
        private readonly NodeFormText _interpolationType;
        private readonly NodeFormText _superProcessType;
        private readonly NodeFormText _comment;
        private readonly NodeFormText _min;
        private readonly NodeFormText _max;
        private readonly NodeFormText _tag;
        private readonly NodeFormText _receiverCode;
        private readonly NodeFormText _codeSignal;

        //Рекордсет подпараметров
        protected RecDao SubParamsRec { get; set; }

        //Генерация по строке таблицы row в рекоррдсет rec, базовая для параметров и подпараметров
        protected void GenerateBase(SubRows row, IRecordSet rec )
        {
            rec.Put("Code", _code.GetText(row));
            rec.Put("Name",_name.GetText(row));
            rec.Put("Units", _units.GetText(row));
            rec.Put("Inputs", _inputs.GetText(row));
            rec.Put("UserExpr1", _userExpr1.GetText(row));
            rec.Put("UserExpr2", _userExpr2.GetText(row));
            rec.Put("InterpolationType", _interpolationType.GetText(row));
            rec.Put("SuperProcessType", _superProcessType.GetText(row));
            rec.Put("Comment", _comment.GetText(row));
            var s = _min.GetText(row);
            rec.Put("Min", s.IsEmpty() ? (double?)null : s.ToDouble());
            s = _max.GetText(row);
            rec.Put("Max", s.IsEmpty() ? (double?)null : s.ToDouble());
            rec.Put("Tag", _tag.GetText(row));
            rec.Put("ReceiverCode", _receiverCode.GetText(row));
            rec.Put("CodeSignal", _codeSignal.GetText(row));
        }

        public abstract void Generate(SubRows row);
    }

    //------------------------------------------------------------------------------------------------------------
    //Генерирующий параметр
    internal class GenParam : GenParamBase
    {
        public GenParam(RecDao rec, ParsingCondition parsingCondition, ParsingGenerator parsingGenerator) 
            :base(rec, parsingCondition, parsingGenerator)
        {
            MakeCondition(rec);
            _task = MakeField(rec, "Task");
            _calcParamType = MakeField(rec, "CalcParamType");
            _defaultValue = MakeField(rec, "DefaultValue");
            CalcParamId = rec.GetInt("CalcParamId");
            rec.Put("ErrMess", ErrMess);
        }

        //Создает условие генерации
        private void MakeCondition(IRecordRead rec)
        {
            var cond = rec.GetString("GenConditions");
            if (cond.IsEmpty())
                _condition = new NodeEmpty();
            else
            {
                var node = ParsingCondition.Parse(cond);
                if (node is NodeError)
                {
                    var errMess = ((NodeError) node).ErrMess;
                    ErrMess += errMess + " (GenConditions); ";
                    _condition = new NodeEmpty();
                }
                else _condition = (NodeIter)node;
            }
        }

        //Условие генерации
        private NodeIter _condition;

        //Поля таблицы
        public int CalcParamId { get; private set; }
        private readonly NodeFormText _task;
        private readonly NodeFormText _calcParamType;
        private readonly NodeFormText _defaultValue;

        //Список подпараметров
        private readonly List<GenSubParam> _subParams = new List<GenSubParam>();
        public List<GenSubParam> SubParams { get { return _subParams; } }

        //Рекордсеты параметров и подпараметров
        private RecDao _paramsRec;

        //Генерация по всему условию
        public void GenerateParams(TablsList tabls, //список всех таблиц для генерации
                                                 RecDao rec, //рекордсет параметров
                                                 RecDao recs) //рекордсет подпараметров
        {
            _paramsRec = rec;
            SubParamsRec = recs;
            _condition.Generate(this, tabls);
        }

        //Генерация по одной строке таблицы
        public override void Generate(SubRows row)
        {
            _paramsRec.AddNew();
            GenerateBase(row, _paramsRec);
            _paramsRec.Put("Task", _task.GetText(row));
            _paramsRec.Put("CalcParamType", _calcParamType.GetText(row));
            _paramsRec.Put("DefaultValue", _defaultValue.GetText(row));
            int id = _paramsRec.GetInt("CalcParamId");
            _paramsRec.Update();
            foreach (var sp in SubParams)
                sp.GenerateParams(row, SubParamsRec, id);
        }
    }

    //------------------------------------------------------------------------------------------------------------
    //Генерирующий подпараметр
    internal class GenSubParam : GenParamBase
    {
        public GenSubParam(RecDao rec, ParsingCondition parsingCondition, ParsingGenerator parsingGenerator) 
            : base(rec, parsingCondition, parsingGenerator)
        {
            MakeCondition(rec);
            OwnerId = rec.GetInt("OwnerId");
            rec.Put("ErrMess", ErrMess);
        }

        //Создает условие генерации
        private void MakeCondition(IRecordRead rec)
        {
            var cond = rec.GetString("GenConditions");
            if (cond.IsEmpty())
                _condition = new NodeSubEmpty();
            else
            {
                var node = ParsingCondition.SubParse(cond);
                if (node is NodeError)
                {
                    var errMess = ((NodeError)node).ErrMess;
                    ErrMess += errMess + " (GenConditions); ";
                    _condition = new NodeSubEmpty();
                }
                else _condition = (NodeSubIter)node;
            }
        }

        //Условие генерации
        private NodeSubIter _condition;

        //Id владельца и в владелец в файле шаблонов
        public int OwnerId { get; private set; }
        public GenParam Owner { get; set; }
        //Id владельца в файле проекта
        private int _projectOwnerId;

        //Генерация по всему условию
        public void GenerateParams(SubRows row, //строка таблицы, по которой генерируется
                                                 RecDao recs, //рекордсет подпараметров
                                                 int projectOwnerId) //Id владельца в проекте
        {
            SubParamsRec = recs;
            _projectOwnerId = projectOwnerId;
            _condition.Generate(this, row);
        }

        //Генерация по одной строке таблицы
        public override void Generate(SubRows row) 
        {
            SubParamsRec.AddNew();
            GenerateBase(row, SubParamsRec);
            SubParamsRec.Put("OwnerId", _projectOwnerId);
            SubParamsRec.Update();
        }
    }
}