using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Windows.Forms;

namespace AuditMonitor
{
    public delegate string DelegateSendCommand(string command, bool showAlerts = false);
    
    public class NewNet
    {
    #region Events
        public static event Action<DateTime> EventNetRead;
        public static event Action<string> EventNewModuleFind;
        public static event Action EventNetReadStop;
        public static event Action<bool> EventWriteArchiveState;
        public static event Action<int> EventArchiveUpdated;
        public static event Action EventConnectArchiveError;
    #endregion

    #region Fields
        private DelegateSendCommand _dlgSendCommand;
        
        public readonly NewNetModules Modules = new NewNetModules();
        
        private ClassVirtualNet _virtualNet;
        private readonly ClassHyperTerminal _hyperTerminal = new ClassHyperTerminal();
        
        private int _readPeriod = 1000;
        private string _archiveFileName = "";

        public bool WriteArchiveByRead { get; set; }
    #endregion
        
    #region Constructor/Destructor
        public NewNet(string virtualModules)
        {
            _dlgSendCommand = VirtSendCommand;
            NewModuleAbstract.DlgSendCommand = VirtSendCommand;
            _virtualNet = new ClassVirtualNet(virtualModules);
            //_writeArchive = false;
            //_writeArchiveCount = 0;
        }

        public NewNet() : this(null) { }
    #endregion

    #region Properties
        public int ReadPeriod //частота периодического опроса, мс
        {
            get { return _readPeriod; }
            set { if (value >= 100) _readPeriod = value; }
        }

        public double ReadPeriodSec //частота периодического опроса, сек
        {
            get { return (double)_readPeriod / 1000; }
            set
            {
                ReadPeriod = (int)(value * 1000);
            }
        }

        public string HtSendCommand(string command, bool showAlertNoResp = false)
        {
            return _hyperTerminal.SendCommand(command, showAlertNoResp);
        }

        public string VirtSendCommand(string command, bool showAlertNoResp = false)
        {
            return _virtualNet.SendCommand(command);
        }

        public string SendCommand(string command, bool showAlertNoResp = false)
        {
            return _dlgSendCommand(command, showAlertNoResp);
        }

        private void ChangeNetType(bool virtNet = false)
        {
            if (!virtNet) _dlgSendCommand = HtSendCommand; else _dlgSendCommand = VirtSendCommand;
            NewModuleAbstract.DlgSendCommand = _dlgSendCommand;
        }

        public byte OpenPort(byte portNum, bool showAlerts = true)
        {
            var port = _hyperTerminal.OpenPort(portNum, showAlerts);
            ChangeNetType(port == 0);
            if ((portNum == 0) && (showAlerts)) MessageBox.Show(@"Виртуальный порт открыт");
            ClassLog.AddLog(ELogType.Event, "ClassAdamNetReader.OpenPort",
                            (port != 0) ? "Порт " + port + " открыт" : "Виртуальный порт открыт");
            return port;
        }

        public byte Port { get { return _hyperTerminal.Port; } }

        public int ModuleTimeOut
        {
            get { return _hyperTerminal.ModuleTimeOut; }
            set { _hyperTerminal.ModuleTimeOut = value; }
        }

        public void InitVirtNet(string modules)
        {
            _virtualNet = new ClassVirtualNet(modules);
        }

        public string VirtualModules
        {
            get
            {
                string virtModules = "";
                for (byte i = 0; i <= 255; i++)
                {
                    //string addr = ClassAbstractAdamModule.ByteToHex(i);
                    string addr = NewModuleAbstract.ByteToHex(i);
                    string stIn = Program.Net.VirtSendCommand("$" + addr + "M");
                    virtModules += (stIn == "!" + addr + "4019P") ? "4019+/" : "/";
                    if (i == 255) break;
                }
                return virtModules;
            }
        }
    #endregion

    #region ScanNet
        //private EFlagState _flagScan = EFlagState.No;
        private int _flagScanI = (int) EFlagState.No;
        private byte _beginAddress;
        private byte _endAddress;
        private Action<string> _actionUpdateView;

        public delegate void DelegateScanNet(byte beginAddress, byte endAddress, Action<string> actionUpdateView = null);
        
        private void ScanNet()
        {
            lock (_hyperTerminal)
            {
                _flagScanI = (int) EFlagState.Yes;
                Modules.Clear();
                for (byte address10 = _beginAddress; address10 <= _endAddress; address10++)
                {
                    if (_actionUpdateView != null) _actionUpdateView(address10.ToString());
                    string address = NewModuleAbstract.ByteToHex(address10);

                    string stIn = _dlgSendCommand("$" + address + "2");
                    if ((stIn.Length == 9) && (stIn.StartsWith("!" + address)))
                    {
                        stIn = _dlgSendCommand("$" + address + "M");
                        if ((stIn.Length > 3) && (stIn.StartsWith("!" + address)))
                        {
                            string mod = stIn.Substring(3);
                            
                            NewModuleAbstract newModule = null;

                            switch (mod)
                            {
                                case "4019P":
                                    newModule = new NewModuleAdam4019Plus(Modules, address);
                                    break;
                            }

                            if (newModule != null)
                            {
                                newModule.ReadModuleSettings();
                                Modules.AddModule(address, newModule);
                                if (EventNewModuleFind != null) EventNewModuleFind(address);
                            }
                        }
                    }
                    //if ((_flagScan == EFlagState.Stop) || (address10 == 255)) break;
                    if ((_flagScanI == (int) EFlagState.Stop) || (address10 == 255)) break;
                }

                if (_actionUpdateView != null) _actionUpdateView(null);
                //_flagScan = EFlagState.No;
                _flagScanI = (int) EFlagState.No;
            }
        }

        public void ScanStart(byte startAddress = (byte) 0, byte endAddress = (byte) 255, Action<string> actionUpdateView = null)
        {
            _beginAddress = startAddress;
            _endAddress = endAddress;
            _actionUpdateView = actionUpdateView;

            ScanNet();
        }

        public void ScanThreadStart(byte startAddress = (byte) 0, byte endAddress = (byte) 255, Action<string> actionUpdateView = null)
        {
            _beginAddress = startAddress;
            _endAddress = endAddress;
            _actionUpdateView = actionUpdateView;

            Thread thScan = new Thread(ScanNet);
            thScan.Start();
        }

        public void ScanStop()
        {
            Interlocked.CompareExchange(ref _flagScanI, (int)EFlagState.Stop, (int)EFlagState.Yes);
            //if (_flagScan == EFlagState.Yes) _flagScan = EFlagState.Stop;
        }

        public void ReadModulesSettings()
        {
            lock (_hyperTerminal)
            {
                foreach (NewModuleAbstract module in Modules) module.ReadModuleSettings();
            }
        }

        public void ProgramChannels()
        {
            lock (_hyperTerminal)
            {
                foreach (NewModuleAbstract module in Modules) module.ProgramModule();
            }
        }
    #endregion

    #region ReadNet
        //потоки
        private Thread _threadRead;      //опрос сети
        private Thread _threadCalc;      //пересчет значений
        private Thread _threadArchive;    //запись в архив
        private readonly AutoResetEvent _autoResetReadStop = new AutoResetEvent(false);
        private readonly AutoResetEvent _autoResetReadNet = new AutoResetEvent(false);
        private readonly AutoResetEvent _autoResetArchive = new AutoResetEvent(false);

        //private readonly Queue<NewChannelValue> _queueChannelValues = new Queue<NewChannelValue>();
        //private readonly Queue<NewChannelValue> _queueArchiveValues = new Queue<NewChannelValue>();
        private readonly Queue<NewChannelValueSerie> _queueCalc = new Queue<NewChannelValueSerie>();
        private readonly Queue<NewChannelValueSerie> _queueArchive = new Queue<NewChannelValueSerie>();
        
        private int _fRead;
        private int _fCalc;
        private int _fArchive;
        private int _fArchiveE;
        //-private bool _fArchiveI;
        //-private bool _fLap;
        
        private void ReadNet(bool readOnlySelected = false)
        {
            lock (_hyperTerminal)
            {
                ClassLog.AddLog(ELogType.Event, null, "Опрос сети начат");
                
                //-NewChannelValue flagValue;

                ////-запись в архив: начало
                //-if(Interlocked.CompareExchange(ref _fArchiveE, 3, 1) == 1)
                //-{
                //-    flagValue = new NewChannelValue(null, DateTime.Now, NewEnumSignalStatus.FlagNewArchiveInterval);
                //-    lock (_queueChannelValues) _queueChannelValues.Enqueue(flagValue);
                //-}
                
                ////-опрос
                //-foreach (NewModuleAdamAbstract module in Modules)
                //-    module.ReadModule(_queueChannelValues, readOnlySelected);
                ////-lock (QueueChannelValues) QueueChannelValues.Enqueue(null);
                
                ////-конец цикла
                //-flagValue = new NewChannelValue(null, DateTime.Now, NewEnumSignalStatus.FlagEndCicle);
                //-_queueChannelValues.Enqueue(flagValue);

                ////-запись в архив: конец
                //-if (Interlocked.CompareExchange(ref _fArchiveE, 2, 0) == 0)
                //-{
                //-    flagValue = new NewChannelValue(null, DateTime.Now, NewEnumSignalStatus.FlagArchiveStop);
                //-    lock (_queueChannelValues) _queueChannelValues.Enqueue(flagValue);
                //-}

                var valueSerie = new NewChannelValueSerie();

                //опрос
                foreach (NewModuleAdamAbstract module in Modules)
                    module.ReadModule(valueSerie.QueueValues, readOnlySelected);

                //Архив
                //запись в архив: начало
                if (Interlocked.CompareExchange(ref _fArchiveE, 3, 1) == 1)
                {
                    valueSerie.ArchiveStatus += (byte) NewEnumSerieStatus.NewArchiveInterval;
                    valueSerie.ArchiveFileName = _archiveFileName;
                }
                
                //запись в архив
                if(_fArchiveE == 3)
                {
                    valueSerie.ArchiveStatus += (byte) NewEnumSerieStatus.WriteArchive;
                    valueSerie.Time = (valueSerie.QueueValues.Count > 0) ? valueSerie.QueueValues.Peek().Time  : DateTime.Now;
                }
                
                //запись в архив: конец
                if (Interlocked.CompareExchange(ref _fArchiveE, 0, 2) == 2)
                {
                    valueSerie.ArchiveStatus += (byte) NewEnumSerieStatus.EndArchiveInterval;
                }

                lock (_queueCalc) _queueCalc.Enqueue(valueSerie);
                _autoResetReadNet.Set();
                
                ClassLog.AddLog(ELogType.Event, null, "Опрос сети произведен");
            }
        }

        public void ReadSingle(string archiveFileName = null, bool readOnlySelected = false)
        {
            if (archiveFileName != null) ArchiveStart(archiveFileName); else ArchiveStop();
            ReadNet(readOnlySelected);
            //-if (archiveFileName != null)
            //-{
            //-    var flagValue = new NewChannelValue(null, DateTime.Now, NewEnumSignalStatus.FlagArchiveStop);
            //-    lock (_queueChannelValues) _queueChannelValues.Enqueue(flagValue);
            //-}
            CalcQueue(false);
            if (archiveFileName != null)
            {
                var valueSerie = new NewChannelValueSerie(NewEnumSerieStatus.EndArchiveInterval, DateTime.Now);
                lock (_queueArchive) _queueArchive.Enqueue(valueSerie);
                ArchiverQueue();
            }
        }

        private void ReadCicle(object readOnlySelected)
        {
            ClassLog.AddLog(ELogType.Event, "ReadCicle", "ThreadRead запущен");
            
            while (Interlocked.CompareExchange(ref _fRead, 3, 2) == 1)
            {
                DateTime timeNext = DateTime.Now.AddMilliseconds(_readPeriod);
                
                ReadNet((bool) readOnlySelected);
                
                if(_fRead == 1)
                {
                    TimeSpan ts = timeNext.Subtract(DateTime.Now);
                    bool res = _autoResetReadStop.WaitOne(ts);
                    if (res)
                    {
                        Interlocked.CompareExchange(ref _fRead, 2, 1);

                        //-var flagValue = new NewChannelValue(null, DateTime.Now, NewEnumSignalStatus.FlagArchiveStop);
                        //-lock (_queueChannelValues) _queueChannelValues.Enqueue(flagValue);
                        var valueSerie = new NewChannelValueSerie(NewEnumSerieStatus.EndArchiveInterval, DateTime.Now);
                        lock (_queueCalc) _queueCalc.Enqueue(valueSerie);

                        _autoResetReadNet.Set();

                        Interlocked.CompareExchange(ref _fCalc, 2, 1);
                        _autoResetReadNet.Set();
                    }
                }
            }

            Interlocked.CompareExchange(ref _fRead, 0, 3);
            ClassLog.AddLog(ELogType.Event, "ReadCicle", "ThreadRead остановлен");
            //if (EventNetReadStop != null) EventNetReadStop(/*DateTime.Now*/);
        }

        public void ReadThreadStart(string archiveFileName = null, bool readOnlySelected = false)
        {
            if (archiveFileName != null) ArchiveStart(archiveFileName); else ArchiveStop();
            
            int i = Interlocked.CompareExchange(ref _fRead, 1, 0);
            if (i != 0)
            {
                _threadRead.Join();
                i = _fRead;
            }
            if (i == 0)
            {
                _threadRead = new Thread(ReadCicle);
                Interlocked.CompareExchange(ref _fRead, 1, 0);

                _threadRead.Start(readOnlySelected);

                i = Interlocked.CompareExchange(ref _fCalc, 1, 2);
                if (i == 3)
                {
                    _threadCalc.Join();
                    i = _fCalc;
                }
                if (i == 0)
                {
                    _threadCalc = new Thread(Calc);
                    Interlocked.CompareExchange(ref _fCalc, 1, 0);
                    _threadCalc.Start();
                }
            }
        }

        public void ReadStop()
        {
            if (Interlocked.CompareExchange(ref _fRead, 2, 1) == 1)
            {
                _autoResetReadStop.Set();
                //ClassLog.AddLog(ELogType.Event, "ReadStop", "ThreadRead запрос остановки");
            }
        }

        public void ReadStopWait()
        {
            _autoResetReadStop.Set();
            _threadRead.Join();
            //_threadCalc.Join();
        }
    #endregion

    #region Сalc
        //private void CalcChannel(NewChannelValue channelValue, bool startArchive = true)
        //{
        //    if (channelValue.Channel != null)
        //    {
        //        //calc
        //        lock (channelValue.Channel) channelValue.CalcValue();
                
        //        //archive
        //        if (_fArchiveI)
        //        {
        //            if (WriteArchiveByRead || channelValue.ValueChanged || _fLap)
        //                lock(_queueArchiveValues) { _queueArchiveValues.Enqueue(channelValue); }
        //        }

        //        //graphic
        //    }
        //    else
        //    {
        //        if(channelValue.Status == NewEnumSignalStatus.FlagNewArchiveInterval) 
        //        {
        //            _fArchiveI = true;
        //            _fLap = true;
        //            lock (_queueArchiveValues) { _queueArchiveValues.Enqueue(channelValue); }
                    
        //            //старт архив
        //            if (startArchive)
        //            {
        //                int i = Interlocked.CompareExchange(ref _fArchive, 1, 2);
        //                if (i == 3)
        //                {
        //                    _threadArchive.Join();
        //                    i = _fArchive;
        //                }
        //                if (i == 0)
        //                {
        //                    _threadArchive = new Thread(Archiver);
        //                    Interlocked.CompareExchange(ref _fArchive, 1, 0);
        //                    _threadArchive.Start();
        //                }
        //            }
        //        }
        //        else if (channelValue.Status == NewEnumSignalStatus.FlagArchiveStop)
        //        {
        //            _fArchiveI = false;
        //            lock (_queueArchiveValues) { _queueArchiveValues.Enqueue(channelValue); }
                    
        //            Interlocked.CompareExchange(ref _fArchive, 2, 1);
        //            _autoResetReadNet.Set();
        //        }
        //        else if (channelValue.Status == NewEnumSignalStatus.FlagEndCicle)
        //        {
        //            if(_fLap) _fLap = false;
                    
        //            lock (_queueArchiveValues) { _queueArchiveValues.Enqueue(channelValue); }
        //            if (EventNetRead != null) EventNetRead(DateTime.Now);
        //            _autoResetArchive.Set();
        //        }
        //    }
        //}

        private void CalcSerie(NewChannelValueSerie valueSerie, bool startArchive = true)
        {
            var archiveSerie = new NewChannelValueSerie
                                   {
                                       ArchiveFileName = valueSerie.ArchiveFileName,
                                       ArchiveStatus = valueSerie.ArchiveStatus,
                                       Time = valueSerie.Time
                                   };

            while(valueSerie.QueueValues.Count > 0)
            {
                NewChannelValue channelValue;
                lock (valueSerie.QueueValues) { channelValue = valueSerie.QueueValues.Dequeue(); }
                
                //calc
                channelValue.CalcValue();

                //archive
                if ((byte)valueSerie.ArchiveStatus >= 1) archiveSerie.QueueValues.Enqueue(channelValue);

                //graphic
                
            }

            if (EventNetRead != null) EventNetRead(DateTime.Now);

            //archive
            if ((byte)valueSerie.ArchiveStatus >= 1)
            {
                lock (_queueArchive) { _queueArchive.Enqueue(archiveSerie); }

                //старт Archiver
                if (valueSerie.InArchiveStatus(NewEnumSerieStatus.NewArchiveInterval))
                {
                    if (startArchive)
                    {
                        int i = Interlocked.CompareExchange(ref _fArchive, 1, 2);
                        if (i == 3)
                        {
                            _threadArchive.Join();
                            i = _fArchive;
                        }
                        if (i == 0)
                        {
                            _threadArchive = new Thread(Archiver);
                            Interlocked.CompareExchange(ref _fArchive, 1, 0);
                            _threadArchive.Start();
                        }
                    }
                }

                //стоп Archiver
                if (valueSerie.InArchiveStatus(NewEnumSerieStatus.EndArchiveInterval))
                {
                    Interlocked.CompareExchange(ref _fArchive, 2, 1);
                }

                _autoResetArchive.Set();
            }
        }

        private void CalcQueue(bool startArchive = true)
        {
            //ClassLog.AddLog(ELogType.Event, "Calc", "Общёт начат");
            
            //-while (_queueChannelValues.Count > 0)
            //-{
            //-    NewChannelValue channelValue;
            //-    lock (_queueChannelValues) { channelValue = _queueChannelValues.Dequeue(); }
            //-    CalcChannel(channelValue, startArchive);
            //-}

            while (_queueCalc.Count > 0)
            {
                NewChannelValueSerie valueSerie;
                lock (_queueCalc) { valueSerie = _queueCalc.Dequeue(); }
                CalcSerie(valueSerie, startArchive);
            }

            //ClassLog.AddLog(ELogType.Event, "Calc", "Общёт произведен");
        }

        private void Calc()
        {
            ClassLog.AddLog(ELogType.Event, "Calc", "Calc запущен");

            while (Interlocked.CompareExchange(ref _fCalc, 3, 2) == 1)
            {
                CalcQueue();
                if (_fCalc == 1) _autoResetReadNet.WaitOne(ReadPeriod);

                //аварийный останов, если Read здох
                //пока не придумал как
            } 

            CalcQueue();
            Interlocked.CompareExchange(ref _fCalc, 0, 3);
            ClassLog.AddLog(ELogType.Event, "Calc", "Calc остановлен");
            if (EventNetReadStop != null) EventNetReadStop(/*DateTime.Now*/);
        }
    #endregion

    #region Archive
        private int _intervalId = -1;
        private int _fArchiveState = 0;
        
        public void ArchiveStart(string archiveFileName)
        {
            _archiveFileName = archiveFileName;
            _fArchiveE = 1;
        }

        public void ArchiveStop()
        {
            Interlocked.CompareExchange(ref _fArchiveE, 2, 1);
            Interlocked.CompareExchange(ref _fArchiveE, 2, 3);
        }

        //private void WriteArchiveChannel(NewChannelValue channelValue)
        //{
        //    Exception ex;

        //    if (channelValue.Channel != null)
        //    {
        //        if (_fArchiveState == 1)
        //        {
        //            Program.Archive.WriteValue(channelValue.Channel.Code, _intervalId, channelValue.CurrentValue, channelValue.Time, (byte) channelValue.Status, out ex);
        //            if (ex != null) ClassLog.AddLog(ELogType.Event, "Archive", ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        if (channelValue.Status == NewEnumSignalStatus.FlagNewArchiveInterval)
        //        {
        //            bool res = Program.Archive.Connect(_archiveFileName, out ex);
        //            if(res)
        //            {
        //                res = Program.Archive.UpdateParams(Program.Net.Modules, out ex);
        //                if(res)
        //                {
        //                    _intervalId = Program.Archive.BeginInterval(channelValue.Time, null, out ex);
        //                    if (ex != null)
        //                    {
        //                        ClassLog.AddLog(ELogType.Event, null, "Ошибка при начале нового интервала архива: " + ex.Message);
        //                        res = false;
        //                    }
        //                }
        //                else
        //                    ClassLog.AddLog(ELogType.Event, null, "Ошибка при обновление списка параметров архива: " + ex.Message);
        //            }
        //            else
        //                ClassLog.AddLog(ELogType.Event, null, "Ошибка подключения к архиву: " + ex.Message);

        //            if (res)
        //                _fArchiveState = 1;
        //            else
        //            {
        //                _fArchiveState = 2;
        //                //добавить событие на ошибку записи в архив (для главной формы) 
        //                ArchiveStop();
        //                if (EventConnectArchiveError != null) EventConnectArchiveError(/*DateTime.Now*/);
        //            }
        //        }
        //        else if (channelValue.Status == NewEnumSignalStatus.FlagArchiveStop)
        //        {
        //            if (_fArchiveState == 1)
        //            {
        //                Program.Archive.WriteEndIntervalValues(_intervalId, channelValue.Time, out ex);
        //                if (ex != null)
        //                    ClassLog.AddLog(ELogType.Event, null, "Ошибка при завершении значей интервала архива: " + ex.Message);
        //                Program.Archive.EndInterval(_intervalId, channelValue.Time, out ex);
        //                if (ex != null)
        //                    ClassLog.AddLog(ELogType.Event, null, "Ошибка при завершении интервала архива: " + ex.Message);
        //            }

        //            Program.Archive.Disconnect();
        //            _intervalId = -1;
        //            _fArchiveState = 0;
        //        }
        //        else if (channelValue.Status == NewEnumSignalStatus.FlagEndCicle)
        //        {
        //            if (_fArchiveState == 1)
        //            {
        //                //обновить ин-цию на форме
        //                if (EventArchiveUpdated != null) EventArchiveUpdated(Program.Archive.ValuesRecordCount);
        //            }
        //        }
        //    }
        //}

        private void ArchiverSerie(NewChannelValueSerie valueSerie)
        {
            Exception ex;
            DateTime endTime = valueSerie.Time;
            
            //новый интервал
            if (valueSerie.InArchiveStatus(NewEnumSerieStatus.NewArchiveInterval))
            {
                bool res = Program.Archive.Connect(valueSerie.ArchiveFileName, out ex);
                if (res)
                {
                    res = Program.Archive.UpdateParams(Program.Net.Modules, out ex);
                    if (res)
                    {
                        _intervalId = Program.Archive.BeginInterval(valueSerie.Time, null, out ex);
                        if (ex != null)
                        {
                            ClassLog.AddLog(ELogType.Event, null, "Ошибка при начале нового интервала архива: " + ex.Message);
                            res = false;
                        }
                    }
                    else
                        ClassLog.AddLog(ELogType.Event, null, "Ошибка при обновление списка параметров архива: " + ex.Message);
                }
                else
                    ClassLog.AddLog(ELogType.Event, null, "Ошибка подключения к архиву: " + ex.Message);

                if (res)
                    _fArchiveState = 1;
                else
                {
                    _fArchiveState = 2;
                    //добавить событие на ошибку записи в архив (для главной формы) 
                    ArchiveStop();
                    if (EventWriteArchiveState != null) EventWriteArchiveState(true);
                    if (EventConnectArchiveError != null) EventConnectArchiveError(/*DateTime.Now*/);
                }
            }

            //значения

            if (_fArchiveState == 1)
            {
                bool archiveError = false;

                while (valueSerie.QueueValues.Count > 0)
                {
                    var channelValue = valueSerie.QueueValues.Dequeue();
                    if (WriteArchiveByRead || channelValue.ValueChanged ||
                        valueSerie.InArchiveStatus(NewEnumSerieStatus.NewArchiveInterval))
                    {
                        Program.Archive.WriteValue(channelValue.Code, _intervalId, channelValue.CurrentValue,
                                                   channelValue.Time, (byte) channelValue.Status, out ex);
                        if (ex != null)
                        {
                            ClassLog.AddLog(ELogType.Event, "Archive", ex.Message);
                            archiveError = true;
                        }
                    }

                    if (channelValue.Time > endTime) endTime = channelValue.Time;
                }

                if (EventWriteArchiveState != null) EventWriteArchiveState(archiveError);
                if (EventArchiveUpdated != null) EventArchiveUpdated(Program.Archive.ValuesRecordCount);
            }


            //конец интервала
            if (valueSerie.InArchiveStatus(NewEnumSerieStatus.EndArchiveInterval))
            {
                if (_fArchiveState == 1)
                {
                    Program.Archive.WriteEndIntervalValues(_intervalId, endTime, out ex);
                    if (ex != null)
                        ClassLog.AddLog(ELogType.Event, null, "Ошибка при завершении значей интервала архива: " + ex.Message);
                    Program.Archive.EndInterval(_intervalId, endTime, out ex);
                    if (ex != null)
                        ClassLog.AddLog(ELogType.Event, null, "Ошибка при завершении интервала архива: " + ex.Message);
                }

                Program.Archive.Disconnect();
                _intervalId = -1;
                _fArchiveState = 0;

                if (EventWriteArchiveState != null) EventWriteArchiveState(false);
            }
        }

        private void ArchiverQueue()
        {
            //ClassLog.AddLog(ELogType.Event, "Archive", "Запись в архив начата");
            
            //-while (_queueArchiveValues.Count > 0)
            //-{
            //-    NewChannelValue channelValue;
            //-    lock (_queueArchiveValues) { channelValue = _queueArchiveValues.Dequeue(); }
            //-    WriteArchiveChannel(channelValue);
            //-}

            while (_queueArchive.Count > 0)
            {
                NewChannelValueSerie valueSerie;
                lock (_queueArchive) { valueSerie = _queueArchive.Dequeue(); }
                ArchiverSerie(valueSerie);
            }

            //ClassLog.AddLog(ELogType.Event, "Archive", "Запись в архив произведена");
        }

        private void Archiver()
        {
            ClassLog.AddLog(ELogType.Event, "Archiver", "Archiver запущен");

            while (Interlocked.CompareExchange(ref _fArchive, 3, 2) == 1)
            {
                ArchiverQueue();
                if (_fArchive == 1) _autoResetArchive.WaitOne(ReadPeriod);

                //аварийный останов, если Calc здох
                //пока не придумал как
            }
            
            ArchiverQueue();
            if (_fArchiveState == 1)
                ClassLog.AddLog(ELogType.Event, "Archiver", "Не завершен интервал (" + _intervalId + ") архива");

            Interlocked.CompareExchange(ref _fArchive, 0, 3);
            ClassLog.AddLog(ELogType.Event, "Archiver", "Archiver остановлен");
        }
    #endregion
    }
}
