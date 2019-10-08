using System.ComponentModel;
using BaseLibrary;

namespace Controller
{
    //Одно значение параметра ручного ввода
    public class HandInputValue : INotifyPropertyChanged
    {
        public HandInputValue() {}

        public HandInputValue(IRecordSet rec, DataType dt)
        {
            Time = rec.GetTime("Time").ToString();
            if (dt.LessOrEquals(BaseLibrary.DataType.Real))
                Value = rec.GetDouble("Value").ToString();
            else if (dt == BaseLibrary.DataType.Time)
                Value = rec.GetTime("TimeValue").ToString();
            else Value = rec.GetString("StrValue");
        }

        //Запись в рекордсет значений клона
        public void ToRecordset(IRecordSet rec, int signalId, DataType dt)
        {
            if (ValueCorrect && TimeCorrect && Value.IsOfType(dt))
            {
                rec.AddNew();
                rec.Put("SignalId", signalId);
                rec.Put("Time", Time);
                if (dt.LessOrEquals(BaseLibrary.DataType.Real))
                    rec.Put("Value", Value.ToDouble());
                else if (dt == BaseLibrary.DataType.Time)
                    rec.Put("TimeValue", Value.ToDateTime());
                else rec.Put("StrValue", Value);
                rec.Put("Nd", 0);
                rec.Update();    
            }
        }

        //Значение
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (value == _value) return;
                _value = value;
                OnPropertyChanged("Value");
                ValueCorrect = value.IsOfType(DataType.ToDataType());
            }
        }

        //Время значения
        private string _time;
        public string Time
        {
            get { return _time; }
            set
            {
                if (value == _time) return;
                _time = value;
                OnPropertyChanged("Time");
                TimeCorrect = value == null || value.IsOfType(BaseLibrary.DataType.Time);
            }
        }

        //Допустимость введенного значения
        private bool _valueCorrect;
        public bool ValueCorrect
        {
            get { return _valueCorrect; }
            set
            {
                if (value == _valueCorrect) return;
                _valueCorrect = value;
                OnPropertyChanged("ValueCorrect");
            }
        }

        //Допустимость введенного времени
        private bool _timeCorrect;
        public bool TimeCorrect
        {
            get { return _timeCorrect; }
            set
            {
                if (value == _timeCorrect) return;
                _timeCorrect = value;
                OnPropertyChanged("TimeCorrect");
            }
        }

        //Тип данных
        private string _dataType;
        public string DataType
        {
            get { return _dataType; }
            set
            {
                if (value == _dataType) return;
                _dataType = value;
                OnPropertyChanged("DataType");
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