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

namespace Controller
{
    /// <summary>
    /// Логика взаимодействия для HandValuesWindow.xaml
    /// </summary>
    public partial class HandValuesWindow : Window
    {
        public HandValuesWindow()
        {
            InitializeComponent();
        }

        private void ButCancel_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            ((GridInputParam)DataContext).HandInputValues.Clear();
        }

        private void ButOK_Click(object sender, RoutedEventArgs e)
        {
            var p = (GridInputParam)DataContext;
            bool b = true;
            foreach (var v in p.HandInputValues)
                b &= v.ValueCorrect && v.TimeCorrect && v.Value.IsOfType(p.DataType.ToDataType());
            if (b || Different.MessageQuestion("Не все значения и времена введены верно. Продолжить сохранение значений?"))
            {
                p.SaveHandValues();
                Hide();
                p.HandInputValues.Clear();    
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            ((GridInputParam)DataContext).HandInputValues.Clear();
        }
    }
}
