using System.Collections.Generic;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для передачи адреса ошибки
    //Реализуется сигналами, расчетными параметрами времени выполнения, потом функциями
    public interface IErrorAddress
    {
        string ErrorAddress { get; }
    }

    //----------------------------------------------------------------------------------------------------
    //Хранилище ошибок
    public class ErrMomFactory
    {
        //Формирование ошибки по сообщению
        public ErrMom MakeError(IErrorAddress addr, string text, ErrorQuality quality = ErrorQuality.Error)
        {
            var descr = AddDescr(text, quality);
            return descr.AddErrMom(addr);
        }
        //Формирование ошибки по номеру
        public ErrMom MakeError(IErrorAddress addr, int number)
        {
            var descr = AddDescr(number, "", ErrorQuality.Error);
            return descr.AddErrMom(addr);
        }
        //Список описаний содержит указанный номер
        public bool HasErrNumber(int number)
        {
            return _errDescrsI.ContainsKey(number);
        }
        //Добавить описание для номера ошибки
        public void AddNumberErrText(int number, string text, ErrorQuality quality)
        {
            AddDescr(number, text, quality);
        }

        //Словари описаний ошибок, ключи - номера и сообщения
        private readonly DicI<ErrDescr> _errDescrsI = new DicI<ErrDescr>();
        private readonly Dictionary<string, ErrDescr> _errDescrsS = new Dictionary<string, ErrDescr>();
        //Объект для блокировки словарей
        private readonly object _locker = new object();

        //Добавляет описание ошибки с числовым ключом
        private ErrDescr AddDescr(int number, string text, ErrorQuality quality)
        {
            ErrDescr errDescr;
            lock (_locker)
            {
                if (!_errDescrsI.ContainsKey(number))
                {
                    errDescr = new ErrDescr(text, quality, number);
                    _errDescrsI.Add(number, errDescr);
                }
                else errDescr = _errDescrsI[number];
            }
            return errDescr;
        }

        //Добавляет описание ошибки со строковым ключом
        private ErrDescr AddDescr(string text, ErrorQuality quality)
        {
            ErrDescr errDescr;
            lock (_locker)
            {
                if (!_errDescrsS.ContainsKey(text))
                {
                    errDescr = new ErrDescr(text, quality);
                    _errDescrsS.Add(text, errDescr);
                }
                else errDescr = _errDescrsS[text];
            }
            return errDescr;
        }
    }
}