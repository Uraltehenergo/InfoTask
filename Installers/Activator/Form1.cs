using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Activator
{
    public partial class Form1 : Form
    {
        private const string Ver = "1.3";
        //Номер генерируется как GUID и берутся первые 8 символов - порядковый номер лицензии, пароль - средние 12 символов из GUID
        //private const string LicenseNumber = "721BF783-0015";
        //private const string Password = "4AC7-4AD3-B79E";
        //private const string Company = "Казанская ТЭЦ-1";
        private const string Password = "0000-0000-0000";
        private const string LicenseNumber = "00000000-0001";
        private const string Company = "ЗАО \"ИЦ \"УралТехЭнерго\"";
        //private const string LicenseNumber = "D08855DB-0016";
        //private const string Password = "213E-42DE-BC3B";
        //private const string Company = "Тюменская ТЭЦ-2";
        private const bool ActivateAnalyzer = true;
        private const bool ActivateConstructor = true;
        private const bool ActivateReporter = true;
        private const bool ActivateMonitor = true;
        private const bool ActivateViewer = true;
        private const bool ActivateRas = true;
        private const bool ActivateRasInfoTask = true;
        private const bool ActivateProjectManager = true;
        private const bool ActivateAnalyzerInfoTask = true;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length >= 4) textBox2.Focus();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length >= 4) textBox3.Focus();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length >= 4) button1.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            User.Text = Company;
            Version.Text = Ver;
            LicNumber.Text = LicenseNumber;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey KernelExistenceKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\InfoTask", false);
                if (KernelExistenceKey.GetValue("KernelExistence") == null)
                {
                    KernelExistenceKey.Close();
                    MessageBox.Show("Сначала установите InfoTask");
                }
                else
                {
                    KernelExistenceKey.Close();
                    if (textBox1.Text.Length != 4 || textBox2.Text.Length != 4 || textBox3.Text.Length != 4)
                        MessageBox.Show("Введены не все символы");
                    else
                    {
                        string curCode = textBox1.Text + "-" + textBox2.Text + "-" + textBox3.Text;
                        if (curCode.Equals(Password))
                        {
                            RegistryKey newKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\UAI");
                            newKey.SetValue("cp", Encrypt(Company, 10), RegistryValueKind.String);
                            newKey.SetValue("un", Encrypt(LicenseNumber, 11), RegistryValueKind.String);
                            newKey.SetValue("ra", Encrypt(ActivateAnalyzer.ToString(), 12), RegistryValueKind.String);
                            newKey.SetValue("rc", Encrypt(ActivateConstructor.ToString(), 13), RegistryValueKind.String);
                            newKey.SetValue("ri", Encrypt(ActivateRasInfoTask.ToString(), 14), RegistryValueKind.String);
                            newKey.SetValue("rr", Encrypt(ActivateReporter.ToString(), 15), RegistryValueKind.String);
                            newKey.SetValue("rm", Encrypt(ActivateMonitor.ToString(), 16), RegistryValueKind.String);
                            newKey.SetValue("rv", Encrypt(ActivateViewer.ToString(), 17), RegistryValueKind.String);
                            newKey.SetValue("sr", Encrypt(ActivateRas.ToString(), 18), RegistryValueKind.String);
                            newKey.SetValue("pm", Encrypt(ActivateProjectManager.ToString(), 19), RegistryValueKind.String);
                            newKey.SetValue("ai", Encrypt(ActivateAnalyzerInfoTask.ToString(), 20), RegistryValueKind.String);
                            SetDate();
                            bool flag = MessageBox.Show("InfoTask успешно активирован","Активация InfoTask", MessageBoxButtons.OK) == DialogResult.OK;
                            if (flag) Close();
                        }
                        else MessageBox.Show("Неверный пароль");
                    }
                }
            }
            catch (Exception) { }
        }

        private static string _alphabet;

        private static void _alphabetFill()
        {
            _alphabet = "";
            for (int i = 32; i <= 125; i++)
            {
                _alphabet += char.ConvertFromUtf32(i);
            }
            _alphabet += "Ё«»ёАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя";
        }

        private string Encrypt(string str, int seed)
        {
            _alphabetFill();
            Random random = new Random(seed);
            string crypted = _alphabet[(int)(random.NextDouble() * 162)].ToString();
            for (int i = 1; i <= str.Length; i++)
            {
                int rnd = (int)(random.NextDouble() * 162);
                crypted += _alphabet[rnd].ToString() +
                           _alphabet[(rnd + _alphabet.IndexOf(str[i - 1]) + 1) % 162].ToString();
            }
            if (crypted.Length == 1)
            {
                crypted = "";
                int rnd = (int)(random.NextDouble() * 50);
                for (int j = 0; j < rnd * 2; j++)
                {
                    crypted += _alphabet[(int)(random.NextDouble() * 162)].ToString();
                }
            }
            return crypted;
        }

        //Запись в реестр даты установки
        public void SetDate()
        {
            RegistryKey dateKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\UAI", false);
            if (dateKey == null || dateKey.GetValue("id") == null || (string)dateKey.GetValue("id") == "")
            {
                dateKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\UAI");
                dateKey.SetValue("id", Encrypt(DateTime.Now.ToOADate().ToString(), 17), RegistryValueKind.String);
            }
            dateKey.Close();
        }
    }
}
