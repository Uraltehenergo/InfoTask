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
        private readonly DicS<CalcUnit> _signals = new DicS<CalcUnit>();
        public DicS<CalcUnit> Signals { get { return _signals; }}
        //Сигнал по умолчанию
        public CalcUnit DefaultSignal { get; set; }
    }

    //--------------------------------------------------------------------------------------------------------------------
    //Один сигнал из SignalsInUse или один отладочный юнит
    public class CalcUnit
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
        public Project Project { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Имена источника и приемника
        public string SourceName { get; private set; }
        public string ReceiverName { get; private set; }
        //Ссылка на сигнал в источнике и пириемнике
        public ProviderSignal SourceSignal { get; set; }
        public ProviderSignal ReceiverSignal { get; set; }
        //Информация для источника и приемника
        public string Inf { get; private set; }
        //Значение для сигнала-константы
        public Moment ConstValue { get; private set; }
        //Является сигналом по умолчанию
        public bool Default { get; private set; }
        //Другие свойства объекта
        public string TagObject { get; private set; }

        //Возвращает значение константы или значение сигнала
        public CalcValue CalcValue
        {
            get { return new CalcValue(ConstValue == null ? SourceSignal.Value : new SingleValue(ConstValue.Clone(Project.ThreadCalc.PeriodBegin)), this); }
        }

        //rec - рекордсет с таблицей SignalsInUse, project - проект, isSignal - является сигналом
        public CalcUnit(IRecordRead rec, Project project, bool isSignal)
        {
            try
            {
                Project = project;
                FullCode = rec.GetString("FullCode");
                CodeObject = rec.GetString("CodeObject");
                CodeSignal = rec.GetString("CodeSignal");
                NameObject = rec.GetString("NameObject");
                NameSignal = rec.GetString("NameSignal");
                Units = rec.GetString("Units");
                DataType = rec.GetString("DataType").ToDataType();
                Default = rec.GetBool("Default");
                TagObject = rec.GetString("TagObject");
                if (FullCode.IsEmpty() || DataType == DataType.Error)
                {
                    Project.ThreadCalc.AddError("Список сигналов загружен с ошибками. Следует проверить настройки коммуникаторов проекта, повторно загрузить список сигналов и скомпилировать проект ", null, "Сигнал=" + FullCode);
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
                    Inf = rec.GetString("Inf", "");    
                }
            }
            catch (Exception ex)
            {
                Project.ThreadCalc.AddError("Список сигналов загружен с ошибками. Следует проверить настройки коммуникаторов проекта, повторно загрузить список сигналов и скомпилировать проект", ex, "Сигнал=" + FullCode);
            }
        }

        //Присоединить сигнал источника
        public void JoinSourceSignal()
        {
            var t = Project.ThreadCalc;
            if (t.IsReadSources)
            {
                var im = Project.Imitator;
                if (t.ImitMode != ImitMode.NoImit && im != null && im.Signals.ContainsKey(FullCode))
                    SourceSignal = im.Signals[FullCode];
                else if (ConstValue == null && t.Sources.ContainsKey(SourceName))
                    SourceSignal = t.Sources[SourceName].AddSignal(Inf, FullCode, DataType);
            }
        }

        //Присоединить сигнал приемника
        public void JoinReceiverSignal()
        {
            var t = Project.ThreadCalc;
            if (t.IsWriteReceivers && t.Receivers.ContainsKey(ReceiverName))
                ReceiverSignal = t.Receivers[ReceiverName].AddSignal(Inf, FullCode, DataType);
        }
    }
}