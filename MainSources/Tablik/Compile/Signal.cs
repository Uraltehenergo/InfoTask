using BaseLibrary;

namespace Tablik
{
    //Один сигнал
    internal class Signal
    {
        //Конструктор, на входе рекорсет rec, objects - список объектов, checkErrors: true - проверка сигналов, false - сигналы для компиляции
        public Signal(IRecordRead rec, DicI<ObjectSignal> objects, bool checkErrors)
        {
            ErrMess = "";
            Code = rec.GetString("CodeSignal");
            Name = rec.GetString("NameSignal");
            Units = rec.GetString("Units");
            DataType = rec.GetString("DataType").ToDataType();
            InUseSource = false;
            InUseReceiver = false;
            ObjectId = rec.GetInt("ObjectId");
            ConstValue = rec.GetString("ConstValue");
            SourceName = rec.GetString("SourceName");
            ReceiverName = rec.GetString("ReceiverName");
            Inf = rec.GetString("Inf");
            Default = rec.GetBool("Default");
            if (checkErrors)
            {
                if (Code.IsEmpty()) 
                    ErrMess += "Не заполнен код (CodeSignal); ";
                if (DataType == DataType.Error)
                    ErrMess += "Недопустимый тип данных (DataType); ";
                if (ConstValue.IsEmpty() && SourceName.IsEmpty() && ReceiverName.IsEmpty())
                    ErrMess += "Должно быть заполнено имя источника (SourceName), имя приемника (ReceiverName) или значение константы (ConstValue); ";
            }
            if (objects.ContainsKey(ObjectId))
            {
                ObjectSignal = objects[ObjectId];
                ObjectSignal.Signals.Add(Code, this);
                FullCode = ObjectSignal.Code + "." + Code;
                if (checkErrors)
                {
                    //((RecDao)rec).Put("FullCode", FullCode);
                    if (Default) objects[ObjectId].DefalutsCount++;
                    if (ErrMess != "") objects[ObjectId].ErrorInSignals = true;
                }
            }
        }

        //Конструктор для параметров ручного ввода
        public Signal(string codeObject, DataType dataType, string sourceName)
        {
            ObjectSignal = new ObjectSignal(codeObject);
            Code = "руч";
            FullCode = codeObject + ".руч";
            DataType = dataType;
            InUseSource = true;
            SourceName = sourceName;
            Default = true;
        }

        public void ToRecordset(IRecordSet rec, bool addnew)
        {
            if (addnew) rec.AddNew();
            rec.Put("FullCode", FullCode);
            rec.Put("CodeSignal", Code);
            rec.Put("CodeObject", ObjectSignal == null ? "Объект" : ObjectSignal.Code);
            rec.Put("NameSignal", Name);
            rec.Put("NameObject", ObjectSignal == null ? null : ObjectSignal.Name);
            rec.Put("TagObject", ObjectSignal == null ? null : ObjectSignal.Tag);
            rec.Put("Units", Units);
            rec.Put("DataType", DataType.ToEnglish());
            rec.Put("SourceName", InUseSource ? SourceName : null);
            rec.Put("ConstValue", ConstValue);
            rec.Put("ReceiverName", InUseReceiver ? ReceiverName : null);
            rec.Put("Inf", Inf);
            rec.Put("Default", Default);
            rec.Put("UsedUnits", TablikCompiler.UsingParamsString(UsingParams));
        }

        //Ссылка на объект
        public ObjectSignal ObjectSignal { get; private set; }

        //Код сигнала
        public string Code { get; private set; }
        //Полное описание сигнала и объекта
        public string FullCode { get; private set; }
        //Тип данных 
        public DataType DataType { get; private set; }
        //Сигнал по умолчанию
        public bool Default { get; private set; }
        //Id объекта
        public int ObjectId { get; private set; }
         //Имя сигнала
        public string Name { get; private set; }
        //Единицы измерения
        public string Units { get; private set; }

        //Значение, для сигналов-констант
        public string ConstValue { get; private set; }
        //Имя источника
        public string SourceName { get; private set; }
        //Имя получателя
        public string ReceiverName { get; private set; }
        //Информация для истоника
        public string Inf { get; private set; }

        //True - если используется, причем не обязательно во включенном параметре
        public bool InUse { get; set; }
        //True - если участвует в расчете как сигнал источника
        public bool InUseSource { get; set; }
        //True - если используется как сигнал приемника
        public bool InUseReceiver { get; set; }
        //Список параметров, использующих данный объект
        private DicS<CalcParam> _usingParams;
        public DicS<CalcParam> UsingParams { get { return _usingParams ?? (_usingParams = new DicS<CalcParam>()); } }
        //Ошибка в сигнале
        public string ErrMess { get; set; }
    }
}