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
using Calculation;
using BaseLibrary;

namespace Controller
{
    /// <summary>
    /// Логика взаимодействия для HandInputWindow.xaml
    /// </summary>
    public partial class HandInputWindow : Window
    {
        public HandInputWindow()
        {
            InitializeComponent();
        }

        private bool _isAbsolute;
        //Загрузка параметров и подготовка формы, если ввод абсолютных значений _isAbsolute=true, иначе ручной ввод
        public void LoadForInput(ThreadController thread, bool isAbsolute)
        {
            DataContext = thread;
            _isAbsolute = isAbsolute;
            if (isAbsolute)
            {
                thread.State = State.AbsoluteEdit;
                gridParams.Columns[5].Visibility = Visibility.Visible; //Время
                gridParams.Columns[6].Visibility = Visibility.Visible; //Значение
                gridParams.Columns[7].Visibility = Visibility.Visible; //Старое время
                gridParams.Columns[8].Visibility = Visibility.Visible; //Старое значение
                gridParams.Columns[9].Visibility = Visibility.Collapsed; //По умолчанию
                Title = "Поток " + thread.Id + ". Редактирование абсолютных значений";
                thread.LoadAbsolute();
                thread.CurrentOperation = "Редактирование абсолютных значений";
            }
            else
            {
                thread.State = State.HandInput;
                gridParams.Columns[5].Visibility = Visibility.Collapsed; //Время
                gridParams.Columns[6].Visibility = Visibility.Collapsed; //Значение
                gridParams.Columns[7].Visibility = Visibility.Collapsed; //Старое время
                gridParams.Columns[8].Visibility = Visibility.Collapsed; //Старое значение
                gridParams.Columns[9].Visibility = Visibility.Visible; //По умолчанию
                Title = "Поток " + thread.Id + ". Параметры ручного ввода";
                thread.StartAtom(Atom.LoadHandInput, thread.LoadHandInput);
                thread.CurrentOperation = "Ручной ввод значений";
            }
        }

        private bool FilterParam(Object item)
        {
            var par = (GridInputParam) item;
            bool e = true;
            e &= par.Project.CheckFilter(FilterProject.Text, false);
            e &= par.Code.CheckFilter(FilterCode.Text);
            e &= par.Name.CheckFilter(FilterName.Text);
            e &= par.DataType.CheckFilter(FilterDataType.Text);
            e &= par.Task.CheckFilter(FilterTask.Text);
            return e;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (ValuesVindow != null)
                {
                    if (ValuesVindow.Visibility == Visibility.Visible) 
                        ((GridInputParam)ValuesVindow.DataContext).SaveHandValues();
                    ValuesVindow.Close();
                    ValuesVindow = null;
                }
            }
            catch { }
            var thread = (ThreadController)DataContext;
            if (_isAbsolute)
            {
                butFilter.Focus();
                bool correct = true;
                foreach (var gip in thread.GridInputParams)
                {
                    correct &= gip.ValueCorrect || gip.Value == null;
                    correct &= !_isAbsolute || gip.TimeCorrect || gip.Time == null;
                    if (gip.Time.IsEmpty() && !gip.Value.IsEmpty())
                        gip.Time = gip.OldTime;
                }
                if (correct || Different.MessageQuestion("Не все введенные значения соответствуют типу данных. Хотите закрыть форму и сохранить ввод?"))
                    thread.StartAtom(Atom.SaveAbsolute, thread.SaveAbsolute);
                else e.Cancel = true;
            }
            thread.State = State.Stopped;
            thread.HandInputWindow = null;
        }

        private bool _filtered;

        private void butFilter_Click(object sender, RoutedEventArgs e)
        {
            _filtered = !_filtered;
            butFilter.Content = _filtered ? "Снять" : "Фильтр";
            var view = (ListCollectionView)CollectionViewSource.GetDefaultView(gridParams.ItemsSource);
            if (_filtered) view.Filter = FilterParam;
            else view.Filter = null;
        }

        //Окно редактирования значений ручного ввода
        public HandValuesWindow ValuesVindow { get; set; }

        private void gridParams_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_isAbsolute || gridParams.SelectedItem == null || (ValuesVindow != null && ValuesVindow.Visibility == Visibility.Visible)) return;
            var p = (GridInputParam)gridParams.SelectedItem;
            var thread = (ThreadController)DataContext;
            p.ValuesFromClone(thread.Projects[p.Project].HandInputProvider.ProviderInstance);
            if (ValuesVindow == null) ValuesVindow = new HandValuesWindow();
            ValuesVindow.DataContext = p;
            ValuesVindow.Show();
        }
    }
}
