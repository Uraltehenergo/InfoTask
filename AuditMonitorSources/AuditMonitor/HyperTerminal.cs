using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using MSCommLib;

namespace AuditMonitor
{
    class ClassHyperTerminal
    {
        #region Static
        public static string[] GetAvailablePorts()
        {
            var availablePorts = SerialPort.GetPortNames();
            for (int i = availablePorts.GetLowerBound(0); i <= availablePorts.GetUpperBound(0); i++)
                availablePorts[i] = availablePorts[i].Substring(3);
            return availablePorts;
        }
        #endregion

        private readonly MSComm _msComm = new MSCommClass();
        
        private int _moduleTimeOut;
        public int ModuleTimeOut //максимальное время ожидания модуля в милисекундах, 0 - бесконечно
        {
            get { return _moduleTimeOut; }
            set
            {
                var newVal = (value >= 0) ? value : 0;
                _moduleTimeOut = newVal;
            }
        }

        public byte Port { get; private set; }

        public ClassHyperTerminal(int moduleTimeOut) { Port = 0; ModuleTimeOut = moduleTimeOut; }
        public ClassHyperTerminal() : this(100) {}
        
        public bool PortOpen
        {
            get
            {
                if (Port == 0) return true;
                return _msComm.PortOpen;
            }
        }

        public byte OpenPort(byte portNum, bool showAlerts = true)
        {
            if (Port != 0) ClosePort();
            if (portNum != 0)
            {
                try
                {
                    _msComm.CommPort = portNum;
                    _msComm.PortOpen = true;
                    if (_msComm.PortOpen) Port = portNum;
                    if (showAlerts) MessageBox.Show(@"Порт " + Port + @" открыт");
                }
                catch
                {
                    Port = 0;
                    if (showAlerts) MessageBox.Show(@"Ошибка открытия порта " + portNum);
                }
            }

            return Port;
        }

        public void ClosePort(bool showAlerts = false)
        {
            if (Port != 0) _msComm.PortOpen = false;
            if (showAlerts) MessageBox.Show(@"Порт " + Port + @" закрыт");
            Port = 0;
        }
        
        public string SendCommand(string command, bool showAlert = false )
        {
            const char endL = (char) 13;
            
            if (PortOpen)
            {
                string stResp = "";
                _msComm.Output = command + endL;
                DateTime timeStart = DateTime.Now;
                double msPast = 0;

                try
                {
                    do
                    {
                        Application.DoEvents();
                        stResp = stResp + _msComm.Input;
                        msPast = DateTime.Now.Subtract(timeStart).TotalMilliseconds;
                    } while ((stResp.IndexOf(endL) < 0) && ((ModuleTimeOut == 0) || (msPast <= ModuleTimeOut)));
                }
                catch (Exception ex)
                {
                    if (showAlert) MessageBox.Show(@"CHyperTerminal.SendCommand Ошибка:" + "\n" + ex.Message);
                    return "";
                }

                if (msPast <= _moduleTimeOut)
                {
                    if (stResp.EndsWith(endL.ToString())) stResp = stResp.Substring(0, stResp.Length - 1);
                    return stResp;
                }
                if (showAlert) MessageBox.Show(@"CHyperTerminal.SendCommand Ошибка:" + "\n" + @"Устройство не отвечает");
                return "";
            }
            if (showAlert) MessageBox.Show(@"CHyperTerminal.SendCommand Ошибка:" + "\n" + @"HyperTerminal не инициализирован");
            return "";
        }
    }
}
