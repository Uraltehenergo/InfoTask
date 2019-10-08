namespace Tablik
{
    //Тип класса
    public enum ClassType
    {
        Single, //Обычный список значений
        Var, //Переменная (левая часть присвоения)
        Void, //Нет значения
        Error, //Значение после ошибки компиляции
        Undef, //Значение еще не определено
        Max, //Максимум в иерархии классов, ошибка
        Calc //Параметр
    }

    //--------------------------------------------------------------------
    
    //Тип элемента
    public enum ExprType
    {
        Const, //Константа
        Signal, //Сигнал
        HandSignal,//Сигнал ручного ввода
        Var, //Переменная слева от присвоения
        VarUse, //Использование значения переменной
        Fun, //Встроенная функция
        Prev, //Первый параметр в цепочке функции Пред
        PrevM, //Не первый параметр в цепочке функции Пред
        Calc, //Расчетный параметр
        Met, //Метод
        Grafic, //График
        Op, //Ключевое слово
        Error //Ошибка
    }

    //---------------------------------------------------------------------------------------------

    //Состояние проекта
    public enum State
    {
        Empty, //Проект не загружен 
        Project,//Проект загружен 
        Signals,//Сигналы загружены
        Compiled,//Компиляция прошла
        Closed,//Был закрыт
        Default 
    }

    //-------------------------------------------------------------
    //Комманды отображаемые индикатором
    public enum Atom
    {
        LoadGeneral,
        OpenHistory,
        SetCompiled,
        SetTmp,
        LoadProject, 
        LoadSignals,
        CompileProject,
        MakeWorkFile,
        CheckSignals,
        Close
    }

    //-------------------------------------------------------------
    //Стадия компиляции параметра
    public enum CompileStage
    {
        NotStarted, //Компиляция еще не начиналась
        Started, //Компиляция идет, типы данных не менялись
        Changed, //Компиляция идет, типы данных менялись
        Finished //Компиляция завершена
    }

    //-------------------------------------------------------------
    public static class ComandConverters
    {
        public static bool LessOrEqual(this ClassType t1, ClassType t2)
        {
            if (t1 == t2) return true;
            if (t1 == ClassType.Undef) return true;
            if (t2 == ClassType.Undef && t1 == ClassType.Error) return false;
            if (t1 == ClassType.Error) return true;
            if (t2 == ClassType.Max) return true;
            return false;
        }

        public static string Code(this State s)
        {
            switch (s)
            {
                case State.Empty: return "Empty";
                case State.Project: return "Project";
                case State.Signals: return "Signals";
                case State.Compiled:return "Compiled";
            }
            return null;
        }

        public static string Code(this Atom c)
        {
            switch (c)
            {
                case Atom.LoadGeneral:
                    return "LoadGeneral";
                case Atom.OpenHistory:
                    return "OpenHistory";
                case Atom.SetCompiled:
                    return "SetCompiled";
                case Atom.SetTmp:
                    return "SetTmp";
                case Atom.LoadProject:
                    return "LoadProject";
                case Atom.LoadSignals:
                    return "LoadSignals";
                case Atom.CompileProject:
                    return "CompileProject";
                case Atom.MakeWorkFile:
                    return "MakeWorkFile";
                case Atom.CheckSignals:
                    return "CheckSignals";
                case Atom.Close:
                    return "Close";
            }
            return null;
        }

        public static string RunMessage(this Atom c)
        {
            switch (c)
            {
                case Atom.LoadGeneral:
                    return "Инициализация";
                case Atom.SetCompiled:
                    return "Задание каталога рабочих файлов";
                case Atom.SetTmp:
                    return "Задание каталога временных файлов";
                case Atom.OpenHistory:
                    return "Открытие файла истории";
                case Atom.LoadProject:
                    return "Загрузка проекта";
                case Atom.LoadSignals:
                    return "Загрузка сигналов";
                case Atom.CompileProject:
                    return "Компиляция";
                case Atom.MakeWorkFile:
                    return "Создание рабочего файла";
                case Atom.CheckSignals:
                    return "Проверка таблиц сигналов";
                case Atom.Close:
                    return "Закрытие";
            }
            return null;
        }
    }
}