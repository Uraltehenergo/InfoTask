using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Odbc;
using System.Text;
using BaseLibrary;
using CommonTypes;
using Provider;

namespace ProvidersAlpha
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "AlphaRegulSource")]
    public class AlphaRegulSource : SourceBase, ISource 
    {
        //Код провайдера
        public override string Code { get { return "AlphaRegulSource"; } }

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                
                var builder = new StringBuilder("Driver={PostgreSQL Unicode}");
                builder.Append("; DataSource=").Append(dic["DataSource"]);
                builder.Append("; SERVER=").Append(dic["SERVER"]);
                builder.Append("; PORT=").Append(dic.GetInt("PORT"));
                builder.Append("; DATABASE=").Append(dic["DATABASE"]);
                builder.Append("; UID=").Append(dic["UID"]);
                builder.Append("; PWD=").Append(dic["PWD"]);
                _connectionString = builder.ToString();

                Hash = "Alpha;Server=" + dic["SERVER"] + ";Database=" + dic["DATABASE"];
            }
        }

        //Соединение с Historian
        private string _connectionString;

        //Задание комманд, вызываемых из меню
        protected override void AddMenuCommands() { }
        //Возвращает выпадающий список для поля настройки, props - словарь значений свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }

        //Освобождение ресурсов
        public void Dispose() { }

        //Проверка соединения
        public bool Check()
        {
            return Logger.Danger(TryCheck, 2, 1000, "Не удалось соединиться с сервером AlfaPlatform");
        }
        private bool TryCheck()
        {
            try
            {
                using (var connection = new OdbcConnection(_connectionString))
                {
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Не удалось соединиться с сервером AlfaPlatform", ex, "", Context);
                return false;
            }
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> nameDic)
        {
            string err = "";
            if (inf["DataSource"].IsEmpty()) err += "Не указано имя источника данных" + Different.NewLine;
            if (inf["SERVER"].IsEmpty()) err += "Не указано имя сервера" + Different.NewLine;
            if (inf["PORT"].IsEmpty()) err += "Не указан порт" + Different.NewLine;
            if (inf["DATABASE"].IsEmpty()) err += "Не задано имя базы данных" + Different.NewLine;
            if (inf["UID"].IsEmpty()) err += "Не задан пользователь" + Different.NewLine;
            if (inf["PWD"].IsEmpty()) err += "Не задан параоль";
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            if (Check())
            {
                CheckConnectionMessage = "Успешное соединение";
                return true;
            }
            Logger.AddError(CheckConnectionMessage = "Не удалось соединиться с архивом Альфа платформа");
            return false;
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; private set; }

        //Открытие соединения
        protected override bool Connect()
        {
            return true;
        }

        //Получение времени источника
        public TimeInterval GetTime()
        {
            TimeIntervals.Clear();
            var ti = new TimeInterval(Different.MinDate, Different.MaxDate);
            TimeIntervals.Add(ti);
            return ti;
        }

        //Словарь сигналов, ключи - nodeid
        private readonly DicI<AlphaSignal> _signalsById = new DicI<AlphaSignal>();

        //Добавить сигнал в провайдер
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var signal = new AlphaSignal(signalInf, code, dataType, this, idInClone);
            _signalsById.Add(signal.NodeId, signal);
            return ProviderSignals.Add(code, signal);
        }
        
        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
        }

        //Подготовка источника перед расчетом
        public void Prepare() { }

        //Условие отбора по списку NodeId сигналов
        private string GetSignalsConditionString()
        {
            var builder = new StringBuilder(" IN (");
            bool isFirst = true;
            foreach (AlphaSignal signal in ProviderSignals.Values)
            {
                if (!isFirst) builder.Append(", ");
                builder.Append(signal.NodeId);
                isFirst = false;
            }
            builder.Append(")");
            return builder.ToString();
        }

        //Чтение данных
        public override void GetValues()
        {
            var builder = new StringBuilder("SELECT * FROM nodes_history");
            builder.Append(" WHERE (nodes_history.time BETWEEN ");
            builder.Append(BeginRead.ToSimaticString()).Append(" AND ");
            builder.Append(EndRead.ToSimaticString()).Append(")");
            builder.Append(" AND nodes_history.NodeId");
            builder.Append(GetSignalsConditionString());
            
            Logger.AddEvent("Запрос значений из Historian", ProviderSignals.Count + " сигналов");
            using (var connection = new OdbcConnection(_connectionString))
            {
                connection.Open();
                var cmd = new OdbcCommand(builder.ToString(),  connection) { CommandType = CommandType.Text };
                using (var rec = cmd.ExecuteReader())
                {
                    Logger.Procent = 30;
                    Logger.AddEvent("Чтение значений из базы");
                    while (rec.Read())
                    {
                        var sig = _signalsById[Convert.ToInt32(rec["NodeId"])];
                        DateTime time = Convert.ToDateTime(rec["Time"]);
                        time = time.ToLocalTime();
                        switch (sig.DataType)
                        {
                            case DataType.Boolean:
                                sig.AddMoment(time, Convert.ToBoolean(rec["ValBool"]));
                                break;
                            case DataType.Integer:
                                sig.AddMoment(time, Convert.ToInt32(rec["ValInt"]));
                                break;
                            case DataType.Real:
                                sig.AddMoment(time, Convert.ToDouble(rec["ValDouble"]));
                                break;
                        }
                    }
                }
            }
        }
    }
}