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

namespace Controller
{
    /// <summary>
    /// Логика взаимодействия для ExportThreadWindow.xaml
    /// </summary>
    public partial class ExportThreadWindow : Window
    {
        public ExportThreadWindow()
        {
            InitializeComponent();
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            if (gridThreads.SelectedItem == null)
                Different.MessageError("Не выбран поток");
            else
            {
                Thread.ExportProject((ThreadController) gridThreads.SelectedItem, Project);
                Close();   
            }
        }

        //Поток, из которого переносится проект
        public ThreadController Thread { get; set; }
        //Переносимый проект
        public Project Project { get; set; }
        //Список потоков
        public List<ThreadController> Threads
        {
            get { return (List<ThreadController>)gridThreads.ItemsSource; }
            set { gridThreads.ItemsSource = value; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
