using System;
using System.Windows.Forms;

namespace Tablik
{
    internal partial class TablikForm : Form
    {
        public TablikForm()
        {
            InitializeComponent();
        }

        //Установка значений разных конторолов и окна
        //private delegate void StatusDelegate();
        public void SetOperaton(string s)
        {
            //Operation.Invoke(new StatusDelegate(() => { Operation.Text = s; }));
            Operation.Text = s;
            Update();
        }
        public void SetProcent(double d)
        {
            //Indicator.Invoke(new StatusDelegate(() => { Indicator.Value = Convert.ToInt32(d); }));
            Indicator.Value = Convert.ToInt32(d);
            Indicator.Refresh();
            Refresh();
        }
    }
}
