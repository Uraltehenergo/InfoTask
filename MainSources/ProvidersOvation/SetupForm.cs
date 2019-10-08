using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;

namespace Provider
{
    public partial class SetupForm : Form
    {
        public SetupForm()
        {
            InitializeComponent();
        }

        //Ссылка на провайдер
        private OvationHistorianSource _provider;
        internal OvationHistorianSource Provider
        {
            get { return _provider; }
            set
            {
                _provider = value;
                var dic = value.Inf.ToPropertyDicS();
                dic.DefVal = "";
                DataSource.Text = dic["DataSource"];
                DatabaseFile.Text = dic["DatabaseFile"];
                if (dic["TypeOfData"] == "Оригинал") IsOriginal.Checked = true;
                else IsCopy.Checked = true;
            }
        }

        //True, если нажата OK, false, если Cancel
        private bool _isOk;

        private void butOK_Click(object sender, EventArgs e)
        {
            _isOk = true;
            Close();  
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            _isOk = false;
            Close();
        }

        private void butCheck_Click(object sender, EventArgs e)
        {
            using (Provider.Logger.Start())
            {
                Provider.DataSource = DataSource.Text ?? "";
                if (Provider.Check()) MessageBox.Show("Соединение установлено");
                else MessageBox.Show(Provider.Logger.Command.ErrorMessage(), "Соединение не установлено");    
            }
        }

        private void SetupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Provider.Inf = !_isOk ? Provider.Inf : 
                (new Dictionary<string, string>
                {
                    {"DataSource", DataSource.Text ?? "" }, 
                    {"DatabaseFile", DatabaseFile.Text ?? "" },
                    {"TypeOfData", IsOriginal.Checked ? "Оригинал" : "Клон" }
                }).ToPropertyString();
            Provider.IsSetup = false;
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {
        }

        private void butOpenDatabase_Click(object sender, EventArgs e)
        {
            var op = new OpenFileDialog
            {
                AddExtension = true,
                CheckFileExists = true,
                DefaultExt = "accbd",
                Multiselect = false,
                Title = @"Файл базы данных клона архива Ovation Historian",
                Filter = @"Файлы MS Access (.accdb) | *.accdb"
            };
            op.ShowDialog();
            if (DaoDb.Check(op.FileName, new[] {"CFG_PT_NAME", "PT_ATTRIB", "PT_HF_HIST"}))
                DatabaseFile.Text = op.FileName;
            else Different.MessageError("Указан недопустимый файл клона архива", op.FileName);
        }
    }
}
