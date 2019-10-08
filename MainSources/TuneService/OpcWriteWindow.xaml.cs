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
using CommonTypes;

namespace TuneService
{
    /// <summary>
    /// Логика взаимодействия для OpcWriteWindow.xaml
    /// </summary>
    public partial class OpcWriteWindow : Window
    {
        public OpcWriteWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {


            using (var rec = new RecDao(Different.GetInfoTaskDir() + @"General\Config.accdb",
                                        "SELECT SysSubTabl.SubParamRowSource FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId " +
                                        "WHERE ((SysTabl.ParamValue='Приемник') AND (SysSubTabl.SubParamName='OPCServerName'));"))
                while (rec.Read())
                {
                    var set = rec.GetString("SubParamRowSource").ToPropertyHashSet();
                    foreach (var s in set.Values)
                        if (!OpcServerName.Items.Contains(s))
                            OpcServerName.Items.Add(s);
                }
            OpcServerName.Items.Add("Matrikon.OPC.Simulation.1");

            using (var sys = new SysTabl(Different.GetInfoTaskDir() + @"Controller\ControllerData.accdb"))
            {
                OpcServerName.Text = sys.SubValue("DebugWriteOPC", "OpcServerName");
                Node.Text = sys.SubValue("DebugWriteOPC", "Node");
                OpcTag.Text = sys.SubValue("DebugWriteOPC", "OpcTag");
                DataType.Text = sys.SubValue("DebugWriteOPC", "DataType");
                TagValue.Text = sys.SubValue("DebugWriteOPC", "TagValue");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            using (var sys = new SysTabl(Different.GetInfoTaskDir() + @"Controller\ControllerData.accdb"))
            {
                sys.PutSubValue("DebugWriteOPC", "OpcServerName", OpcServerName.Text);
                sys.PutSubValue("DebugWriteOPC", "Node", Node.Text);
                sys.PutSubValue("DebugWriteOPC", "OpcTag", OpcTag.Text);
                sys.PutSubValue("DebugWriteOPC", "DataType", DataType.Text);
                sys.PutSubValue("DebugWriteOPC", "TagValue", TagValue.Text);
            }
        }

        private void ButCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opc = new DebugOpcServer(OpcServerName.Text, Node.Text);
                if (opc.Check()) MessageBox.Show("Успешное соединение");
                else Different.MessageError("Ошибка соединения." + Environment.NewLine + opc.Logger.Command.ErrorMessage());
                opc.Dispose();
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка соединения");
            }
        }

        private void ButWriteOpc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dt = DataType.Text.ToDataType();
                if (!TagValue.Text.IsOfType(dt))
                    Different.MessageError("Недопустимое значение для указанного типа данных");
                else
                {
                    var opc = new DebugOpcServer(OpcServerName.Text, Node.Text);
                    opc.Logger.Start();
                    opc.Connect();
                    opc.AddGroup("Check");
                    opc.AddSignalByTag(OpcTag.Text, dt, TagValue.Text);
                    opc.Prepare();
                    opc.WriteValues();
                    if (!opc.Logger.Command.IsError) MessageBox.Show("Значение успешно записано");
                    else Different.MessageError("Ошибка при записи значения." + Environment.NewLine + opc.Logger.Command.ErrorMessage());
                    opc.Dispose();    
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка при записи значения");
            }
        }
    }
}
