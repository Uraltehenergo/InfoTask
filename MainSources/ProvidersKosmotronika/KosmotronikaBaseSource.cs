using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Типы мгновенных значений
    internal enum KosmCom
    {
        //Чтение значений выходов
        OutsBegin,//Срез на начало периода
        OutsMoments,//Изменения за период
        //Чтение аналоговых значений
        AnalogBegin,//Срез на начало периода
        AnalogMoments//Изменения за период
    }

    //-----------------------------------------------------------------------
    //Базовый класс для источников Космотроники
    public abstract class KosmotronikaBaseSource : SourceBase, ISource
    {
        //Настройки провайдера
        public abstract string Inf { get; set; }

        //Словарь комманд открытия дилогов, ключи - имена свойств, вторые ключи - названия пунктов меню
        protected override void AddMenuCommands() { }

        //Соединение с провайдером
        protected OleDbConnection Connection { get; set; }

        //Имя ретро-сервера или путь к архиву
        internal string DataSource;
        //Строка соединения с провайдером
        protected abstract string ConnectionString { get; }

        //Открытие соединения
        protected override bool Connect()
        {
            Dispose();
            if (DataSource.IsEmpty())
                return IsConnected = false;
            try
            {
                Logger.AddEvent("Соединение с архивом ПТК Космотроника");
                Connection = new OleDbConnection(ConnectionString);
                Connection.Open();
                return IsConnected = Connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка соединения с архивом ПТК Космотроника", ex);
                return IsConnected = false;
            }
        }

        public abstract bool Check();
        public abstract string CheckSettings(Dictionary<string, string> inf, Dictionary<string, string> names);
        public abstract bool CheckConnection();

        //Cтрока для вывода сообщения о последней проверке соединения
        public string CheckConnectionMessage { get; protected set; }
        //Возвращает выпадающий список для поля настройки, props - словарь значение свойств, propname - имя свойства для ячейки со списком
        public List<string> ComboBoxList(Dictionary<string, string> props, string propname)
        {
            return new List<string>();
        }

        //Освобождение ресурсов, занятых провайдером
        public void Dispose()
        {
            try { if (KosmReader != null) KosmReader.Close(); } catch { }
            try
            {
                if (Connection != null)
                {
                    Connection.Close();
                    Connection.Dispose();    
                }
            } 
            catch { }
        }

        //Получение времени архива, True - если успешно
        public TimeInterval GetTime()
        {
            if (!Logger.Danger(TryGetTime, 2, 500, "Не удалось определить временной диапазон архива ПТК Космотроника")) return null;
            return new TimeInterval(BeginTime, EndTime);
        }

        private bool TryGetTime()
        {
            if (!IsConnected && !Connect()) return false;
            try
            {
                Logger.AddEvent("Определение диапазона источника");
                var cmd = new OleDbCommand("Exec RT_ARCHDATE", Connection);
                using (OleDbDataReader rec = cmd.ExecuteReader())
                {
                    if (rec == null) return false;
                    rec.Read();
                    BeginTime = (DateTime)rec[0];
                    EndTime = (DateTime)rec[1];
                    TimeIntervals.Clear();
                    TimeIntervals.Add(new TimeInterval(BeginTime, EndTime));
                    Logger.AddEvent("Диапазон источника определен", BeginTime + " - " + EndTime);
                    return BeginTime.ToString() != "0:00:00";
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка определения диапазона архива ПТК Космотроника", ex);
                IsConnected = false;
                return false;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Чтение значений

        //Словарь сигналов. Один элемент словаря - один выход, для выхода список битов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _outs = new Dictionary<ObjectIndex, ObjectKosm>();
        //Словарь аналоговых сигналов
        private readonly Dictionary<ObjectIndex, ObjectKosm> _analogs = new Dictionary<ObjectIndex, ObjectKosm>();
        
        //Комманда для получения данных из архива, ее тип и получаемый рекордсет
        internal KosmCom KosmCom { get; private set; }
        protected OleDbDataReader KosmReader { get; private set; }
        
        //Очистка списка сигналов
        public void ClearSignals()
        {
            ProviderSignals.Clear();
            _outs.Clear();
            _analogs.Clear();
        }

        //Добавляет один сигнал в список
        public ProviderSignal AddSignal(string signalInf, string code, DataType dataType, int idInClone = 0)
        {
            var sig = new ProviderSignal(signalInf, code, dataType, this, idInClone);
            //Заполнение SignalsLists
            var ind=new ObjectIndex
                        {
                            Sn = sig.Inf.GetInt("SysNum"),
                            NumType = sig.Inf.GetInt("NumType"),
                            Appartment = sig.Inf.GetInt("Appartment"),
                            Out = sig.Inf.GetInt("NumOut")
                        };
            ObjectKosm obj;
            if (ind.Out == 1 && (ind.NumType == 1 || ind.NumType == 3 || ind.NumType == 32))
            {
                if (_analogs.ContainsKey(ind)) obj = _analogs[ind];
                else
                {
                    obj = new ObjectKosm(ind, sig.Code);
                    _analogs.Add(ind, obj);
                }
            }
            else
            {
                if (_outs.ContainsKey(ind)) obj = _outs[ind];
                else
                {
                    obj = new ObjectKosm(ind, sig.Code);
                    _outs.Add(ind, obj);
                }
            }
            var nsig = obj.AddSignal(sig);
            if (nsig == sig) ProviderSignals.Add(sig.Code, nsig);
            return nsig;
        }

        public void Prepare()
        {
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //Период текущего получения данных из архива
        private DateTime _periodBegin;
        private DateTime _periodEnd;

        //Чтение данных из архива
        public override void GetValues()
        {
            if (!IsConnected && !Connect()) return;
            try
            {
                foreach (var sig in ProviderSignals.Values)
                    if (sig.Value != null && sig.Value.Moments != null)
                        sig.Value.Moments.Clear();
                if (_analogs.Count > 0)
                    using (Logger.Start(5, _outs.Count > 0 ? 45 : 95))
                    {
                        Logger.AddEvent("Разбиение списка аналоговых сигналов на блоки", _analogs.Count + " сигналов");
                        var parts = MakeParts(_analogs);
                        Logger.Procent = 10;
                        Logger.AddEvent("Срез данных по аналоговым сигналам");
                        KosmCom = KosmCom.AnalogBegin;
                        using (Logger.Start(10, BeginRead != EndRead ? 30 : 100))
                        {
                            _periodBegin = BeginRead;
                            _periodEnd = EndRead;
                            ReadValuesByParts(ReadPartValues, FormPartValues, parts);
                        }
                        foreach (var ob in _analogs.Values)
                            AddBeginToObject(ob);
                        if (BeginRead != EndRead)
                        {
                            Logger.AddEvent("Изменения значений по аналоговым сигналам");
                            KosmCom = KosmCom.AnalogMoments;
                            ReadOnePeriod(parts);
                        }
                    }
                if (_outs.Count > 0)
                    using (Logger.Start(_outs.Count > 0 ? 45 : 5, 95))
                    {
                        Logger.AddEvent("Разбиение списка выходов на блоки", _outs.Count + " сигналов");
                        var parts = MakeParts(_outs);
                        Logger.Procent = 10;
                        Logger.AddEvent("Срез данных по выходам");
                        KosmCom = KosmCom.OutsBegin;
                        using (Logger.Start(10, BeginRead != EndRead ? 30 : 100))
                        {
                            _periodBegin = BeginRead;
                            _periodEnd = EndRead;
                            ReadValuesByParts(ReadPartValues, FormPartValues, parts);
                        }
                        foreach (var ob in _outs.Values)
                            AddBeginToObject(ob);
                        if (BeginRead != EndRead)
                        {
                            Logger.AddEvent("Изменения значений по выходам");
                            KosmCom = KosmCom.OutsMoments;
                            ReadOnePeriod(parts);
                        }
                    }
                foreach (var sig in ProviderSignals.Values)
                    NumWrite += sig.MakeEnd(EndRead);
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при получении данных из источника", ex);
                IsConnected = false;
            }
        }

        //Чтение одного периода длиной сутки или меньше
        private void ReadOnePeriod(List<List<ProviderObject>> parts)
        {
            using (Logger.Start(30))
            {
                _periodBegin = BeginRead;
                _periodEnd = Different.MinDate;
                double d = 100.0 / (EndRead.Subtract(BeginRead).Add(new TimeSpan(0, 0, 0, 0, -1)).TotalDays + 1);
                double p = 0;
                while (_periodEnd < EndRead)
                    using (Logger.Start(p, p + d))
                    {
                        _periodEnd = _periodBegin.AddDays(1).AddMilliseconds(1) > EndRead ? EndRead : _periodBegin.AddDays(1);
                        ReadValuesByParts(ReadPartValues, FormPartValues, parts);
                        _periodBegin = _periodEnd;
                        p += d;
                    }
            }
        }
        
        private void AddBeginToObject(ObjectKosm ob)
        {
            if (ob.ValueSignal != null && ob.ValueSignal.BeginMoment != null)
                NumWrite += ob.ValueSignal.AddBegin();
            if (ob.StateSignal != null && ob.StateSignal.BeginMoment != null)
                NumWrite += ob.StateSignal.AddBegin();
            if (ob.PokSignal != null && ob.PokSignal.BeginMoment != null)
                NumWrite += ob.PokSignal.AddBegin();
            foreach (var bsig in ob.BitSignals.Values)
                if (bsig.BeginMoment != null)
                    NumWrite += bsig.AddBegin();
        }

        //Формирует список блоков для чтения сигналов из словаря сигналов
        private List<List<ProviderObject>> MakeParts(Dictionary<ObjectIndex, ObjectKosm> dic)
        {
            var parts = new List<List<ProviderObject>>();
            try
            {
                double len = EndRead.Subtract(BeginRead).TotalHours;
                if (len > 24) len = 24;
                int maxj = 3000;
                if (this is KosmotronikaArchDbSource) maxj = 25;
                else
                {
                    if (len > 0.0001) maxj = Math.Min(3000, Convert.ToInt32(2500 / len));
                    if (maxj == 0) maxj = 1;    
                }
                foreach (var ob in dic.Values)
                {
                    if (parts.Count == 0 || parts[parts.Count-1].Count == maxj)
                        parts.Add(new List<ProviderObject>());
                    parts[parts.Count-1].Add(ob);
                }
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка при разделении списка сигналов на блоки", ex);
            }
            return parts;
        }

         //Запрос значений по одному блоку сигналов
        private bool ReadPartValues(List<ProviderObject> part)
        {
            Logger.AddEvent("Подготовка получения данных", part.Count + " технологических модулей");
            var comm = new OleDbCommand { CommandType = CommandType.Text, Connection = Connection };
            try
            {
                bool isAnalog = KosmCom == KosmCom.AnalogBegin || KosmCom == KosmCom.AnalogMoments;
                var nums = new ushort[part.Count, isAnalog ? 3 : 4];
                for (int i = 0; i < part.Count; i++)
                {
                    var ob = (ObjectKosm)part[i];
                    nums[i, 0] = (ushort)ob.Sn;
                    nums[i, 1] = (ushort)ob.NumType;
                    if (this is KosmotronikaRetroSource)
                    {
                        nums[i, 2] = (ushort)ob.Appartment;
                        if (!isAnalog) nums[i, 3] = (ushort)ob.Out;    
                    }
                    else if (!isAnalog) nums[i, 2] = (ushort)ob.Out;    
                }

                var parSysNums = new OleDbParameter("Sysnums", OleDbType.Variant) { Value = nums };
                var parBeginTime = new OleDbParameter("BeginTime", OleDbType.DBTimeStamp) { Value = _periodBegin };
                var parEndTime = new OleDbParameter("EndTime", OleDbType.DBTimeStamp) { Value = _periodEnd };
                comm.Parameters.Add(parBeginTime);
                switch (KosmCom)
                {
                    case KosmCom.OutsBegin:
                        comm.CommandText = "Exec ST_OUT ?, ?";                                           
                        break;
                    case KosmCom.OutsMoments:
                        comm.CommandText = "Exec RT_EXTREAD ? , ? , ?";                        
                        comm.Parameters.Add(parEndTime);                        
                        break;
                    case KosmCom.AnalogBegin:
                        comm.CommandText = "Exec ST_ANALOG ?, ?";                        
                        break;
                    case KosmCom.AnalogMoments:
                        comm.CommandText = "Exec RT_ANALOGREAD ? , ? , ?";
                        comm.Parameters.Add(parEndTime);                        
                        break;
                }
                comm.Parameters.Add(parSysNums);
            }
            catch (Exception ex)
            {
                Logger.AddError("Ошибка подготовки параметров команды считывания", ex);
                return IsConnected = false;
            }

            Logger.AddEvent("Чтение значений из архива ПТК Космотроника", _periodBegin + " - " + _periodEnd);
            using (Logger.Start(10, 70))
            {
                try { if (KosmReader != null) KosmReader.Close(); }
                catch { }
                Logger.AddEvent("Запрос значений из архива ПТК Космотроника");
                try
                {
                    Logger.StartProcess(Hash);
                    KosmReader = comm.ExecuteReader();
                }
                finally { Logger.FinishProcess(Hash); }

                Logger.AddEvent("Запрос из архива отработал");
                if ((KosmCom == KosmCom.AnalogBegin || KosmCom == KosmCom.OutsBegin) && !KosmReader.HasRows)
                {
                    Logger.AddError("Значения из источника не получены", null, part[0].Inf + " и др.", "", false);
                    return IsConnected = false;
                }
            }
            return true;
        }

        //Чтение срезов по одному блоку аналоговых сигналов
        private KeyValuePair<int, int> FormPartValues()
        {
            var res = new KeyValuePair<int, int>(0, 0);
            try
            {
                using (Logger.Start(70))
                {
                    lock (KosmReader)
                    {
                        switch (KosmCom)
                        {
                            case KosmCom.OutsBegin:
                                //res = ProcessOuts(KosmReader, BeginRead); //Установка среза
                                res = ProcessOuts(KosmReader);
                                break;
                            case KosmCom.OutsMoments:
                                res = ProcessOuts(KosmReader);
                                break;
                            case KosmCom.AnalogBegin:
                                //res = ProcessAnalog(KosmReader, BeginRead); //Установка среза
                                res = ProcessAnalog(KosmReader);
                                break;
                            case KosmCom.AnalogMoments:
                                res = ProcessAnalog(KosmReader);
                                break;
                        }
                    }
                }
            }
            finally { try { KosmReader.Close(); } catch { } }
            return res;
        }

        //Получение значений выходов из рекорсета rec, который вернула комманда, 
        //Tсли time != null, то такое время устанавливается для всех значений
        private KeyValuePair<int, int> ProcessOuts(IDataReader rec, DateTime? begtime = null)
        {
            int dn = this is KosmotronikaRetroSource ? 1 : 0;
            int nread = 0, nwrite = 0;
            while (rec.Read())
            {
                nread++;
                ObjectKosm ob = null;
                try
                {
                    var curSignal = new ObjectIndex
                        {
                            Sn = Convert.ToInt32(rec[0]),
                            NumType = Convert.ToInt32(rec[1]),
                            Appartment = this is KosmotronikaRetroSource ? Convert.ToInt32(rec[2]) : 0,
                            Out = Convert.ToInt32(rec[5+dn])
                        };
                    if (_outs.ContainsKey(curSignal))
                    {
                        DateTime time = begtime ?? Convert.ToDateTime(rec[2+dn]);
                        if (time < _periodEnd || time == _periodBegin)
                        {
                            ob = _outs[curSignal];
                            int ndint = Convert.ToInt32(rec[7+dn]);
                            int nd = ndint.GetBit(15) || ndint.GetBit(5) ? 1 : 0;
                            int pok = Convert.ToInt32(rec[4+dn]);
                            var s = (string)rec[8+dn];
                            uint iMean = 0;
                            double dMean = 0;
                            bool isInt = false;
                            if (s.IndexOf("0x") >= 0)
                            {
                                iMean = Convert.ToUInt32((string)rec[8+dn], 16);
                                isInt = true;
                            }
                            else dMean = Convert.ToDouble((string)rec[8+dn]);
                            if (ob.StateSignal != null)
                                nwrite += ob.StateSignal.AddMoment(time, ndint);
                            if (ob.PokSignal != null)
                                nwrite += ob.PokSignal.AddMoment(time, pok, nd);
                            if (ob.ValueSignal != null)
                                switch (ob.ValueSignal.DataType)
                                {
                                    case DataType.Boolean:
                                        nwrite += ob.ValueSignal.AddMoment(time, isInt ? iMean > 0 : dMean > 0, nd);
                                        break;
                                    case DataType.Integer:
                                        nwrite += ob.ValueSignal.AddMoment(time, isInt ? (int)iMean : Convert.ToInt32(dMean), nd);
                                        break;
                                    case DataType.Real:
                                        nwrite += ob.ValueSignal.AddMoment(time, isInt ? iMean : dMean, nd);
                                        break;
                                }
                            foreach (int i in ob.BitSignals.Keys)
                            {
                                if (!isInt) iMean = Convert.ToUInt32(dMean);
                                nwrite += ob.BitSignals[i].AddMoment(time, iMean.GetBit(i), nd);
                            }    
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddErrorObject(ob == null ? "" : ob.Inf, "Ошибка при чтении значений из рекордсета", ex);
                }
            }
            return new KeyValuePair<int, int>(nread, nwrite);
        }

        //Получение значений аналоговых сигналов из рекорсета rec, который вернула комманда, 
        //Если begtime != null, то такое время устанавливается для всех значений
        private KeyValuePair<int, int> ProcessAnalog(IDataReader rec, DateTime? begtime = null)
        {
            int dn = this is KosmotronikaRetroSource ? 1 : 0;
            int nread = 0, nwrite = 0;
            while (rec.Read())
            {
                nread++;
                var curInd = new ObjectIndex
                {
                    Sn = Convert.ToInt32(rec[0]),
                    NumType = Convert.ToInt32(rec[1]),
                    Appartment = this is KosmotronikaRetroSource ? Convert.ToInt32(rec[2]) : 0,
                    Out = 1
                };
                if (_analogs.ContainsKey(curInd))
                {
                    DateTime time = begtime ?? Convert.ToDateTime(rec[2+dn]);
                    if (time < EndRead || time == BeginRead)
                    {
                        ObjectKosm ob = _analogs[curInd];
                        int ndint = Convert.ToInt32(rec[5+dn]);
                        int nd = ndint.GetBit(15) || ndint.GetBit(5) ? 1 : 0;
                        int pok = Convert.ToInt32(rec[4+dn]);
                        var dMean = (Single)rec[7+dn];
                        if (ob.ValueSignal != null && ob.ValueSignal.DataType == DataType.Real)
                            nwrite += ob.ValueSignal.AddMoment(time, dMean, nd);
                        if (ob.StateSignal != null)
                            nwrite += ob.StateSignal.AddMoment(time, ndint);
                        if (ob.PokSignal != null)
                            nwrite += ob.PokSignal.AddMoment(time, pok, nd);    
                    }
                }
            }
            return new KeyValuePair<int, int>(nread, nwrite);
        }
    }
}
