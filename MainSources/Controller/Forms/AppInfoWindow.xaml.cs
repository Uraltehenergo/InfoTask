using System;
using System.Collections.Generic;
using System.IO;
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
using System.Diagnostics;
using BaseLibrary;
using Calculation;
using VersionSynch;

namespace Controller
{
    /// <summary>
    /// Логика взаимодействия для AppInfoWindow.xaml
    /// </summary>
    public partial class AppInfoWindow : Window
    {
        public AppInfoWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var sys = new SysTabl(General.ConfigFile))
                    AppVersion.Text = sys.SubValue("InfoTask", "AppVersion");
                var dbv = new DbVersion();
                AppUserOrg.Text = dbv.AUser("Controller");
                LicenseNumber.Text = dbv.ANumber("Controller");
            }
            catch { }
        }

        private void ButBase_OnClick(object sender, RoutedEventArgs e)
        {
            Different.OpenDocument(General.DocsDir + "InfoTask-UG.01-Base.pdf");
        }

        private void ButController_Click(object sender, RoutedEventArgs e)
        {
            Different.OpenDocument(General.DocsDir + "InfoTask-UG.04-Controller.pdf");
        }
    }
}
