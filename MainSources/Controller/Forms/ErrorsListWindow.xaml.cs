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
    /// Логика взаимодействия для ErrorsListWindow.xaml
    /// </summary>
    public partial class ErrorsListWindow : Window
    {
        public ErrorsListWindow()
        {
            InitializeComponent();
        }

        public ThreadCalc ViewModel { get; set; }
        //Ссылка на поток
        private ThreadController ThreadC { get { return (ThreadController)DataContext; } }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ThreadC.CloseErrorsList();
            ThreadC.ErrorsListWindow = null;
        }

        private void butDeleteAll_Click(object sender, RoutedEventArgs e)
        {
            if (Different.MessageQuestion("Удалить все сообщения из списка?"))
                ThreadC.GridErrors.Clear();
        }
    }
}
