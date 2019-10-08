using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Calculation;
using BaseLibrary;
using CommonTypes;

namespace Controller
{
    /// <summary>
    /// Логика взаимодействия для ThreadWindow.xaml
    /// </summary>
    public partial class ThreadWindow : Window
    {
        public ThreadWindow()
        {
            InitializeComponent();
        }

        //Ссылка на приложение
        public App App { get; set; }

        public ThreadCalc ViewModel { get; set; }
        //Ссылка на поток
        private ThreadController ThreadC { get { return (ThreadController) DataContext; } }

        private bool CorrectTime(double period, double time)
        {
            return Math.Abs(Convert.ToInt32(time/period) - time/period) < 0.000000001;
        }

        private void butRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime beg, en;
                bool b = DateTime.TryParse(periodBegin.Text, out beg); 
                b &= DateTime.TryParse(periodEnd.Text, out en);
                if (b && ThreadC.State == State.Stopped)
                {
                    if (ThreadC.IsPeriodic)
                    {
                        if (ThreadC.PeriodLength <= 0 || ThreadC.PeriodLength > 60) b = false;
                        else
                        {
                            b &= CorrectTime(ThreadC.PeriodLength, 60.0);
                            b &= CorrectTime(ThreadC.PeriodLength, beg.TimeOfDay.Minutes + beg.TimeOfDay.Seconds / 60.0);
                            b &= CorrectTime(ThreadC.PeriodLength, en.TimeOfDay.Minutes + en.TimeOfDay.Seconds / 60.0);
                            if (b && (ThreadC.PeriodBegin < ThreadC.SourcesBegin) && 
                                !Different.MessageQuestion("Указанный интервал расчета не попадает в диапазон источников. Все равно запустить расчет?"))
                                return;    
                        }
                    }
                    else
                    {
                        var dlen = Convert.ToInt32(ThreadC.PeriodEnd.Subtract(ThreadC.PeriodBegin).TotalDays);
                        if (dlen > 5 &&  !Different.MessageQuestion("Задан очень длинный интервал расчета (" + dlen + " суток). Вы уверены, что хотите провести расчет за этот интервал?"))
                            return;
                        if ((ThreadC.PeriodEnd > ThreadC.SourcesEnd || ThreadC.PeriodBegin < ThreadC.SourcesBegin) &&
                            !Different.MessageQuestion("Указанный интервал расчета не попадает в диапазон источников. Все равно провести расчет?"))
                            return;
                    }    
                }
                if (b) ThreadC.StartCalc();
                else Different.MessageError("Недопустимые границы или длительность периода расчета");
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка при запуске расчета");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                App.ThreadWindows.Remove(ThreadC.Id);
            }
            catch {}
        }

        private void butUpdateSources_Click(object sender, RoutedEventArgs e)
        {
            ThreadC.RefreshTime();
        }

        private void butBreak_Click(object sender, RoutedEventArgs e)
        {
            ThreadC.BreakCalc();
        }

        private void periodBegin_TextChanged(object sender, TextChangedEventArgs e)
        {
            DateTime res;
            if (DateTime.TryParse(periodBegin.Text, out res))
                ThreadC.PeriodEnd = ThreadC.PeriodBegin.AddMinutes(ThreadC.PeriodLength);
        }

        private void butHandInput_Click(object sender, RoutedEventArgs e)
        {
            var w = ThreadC.HandInputWindow;
            if (w != null)
            {
                w.Focus();
                if (w.WindowState == WindowState.Minimized)
                    w.WindowState = WindowState.Normal;
            }
            else
            {
                w = new HandInputWindow();
                ThreadC.HandInputWindow = w;
                w.LoadForInput(ThreadC, false);
                w.Show();
            }
        }

        private void butAbsolute_Click(object sender, RoutedEventArgs e)
        {
            var w = ThreadC.HandInputWindow;
            if (w != null)
            {
                w.Focus();
                if (w.WindowState == WindowState.Minimized)
                    w.WindowState = WindowState.Normal;
            }
            else
            {
                w = new HandInputWindow();
                ThreadC.HandInputWindow = w;
                w.LoadForInput(ThreadC, true);
                w.Show();
            }
        }

        private void butSetup_Click(object sender, RoutedEventArgs e)
        {
            var w = ThreadC.SetupWindow;
            if (w != null)
            {
                w.Focus();
                if (w.WindowState == WindowState.Minimized)
                    w.WindowState = WindowState.Normal;
            }
            else
            {
                ThreadC.BeginSetup();
                ThreadC.SetupWindow = new SetupWindow { DataContext = ThreadC, ViewModel = ThreadC, Title = "Поток " + ThreadC.Id + ". Настройка" };
                ThreadC.SetupWindow.Show();   
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ThreadC.RefreshTime();
        }

        private void butMessages_Click(object sender, RoutedEventArgs e)
        {
            var w = ThreadC.ErrorsListWindow;
            if (w != null)
            {
                w.Focus();
                if (w.WindowState == WindowState.Minimized)
                    w.WindowState = WindowState.Normal;
            }
            else
            {
                ThreadC.LoadErrorsList();
                ThreadC.ErrorsListWindow = new ErrorsListWindow { DataContext = ThreadC, ViewModel = ThreadC };
                ThreadC.ErrorsListWindow.Show();
            }
        }

        private void butArchiveRanges_Click(object sender, RoutedEventArgs e)
        {
            foreach (var p in ThreadC.Providers)
                if (p.ProviderType == ProviderType.Archive)
                {
                    var ar = (IArchive)General.RunProvider(p.Code, p.Name, p.Inf, (ThreadCalc)DataContext);
                    string s = "Диапазоны архива результатов\n";
                    foreach (var proj in ThreadC.Projects.Values)
                    {
                        var ranges = ar.ReadRanges(proj.Code, ReportType.Calc);
                        s += "\nПроект: " + proj.Code + "\n";
                        if (ranges.Count == 0) s += "Архив пустой\n";
                        foreach (var range in ranges)
                            s += range.Key.ToRussian() + ": " + range.Value.Begin + " - " + range.Value.End + "\n";
                    }
                    Different.MessageInfo(s);
                }
        }
    }
}
