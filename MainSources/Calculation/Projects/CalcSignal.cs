using System;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Объект
    public class CalcObject
    {
        public CalcObject(string code)
        {
            Code = code;
        }

        //Код объекта
        public string Code { get; private set; }
        //Словарь сигналов
        private readonly DicS<CalcSignal> _signals = new DicS<CalcSignal>();
        public DicS<CalcSignal> Signals { get { return _signals; }}
        //Сигнал по умолчанию
        public CalcSignal DefaultSignal { get; set; }
    }

    //--------------------------------------------------------------------------------------------------------------------
    //Один сигнал из SignalsInUse 
    public class CalcSignal
    {
        //Полный код сигнала
        public string FullCode { get; private set; }
        //Код сигнала и объекта
        public string CodeObject { get; private set; }
        public string CodeSignal { get; private set; }
        //Имя сигнала и объекта
        public string NameObject { get; private set; }
        public string NameSignal { get; private set; }
        //Единицы измерения
        public string Units { get; private set; }
        //Ссылка на объект
        public CalcObject Object { get; set; }
        //Project содержащий данный сигнал
        private readonly Project _project;
        //Тип данных
        public DataType DataType { get; private set; }
        //Имена источника и приемника
        public string SourceName { get; private set; }
        public string ReceiverName { get; private set; }
        //Ссылка на сигнал в источнике и пириемнике
        public ProviderSignal SourceSignal { get; private set; }
        public ProviderSignal ReceiverSignal { get; private set; }
        //Информация для источника и приемника
        private readonly string _inf;
        //Значение для сигнала-константы
        public Moment ConstValue { get; private set; }
        //Является сигналом по умолчанию
        public bool Default { get; private set; }

        //Является расчетным сигналом
        public bool HasFormula { get; private set; }
        //Элемент выражения - сигнал, нужен для записи значений расчетных сигналов
        public SingleValue CalcSignalValue { get; set; }
        
        //Возвращает значение константы или значение сигнала
        public CalcValue CalcValue
        {
            get { return new CalcValue(ConstValue == null ? SourceSignal.Value : new SingleValue(ConstValue.Clone(_project.ThreadCalc.PeriodBegin)), this); }
        }

        //rec - рекордсет с таблицей SignalsInUse, project - проект, isSignal - является сигналом
        public CalcSignal(IRecordRead rec, Project project, bool isSignal)
        {
            try
            {
                _project = project;
                FullCode = rec.GetString("FullCode");
                CodeObject = rec.GetString("CodeObject");
                CodeSignal = rec.GetString("CodeSignal");
                NameObject = rec.GetString("NameObject");
                NameSignal = rec.GetString("NameSignal");
                Units = rec.GetString("Units");
                DataType = rec.GetString("DataType").ToDataType();
                Default = rec.GetBool("Default");
                HasFormula = rec.GetBool("HasFormula");
                if (FullCode.IsEmpty() || DataType == DataType.Error)
                {
                    _project.ThreadCalc.AddError("Список сигналов загружен с ошибками. Следует проверить настройки коммуникаторов проекта, повторно загрузить список сигналов и скомпилировать проект ", null, "Сигнал=" + FullCode);
                    return;
                }
                if (isSignal)
                {
                    SourceName = rec.GetString("SourceName");
                    string s = rec.GetString("ConstValue");
                    if (!s.IsEmpty())
                    {
                        ConstValue = new Moment(DataType, s);
                        if (s == "=Null") ConstValue.Error |= new ErrorCalc("Недопустимое значение сигнала-константы", SourceName + "." + FullCode);
                    }
                    ReceiverName = rec.GetString("ReceiverName");
                    _inf = rec.GetString("Inf", "");    
                }
            }
            catch (Exception ex)
            {
                _project.ThreadCalc.AddError("Список сигналов загружен с ошибками. Следует проверить настройки коммуникаторов проекта, повторно загрузить список сигналов и скомпилировать проект", ex, "Сигнал=" + FullCode);
            }
        }

        //Присоединить сигнал источника
        public void JoinSourceSignal()
        {
            var t = _project.ThreadCalc;
            var im = _project.Imitator;
            if (t.ImitMode != ImitMode.NoImit && im != null && im.Signals.ContainsKey(FullCode))
                SourceSignal = im.Signals[FullCode];
            else
            {
                if (!HasFormula && t.Sources.ContainsKey(SourceName))
                    SourceSignal = t.Sources[SourceName].AddSignal(_inf, FullCode, DataType);
                if (HasFormula)
                    t.ProvidersDic[SourceName].CalcSignals.Add(FullCode, this);    
            }
        }

        //Присоединить сигнал приемника
        public void JoinReceiverSignal()
        {
            var t = _project.ThreadCalc;
            if (t.Receivers.ContainsKey(ReceiverName))
                ReceiverSignal = t.Receivers[ReceiverName].AddSignal(_inf, FullCode, DataType);
        }
    }
}