using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Timer = System.Threading.Timer;
//using Timer = System.Timers.Timer;

namespace AuditMonitor
{
    //public delegate string DelegateSendCommand(string command, bool showAlerts = false);

    //public class ClassAdamModules
    //{
    //    private readonly Dictionary<string, ClassAbstractAdamModule> _modules =
    //                     new Dictionary<string, ClassAbstractAdamModule>();

    //    public ClassAbstractAdamModule this[string address]
    //    {
    //        get { return _modules.ContainsKey(address) ? _modules[address] : null; }
    //    }

    //    public IEnumerator GetEnumerator()
    //    {
    //        return _modules.Values.GetEnumerator();
    //    }

    //    public int Count { get { return _modules.Count; } }

    //    public bool AddModule(string address, ClassAbstractAdamModule module)
    //    {
    //        if(! _modules.ContainsKey(address))
    //        {
    //            _modules.Add(address, module);
    //            return true;
    //        }
    //        return false;
    //    }

    //    public void Clear()
    //    {
    //        _modules.Clear();
    //    }

    //    public bool CheckCodeInModules(string code)
    //    {
    //        foreach (ClassAbstractAdamModule module in this)
    //            foreach (ClassAbstractAdamChannel channel in module.Channel)
    //                if (channel.Code == code) return true;
    //        return false;
    //    }
    //}
    
    //public class ClassAdamNetOld
    //{
    //#region Events
    //    public static event Action<DateTime> EventNetRead;
    //    public static event Action<string> EventNewModuleFind;
    //    public static event Action<Exception> EventWriteArchiveState;
    //    public static event Action<int> EventArchiveUpdated;
    //#endregion 
        
    //    private Timer _timer;
    //    public readonly ClassAdamModules Modules = new ClassAdamModules();
        
    //    private readonly ClassHyperTerminal _hyperTerminal = new ClassHyperTerminal();
    //    private ClassVirtualNet _virtualNet;

    //    public bool WriteArchiveByRead { get; set; }
    //    private int _readPeriod = 1000;

    //    private DelegateSendCommand _dlgSendCommand;
        
    //    private EFlagState _flagScan = EFlagState.No;
    //    private bool _flagRead;
    //    private bool _firstRead;
    //    private long _writeArchiveCount;
    //    private bool _writeArchive;
        
    //    public ClassAdamNetOld(string virtualModules)
    //    {
    //        _dlgSendCommand = VirtSendCommand;
    //        ClassAbstractAdamModule.DlgSendCommand = VirtSendCommand;
    //        _virtualNet = new ClassVirtualNet(virtualModules);
    //        //_writeArchive = false;
    //        //_writeArchiveCount = 0;
    //    }

    //    public ClassAdamNetOld() : this(null) {}

    //    public int ReadPeriod
    //    {
    //        get { return _readPeriod; }
    //        set
    //        {
    //            if (value >= 100)
    //            {
    //                _readPeriod = value;
    //                if (_timer != null)
    //                {
    //                    long dueTime = _flagRead ? _readPeriod : 0;
    //                    if (_flagRead) _timer.Change(dueTime, _readPeriod);
    //                }
    //            }
    //        }
    //    }

    //    public double GetReadPeriod() { return (double)_readPeriod / 1000; }
    //    public double SetReadPeriod(double readPeriod)
    //    {
    //        ReadPeriod = (int)(readPeriod * 1000);
    //        return GetReadPeriod();
    //    }
        
    //    public string HtSendCommand(string command, bool showAlertNoResp = false)
    //    {
    //        return _hyperTerminal.SendCommand(command, showAlertNoResp);
    //    }

    //    public string VirtSendCommand(string command, bool showAlertNoResp = false)
    //    {
    //        return _virtualNet.SendCommand(command);
    //    }

    //    public string SendCommand(string command, bool showAlertNoResp = false)
    //    {
    //        return _dlgSendCommand(command, showAlertNoResp);
    //    }

    //    private void ChangeNetType(bool virtNet = false)
    //    {
    //        if (!virtNet)_dlgSendCommand = HtSendCommand;  else _dlgSendCommand = VirtSendCommand;
    //        ClassAbstractAdamModule.DlgSendCommand = _dlgSendCommand;
    //    }

    //    public byte OpenPort(byte portNum, bool showAlerts = true)
    //    {
    //        var port = _hyperTerminal.OpenPort(portNum, showAlerts);
    //        ChangeNetType(port == 0);
    //        if ((portNum == 0) && (showAlerts)) MessageBox.Show(@"Виртуальный порт открыт");
    //        ClassLog.AddLog(ELogType.Event, "ClassAdamNetReader.OpenPort",
    //                        (port != 0) ? "Порт " + port + " открыт" : "Виртуальный порт открыт");
    //        return port;
    //    }

    //    public byte Port { get { return _hyperTerminal.Port; } }

    //    public int ModuleTimeOut
    //    {
    //        get { return _hyperTerminal.ModuleTimeOut; }
    //        set { _hyperTerminal.ModuleTimeOut = value; }
    //    }

    //    //Методы
    //    public void InitVirtNet(string modules)
    //    {
    //        _virtualNet = new ClassVirtualNet(modules);
    //    }

    //    public delegate void DelegateScanNet(byte beginAddress, byte endAddress, Action<string> actionUpdateView = null);

    //    public void ScanNet(byte startAddress = (byte) 0, byte endAddress = (byte) 255,
    //                        Action<string> actionUpdateView = null)
    //    {
    //        _flagScan = EFlagState.Yes;
    //        Modules.Clear();
    //        for (byte address10 = startAddress; address10 <= endAddress; address10++)
    //        {
    //            if (actionUpdateView != null) actionUpdateView(address10.ToString());
    //            string address = ClassAbstractAdamModule.ByteToHex(address10);

    //            string stIn = _dlgSendCommand("$" + address + "2");
    //            if ((stIn.Length == 9) && (stIn.StartsWith("!" + address)))
    //            {
    //                //выбор модуля 
    //                var newModule = new ClassModuleAdam4019Plus(address);
    //                newModule.ReadModuleSettings();
    //                Modules.AddModule(address, newModule);
    //                if (EventNewModuleFind != null) EventNewModuleFind(address);
    //            }
    //            if ((_flagScan == EFlagState.Stop) || (address10 == 255)) break;
    //        }

    //        if (actionUpdateView != null) actionUpdateView(null);
    //        _flagScan = EFlagState.No;
    //    }

    //    public void StopScan()
    //    {
    //        if (_flagScan == EFlagState.Yes) _flagScan = EFlagState.Stop;
    //    }

    //    public void ReadModulesSettings()
    //    {
    //        _flagRead = true;
    //        foreach (ClassAbstractAdamModule module in Modules) module.ReadModuleSettings();
    //        _flagRead = false;
    //    }

    //    public void ProgramChannels()
    //    {
    //        _flagRead = true;
    //        foreach (ClassAbstractAdamModule module in Modules) module.ProgramModule();
    //        _flagRead = false;
    //    }

    //    public void ReadNet(bool readOnlySelected = true)
    //    {
    //        ReadNet(readOnlySelected as object);
    //    }

    //    public void WriteArchive(bool checkValueChanged)
    //    {
    //        //lock (this)
    //        //{
    //        _writeArchiveCount += 1;

    //        foreach (ClassAbstractAdamModule module in Modules)
    //            foreach (ClassAbstractAdamChannel channel in module.Channel)
    //            {
    //                if ((!checkValueChanged) || (channel.ValueChanged))
    //                {
    //                    if (channel.ArchiveId != null)
    //                    {
    //                        Exception exc;

    //                        Program.Archive.WriteValue((int)channel.ArchiveId, channel.Time, channel.Signal,
    //                                               channel.Value, Convert.ToByte(channel.Status), out exc);
    //                        if (exc != null)
    //                        {
    //                            ClassLog.AddLog(ELogType.Event, null,
    //                                            "Ошибка при записи в архив (" + module.Address + "." + channel.Channel +
    //                                            "): " + exc.Message);
    //                        }

    //                        if (EventWriteArchiveState != null) EventWriteArchiveState(exc);
    //                    }
    //                }
    //            }

    //        if (EventArchiveUpdated != null) EventArchiveUpdated(Program.Archive.RecordCount ?? 0);

    //        _writeArchiveCount -= 1;
    //        //}
    //    }
        
    //    public void CyclicReadStart(bool readOnlySelected = false)
    //    {
    //        _firstRead = true;
    //        _timer = new Timer(ReadNet, readOnlySelected, 0, _readPeriod);
    //    }

    //    public void CyclicReadStart(double readPeriod, bool readOnlySelected = false)
    //    {
    //        SetReadPeriod(readPeriod);
    //        CyclicReadStart(readOnlySelected);
    //    }

    //    public void CyclicReadStop()
    //    {
    //        _timer.Dispose();
    //        _timer = null;
    //        while (_flagRead) {}
    //    }

    //    public delegate void DelegateCyclicReadStart(bool readOnlySelected = false);
    //    public delegate void DelegateCyclicReadStop();
        
    //    private void ReadNet(object obj)
    //    {
    //        _flagRead = true;

    //        bool checkValueChanged = (!_firstRead) && (!WriteArchiveByRead);
    //        _firstRead = false;

    //        ClassLog.AddLog(ELogType.Event, null, "Опрос сети начат");

    //        lock (_hyperTerminal)
    //        {
    //            foreach (ClassAbstractAdamModule module in Modules) module.ReadModule((bool) obj);
    //        }

    //        ClassLog.AddLog(ELogType.Event, null, "Опрос сети произведен");
    //        if (EventNetRead != null) EventNetRead(DateTime.Now);

    //        if (_writeArchive)
    //        {
    //            WriteArchive(checkValueChanged);
    //        }
            
    //        _flagRead = false;
    //    }

    //    public void WriteArchiveStart()
    //    {
    //        _writeArchive = true;
    //    }

    //    public void WriteArchiveStop()
    //    {
    //        bool flag = true;
    //        _writeArchive = false;

    //        while (flag)
    //        {
    //            //lock (this)
    //            //{
    //            flag = (_writeArchiveCount != 0);
    //            //}
    //            //Thread.Sleep(100);
    //        }
    //    }

    //    public string VirtualModules
    //    {
    //        get
    //        {
    //            string virtModules = "";
    //            for (byte i = 0; i <= 255; i++)
    //            {
    //                string addr = ClassAbstractAdamModule.ByteToHex(i);
    //                string stIn = Program.Net.VirtSendCommand("$" + addr + "M");
    //                virtModules += (stIn == "!" + addr + "4019P") ? "4019+/" : "/";
    //                if (i == 255) break;
    //            }
    //            return virtModules;
    //        }
    //    }
    //}

    //public class ClassAdamNet
    //{
    //    //#region Events
    //    public static event Action<DateTime> EventNetRead;
    //    public static event Action<string> EventNewModuleFind;
    //    public static event Action<Exception> EventWriteArchiveState;
    //    public static event Action<int> EventArchiveUpdated;
    //    //#endregion

    //    public readonly ClassAdamModules Modules = new ClassAdamModules();
    //    public readonly Queue<ClassChannelValue> QueueChannelValues = new Queue<ClassChannelValue>();

    //    private ClassVirtualNet _virtualNet;
    //    private readonly ClassHyperTerminal _hyperTerminal = new ClassHyperTerminal();
    //    private DelegateSendCommand _dlgSendCommand;

    //    private int _readPeriod = 1000;

    //    public bool WriteArchiveByRead { get; set; }
        
    //    #region Constructor/Destructor
    //    public ClassAdamNet(string virtualModules)
    //    {
    //        _dlgSendCommand = VirtSendCommand;
    //        ClassAbstractAdamModule.DlgSendCommand = VirtSendCommand;
    //        _virtualNet = new ClassVirtualNet(virtualModules);
    //        //_writeArchive = false;
    //        //_writeArchiveCount = 0;
    //    }

    //    public ClassAdamNet() : this(null) { }
    //    #endregion

    //    public int ReadPeriod
    //    {
    //        get { return _readPeriod; }
    //        set
    //        {
    //            if (value >= 100)
    //            {
    //                _readPeriod = value;
    //                if (_timer != null) { _timer.Change(0, _readPeriod); }
    //            }
    //        }
    //    }

    //    public double ReadPeriodSec
    //    {
    //        get { return (double)_readPeriod / 1000; }
    //        set
    //        {
    //            ReadPeriod = (int)(value * 1000);
    //        }
    //    }
        
    //    public string HtSendCommand(string command, bool showAlertNoResp = false)
    //    {
    //        return _hyperTerminal.SendCommand(command, showAlertNoResp);
    //    }

    //    public string VirtSendCommand(string command, bool showAlertNoResp = false)
    //    {
    //        return _virtualNet.SendCommand(command);
    //    }

    //    public string SendCommand(string command, bool showAlertNoResp = false)
    //    {
    //        return _dlgSendCommand(command, showAlertNoResp);
    //    }

    //    private void ChangeNetType(bool virtNet = false)
    //    {
    //        if (!virtNet) _dlgSendCommand = HtSendCommand; else _dlgSendCommand = VirtSendCommand;
    //        ClassAbstractAdamModule.DlgSendCommand = _dlgSendCommand;
    //    }

    //    public byte OpenPort(byte portNum, bool showAlerts = true)
    //    {
    //        var port = _hyperTerminal.OpenPort(portNum, showAlerts);
    //        ChangeNetType(port == 0);
    //        if ((portNum == 0) && (showAlerts)) MessageBox.Show(@"Виртуальный порт открыт");
    //        ClassLog.AddLog(ELogType.Event, "ClassAdamNetReader.OpenPort",
    //                        (port != 0) ? "Порт " + port + " открыт" : "Виртуальный порт открыт");
    //        return port;
    //    }

    //    public byte Port { get { return _hyperTerminal.Port; } }

    //    public int ModuleTimeOut
    //    {
    //        get { return _hyperTerminal.ModuleTimeOut; }
    //        set { _hyperTerminal.ModuleTimeOut = value; }
    //    }

    //    public void InitVirtNet(string modules)
    //    {
    //        _virtualNet = new ClassVirtualNet(modules);
    //    }

    //    public string VirtualModules
    //    {
    //        get
    //        {
    //            string virtModules = "";
    //            for (byte i = 0; i <= 255; i++)
    //            {
    //                string addr = ClassAbstractAdamModule.ByteToHex(i);
    //                string stIn = Program.Net.VirtSendCommand("$" + addr + "M");
    //                virtModules += (stIn == "!" + addr + "4019P") ? "4019+/" : "/";
    //                if (i == 255) break;
    //            }
    //            return virtModules;
    //        }
    //    }

    //    #region ScanNet
    //    private EFlagState _flagScan = EFlagState.No;

    //    public delegate void DelegateScanNet(byte beginAddress, byte endAddress, Action<string> actionUpdateView = null);

    //    public void ScanNet(byte startAddress = (byte) 0, byte endAddress = (byte) 255,
    //                        Action<string> actionUpdateView = null)
    //    {
    //        lock (_hyperTerminal)
    //        {
    //            _flagScan = EFlagState.Yes;
    //            Modules.Clear();
    //            for (byte address10 = startAddress; address10 <= endAddress; address10++)
    //            {
    //                if (actionUpdateView != null) actionUpdateView(address10.ToString());
    //                string address = ClassAbstractAdamModule.ByteToHex(address10);

    //                string stIn = _dlgSendCommand("$" + address + "2");
    //                if ((stIn.Length == 9) && (stIn.StartsWith("!" + address)))
    //                {
    //                    //выбор модуля 
    //                    var newModule = new ClassModuleAdam4019Plus(address);
    //                    newModule.ReadModuleSettings();
    //                    Modules.AddModule(address, newModule);
    //                    if (EventNewModuleFind != null) EventNewModuleFind(address);
    //                }
    //                if ((_flagScan == EFlagState.Stop) || (address10 == 255)) break;
    //            }

    //            if (actionUpdateView != null) actionUpdateView(null);
    //            _flagScan = EFlagState.No;
    //        }
    //    }

    //    public void StopScan()
    //    {
    //        if (_flagScan == EFlagState.Yes) _flagScan = EFlagState.Stop;
    //    }

    //    public void ReadModulesSettings()
    //    {
    //        lock (_hyperTerminal)
    //        {
    //            //_readCount++;
    //            foreach (ClassAbstractAdamModule module in Modules) module.ReadModuleSettings();
    //            //_readCount--;
    //        }
    //    }

    //    public void ProgramChannels()
    //    {
    //        lock (_hyperTerminal)
    //        {
    //            //_readCount++;
    //            foreach (ClassAbstractAdamModule module in Modules) module.ProgramModule();
    //            //_readCount--;
    //        }
    //    }
    //    #endregion

    //    #region ReadNet
    //    public delegate void DelegateCyclicReadStart(bool readOnlySelected = false);
    //    public delegate void DelegateCyclicReadStop();
    //    private readonly AutoResetEvent _eventRead = new AutoResetEvent(false);
    //    private Thread _threadHandler;
    //    private Timer _timer;
    //    private int _flagCicle;
    //    private int _flagHeader;
    //    private int _writeArchiveSost;
    //    //private int _writeArchive;
    //    private int _flagArchive;
        
    //    public void CyclicReadStart(bool readOnlySelected = false)
    //    {
    //        if (Interlocked.CompareExchange(ref _flagCicle, 1, 2) != 2)
    //            while (Interlocked.CompareExchange(ref _flagCicle, 1, 0) != 0)
    //                Thread.Sleep(1);

    //        if (_timer == null) _timer = new Timer(ReadNet, readOnlySelected, 0, _readPeriod);
            
    //        if (Interlocked.CompareExchange(ref _flagHeader, 1, 2) != 2)
    //        {
    //            if (_flagHeader != 1)
    //            {
    //                while (Interlocked.CompareExchange(ref _flagHeader, 1, 0) != 0) { Thread.Sleep(1); }
    //                _threadHandler = new Thread(Handler);
    //                _threadHandler.Start();
    //            }
    //        }
    //    }
        
    //    public void CyclicReadStart(double readPeriod, bool readOnlySelected = false)
    //    {
    //        ReadPeriodSec = readPeriod;
    //        CyclicReadStart(readOnlySelected);
    //    }

    //    public void CyclicReadStop()
    //    {
    //        Interlocked.CompareExchange(ref _flagCicle, 2, 1);
    //    }

    //    public void ReadNet(bool readOnlySelected = true)
    //    {
    //        ReadNet(readOnlySelected as object);
    //    }

    //    private void ReadNet(object obj)
    //    {
    //        lock (_hyperTerminal)
    //        {
    //            if (_flagCicle == 1)
    //            {
    //                ClassLog.AddLog(ELogType.Event, null, "Опрос сети начат");

    //                foreach (ClassAbstractAdamModule module in Modules) module.ReadModule((bool) obj);
    //                lock (QueueChannelValues) { QueueChannelValues.Enqueue(null); }
    //                if (_eventRead != null) _eventRead.Set();

    //                ClassLog.AddLog(ELogType.Event, null, "Опрос сети произведен");
    //            }
    //            else if (_flagCicle == 2)
    //            {
    //                lock (QueueChannelValues)
    //                {
    //                    foreach (ClassAbstractAdamModule module in Modules) module.ReadModuleNoRead(DateTime.Now);
    //                    QueueChannelValues.Enqueue(null);
    //                }
    //                if (_eventRead != null) _eventRead.Set();
                    
    //                if (Interlocked.CompareExchange(ref _flagCicle, 3, 2) == 2)
    //                {
    //                    _timer.Dispose();
    //                    _timer = null;
    //                    //Interlocked.CompareExchange(ref _flagCicle, 0, 3);
    //                    _flagCicle = 0;
    //                    Interlocked.CompareExchange(ref _flagHeader, 2, 1);
    //                }
    //            }
    //        }
    //    }

    //    public void Handler()
    //    {
    //        ClassLog.AddLog(ELogType.Event, null, "запущен");

    //        bool flagContinue = true;

    //        while (flagContinue)
    //        {
    //            while (QueueChannelValues.Count > 0)
    //            {
    //                ClassChannelValue channelValue;
    //                lock (QueueChannelValues) { channelValue = QueueChannelValues.Dequeue(); }
    //                ChannelHandler(channelValue);
    //            }

    //            if(Interlocked.CompareExchange(ref _flagHeader, 3, 2) == 2)
    //                flagContinue = false;
    //            else 
    //                _eventRead.WaitOne();
    //        }

    //        ClassLog.AddLog(ELogType.Event, null, "закончен");
    //        //Interlocked.CompareExchange(ref _flagHeader, 0, 3);
    //        _flagHeader = 0;
    //    }

    //    public void ChannelHandler(ClassChannelValue channelValue)
    //    {
    //        if (channelValue != null)
    //        {
    //            //calc
    //            channelValue.CalcValue();
    //            //graphic
                
    //            //archive
    //            //if (_writeArchiveSost == 0) _writeArchiveSost = 1 + (_writeArchive ? 1 : 0);
    //            if (_writeArchiveSost == 0)
    //            {
    //                _writeArchiveSost = 1 + _flagArchive;

    //                if (_writeArchiveSost == 3)
    //                {
    //                    ClassLog.AddLog(ELogType.Event, null, "Архив: запрос останова");
    //                    Interlocked.CompareExchange(ref _flagArchive, 3, 2);
    //                }
    //                else if (_writeArchiveSost == 4)
    //                {
    //                    //WriteArchiveStop;

    //                    Exception exc;
    //                    Program.Archive.WriteStopValues(out exc);
    //                    if (exc != null) 
    //                        ClassLog.AddLog(ELogType.Event, null, "Ошибка при записи в архив значений NoRead: " + exc.Message);

    //                    ClassLog.AddLog(ELogType.Event, null, "Архив остановлен");
    //                    Interlocked.CompareExchange(ref _flagArchive, 0, 3);
    //                }
    //            }
                
    //            if ((_writeArchiveSost == 2) || (_writeArchiveSost == 3)) WriteArchive(channelValue);
    //        }
    //        else
    //        {
    //            if (EventNetRead != null) EventNetRead(DateTime.Now);
    //            if ((_writeArchiveSost == 2) && (EventArchiveUpdated != null)) EventArchiveUpdated(Program.Archive.RecordCount ?? 0);
    //            _writeArchiveSost = 0;
    //        }
    //    }

    //    public void WriteArchiveStart()
    //    {
    //        //_writeArchive = true;

    //        if (Interlocked.CompareExchange(ref _flagArchive, 1, 2) != 2)
    //            while (Interlocked.CompareExchange(ref _flagArchive, 1, 0) != 0)
    //                Thread.Sleep(1);
    //    }

    //    public void WriteArchiveStop()
    //    {
    //        //_writeArchive = false;
    //        //while (_writeArchiveSost != 0)
    //        //{
    //        //    Thread.Sleep(1);
    //        //}

    //        Interlocked.CompareExchange(ref _flagArchive, 2, 1);
    //        while (_flagArchive != 0) Thread.Sleep(1);
    //    }

    //    public void WriteArchive(ClassChannelValue channelValue)
    //    {
    //        if ((channelValue.Channel.ValueChanged) || (WriteArchiveByRead))
    //        {
    //            Exception exc;

    //            Program.Archive.WriteValue(channelValue, out exc);
    //            if (exc != null)
    //            {
    //                ClassLog.AddLog(ELogType.Event, null,
    //                                "Ошибка при записи в архив (" + channelValue.Channel.Module.Address + "." +
    //                                channelValue.Channel.Channel + "): " + exc.Message);
    //            }

    //            if (EventWriteArchiveState != null) EventWriteArchiveState(exc);
    //            //if (EventArchiveUpdated != null) EventArchiveUpdated(Program.Archive.RecordCount ?? 0);
    //        }
    //    }
    //    #endregion
    //}
};