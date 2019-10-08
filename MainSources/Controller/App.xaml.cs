using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using BaseLibrary;
using CommonTypes;
using Calculation;
using VersionSynch;
using Application = System.Windows.Application;

namespace Controller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Mutex.OpenExisting("InfoTask.Controller");
                MessageBox.Show("Нельзя запустить более одного контроллера одновременно");
                Current.Shutdown();
            }
            catch
            {
                if (new DbVersion().ACheck("Controller") < 1) Current.Shutdown();
                Mutex = new Mutex(false, "InfoTask.Controller"); 
            }
            try
            {
                General.Initialize();
                Synchro = new Synchro();
                MonitorHistory = new MonitorHistory
                {
                    Inf = SysTabl.ValueS(General.ControllerFile, "MonitorHistoryProps")
                };
                using (var rec = new ReaderAdo(General.ControllerFile, "SELECT * FROM Threads WHERE ApplicationType='Controller' ORDER BY ThreadId"))
                    while (rec.Read()) AddThread(rec);

                Window = new MainCalc();
                Window.Show();
                Window.App = this;
                Window.DataContext = this;
            }
            catch (Exception ex)
            {
                ex.MessageError();
            }
        }

        //Ссылка на провайдер истории ошибок всего монитора
        internal MonitorHistory MonitorHistory { get; private set; }

        //Загрузка потока контроллера
        public void AddThread(IRecordRead rec)
        {
            var t = new ThreadController(rec) { App = this, Synchro = Synchro};
            ThreadsDic.Add(t.Id, t);
            Threads.Add(t);
            MonitorHistory.AddHistoryTables(t.Id);
        }

        protected override void OnExit( ExitEventArgs e)
        {
            try
            {
                Mutex.Close();
                Mutex.Dispose();    
            }
            catch {}
        }

        //Мьютекс, чтобы не открывать более одного контроллера
        public Mutex Mutex { get; set; }

        //Главное окно
        public MainCalc Window { get; private set; }

        //Объект синхронизации
        public Synchro Synchro { get; private set; }

        //Словарь всех потоков, ключи - Id
        private readonly Dictionary<int, ThreadCalc> _threadsDic = new Dictionary<int, ThreadCalc>();
        public Dictionary<int, ThreadCalc> ThreadsDic { get { return _threadsDic; } }
        //Список потоков для формы
        private readonly ObservableCollection<ThreadCalc> _threads = new ObservableCollection<ThreadCalc>();
        public ObservableCollection<ThreadCalc> Threads { get { return _threads; } }

        //Фильтр для списка потоков
        private string _threadFilter;
        public string ThreadFilter
        {
            get { return _threadFilter; }
            set
            {
                if (value == _threadFilter) return;
                _threadFilter = value;
                OnPropertyChanged("ThreadFilter");
            }
        }

        //Окна потоков
        private readonly Dictionary<int, ThreadWindow> _threadWindows = new Dictionary<int, ThreadWindow>();
        public Dictionary<int, ThreadWindow> ThreadWindows { get { return _threadWindows; } }

        //Для обновления строк в форме списка потоков
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

