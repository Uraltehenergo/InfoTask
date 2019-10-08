using System.Collections.Generic;
using System.ComponentModel;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один провайдер для формы настроек 
    public class Provider : INotifyPropertyChanged
    {
        private Provider()
        {}

        public Provider(IRecordRead rec, ThreadCalc thread)
        {
            ThreadCalc = thread;
            Type = rec.GetString("ProviderType");
            ProviderType = Type.ToProviderType();
            CanBeNotOtm = !ProviderType.IsProviderSource();
            Name = rec.GetString("ProviderName");
            Code = rec.GetString("ProviderCode");
            Inf = rec.GetString("ProviderInf");
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
        public Provider Copy(ThreadCalc thread)
        {
            return new Provider
                        {
                            ThreadCalc = thread,
                            Type = Type,
                            ProviderType = ProviderType,
                            CanBeNotOtm = CanBeNotOtm,
                            Name = Name,
                            Code = Code,
                            Inf = Inf,
                            Otm = Otm,
                            Codes = Codes
                        };
        }

        //Тип провайдера 
        public string Type { get; private set; }
        public ProviderType ProviderType { get; private set; }
        //Имя провайдера
        public string Name { get; private set; }
        public string NameStr { get { return Name; } }
        //Провайдер может быть отключен (кроме источников)
        public bool CanBeNotOtm { get; private set; }

        //Код провайдера
        private string _code;
        public string Code
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
        public string Inf
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
        public bool Otm
        {
            get { return _otm; }
            set
            {
                if (value == _otm) return;
                _otm = value;
                OnPropertyChanged("Otm");
            }
        }
        
        //Список возможных кодов для данного провайдера
        public IEnumerable<string> Codes { get; set; }
        
        //Cсылка на сам провайдер
        public IProvider ProviderInstance { get; set; }
        //Ссылка на поток
        public ThreadCalc ThreadCalc { get; private set; }
        //Очередь в General
        public Queue<int> ProviderQueue { get; set; } 

        //Список проектов
        private readonly SetS _projects = new SetS();
        public SetS Projects { get { return _projects; } }

        //Для обновления строк в форме списка потоков
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
