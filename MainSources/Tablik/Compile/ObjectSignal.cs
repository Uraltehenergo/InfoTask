using BaseLibrary;

namespace Tablik
{
    //Один объект, содержащий сигналы
    internal class ObjectSignal
    {
        //rec - рекордсет таблицы Objects, checkErrors: true - проверка сигналов, false - сигналы для компиляции
        public ObjectSignal(IRecordRead rec, bool checkErrors)
        {
            Id = rec.GetInt("ObjectId");
            Code = rec.GetString("CodeObject");
            Name = rec.GetString("NameObject");
            Tag = rec.GetString("TagObject");
            if (checkErrors)
            {
                ErrMess = "";
                if (Id <= 0) ErrMess += "Не заполнен идентификатор (ObjectId); ";
                if (Code.IsEmpty()) ErrMess += "Не заполнен код (CodeObject); ";
                if (rec.GetString("CommName").IsEmpty())
                    ErrMess += "Не заполнено имя коммуникатора (CommName); ";
                DefalutsCount = 0;
                ErrorInSignals = false;
            }
        }

        //Конструктор для объктов ручного ввода
        public ObjectSignal(string code)
        {
            Code = code;
        }

        //Код объекта
        public string Code { get; private set; }
        //Id объекта
        public int Id { get; private set; }
        //Имя объекта
        public string Name { get; private set; }
        //Дополнительные свойства
        public string Tag { get; private set; }

        //Ошибка в объекте
        public string ErrMess { get; set; }
        //Количество сигналов по умолчанию
        public int DefalutsCount { get; set; }
        //Ошибка в сигналах
        public bool ErrorInSignals { get; set; }
        //True - если используется один из сигналов, причем не обязательно во включенном параметре
        public bool InUse { get; set; }

        //Словарь сигналов
        private readonly DicS<Signal> _signals = new DicS<Signal>();
        public DicS<Signal> Signals { get { return _signals; } }
    } 
}