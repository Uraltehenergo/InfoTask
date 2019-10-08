using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;
using Controller;

namespace ControllerMonitor
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ControllerDataFile = Different.GetInfoTaskDir() + @"Controller\ControllerData.accdb";
            using (var sys = new SysTabl(ControllerDataFile))
            {
                MonitorHistory = new MonitorHistory
                {
                    Inf = sys.Value("MonitorHistoryProps")
                };
                CheckTime = sys.Value("MonitorHistoryCheckTime").ToDateTime();    
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(ThreadsForm = new ThreadsForm());
        }

        //Форма списка потоков
        internal static ThreadsForm ThreadsForm { get; private set; }

        //Ссылка на провайдер истории
        internal static MonitorHistory MonitorHistory { get; private set; }

        //Путь к ControllerData
        internal static string ControllerDataFile { get; private set; }
        //Время последнего квитирования
        internal static DateTime CheckTime { get; set; }
        //Время последнего отображения всплывающего окна ошибки
        internal static DateTime BallonTime { get; set; }

        //Список потоков
        private static readonly DicI<ThreadHistory> _threads = new DicI<ThreadHistory>();
        internal static DicI<ThreadHistory> Threads { get { return _threads; } }

        //Загрузка списка потоков
        internal static void LoadThreads()
        {
            ThreadHistory errThread = null;
            Threads.Clear();
            try
            {
                foreach (var table in SqlDb.SqlTablesList(MonitorHistory.SqlProps).Where(x => x.StartsWith("ErrorsList") && x != "ErrorsListTemplate"))
                {
                    var t = new ThreadHistory(table.Substring(10).ToInt());
                    t.LoadFromDb();
                    Threads.Add(t.Id, t);
                    if (t.LastTime > CheckTime && (errThread == null || t.LastTime > errThread.LastTime))
                        errThread = t;
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка соединения с базой данных истории монитора");
            }

            if (!ThreadsForm.Visible)
            {
                if (errThread != null && errThread.LastTime > BallonTime)
                {
                    ThreadsForm.NotifyIcon.BalloonTipText = errThread.LastText;
                    ThreadsForm.NotifyIcon.ShowBalloonTip(10000);
                    BallonTime = errThread.LastTime;
                }
            }
            else ThreadsForm.ReloadThreads(errThread);
        }
    }
}
