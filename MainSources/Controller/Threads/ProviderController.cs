using System.ComponentModel;
using BaseLibrary;
using CommonTypes;
using Calculation;

namespace Controller
{
    public class ProviderController : Provider, INotifyPropertyChanged
    {
        private ProviderController(){}

        internal ProviderController(IRecordRead rec, ThreadCalc thread) : base(rec, thread)
        {
            Otm = true;
        }

        public void ToRecordset(RecDao rec, bool addnew = false)
        {
            if (addnew) rec.AddNew();
            rec.Put("ThreadId", ThreadCalc.Id);
            rec.Put("ProviderType", Type);
            rec.Put("ProviderName", Name);
            rec.Put("ProviderCode", Code);
            rec.Put("ProviderInf", Inf);
            rec.Put("Otm", Otm);
        }
        
        //Создает копию провайдера
        public ProviderController Copy(ThreadCalc thread)
        {
            return new ProviderController
            {
                ThreadCalc = thread,
                Type = Type,
                ProviderType = ProviderType,
                Name = Name,
                Code = Code,
                Inf = Inf,
                Otm = Otm,
                Codes = Codes
            };
        }

        //Провайдер может быть отключен (архивы и приемники)
        public bool CanBeNotOtm { get { return !ProviderType.IsProviderSource(); }}

        //Код провайдера
        private string _code;
        public override string Code
        {
            get { return _code; }
            set
            {
                if (value == _code) return;
                _code = value;
                OnPropertyChanged("Code");
            }
        }

        //Настройки провайдера
        private string _inf;
        public override string Inf
        {
            get { return _inf; }
            set
            {
                if (value == _inf) return;
                _inf = value;
                OnPropertyChanged("Inf");
            }
        }

        //Если false, то работа с провайдером не производится
        private bool _otm;
        public override bool Otm
        {
            get { return _otm; }
            set
            {
                if (value == _otm) return;
                _otm = value;
                OnPropertyChanged("Otm");
            }
        }

        //Для обновления строк в форме списка потоков
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}