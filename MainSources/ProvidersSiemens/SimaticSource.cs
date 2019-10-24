 using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    [Export(typeof(IProvider))]
    [ExportMetadata("Code", "SimaticSource")]
    public class SimaticSource : SourceBase, ISource 
    {
        //Код провайдера
        public override string Code { get { return "SimaticSource"; } }

        //Настройки провайдера
        public string Inf
        {
            get { return ProviderInf; }
            set
            {
                ProviderInf = value;
                var dic = ProviderInf.ToPropertyDicS();
                dic.DefVal = "";
                _mainArchive = new SimaticArchive(this, dic["SQLServer"], false);
                _reserveArchive = new SimaticArchive(this, dic["SQLServerReserve"], true);
                Hash = _mainArchive.Hash + ";" + _reserveArchive.Hash;
            }
        }

        protected override void AddMenuCommands() { }

        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return null;
        }

        //Соединение с архивами
        #region
        //Путь к SimaticCommData
        private readonly string _commDataFile = Different.GetInfoTaskDir() + @"Providers\Siemens\SimaticCommData.accdb";
        internal string CommDataFile { get { return _commDataFile; } }

        //Основная и дублирующая базы данных
        private SimaticArchive _mainArchive;
        private SimaticArchive _reserveArchive;
        
        //Открытие соединения
        protected override bool Connect()
        {
            using (var sys = new SysTabl(CommDataFile))
            {
                _mainArchive.SuccessTime = sys.SubValue("SourceInfo", "MainArchiveSuccessTime").ToDateTime();
                _reserveArchive.SuccessTime = sys.SubValue("SourceInfo", "ReserveArchiveSuccessTime").ToDateTime();
            }
            var archives = new SimaticArchive[2];
            int b = _reserveArchive.SuccessTime > _mainArchive.SuccessTime ? 0 : 1;
            archives[b] = _reserveArchive;
            archives[1-b] = _mainArchive;
            for (int iter = 1; iter <= 2; iter++)
            {
                if (IsConnected) Disconnect();
                foreach (var ar in archives)
                {
                    Logger.AddEvent((iter == 1 ? "Соединение" : "Повторное соединение") + " с архивом", ar.IsReserve ? "Резервный" : "Основной");
                    var con = ar.Connnect();
                    if (con != null && con.State == ConnectionState.Open)
                    {
                        _conn = con;
                        SysTabl.PutSubValueS(CommDataFile, "SourceInfo", (ar.IsReserve ? "Reserve" : "Main") + "ArchiveSuccessTime", DateTime.Now.ToString());
                        return IsConnected = true;
                    }    
                }
                Thread.Sleep(30);
            }
            Logger.AddError("Не удалось соединиться ни с основным, не с резервным сервером архива");
            return IsConnected = false;
        }

        //Закрытие соединений
        private void Disconnect()
        {
            if (_mainArchive!= null)
                _mainArchive.Disconnect();
            if (_reserveArchive != null)
                _reserveArchive.Disconnect();
            IsConnected = false;
        }

        //Проверка соединения
        public bool Check()
        {
            return Connect();
        }

        //Проверка настроек
        public string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names)
        {
            string err = "";
            if (!inf.ContainsKey("SQLServer") || inf["SQLServer"].IsEmpty()) 
                err += "Не указан основной архивный сервер" + Different.NewLine;
            return err;
        }

        //Проверка соединения
        public bool CheckConnection()
        {
            CheckConnectionMessage = "";
            bool bres = _mainArchive.CheckConnection();
            CheckConnectionMessage += Different.NewLine;
            return bres | _reserveArchive.CheckConnection();
        }

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; internal set; }

        //Закрыть все соединения
        public void Dispose()
        {
            Disconnect();
        }
        #endregion
        
        public TimeInterval GetTime()
        {
            return new TimeInterval(Different.MinDate, DateTime.Now);
        }

        //Словари сигналов, ключи полные коды и Id
        private readonly Dictionary<int, SimaticObject> _objectsId = new Dictionary<int, SimaticObject>();
        //Список объектов, разбитый на блоки
        private readonly List<List<ProviderObject>> _parts = new List<List<ProviderObject>>();

        //Подготовка
        public void Prepare() { }

        //Добавить сигнал в провайдер
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var sig = new SimaticSignal(signalInf, code, dataType, this, idInClone);
            if (!_objectsId.ContainsKey(sig.Id))
            {
                var ob = new SimaticObject(sig);
                _objectsId.Add(sig.Id, ob);
                if (_parts.Count == 0 || _parts[_parts.Count - 1].Count == 500)
                    _parts.Add(new List<ProviderObject>());
                _parts[_parts.Count - 1].Add(ob);
                ProviderSignals.Add(sig.Code, sig);
                return sig;
            }
            var addsig = _objectsId[sig.Id].AddSignal(sig);
            return ProviderSignals.Add(addsig.Code, addsig);
        }

        //Очистка списка сигналов
        public void ClearSignals()
        {
            _objectsId.Clear();
            ProviderSignals.Clear();
            _parts.Clear();
        }

        //Соединение, из которого считываются данные
        private OleDbConnection _conn;
        //Рекордсет со значениями
        private ReaderAdo _rec;
        //Читать срез значений
        private bool _isCut;

        //Запрос значений по одному блоку сигналов
        private bool ReadPartValues(List<ProviderObject> part)
        {
            var sb = new StringBuilder("TAG:R, ");
            if (part.Count == 1)
                sb.Append(((SimaticObject)part[0]).Id);
            else
            {
                sb.Append("(").Append(((SimaticObject) part[0]).Id);
                for (int i = 1; i < part.Count; i++)
                    sb.Append(";").Append(((SimaticObject) part[i]).Id);
                sb.Append(";-2)");
            }

            DateTime beg = _isCut ? BeginRead.AddSeconds(-60) : BeginRead;
            DateTime en = _isCut ? BeginRead.AddSeconds(10) : EndRead;
            sb.Append(", ").Append(beg.ToSimaticString());
            sb.Append(", ").Append(en.ToSimaticString());
            
            //sb.Append(", ").Append(BeginRead.ToSimaticString()).Append(", ");
            //if (!_isCut) sb.Append(EndRead.ToSimaticString());
            //else
                //Срез, интервал 9 - секунд, 257 (FIRST_INTERPOLATED) - первое значение за период, полученное через линейную интерполяцию
                //sb.Append(BeginRead.AddSeconds(8).ToSqlString()).Append(", 'TIMESTEP=9,257'");
            
            Logger.AddEvent("Запрос значений из архива", part.Count + " тегов");
            _rec = new ReaderAdo(_conn, sb.ToString());
            Logger.AddEvent("Запрос из архива отработал");
            return true;
            //Прерывание запроса к архиву по таймауту
            /*_isReady = false;
            var t = new Thread(() => 
                { 
                    _rec = new ReaderAdo(_conn, sb.ToString());
                    lock (_isReadyLock) _isReady = true;
                });
            t.Start();
            for (int i = 0; i < 2000; i++)
            {
                t.Join(100);
                lock (_isReadyLock)
                    if (_isReady) return true;
            }
            t.Abort();
            return false;*/
        }

        //Формирование значений по одному блоку сигналов
        private KeyValuePair<int, int> FormPartValues()
        {
            int nread = 0, nwrite = 0;
            //var set = new HashSet<int>();
            using (_rec)
                while (_rec.Read())
                {
                    SimaticObject ob = null;
                    try
                    {
                        int id = _rec.GetInt("ValueId"); //0
                        if (_objectsId.ContainsKey(id))
                        {
                            ob = _objectsId[id];
                            DateTime time = _rec.GetTime("TimeStamp").ToLocalTime(); //1
                            if (!_isCut || time <= BeginRead)
                            {
                                int quality = _rec.GetInt("Quality");//3
                                int nd = quality == 128 ? 0 : 1;
                                nread++;
                                if (ob.SignalValue != null)
                                    nwrite += ob.SignalValue.AddMoment(ob.SignalValue.DataType, time, _rec.Reader["RealValue"], nd, _isCut); //2
                                if (ob.SignalQuality != null)
                                    nwrite += ob.SignalQuality.AddMoment(time, quality, 0, _isCut);
                                if (ob.SignalFlags != null)
                                    nwrite += ob.SignalFlags.AddMoment(time, _rec.GetInt("Flags"), nd, _isCut);  //4  
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AddErrorObject(ob == null ? "" : ob.Tag, "Ошибка при чтении значений из рекордсета", ex);
                    }
                }
            foreach (var sig in ProviderSignals.Values)
                if (sig.BeginMoment != null)
                    NumWrite += sig.AddBegin(BeginRead); //ab было без параметра
            return new KeyValuePair<int, int>(nread, nwrite);
        }

        //Создает блоки для чтения среза, только по тем парамеьрам, для которых срез еще не прочитан
        private List<List<ProviderObject>> MakeBeginParts()
        {
            var res = new List<List<ProviderObject>>();
            int n = 0;
            List<ProviderObject> bpart = null;
            foreach (var part in _parts)
                foreach (SimaticObject ob in part)
                    if (ob.SignalValue.BeginMoment == null)
                    {
                        if (n++ % 500 == 0) 
                            res.Add(bpart = new List<ProviderObject>());
                        bpart.Add(ob);
                    }
            return res;
        }

        private void MakeEnd()
        {
            int w = 0;
            foreach (var sig in ProviderSignals.Values)
                w += sig.MakeEnd(EndRead);
            int r = _objectsId.Values.Count(ob => ob.SignalValue.BeginMoment == null);
            NumWrite += w;
            Logger.AddEvent("Чтение значений из Historian завершено", "Добавлено " + w + " значений в конец. " + r + " неопределенных срезов");
        }

        //Чтение значений
        public override void GetValues()
        {
            if (Connect())
            {
                Logger.AddEvent("Чтение среза значений сигналов");
                _isCut = true;
                ReadValuesByParts(ReadPartValues, FormPartValues, MakeBeginParts());
                Logger.AddEvent("Чтение изменений значений сигналов");
                _isCut = false;
                ReadValuesByParts(ReadPartValues, FormPartValues, _parts);
                MakeEnd();
            }
        }
    }
}

