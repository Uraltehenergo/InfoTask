using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Controller
{
    /// <summary>
    /// Логика взаимодействия для SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
        }


        public ThreadCalc ViewModel { get; set; }

        //Ссылка на поток
        private ThreadController ThreadC { get { return (ThreadController)DataContext; } }

        
        private void butAddFile_Click(object sender, RoutedEventArgs e)
        {
            ThreadC.AddProject();
        }

        private void butDel_Click(object sender, RoutedEventArgs e)
        {
            if (gridProjects.SelectedItem == null) return;
            var p = (Project)gridProjects.SelectedItem;
            if (MessageBox.Show("Удалить проект " + p.Code + "?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                ThreadC.DeleteProject(p);
        }

        private void butExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (gridProjects.SelectedItem == null) return;
                var p = (Project)gridProjects.SelectedItem;
                var list = new List<ThreadController>();
                foreach (var t in ThreadC.App.Threads)
                {
                    if (t is ThreadController && (t.State == State.Stopped || t.State == State.Setup) && !t.Projects.ContainsKey(p.Code))
                        list.Add((ThreadController)t);
                }
                (new ExportThreadWindow
                {
                    Project = (Project)gridProjects.SelectedItem,
                    Thread = ThreadC,
                    Threads = list
                }).Show();
            }
            catch { }
        }

        private void gridProviders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (gridProviders.CurrentItem == null || gridProviders.CurrentColumn == null) return;
                var s = (string)gridProviders.CurrentColumn.Header;
                if (s == "Свойства" || s == "Тип" || s == "Имя")
                    ThreadC.SetupProvider(((Provider)gridProviders.CurrentItem));
            }
            catch { }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gridProviders.CurrentItem == null) return;
            if (e.AddedItems == null || e.AddedItems.Count == 0) return;
            ((Provider)gridProviders.CurrentItem).Code = (string)e.AddedItems[0];
        }

        private void gridProjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            butAddFile.Focus();
            ThreadC.SaveSetup();
            ThreadC.CheckSetup();
            ThreadC.SetupWindow = null;
            ThreadC.ReadTime();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ButClearArchive.Visibility = General.DebugMode ? Visibility.Visible : Visibility.Hidden;
        }

        private void ButClearArchive_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var p in ThreadC.Providers)
                    if (p.ProviderType == ProviderType.Archive)
                    {
                        var ar = (IArchive)General.RunProvider(p.Code, p.Name, p.Inf, (ThreadCalc)DataContext);
                        string s = "Произвести очистку архива?\nПровайдер: " + p.Code + "\n";
                        foreach (var proj in ThreadC.Projects.Values)
                        {
                            var ranges = ar.ReadRanges(proj.Code, ReportType.Calc);
                            s += "\nПроект: " + proj.Code + "\n";
                            if (ranges.Count == 0) s += "Архив пустой\n";
                            foreach (var range in ranges)
                                s += range.Key.ToRussian() + ": " + range.Value.Begin + " - " + range.Value.End + "\n";
                        }
                        if (Different.MessageQuestion(s))
                        {
                            ar.ClearArchive();
                            Different.MessageInfo("Архив успешно очищен");    
                        }
                    }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Не удалось очистить архив \n" + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
