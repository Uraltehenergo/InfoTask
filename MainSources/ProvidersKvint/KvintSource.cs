using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "KvintSource")]
    public class KvintSource : SourceBase, ISource 
    {
        //Код провайдера
        public override string Code { get { return "MirSource"; } }

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                _serverName = dic["ServerName"];
                Hash = "Kvint;serverName=" + _serverName;
            }
        }

        protected override void AddMenuCommands() { }

        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }

        //Все соединения и так закрыты
        public void Dispose()
        {
            //try {CsApi.Done();} catch {}
        }

        //Имя сервера
        private string _serverName;

        //Проверка соединения
        public bool Check()
        {
            //return Logger.Danger(CsApi.Init, 2, 500, "Не удалось соединиться с архивом ПТК Квинт");
            return true;
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            return "";
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Не удалось соединиться с архивом ПТК Квинт");
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        //Открытие соединения
        protected override bool Connect()
        {
            return true;
            //try { return CsApi.Init();}
            //catch (Exception ex)
            //{
            //    Logger.AddError("Не удалось соединиться с архивом ПТК Квинт", ex);
            //    return false;
            //}
        }

        //Добавить сигнал в провайдер
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            return ProviderSignals.Add(code, new KvintSignal(signalInf, code, dataType, this, idInClone));
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
        }

        //Подготовка провайдера, чтение Handle сигналов
        public void Prepare()
        {
            //CsApi.Init();
            foreach (KvintSignal sig in ProviderSignals.Values)
                sig.GetHandler(_serverName);
        }

        //Получение времени источника
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            var t = new TimeInterval(Different.MinDate.AddYears(1), DateTime.Now);
            TimeIntervals.Add(t);
            return t;
        }

        //Чтение данных
        public override void GetValues()
        {
            int n = 0;
            foreach (KvintSignal sig in ProviderSignals.Values)
            {
                try { n += sig.ReadValues(BeginRead, EndRead); }
                catch(Exception ex) { Logger.AddError("Ошибка при чтении архива", ex); }
            }
            Logger.AddEvent("Данные из архива прочитаны", n + " значений");
        }
    }
}