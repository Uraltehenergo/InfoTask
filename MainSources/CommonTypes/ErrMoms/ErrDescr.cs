using System.Collections.Generic;

namespace CommonTypes
{
    //Описание ошибки
    internal class ErrDescr
    {
        internal ErrDescr(string text, ErrorQuality quality, int number = 0)
        {
            Text = text;
            Quality = quality;
            Number = number;
        }

        //Сообщение об ошибке
        public string Text { get; private set; }
        //Числовое значение недостоверности
        public int Number { get; private set; }
        //Качество ошибки
        public ErrorQuality Quality { get; private set; }

        //Словарь всех адресов ошибок с данным описанием
        private readonly Dictionary<IErrorAddress, ErrMom> _addrs = new Dictionary<IErrorAddress, ErrMom>();

        //Добавляет ошибку с указанным адресом, если такого адреса еще не было
        public ErrMom AddErrMom(IErrorAddress addr)
        {
            var locker = new object();
            lock (locker)
            {
                if (!_addrs.ContainsKey(addr))
                {
                    var errMom = new ErrMom(this, addr);
                    _addrs.Add(addr, errMom);
                    return errMom;
                }
                return _addrs[addr];
            }
        }
    }
}