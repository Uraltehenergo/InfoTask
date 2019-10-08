using System;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using Calculation;

namespace ControllerClient
{
    //Класс, вызываемый через Com
    //Все функции возвращают ошибку или ""
    public class Controller
    {
        //Открывает локальный поток расчета
        //id - Id потока
        public string OpenLocal(int id, bool showIndicator = true)
        {
            _thread = new ThreadApp();
            _thread.Open(id, showIndicator);
            return _thread.ErrMessage();
        }

        //Открывает поток расчета на другом компьютере через WCF
        public string OpenRemout(int id, string address)
        {
            throw new NotImplementedException();
        }

        //Возвращает Id потока
        public int ThreadId
        { get { return (_thread == null ? 0 : _thread.ThreadId());} }

        //Закрытие потока
        public string Close()
        {
            try
            {
                _thread.Close();
                return _thread.ErrMessage();
            }
            finally
            {
                GC.Collect();
            }
        }

        //Загрузка настроек и проверка соединения с провайдерами
        public string LoadSetup()
        {
            return _thread.LoadSetup();
        }

        //Получение времени источников
        public string GetSourcesTime()
        {
            _thread.GetSourcesTime();
            var t = _thread.SourcesTime();
            SourcesBegin = t.Begin;
            SourcesEnd = t.End;
            SourcesTimeString = _thread.SourcesTimeString();
            return _thread.ErrMessage();
        }

        //Начало и конец периода источников
        public DateTime SourcesBegin { get; private set; }
        public DateTime SourcesEnd { get; private set; }

        //Строка содержащая времена всех источников в формате Имя Провайдера=Начало периода-Конец периода;
        public string SourcesTimeString { get; private set;}

        //Задание операций, выполняемых во время расчета
        public void SetCalcOperations(bool readSources, bool writeArchives, bool writeReceivers)
        {
            _thread.SetCalcOperations(readSources, writeArchives, writeReceivers);
        }

        //Параметры отладочного сохранения
        public void SetDebugOperations(bool saveSignals, bool saveParams, bool saveMethods, bool saveVariables, bool saveReceiversSignals, bool saveValues)
        {
            _thread.SetDebugOperations(saveSignals, saveParams, saveMethods, saveVariables, saveReceiversSignals, saveValues);
        }

        //Расчет, timeBegin - начало, timeEnd - конец, intervalName - имя
        //writeArchiveType - Single, Named, NamedAdd, NameAddParams, Periodic, periodLength - длина интервала в минутах
        public string Calc(DateTime timeBegin, DateTime timeEnd, string imitMode = "Default", string intervalName = "", string writeArchiveType = "Single", int periodLength = 15)
        {
            string res = _thread.Calc(timeBegin, timeEnd, imitMode, intervalName, writeArchiveType, periodLength);
            IntervalId = _thread.IntervalId();
            return res;
        }

        //Расчет с сохранением результатов в ведомость анализатора (РАС)
        // timeBegin - начало, timeEnd - конец, task - имя задачи, содержащей набор, vedFile - файл ведомости
        public string AnalyzerCalc(DateTime timeBegin, DateTime timeEnd, string vedFile, string task = "")
        {
            return _thread.AnalyzerCalc(timeBegin, timeEnd, vedFile, task);
        }

        //Id интервала в архиве с результатами последнего расчета
        public int IntervalId { get; private set; }

        //Текст ошибки последней операции
        public string ErrorMessage
        {
            get { return _thread.ErrMessage(); }
        }

        //Ссылка на сам поток 
        private IThreadApp _thread;
    }
}
