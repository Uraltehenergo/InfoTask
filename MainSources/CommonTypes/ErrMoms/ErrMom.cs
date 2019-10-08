using BaseLibrary;

namespace CommonTypes
{
    //Ошибка с указанием описания и адреса
    public class ErrMom
    {
        //Ссылка на объект, породивший ошибку
        public IErrorAddress AddressLink { get; private set; }
        //Адрес происхождения ошибки
        public string Address { get { return AddressLink == null ? "" : AddressLink.ErrorAddress;} }
        //Сообщение об ошибке
        public string Text { get { return _errDescr.Text; } }
        //Качество ошибки
        public ErrorQuality Quality { get { return _errDescr.Quality; } }
        //Недостоверность как число
        public int Nd { get { return Quality.ToNumber(); } }

        internal ErrMom(ErrDescr errDescr, IErrorAddress address)
        {
            _errDescr = errDescr;
            AddressLink = address;
        }

        //Ссылка на описание ошибки
        private readonly ErrDescr _errDescr;
    }
}