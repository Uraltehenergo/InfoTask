using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AuditMonitor
{
    internal class ClassVirtualChannelAdam4019Plus
    {
        //Канал
        private string _inputRange;
        
        public string InputRange
        {
            get { return _inputRange; }
            set
            {
                switch (value)
                {
                    case "02": // +-100 mV
                    case "03": // +-500 mV
                    case "04": // +-1 V
                    case "05": // +-2.5 mV
                    case "07": // +4 ~ 20 mA
                    case "08": // +-10 V
                    case "09": // +-5 V
                    case "0D": // +-20 mA
                    case "0E": // Type J Thermocouple 0 ~ 760 C
                    case "0F": // Type K Thermocouple 0 ~1370 C
                    case "10": // Type T Thermocouple -100 ~ 400 C
                    case "11": // Type E Thermocouple 0 ~ 1000 C
                    case "12": // Type R Thermocouple 500 ~ 1750 C
                    case "13": // Type S Thermocouple 500 ~ 1750 C
                    case "14": // Type B Thermocouple 500 ~ 1800 C
                        _inputRange = value;
                        break;
                }
            }
        }

        public bool SetInputRange(string inputRange)
        {
            switch (inputRange)
            {
                case "02": // +-100 mV
                case "03": // +-500 mV
                case "04": // +-1 V
                case "05": // +-2.5 mV
                case "07": // +4 ~ 20 mA
                case "08": // +-10 V
                case "09": // +-5 V
                case "0D": // +-20 mA
                case "0E": // Type J Thermocouple 0 ~ 760 C
                case "0F": // Type K Thermocouple 0 ~1370 C
                case "10": // Type T Thermocouple -100 ~ 400 C
                case "11": // Type E Thermocouple 0 ~ 1000 C
                case "12": // Type R Thermocouple 500 ~ 1750 C
                case "13": // Type S Thermocouple 500 ~ 1750 C
                case "14": // Type B Thermocouple 500 ~ 1800 C
                    _inputRange = inputRange;
                    return true;
                default:
                    return false;
            }
        }
    }

    internal class ClassVirtualAdamModule4019Plus
    {
        static private readonly Random Rnd = new Random();
        
        //Устройство
        private readonly string _address;
        private const string Name = "4019P";
        private const string FirmwareVersion = "1.0";
        private string _baudRate;
        private string _inputRange;
        private string _dataFormat;
        private bool _checkSumStatus;
        private bool _integrationTime; 
        private readonly ClassVirtualChannelAdam4019Plus[] _channels = new ClassVirtualChannelAdam4019Plus[8];
        
        private bool SetBaudRate(string baudRate)
        {
            switch (baudRate)
            {
                case "03": // 1200 bps
                case "04": // 2400 bps
                case "05": // 4800 bps
                case "06": // 9600 bps
                case "07": // 19.2 kbps
                case "08": // 38.4 kbps
                    _baudRate = baudRate;
                    return true;
                default:
                    return false;
            }
        }

        private bool SetInputRange(string inputRange)
        {
            switch (inputRange)
            {
                case "00":
                case "FF":
                    _inputRange = inputRange;
                    return true;
                default:
                    return false;
            }
        }
        
        private bool SetDataFormat(string dataFormat)
        {
            switch (dataFormat)
            {
                case "0": // Engineering units
                case "1": // % of FSR
                case "2": // two's complement of hexadecimal
                    _dataFormat = dataFormat;
                    return true;
                default:
                    return false;
            }
        }

        private bool SetF(string hF)
        {
            switch (hF)
            {
                case "0":
                    _checkSumStatus = false;
                    _integrationTime = false;
                    return true;
                case "4":
                    _checkSumStatus = true ;
                    _integrationTime = false;
                    return true;
                case "8":
                    _checkSumStatus = false;
                    _integrationTime = true;
                    return true;
                case "C":
                    _checkSumStatus = true;
                    _integrationTime = true;
                    return true;
                default:
                    return false;
            }
        }

        private string GetFf()
        {
            //Возвращает подстроку FF в команде $AA2
            //--var b = (byte) ((ClassAbstractAdamModule.HexToByte(_dataFormat) ?? 0) +
            //--                (_checkSumStatus ? Math.Pow(2, 6) : 0) + (_integrationTime ? Math.Pow(2, 7) : 0));
            //--return ClassAbstractAdamModule.ByteToHex(b);

            var b = (byte) ((NewModuleAbstract.HexToByte(_dataFormat) ?? 0) +
                            (_checkSumStatus ? Math.Pow(2, 6) : 0) + (_integrationTime ? Math.Pow(2, 7) : 0));
            return NewModuleAbstract.ByteToHex(b);
        }

        public ClassVirtualAdamModule4019Plus(string address)
        {
            _address = address;
            _baudRate = "06";
            _inputRange = "FF";
            _dataFormat = "0";
            _checkSumStatus = false;
            _integrationTime = false;
            for (byte channel = 0; channel <= 7; channel ++)
            {
                _channels[channel] = new ClassVirtualChannelAdam4019Plus {InputRange = "02"};
            }
        }
        
        public string SendCommand(string command)
        {
            //Команда %AANNTTCCFF - Configuration
            if (((command.Length  == 11) || ((command.Length == 13) && (command.Substring(12) == "\n"))) && 
               (command.StartsWith("%" + _address)))
            {
                //string oldDevicAddress = _address;
                string oldInputRange = _inputRange;
                string oldBaudRate = _baudRate;
                string oldFF = GetFf();

                bool res = ((command.Substring(3, 2)) == _address);
                res = res & SetInputRange(command.Substring(5, 2));
                res = res & SetBaudRate(command.Substring(7, 2));
                res = res & SetF(command.Substring(9, 1));
                res = res & SetDataFormat(command.Substring(10, 1));

                if (res) return "!" + _address + (command.Length == 11 ? "" : "\n");
                //else
                {
                    SetInputRange(oldInputRange);
                    SetBaudRate(oldBaudRate);
                    SetDataFormat(oldFF.Substring(1, 1));
                    SetF(oldFF.Substring(0, 1));
                    return "!" + _address + (command.Length == 11 ? "" : "\n");
                }
            }

            //Команда $AA2 - Configuration Status
            if ((command == "$" + _address + "2") || (command == "$" + _address + "2\n"))
                return "!" + _address + _inputRange + _baudRate + GetFf() + (command.Length == 4 ? "" : "\n");
            
            //Команда $AAF - Read Firmware Version
            if ((command == "$" + _address + "F") || (command == "$" + _address + "F\n"))
                return "!" + _address + FirmwareVersion + (command.Length == 4 ? "" : "\n");
            
            //Команда $AAM - Read Module Name
            if ((command == "$" + _address + "M") || (command == "$" + _address + "M\n"))
                return "!" + _address + Name + (command.Length == 4 ? "" : "\n");
                      
            //Команда #AA - Analog Data In
            if ((command == "#" + _address) || (command == "#" + _address + "\n"))
            {
                string st = "";
                for (byte channel = 0; channel <= 7; channel++) st = st + GetChannelValue(channel);
                return ">" + st + (command.Length == 3 ? "" : "\n");
            }

            //Команда #AAN - Read Analog Input from Channel N
            if (((command.Length == 4) || ((command.Length == 6) && (command.Substring(5) == "\n"))) &&
               (command.StartsWith("#" + _address)))
            {
                if ("01234567".IndexOf(command.Substring(3, 1)) > -1)
                    return ">" + GetChannelValue(byte.Parse(command.Substring(3, 1))) + (command.Length == 4 ? "" : "\n");
                return "";
            }

            //Команда $AA5VV - Enable/disable Channels for Multiplexing command

            //Команда $AA6 - Read Channel Status command

            //Команда $AA0 - Span Calibration command

            //Команда $AA1 - Offset Calibration command
            
            //Команда $AAB - Channel Diagnose command

            //Команда $AA3 - CJC Status
            if ((command == "$" + _address + "3") || (command == "$" + _address + "3\n"))
            {
                double value = GetValue(1, 0.25, 0.75)*100;
                string st = value.ToString().Replace(",", ".");
                if (st.IndexOf(".") == -1) st = st + ".0";
                if (st.Length > 6) st = st.Substring(0, 6);
                else while (st.Length < 6) st = "0" + st;
                return ">+" + st + (command.Length == 4 ? "" : "\n");
            }

            //Команда $AA9SNNNN - CJC Offset Calibration

            //Команда $AA0Ci Single Channel Span Calibration

            //Команда $AA1Ci Single Channel Offset Calibration
                
            //Команда $AA7CiRrr - Single Channel Range Configuration
            if (((command.Length == 9) || ((command.Length == 10) && (command.Substring(10) == "\n"))) &&
               (command.StartsWith("$" + _address + "7C")) && (command.Substring(6, 1) == "R"))
            {
                bool res = false;
                if ("01234567".IndexOf(command.Substring(5, 1)) > -1)
                    res = _channels[byte.Parse(command.Substring(5, 1))].SetInputRange(command.Substring(7, 2));

                return (res ? "!" : "?") + _address + (command.Length == 9 ? "" : "\n");
            }
                
            //Команда $AA8Ci - Read Single Channel Range Configuration
            if (((command.Length == 6) || ((command.Length == 8) && (command.Substring(7) == "\n"))) &&
               (command.StartsWith("$" + _address + "8C")))
            {
                string st = command.Substring(5, 1);
                if ("01234567".IndexOf(st) > -1)
                {
                    byte channel = byte.Parse(st);
                    return "!" + _address + "C" + st + "R" + _channels[channel].InputRange + (command.Length == 6 ? "" : "\n");
                }
                return "?" + _address + (command.Length == 6 ? "" : "\n");
            }

            //Команда $AAXnnnn Set Communication WDT

            //Команда $AAY Get Communication WDT setting
            
            //Другая (неверная) команда
            return "";
        }
        
        private string GetChannelValue(byte channel)
        {
            //Генерирует случайное значение канала в зависимости от свойст устройства и канала
            
            //if ((channel == 7) && ((DateTime.Now.Second % 2) == 1)) return "+888888";
            /*(((DateTime.Now.Millisecond % 256) == ClassAbstractAdamModule.HexToByte(_address)) && */
            if ((DateTime.Now.Millisecond % 10) == channel) return "+888888";

            //--byte? a = ClassAbstractAdamModule.HexToByte(_address);
            byte? a = NewModuleAbstract.HexToByte(_address);
            double value = GetValue(3, 20 * (a != null ? (byte)a : 0) + 1, 2.5 * channel);
            
            switch (_dataFormat)
            {
                case "0": // Engineering units
                    switch (_channels[channel].InputRange)
                    {
                        case "02": // +-100 mV
                            value = 200*value - 100;
                            break;
                        case "03": // +-500 m
                            value = 1000*value - 500;
                            break;
                        case "04": // +-1 V
                            value = 2*value - 1;
                            break;
                        case "05": // +-2.5 mV
                            value = 5*value - 2.5;
                            break;
                        case "07": // +4 ~ 20 mA
                            value = 16*value + 4;
                            break;
                        case "08": // +-10 V
                            value = 20*value - 10;
                            break;
                        case "09": // +-5 V
                            value = 10*value - 5;
                            break;
                        case "0D": // +-20 mA
                            value = 40*value - 20;
                            break;
                        case "0E": // Type J Thermocouple 0 ~ 760 C
                            value = 760*value;
                            break;
                        case "0F": // Type K Thermocouple 0 ~1370 C
                            value = 1370*value;
                            break;
                        case "10": // Type T Thermocouple -100 ~ 400 C
                            value = 500*value - 100;
                            break;
                        case "11": // Type E Thermocouple 0 ~ 1000 C
                            value = 1000*value;
                            break;
                        case "12": // Type R Thermocouple 500 ~ 1750 C
                            value = 1250*value + 500;
                            break;
                        case "13": // Type S Thermocouple 500 ~ 1750 C
                            value = 1250*value + 500;
                            break;
                        case "14": // Type B Thermocouple 500 ~ 1800 C
                            value = 1300*value + 500;
                            break;
                    }
                    break;

                case "1": // % of FSR
                case "2": // two's complement of hexadecimal
                    switch (_channels[channel].InputRange)
                    {
                        case "02": // +-100 mV
                        case "03": // +-500 m
                        case "04": // +-1 V
                        case "05": // +-2.5 mV
                        case "08": // +-10 V
                        case "09": // +-5 V
                        case "0D": // +-20 mA
                            value = 200*value - 100;
                            break;
                        
                        case "07": // +4 ~ 20 mA
                        case "0E": // Type J Thermocouple 0 ~ 760 C
                        case "0F": // Type K Thermocouple 0 ~1370 C
                        case "10": // Type T Thermocouple -100 ~ 400 C
                        case "11": // Type E Thermocouple 0 ~ 1000 C
                        case "12": // Type R Thermocouple 500 ~ 1750 C
                        case "13": // Type S Thermocouple 500 ~ 1750 C
                        case "14": // Type B Thermocouple 500 ~ 1800 C
                            value = 100*value;
                            break;
                    }
                    break;
            }
            string stVal = "";
            switch (_dataFormat)
            {
                case "0": // Engineering units
                    stVal = (value >= 0) ? "+" + value : value.ToString();
                    stVal = stVal.Substring(0, 7).Replace(",", ".");
                    if (stVal.IndexOf(".") == -1) stVal = stVal + ".";
                    while (stVal.Length < 7) stVal = stVal + "0";
                    break;
                case "1": // % of FSR
                    value = Math.Round(value, 2);
                    stVal = (value >= 0) ? "+" + value : value.ToString();
                    stVal = stVal.Replace(",", ".");
                    int i = stVal.IndexOf(".");
                    if (i == -1)
                    {
                        stVal = stVal + "."; 
                        i = stVal.Length;
                    }
                    for(; i<=5; i++) stVal = stVal.Substring(0, 1) + "0" + stVal.Substring(1);
                    while (stVal.Length <= 7) stVal = stVal + "0";
                    break;
                case "2": // two's complement of hexadecimal
                    var val = (int)Math.Round(327.67 * value);
                    if (val < 0) val = (int)Math.Pow(16, 4) - val;
                    stVal = Convert.ToString(val, 16);
                    break;
            }
            return stVal;
        }

        private static double GetValue(byte lineType, double a = 1, double b = 0)
        {
            //Генерирует случайное значение для функции GetChannelValue в диапазоне 0 - 1
            //LineType - способ генерации: = 0 - случайно (a, b игнорируются)
            //                             = 1 - со случайным отклонением
            //                                   a - предыдущее значение
            //                                   b - зона нечувствительности (0 - 1)
            //                             = 2 - прямая с отражением
            //                                   a - время пробега интервала 0 - 1 (сек)
            //                                   b - начальный временной сдвиг (сек)
            //                             = 3 - синусоида
            //                                   a - период (сек)
            //                                   b - начальный временной сдвиг (сек)

            double value;
            switch (lineType)
            {
                case 1:
                    if ((a >= 0) && (a <= 1))
                    {
                        value = (Rnd.NextDouble() > b) ? (Rnd.NextDouble() + 0.5) * a : a;
                        if (value > 1) value = 1; 
                        else if (value < 0) value = 0;
                    }
                    else value = Rnd.NextDouble();
                    break;
                case 2:
                    value = ((DateTime.Now.TimeOfDay.TotalSeconds + b)%(2*a))/a;
                    if (value > 1) value = 2 - value;
                    break;
                case 3:
                    value = ((DateTime.Now.TimeOfDay.TotalSeconds + b)%a)/a;
                    value = (Math.Sin(value*2*Math.PI) + 1)*0.5;
                    break;
                default:
                    value = Rnd.NextDouble();
                    break;
            }
            return value;
        }
    }
}