﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AuditMonitor
{
    public enum EFlagState
    {
        No = 0,       //флаг не установлен
        Yes = 1,      //флаг установлен
        Stop = 2,     //сигнал на снятие
        Inhibit = 3   //запрет на установку
    }

    //class ClassChannelAdam4019Plus: ClassAbstractAdamChannel
    //{
    //    //свойства модуля
    //    private string _channelRange; //тип канала (настройка модуля)
    //    private string _inLevel;      //тип значения (определяет первичную обработку)                   

    //    public override string ChannelType { get { return "Adam4019+"; } }

    //    public override string ChannelRange
    //    {
    //        get{ return _channelRange; }
    //        set
    //        {
    //            switch(value)
    //            {
    //                case null:
    //                case "02": // +-100 mV
    //                case "03": // +-500 mV
    //                case "04": // +-1 V
    //                case "05": // +-2.5 mV
    //                case "07": // +4 ~ 20 mA
    //                case "08": // +-10 V
    //                case "09": // +-5 V
    //                case "0D": // +-20 mA
    //                case "0E": // Type J Thermocouple 0 ~ 760 C
    //                case "0F": // Type K Thermocouple 0 ~1370 C
    //                case "10": // Type T Thermocouple -100 ~ 400 C
    //                case "11": // Type E Thermocouple 0 ~ 1000 C
    //                case "12": // Type R Thermocouple 500 ~ 1750 C
    //                case "13": // Type S Thermocouple 500 ~ 1750 C
    //                case "14": // Type B Thermocouple 500 ~ 1800 C
    //                    _channelRange = value;
    //                    break;
    //                default:
    //                    _channelRange = null;
    //                    break;
    //            }
    //        }
    //    }

    //    public override string InLevel
    //    {
    //        get { return _inLevel; }
    //        set
    //        {
    //            switch (ChannelRange)
    //            {
    //                case "02":
    //                    switch (value)
    //                    {
    //                        case "ТХК K":
    //                        case "ТХК L":
    //                        case "ТХК R":
    //                        case "ТХК S":
    //                        case "ТХК B":
    //                        case "ТХК J":
    //                        case "ТХК T":
    //                        case "ТХК E":
    //                        case "ТХК N":
    //                        case "ТХК A1":
    //                        case "ТХК A2":
    //                        case "ТХК A3":
    //                        case "ТХК M":
    //                            _inLevel = value;
    //                            return;
    //                        default:
    //                            _inLevel = null;
    //                            return;
    //                    }
                    
    //                case "05":
    //                case "07":
    //                    switch (value)
    //                    {
    //                        case "4-20 mA":
    //                            _inLevel = value;
    //                            return;
    //                        default:
    //                            _inLevel = null;
    //                            return;
    //                    }
                    
    //                default:
    //                    base.InLevel = null;
    //                    break;
    //            }
    //        }
    //    }

    //    public override void CalcVal(string signal, ref EChannelStatus status, out double? val)
    //    {
    //        val = null;

    //        if (status == EChannelStatus.NoError)
    //        {
    //            if (signal != null)
    //            {
    //                switch (ChannelRange)
    //                {
    //                    case "0E": // Type J Thermocouple 0 ~ 760 C
    //                    case "0F": // Type K Thermocouple 0 ~1370 C
    //                    case "10": // Type T Thermocouple -100 ~ 400 C
    //                    case "11": // Type E Thermocouple 0 ~ 1000 C
    //                    case "12": // Type R Thermocouple 500 ~ 1750 C
    //                    case "13": // Type S Thermocouple 500 ~ 1750 C
    //                    case "14": // Type B Thermocouple 500 ~ 1800 C
    //                        if ((signal == "-0000") || (signal == "+9999"))
    //                        {
    //                            status = EChannelStatus.ThermocoupleOutOfRange;
    //                            return;
    //                        }
    //                        break;
    //                }

    //                if (/*(status == EChannelStatus.NoError) &&*/ (signal != "+888888") && (signal != "888888"))
    //                {
    //                    double value;
    //                    if (double.TryParse(signal, NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat,
    //                                        out value))
    //                    {
    //                        val = SignalTransform(value, out status);

    //                        //if ((status == EChannelStatus.NoError)&&(val!=null)) 
    //                        //    val = SignalConversion((double)val, out status);
    //                    }
    //                    else { status = EChannelStatus.SignalError; return; }
    //                }
    //                else { status = EChannelStatus.SignalError; return; }
    //            }
    //            else { status = EChannelStatus.ReadError; return; }
    //        }

    //        /*if (status != EChannelStatus.NoError)
    //        {
    //            NewValue = null;
    //            ValueChanged = (Channel.Value != null);
    //            Val = null;
    //            Value = null;
    //            Status = status;
    //        }*/
    //    }

    //    protected /*override*/ double? SignalTransform(double value, out EChannelStatus status)
    //    {
    //        double? val = value;
    //        EChannelStatus stat = EChannelStatus.NoError;

    //        switch (ChannelRange)
    //        {
    //            case "02":
    //                if ((InLevel != null) && (InLevel.StartsWith("ТХК ")))
    //                {
    //                    string thType = InLevel.Substring(4);

    //                    ClassAbstractAdamChannel cjc = Module.Cjc;
    //                    if ((cjc != null) && (cjc.Status == EChannelStatus.NoError) && (cjc.Value != null))
    //                        val = MVtoTh(thType, value, (double) cjc.Value, out stat);
    //                    else
    //                    {
    //                        val = null;
    //                        stat = EChannelStatus.TransformCjcError;
    //                    }
    //                }
    //                break;

    //                //switch (InLevel)
    //                //{
    //                //    case "ТХК L":
    //                //        ClassAbstractAdamChannel cjc = Module.Cjc;
    //                //        if ((cjc != null) && (cjc.Status == EChannelStatus.NoError) && (cjc.Value != null))
    //                //            val = MVtoThL(value, (double)cjc.Value, out stat);
    //                //        else
    //                //        {
    //                //            val = null;
    //                //            stat = EChannelStatus.TransformCjcError;
    //                //        }
    //                //        break;

    //                //    case "ТХК K":
    //                //        //cjc = Module.Cjc();
    //                //        cjc = Module.Cjc;
    //                //        if ((cjc != null) && (cjc.Status == EChannelStatus.NoError) && (cjc.Value != null))
    //                //            val = MVtoThK(value, (double)cjc.Value, out stat);
    //                //        else
    //                //        {
    //                //            val = null;
    //                //            stat = EChannelStatus.TransformCjcError;
    //                //        }
    //                //        break;

    //                //    default:
    //                //        value = newValue;
    //                //        status = EChannelStatus.NoError;
    //                //        break;
    //                //}
    //                //break;

    //            case "05":
    //                switch (InLevel)
    //                {
    //                    case "4-20 mA":
    //                        if ((Min != null) && (Max != null)) 
    //                            val = ChangeScale(value, 0.5, 2.5, (double)Min, (double)Max);
    //                        break;
    //                }
    //                break;

    //            case "07":
    //                switch (InLevel)
    //                {
    //                    case "4-20 mA":
    //                        if ((Min != null) && (Max != null))
    //                            val = ChangeScale(value, 4, 20, (double)Min, (double)Max);
    //                        break;
    //                }
    //                break;
    //        }

    //        status = stat;
    //        return val;
    //    }

    //    public ClassChannelAdam4019Plus(ClassAbstractAdamModule module, int channel) : base(module, channel)
    //    {
    //        _channelRange = null;
    //    }
    //}

    //class ClassModuleAdam4019Plus: ClassAbstractAdamModule
    //{
    //    private const int ChannelCount = 8;
        
    //    private string _dataFormat;                   //тип единиц измерения
    //    private string _inputRange;                   //входной диапазон канала ("FF")
    //    private string _baudRate;                     //скорость передачи данных ("06")
        
    //    public override string ModuleType{ get { return "Adam4019+"; } }
        
    //    public override string DataFormat
    //    {
    //        get { return _dataFormat; }
    //        set
    //        {
    //            if (value != null)
    //                _dataFormat = ((value.Length == 2) && ("048C".IndexOf(value.Substring(0, 1)) > -1) &&
    //                               ("012".IndexOf(value.Substring(1, 1)) > -1)) ? value : null;
    //            else
    //                _dataFormat = null;
    //        }
    //    }

    //    public override string InputRange
    //    {
    //        get{ return _inputRange; }
    //        set
    //        {
    //            switch(value)
    //            {
    //                case null:
    //                case "00":
    //                case "FF":
    //                    _inputRange = value;
    //                    break;
    //            }
    //        }
    //    }

    //    public override string BaudRate
    //    {
    //        get { return _baudRate; }
    //        set
    //        {
    //            switch (value)
    //            {
    //                case null:
    //                case "03": // 1200 bps
    //                case "04": // 2400 bps
    //                case "05": // 4800 bps
    //                case "06": // 9600 bps
    //                case "07": // 19.2 kbps
    //                case "08": // 38.4 kbps
    //                    _baudRate = value;
    //                    break;
    //            }
    //        }
    //    }
        
    //    public override ClassAbstractAdamChannel Cjc
    //    {
    //        get { return Channel[ChannelCount]; }
    //    }

    //    public ClassModuleAdam4019Plus(string address) : base(address, ChannelCount + 1)
    //    {
    //        byte? adress10 = HexToByte(address);
    //        Address = (adress10 != null) ? address : null;
    //        _dataFormat = null;
    //        _inputRange = null;
    //        _baudRate = null;

    //        for (byte channel = 0; channel < ChannelCount; channel++)
    //            Channel[channel] = new ClassChannelAdam4019Plus(this, channel);
    //        Channel[ChannelCount] = new ClassAdamCjcChannel(this, ChannelCount);
    //    }
        
    //    public override void ReadModuleSettings()
    //    {
    //        if (Address != null)
    //        {
    //            //DateTime time = DateTime.Now;
                
    //            string stIn = SendCommand("$" + Address + "2");
    //            if ((stIn.Length == 9) && (stIn.StartsWith("!" + Address)))
    //            {
    //                InputRange = stIn.Substring(3, 2);
    //                BaudRate = stIn.Substring(5, 2);
    //                DataFormat = stIn.Substring(7, 2);

    //                foreach (ClassAbstractAdamChannel channel in Channel)
    //                {
    //                    if (!channel.IsCjcChannel)
    //                    {
    //                        stIn = SendCommand("$" + Address + "8C" + channel.Channel);
    //                        if ((stIn.Length == 8) && (stIn.StartsWith("!" + Address + "C" + channel.Channel)))
    //                        {
    //                            channel.ChannelRange = stIn.Substring(6, 2);
    //                        }
    //                        else
    //                        {
    //                            ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ReadChannelsSettings",
    //                                             "Не удалось прочитать конфигурацию канала " + channel.Channel + " модуля " + Address
    //                                             + " (" + Address10 + ")", DateTime.Now);
    //                            channel.ChannelRange = null;
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ReadChannelsSettings",
    //                                 "Не удалось прочитать конфигурацию модуля " + Address + " (" + Address10 + ")"
    //                                 , DateTime.Now);

    //                InputRange = null;
    //                BaudRate = null;
    //                DataFormat = null;
    //                foreach (ClassAbstractAdamChannel channel in Channel)
    //                    if (!channel.IsCjcChannel) channel.ChannelRange = null; 
    //            }

    //            stIn = SendCommand("$" + Address + "M");
    //            Name = (stIn.StartsWith("!" + Address)) ? stIn.Substring(3) : null;
    //            if (Name == null)
    //                ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ReadChannelsSettings",
    //                                 "Не удалось прочитать имя модуля " + Address + " (" + Address10 + ")", DateTime.Now);
    //            stIn = SendCommand("$" + Address + "F");
    //            FirmwareVersion = (stIn.StartsWith("!" + Address)) ? stIn.Substring(3) : null;
    //            if (FirmwareVersion == null)
    //                ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ReadChannelsSettings",
    //                                 "Не удалось прочитать версию ПО модуля " + Address + " (" + Address10 + ")", DateTime.Now);
    //        }
    //    }

    //    public override void ProgramModule()
    //    {
    //        if (Address != null)
    //        {
    //            //DateTime time = DateTime.Now;
    //            int res = 0;

    //            string stIn = SendCommand("$" + Address + "2");
    //            if (stIn.StartsWith("!" + Address) && (stIn.Length == 9))
    //            {
    //                if (_inputRange == null) InputRange = stIn.Substring(3, 2);
    //                if (_baudRate == null) BaudRate = stIn.Substring(5, 2);
    //                if (_dataFormat == null) DataFormat = stIn.Substring(7, 2);
    //            }

    //            if (stIn != "!" + Address + _inputRange + _baudRate + _dataFormat)
    //            {
    //                stIn = SendCommand("%" + Address + Address + _inputRange + _baudRate + _dataFormat);
    //                if (stIn != "!" + Address)
    //                {
    //                    ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ProgramChannels",
    //                                     "Не удалось установить конфигурацию модуля " + Address + " (" + Address10 + ")",
    //                                     DateTime.Now);
    //                    res = res + 1;

    //                    stIn = SendCommand("$" + Address + "2");
    //                    if ((stIn.Length == 9) && (stIn.StartsWith("!" + Address)))
    //                    {
    //                        InputRange = stIn.Substring(3, 2);
    //                        BaudRate = stIn.Substring(5, 2);
    //                        DataFormat = stIn.Substring(7, 2);
    //                    }
    //                    else
    //                    {
    //                        ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ProgramChannels",
    //                                "Не удалось проситать конфигурацию модуля " + Address + " (" + Address10 + ")", 
    //                                DateTime.Now);
    //                        res = res + 2;

    //                        InputRange = null;
    //                        BaudRate = null;
    //                        DataFormat = null;

    //                        foreach (ClassAbstractAdamChannel channel in Channel)
    //                            if (!channel.IsCjcChannel) channel.ChannelRange = null;
    //                    }
    //                }
    //            }

    //            if ((res == 0) || (res == 1))
    //            {
    //                foreach (ClassAbstractAdamChannel channel in Channel)
    //                {
    //                    if (!channel.IsCjcChannel)
    //                    {
    //                        stIn = SendCommand("$" + Address + "8C" + channel.Channel);
    //                        if (stIn.StartsWith("!" + Address + "C" + channel.Channel + "R") && (stIn.Length == 8))
    //                            if (channel.ChannelRange == null) channel.ChannelRange = stIn.Substring(6, 2);

    //                        if (stIn != "!" + Address + "C" + channel.Channel + "R" + channel.ChannelRange)
    //                        {
    //                            stIn = SendCommand("$" + Address + "7C" + channel.Channel + "R" + channel.ChannelRange);
    //                            if (stIn != "!" + Address)
    //                            {
    //                                ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ProgramChannels",
    //                                                 "Не удалось установить конфигурацию канала " + channel.Channel +
    //                                                 " модуля " + Address + " (" + Address10 + ")", DateTime.Now);

    //                                stIn = SendCommand("$" + Address + "8C" + channel.Channel);
    //                                if ((stIn.Length == 8) && (stIn.StartsWith("!" + Address + "C" + channel.Channel + "R")))
    //                                    channel.ChannelRange = stIn.Substring(6, 2);
    //                                else
    //                                {
    //                                    ClassLog.AddLog(ELogType.ErrorModuleSettings, "ClassModuleAdam4019Plus.ProgramChannels",
    //                                             "Не удалось прочитать конфигурацию канала " + channel.Channel + " модуля " +
    //                                             Address + " (" + Address10 + ")", DateTime.Now);
    //                                    channel.ChannelRange = null;
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
        
    //    public /*override*/ void ReadModuleOld(bool readOnlySelected = true)
    //    {
    //        if (Address != null)
    //        {
    //            string stOut;
    //            string stIn;
    //            ClassChannelValue signalValue;

    //            Time = DateTime.Now;

    //            //CJC
    //            if (Cjc.Selected)
    //            {
    //                stOut = "$" + Address + "3";
    //                stIn = SendCommand(stOut);

    //                if ((stIn.Length == 8) && (stIn.StartsWith(">")))
    //                    signalValue = new ClassChannelValue(Cjc, (DateTime) Time, stIn.Substring(1));
    //                else
    //                {
    //                    ClassLog.AddLog(ELogType.ErrorModuleRead, "ClassModuleAdam4019Plus.ReadModule",
    //                                    "Ошибка чтения ТХС модуля " + Address, (DateTime) Time);

    //                    signalValue = new ClassChannelValue(Cjc, (DateTime) Time, EChannelStatus.ReadError);
    //                }
    //            }
    //            else
    //                signalValue = new ClassChannelValue(Cjc, (DateTime) Time, EChannelStatus.NoRead);

    //            signalValue.CalcValue();

    //            //Channels
    //            stOut = "#" + Address;
    //            stIn = SendCommand(stOut);

    //            if ((stIn.Length == 57) && (stIn.StartsWith(">")))
    //            {
    //                for (byte channel = 0; channel < ChannelCount; channel++)
    //                {
    //                    if ((!readOnlySelected) || (Channel[channel].Selected))
    //                    {
    //                        string signal = stIn.Substring(1 + (channel)*7, 7);
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime) Time, signal);
    //                    }
    //                    else
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime) Time, EChannelStatus.NoRead);

    //                    signalValue.CalcValue();
    //                }
    //            }
    //            else
    //            {
    //                ClassLog.AddLog(ELogType.ErrorModuleRead, "ClassModuleAdam4019Plus.ReadModule",
    //                                "Ошибка чтения сигналов модуля " + Address, (DateTime) Time);

    //                for (byte channel = 0; channel < ChannelCount; channel++)
    //                {
    //                    if ((!readOnlySelected) || (Channel[channel].Selected))
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime) Time, EChannelStatus.ReadError);
    //                    else
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime) Time, EChannelStatus.NoRead);

    //                    signalValue.CalcValue();
    //                }
    //            }
    //        }
    //    }

    //    public override void ReadModule(bool readOnlySelected = true)
    //    {
    //        if (Address != null)
    //        {
    //            string stOut;
    //            string stIn;
    //            ClassChannelValue signalValue;

    //            Time = DateTime.Now;

    //            //CJC
    //            if (Cjc.Selected)
    //            {
    //                stOut = "$" + Address + "3";
    //                stIn = SendCommand(stOut);

    //                if ((stIn.Length == 8) && (stIn.StartsWith(">")))
    //                    signalValue = new ClassChannelValue(Cjc, (DateTime)Time, stIn.Substring(1));
    //                else
    //                {
    //                    ClassLog.AddLog(ELogType.ErrorModuleRead, "ClassModuleAdam4019Plus.ReadModule",
    //                                    "Ошибка чтения ТХС модуля " + Address, (DateTime)Time);

    //                    signalValue = new ClassChannelValue(Cjc, (DateTime)Time, EChannelStatus.ReadError);
    //                }
    //            }
    //            else
    //                signalValue = new ClassChannelValue(Cjc, (DateTime)Time, EChannelStatus.NoRead);

    //            //signalValue.CalcValue();
    //            lock (Program.Net.QueueChannelValues) { Program.Net.QueueChannelValues.Enqueue(signalValue); }

    //            //Channels
    //            stOut = "#" + Address;
    //            stIn = SendCommand(stOut);

    //            if ((stIn.Length == 57) && (stIn.StartsWith(">")))
    //            {
    //                for (byte channel = 0; channel < ChannelCount; channel++)
    //                {
    //                    if ((!readOnlySelected) || (Channel[channel].Selected))
    //                    {
    //                        string signal = stIn.Substring(1 + (channel) * 7, 7);
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime)Time, signal);
    //                    }
    //                    else
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime)Time, EChannelStatus.NoRead);

    //                    //signalValue.CalcValue();
    //                    lock (Program.Net.QueueChannelValues) { Program.Net.QueueChannelValues.Enqueue(signalValue); }
    //                }
    //            }
    //            else
    //            {
    //                ClassLog.AddLog(ELogType.ErrorModuleRead, "ClassModuleAdam4019Plus.ReadModule",
    //                                "Ошибка чтения сигналов модуля " + Address, (DateTime)Time);

    //                for (byte channel = 0; channel < ChannelCount; channel++)
    //                {
    //                    if ((!readOnlySelected) || (Channel[channel].Selected))
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime)Time, EChannelStatus.ReadError);
    //                    else
    //                        signalValue = new ClassChannelValue(Channel[channel], (DateTime)Time, EChannelStatus.NoRead);

    //                    //signalValue.CalcValue();
    //                    lock (Program.Net.QueueChannelValues) { Program.Net.QueueChannelValues.Enqueue(signalValue); }
    //                }
    //            }
    //        }
    //    }
    //}
}
