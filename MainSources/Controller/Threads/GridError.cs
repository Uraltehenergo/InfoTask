using System;
using System.ComponentModel;
using BaseLibrary;

namespace Controller
{
    //Одно описание ошибки для добавления в Grid сообщений
    public class GridError : INotifyPropertyChanged
    {
        public GridError(IRecordRead rec)
        {
            Id = rec.GetInt("Id");
            Description = rec.GetString("Description");
            Params = rec.GetString("Params");
            Time = rec.GetTime("Time").ToString();
            _command = rec.GetString("Command");
            _context = rec.GetString("Context");
            Command = _command + "\n" + _context;
            _periodBegin = rec.GetTimeNull("PeriodBegin");
            _periodEnd = rec.GetTimeNull("PeriodEnd");
            Period = _periodEnd == null ? null : (_periodBegin + "\n" + _periodEnd);
        }

        public void ToRecordset(RecDao rec)
        {
            rec.AddNew();
            rec.Put("Id", Id);
            rec.Put("Description", Description);
            rec.Put("Params", Params);
            rec.Put("Time", DateTime.Parse(Time));
            rec.Put("Command", _command);
            rec.Put("Context", _context);
            if (_periodBegin != null) 
                rec.Put("PeriodBegin", _periodBegin);
            if (_periodEnd != null)
                rec.Put("PeriodEnd", _periodEnd);
            rec.Update();
        }

        //Id в таблице ErrorsList 
        public int Id { get; private set; }
        //Имя ошибки
        public string Description { get; private set; }
        //Момент возникновения ошибки
        public string Time { get; private set; }
        //Имя ошибки
        public string Params { get; private set; }
        //Выполняемая комманда
        private readonly string _command;
        //Провайдер, проект и т.п. в котором выполняется команда
        private readonly string _context;
        //Комманда + контекст
        public string Command { get; private set; }
        //Начало периода обработки
        private readonly DateTime? _periodBegin;
        //Конец периода обработки
        private readonly DateTime? _periodEnd;
        //Налало и конец периода
        public string Period { get; private set; }

        //Для обновления строк в форме списка потоков
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
