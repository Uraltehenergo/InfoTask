using System;
using System.ServiceModel;
using BaseLibrary;

namespace Calculation
{
    //todo: Адаптировать потоки под удаленный запуск
    //Интерфейс потока приложения
    [ServiceContract]
    public interface IThreadApp
    {
        //Задание Id потока (id) и чтение настроек, showIndicator - отображать окно с индикатором
        [OperationContract]
        string Open(int id, bool showIndicator);
        //Возвращает Id потока
        [OperationContract]
        int ThreadId();
        //Закрытие потока
        [OperationContract]
        string Close();
        //Загрузка настроек и проверка соединения с провайдерами
        [OperationContract]
        string LoadSetup();
        //Разовый расчет, timeBegin - начало, timeEnd - конец, intervalName - имя, writeArchiveType - Single, Named, NamedAdd или NamedAddParams
        [OperationContract]
        string SingleCalc(DateTime timeBegin, DateTime timeEnd, string intervalName = "", string writeArchiveType = "Single");
        //Периодический расчет за период от startTime до stopTime, длина периода читается из ControllerData
        [OperationContract]
        string PeriodicCalc(DateTime startTime, DateTime stopTime);
        //Задание параметров отладочного сохранения
        [OperationContract]
        void SetDebugOperations(bool saveSignals, bool saveParams, bool saveMethods, bool saveVariables, bool saveReceiversSignals, bool saveValues);
        //Интервал последнего расчета
        [OperationContract]
        int IntervalId();
        //Получение времени источников
        [OperationContract]
        string GetSourcesTime();
        //Время источников 
        [OperationContract]
        TimeInterval SourcesTime();
        //Строка содержащая времена всех источников в формате Имя Провайдера=Начало периода-Конец периода;
        [OperationContract]
        string SourcesTimeString();
        //Текущее сообщение об ошибке потока
        [OperationContract]
        string ErrMessage();
    } 
}