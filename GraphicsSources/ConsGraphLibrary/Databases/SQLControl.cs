using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace BaseLibrary
{
    public partial class SQLControl : UserControl
    {
        public SQLControl()
        {
            InitializeComponent();
        }

        public string Identification
        {
            get { return Ident.Text; }
            set 
            { 
                Ident.Text = value;
                bool b = Ident.Text != "Windows";
                Login.Enabled = b;
                Password.Enabled = b;
            }
        }

        private void Ident_SelectedIndexChanged(object sender, EventArgs e)
        {
            Identification = Ident.Text;
        }

        //Функция для для дополнительной проверки корректности соединения
        public Func<SqlProps, bool> AdditionalCheck { get; set; }

        private void butCheck_Click(object sender, EventArgs e)
        {
            var props = new SqlProps(ServerName.Text, DatabaseName.Text, Identification != "Windows", Login.Text, Password.Text);
            try
            {
                SqlDb.Connect(ServerName.Text, DatabaseName.Text, Identification != "Windows", Login.Text, Password.Text).Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Соединение не установлено\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            try
            {
                if (AdditionalCheck != null && !AdditionalCheck(props))
                    MessageBox.Show("Соединение не установлено", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                else MessageBox.Show("Успешное соединение", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("База данных не является архивом результатов или к ней нет доступа" + "\n" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        
        private void ServerName_DropDown(object sender, EventArgs e)
        {
            ServerName.Items.Clear();
            DataTable sqlSources = SqlDataSourceEnumerator.Instance.GetDataSources();
            foreach (DataRow source in sqlSources.Rows)
            {
                string instanceName = source["InstanceName"].ToString();
                if (!instanceName.IsEmpty())
                    ServerName.Items.Add(source["InstanceName"].ToString() + '\\' + source["ServerName"]);
                else
                    ServerName.Items.Add(source["ServerName"].ToString() + source["Version"]);
            }
        }

        private void DatabaseName_DropDown(object sender, EventArgs e)
        {
            try
            {
                DatabaseName.Items.Clear();
                using (var con = SqlDb.ConnectServer(ServerName.Text, Identification != "Windows", Login.Text, Password.Text))
                {
                    DataTable databases = con.GetSchema("Databases");
                    foreach (DataRow database in databases.Rows)
                        DatabaseName.Items.Add(database.Field<String>("database_name"));
                }
            }
            catch { }
        }
    }
}
