using BaseLibrary;

namespace Tablik
{
    //Одна группа таблиц
    public class Tabl
    {
         //Поля таблиц четырех уровнеий вложенности
        //Для каждой вложеноости - словарь типов данных полей
        private readonly DicS<DataType>[] _fields = new DicS<DataType>[4];
        public DicS<DataType>[] Fields { get { return _fields; } }
    }
}