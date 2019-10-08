using System;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    //Один сигнал
    internal class KvintSignal : ProviderSignal
    {
        public KvintSignal(string signalInf, string code, DataType dataType, IProvider provider, int idInClone = 0) 
            : base(signalInf, code, dataType, provider, idInClone)
        {
            var inf = signalInf.ToPropertyDicS();
            _marka = inf["Marka"];
            _paramName = inf["ParamName"];
            _cardId = inf.GetInt("CardId");
            _paramNo = inf.GetInt("ParamNo");
        }

        //Марка объекта
        private readonly string _marka;
        //Имя сигнала
        private readonly string _paramName;
        //Id объекта
        private readonly int _cardId;
        //Номер сигнала
        private readonly int _paramNo;
        //Id для получения данных
        private int _handler;

        //Получение Handler
        public void GetHandler(string serverName)
        {
            GetHandlerById();
            GetHandlerById();
        }

        private void GetHandlerById()
        {
            _handler = CsApi.OpenParamById(_cardId, _paramNo, 4117); //0x1015
        }

        private void GetHandlerByMarka()
        {
            _handler = CsApi.OpenParamByName(_marka, _paramName, 4117);
        }

        private void GetHandlerExternal(string serverName)
        {
            _handler = CsApi.OpenParamExternal(_cardId, _paramNo, serverName, 4117);
        }

        //Прочитать значения за период, возвращает количество прочитанных значений
        public int ReadValues(DateTime beg, DateTime en)
        {
            int n = 0;
            try
            {
                CsData m = new CsData();
                var dbeg = beg.TimeToKvint();
                var den = en.TimeToKvint();
                if (CsApi.FindFirst(_handler, ref m, dbeg, -1)) 
                    n += AddValue(m);
                if (CsApi.FindFirst(_handler, ref m, dbeg)) 
                {
                    n += AddValue(m);
                    while (true)
                    {
                        if (!CsApi.FindNext(_handler, ref m) || m.Time >= den) break;
                        n += AddValue(m);
                    }
                }
            }
            catch (Exception ex)
            {
                Provider.Logger.AddError("Ошибка при чтении значений", ex);
            }
            return n;
        }

        private int AddValue(CsData m)
        {
            DateTime t = m.Time.KvintToTime();
            double d = m.Value;
            int e = m.ErrorCode;
            if (e == 0) AddMoment(t, d);
            else AddMoment(new Moment(DataType, d, t, 2, new ErrorCalc("Ошибка " + e, Code)));
            return 1;
        }
    }
}