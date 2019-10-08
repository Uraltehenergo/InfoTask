namespace Calculation
{
    //Тип CalcValue
    public enum CalcValueType
    {
        Single, //SingleValue
        Var,
        Void,
        IntArray,
        StringArray,
    }

    //-----------------------------------------------------------------------
    //Состояния поведения интерфейса потока
    public enum State
    {
        Stopped, //Остановлен
        Calc, //Расчет
        FinishCalc, //Завершение расчета после нажатия Стоп
        Waiting, //Ожидание нового расчета
        GetTime, //Получение времени источников
        FinishWaiting, //Завершение ожидания после нажатия Стоп
        Setup,//Настройка
        HandInput,//Ручной ввод
        AbsoluteEdit, //Редактирование абсолютных значений
        ErrorsList //Просмотр списка ошибок
    }

    //---------------------------------------------------------------------
    //Комманды для отображения
    public enum ViewAtom
    {
        Calc,
        PrepareCalc,
        Waiting,
        ErrorWaiting
    }

    //---------------------------------------------------------------------
    //Состояния и атомарные комманды потока
    public enum Atom
    {
        OpenThread,
        DeleteThread,
        CloseThread,
        ReadSetup,
        CheckProviders,
        ReadTime,
        Setup,
        PasteProject,
        LoadHandInput,
        LoadAbsolute,
        SaveAbsolute,
        Run,
        Stop,
        Next,
        DeleteItems,
        ReadProject,
        PrepareResult,
        PrepareProviders,
        PrepareArchive,
        Wait,
        ErrorWait,
        ReadSource,
        SaveSourceDebug,
        CalculateProject,
        SaveCalcDebug,
        WriteReceiver,
        SaveReceiverDebug,
        AccumulateProject,
        WriteArchive,
        WriteVed,
        UpdateAbsoluteEdit,
        SaveDebug,
        BreakCalc,
        SetCalcOperations,
        SetDebugOperations,
    }

    //---------------------------------------------------------------------
    //Класс конвертеров для комманд и состояний
    public static class CommandConverters
    {
        //Имена комманд отбражения на русском
        public static string ToRussian(this ViewAtom viewAtom)
        {
            switch (viewAtom)
            {
                case ViewAtom.Calc:
                    return "Расчет";
                case ViewAtom.PrepareCalc:
                    return "Подготовка расчета";
                case ViewAtom.Waiting:
                    return "Ожидание";
                case ViewAtom.ErrorWaiting:
                    return "Дополнительное ожидание";
            }
            return "";
        }
        
        //Имена атомарных комманд и состояние на русском 
        public static string ToRussian(this Atom atom)
        {
            switch (atom)
            {
                case Atom.OpenThread:
                    return "Открытие потока";
                case Atom.DeleteThread:
                    return "Удаление потока";
                case Atom.CloseThread:
                    return "Закрытие потока";
                case Atom.ReadSetup:
                    return "Чтение настроек";
                case Atom.CheckProviders:
                    return "Проверка провайдеров";
                case Atom.ReadTime:
                    return "Получение времени источников";
                case Atom.Setup:
                    return "Настройка";
                case Atom.PasteProject:
                    return "Добавление проекта";
                case Atom.LoadHandInput:
                    return "Подготовка к ручному вводу";
                case Atom.LoadAbsolute:
                    return "Подготовка абсолютных значений";
                case Atom.SaveAbsolute:
                    return "Сохранение абсолютных значений";
                case Atom.Run:
                    return "Запуск расчета";
                case Atom.Stop:
                    return "Завершение расчета";
                case Atom.Next:
                    return "Новый период расчета";
                case Atom.DeleteItems:
                    return "Очистка провайдеров";
                case Atom.PrepareProviders:
                    return "Подготовка провайдеров";
                case Atom.PrepareResult:
                    return "Подготовка файла результатов";
                case Atom.ReadProject:
                    return "Загрузка проекта";
                case Atom.PrepareArchive:
                    return "Подготовка архива";
                case Atom.Wait:
                    return "Ожидание";
                case Atom.ErrorWait:
                    return "Дополнительное ожидание";
                case Atom.ReadSource:
                    return "Чтение из источника";
                case Atom.SaveSourceDebug:
                    return "Запись значений сигналов";
                case Atom.CalculateProject:
                    return "Вычисления";
                case Atom.SaveCalcDebug:
                    return "Запись значений параметров";
                case Atom.WriteReceiver:
                    return "Запись в приемник";
                case Atom.SaveReceiverDebug:
                    return "Сохранение значений приемника";
                case Atom.AccumulateProject:
                    return "Накопление результатов";
                case Atom.WriteArchive:
                    return "Запись в архив";
                case Atom.WriteVed:
                    return "Запись в ведомость";
                case Atom.UpdateAbsoluteEdit:
                    return "Обновление из архивного ввода";
                case Atom.SaveDebug:
                    return "Сохранение отладочных данных";
                case Atom.BreakCalc:
                    return "Прерывание расчета";
                case Atom.SetCalcOperations:
                    return "Задание операций расчета";
                case Atom.SetDebugOperations:
                    return "Задание отладочного сохранения";
            }
            return null;
        }
    }
}
