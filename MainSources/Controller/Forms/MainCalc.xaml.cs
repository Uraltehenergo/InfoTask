using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BaseLibrary;
using CommonTypes;
using Calculation;
using MessageBox = System.Windows.MessageBox;

namespace Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainCalc : Window
    {
        public MainCalc()
        {
            InitializeComponent();
        }

        //Ссылка на приложение
        public App App { get; set; }
        //Окно справки
        public AppInfoWindow AppInfoWindow { get; private set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            butBreak.Visibility = General.DebugMode ? Visibility.Visible : Visibility.Hidden;
            butStart.Visibility = butBreak.Visibility;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool stop = true;
            foreach (var t in App.Threads)
                stop &= t.State != State.Calc && t.State != State.FinishCalc && t.State != State.Waiting && t.State != State.FinishWaiting && t.State != State.GetTime;
            if (!stop)
            {
                Different.MessageInfo("Для закрытия контроллера расчетов нужно чтобы все потоки были остановлены");
                e.Cancel = true;
            }
            else
            {
                try { if (AppInfoWindow != null) AppInfoWindow.Close(); } catch {}
                foreach (var tt in App.Threads)
                {
                    try
                    {
                        var t = (ThreadController) tt;
                        if (t.HandInputWindow != null && t.HandInputWindow.IsVisible)
                        {
                            var vw = t.HandInputWindow.ValuesVindow;
                            if (vw != null && vw.Visibility == Visibility.Visible)
                            {
                                ((GridInputParam)vw.DataContext).SaveHandValues();
                                vw.Close();
                            }
                            t.HandInputWindow.ValuesVindow = null;
                            if (t.HandInputWindow.IsVisible) t.HandInputWindow.Close();
                        }
                        if (t.SetupWindow != null && t.SetupWindow.IsVisible) t.SetupWindow.Close();
                        if (t.ThreadWindow != null && t.ThreadWindow.IsVisible) t.ThreadWindow.Close();
                        t.CloseThread();
                    }
                    catch{}
                }
                try { DaoDb.Compress(General.ControllerFile, 100000000, General.TmpDir);}
                catch { }
            }
        }

        private void butAddThread_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var rec = new RecDao(General.ControllerFile, "Threads"))
                {
                    rec.AddNew();
                    rec.Put("ApplicationType", ApplicationType.Controller.ToEnglish());
                    rec.Put("IsPeriodic", true);
                    var d = DateTime.Now.ToString();
                    rec.Put("TimeAdd", d);
                    rec.Put("TimeChange", d);
                    rec.Update();
                    rec.MoveLast();
                    App.AddThread(rec);    
                }
            }
            catch(Exception ex)
            {
                ex.MessageError("Ошибка создания потока");
            }
        }

        private void gridThreads_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //var t = (ThreadCalc) e.Row.DataContext;
            //switch (t.CalcType + t.CalcMode)
            //{
            //    case "РазовыйОстановлен":
            //        e.Row.Background = Brushes.LightGoldenrodYellow;
            //        return;
            //    case "РазовыйЗапущен":
            //        e.Row.Background = Brushes.Yellow;
            //        return;
            //    case "ПериодическийОстановлен":
            //        e.Row.Background = Brushes.LightCyan;
            //        return;
            //    case "ПериодическийВыравнивание":
            //        e.Row.Background = Brushes.Turquoise;
            //        return;
            //    case "ПериодическийСинхронный":
            //        e.Row.Background = Brushes.LimeGreen;
            //        return;
            //    default:
            //        e.Row.Background = Brushes.White;
            //        return;
            //}
        }

        //Открывает окно потока
        private void OpenThreadWindow()
        {
            if (gridThreads.SelectedItem == null)
            {
                Different.MessageInfo("Не выбран поток");
                return;
            }
            var t = (ThreadController)gridThreads.SelectedItem;
            if (App.ThreadWindows.ContainsKey(t.Id))
                App.ThreadWindows[t.Id].Focus();
            else
            {
                var w = new ThreadWindow { DataContext = t, App = App};
                t.ThreadWindow = w;
                App.ThreadWindows.Add(t.Id, w);
                w.Show();
                t.UpdateState();
            }
            gridThreads.UnselectAllCells();
        }

        private void gridThreads_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenThreadWindow();
        }

        private void butOpenThread_Click(object sender, RoutedEventArgs e)
        {
            OpenThreadWindow();
        }

        private void butDeleteThread_Click(object sender, RoutedEventArgs e)
        {
            if (gridThreads.SelectedItem == null) return;
            var t = (ThreadController) gridThreads.SelectedItem;
            if (t.Projects.Count > 0)
            {
                Different.MessageError("Для удаления потока нужно сначала удалить из него все проекты");
                return;
            }
            try
            {
                t.HandInputWindow.Close();
                t.HandInputWindow = null;   
            }
            catch {}
            try
            {
                t.SetupWindow.Close();
                t.SetupWindow = null;
            }
            catch { }
            try
            {
                t.ThreadWindow.Close();
                t.ThreadWindow = null;
            }
            catch { }
            App.ThreadsDic.Remove(t.Id);
            App.Threads.Remove(t);
            t.DeleteThread();
        }

        private void butBreak_Click(object sender, RoutedEventArgs e)
        {
            General.Paused = true;
        }

        private void butStart_Click(object sender, RoutedEventArgs e)
        {
            General.Paused = false;
        }

        private void ButHelp_Click(object sender, RoutedEventArgs e)
        {
            AppInfoWindow = new AppInfoWindow();
            AppInfoWindow.Show();
            AppInfoWindow.Activate();
        }

        private void ButSetupHistory_Click(object sender, RoutedEventArgs e)
        {
            App.MonitorHistory.Inf = App.MonitorHistory.Setup();
            SysTabl.PutValueS(General.ControllerFile, "MonitorHistoryProps", App.MonitorHistory.Inf);
            foreach (ThreadController t in gridThreads.ItemsSource)
                App.MonitorHistory.AddHistoryTables(t.Id);
        }
    }
}
