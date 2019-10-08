using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AuditMonitor
{
    public abstract class NewModuleAbstract
    {
    #region Static
        public static byte? HexToByte(string hex)
        {
            byte dec;
            if (byte.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture.NumberFormat, out dec)) return dec;
            return null;
        }

        public static string ByteToHex(byte dec)
        {
            var hex = Convert.ToString(dec, 16);
            if (hex.Length == 1) hex = "0" + hex;
            return hex.ToUpper();
        }

        public static DelegateSendCommand DlgSendCommand;

        public static string SendCommand(string command)
        {
            return (DlgSendCommand != null) ? DlgSendCommand(command) : null;
        }
    #endregion

    #region Fields/AutoProperties
        //Net
        public readonly NewNetModules Net;                       //сеть
        public abstract string ModuleType { get; }               //тип модуля

        private byte _address10;                                 //адрес модуля в десятичной форме (0 - 255)

        public DateTime? Time { get; protected set; }            //время последнего опроса модуля

        public readonly NewModuleChannels Channels;              //список каналов
    #endregion

    #region Properties
        public byte Address10
        {
            get { return _address10; }
            internal set
            {
                if (Net != null)
                {
                    if (!Net.ContainsAddress(_address10))
                        _address10 = value;
                }
                else
                    _address10 = 0;
            }
        }
        
        public string Address
        {
            get { return ByteToHex(_address10); }
            internal set
            {
                byte address10 = HexToByte(value) ?? 0;
                Address10 = address10;
            }
        }

        public int ChannelSelectedCount
        {
            get
            {
                byte cnt = 0;
                foreach (NewChannelAbstract chn in Channels) { if (chn.Selected) cnt++; }
                return cnt;
            }
        }
    #endregion

    #region Constructors/Destructors
        protected NewModuleAbstract(NewNetModules net, byte address10, byte channelCount)
        {
            Net = net;
            Address10 = address10;

            Channels = new NewModuleChannels(channelCount);
            //Name = null;
            //FirmwareVersion = null;
            Time = null;
        }

        protected NewModuleAbstract(NewNetModules net, string address, byte channelCount) //: this(net, HexToByte(address) ?? 0, channelCount)
        {
            Net = net;
            Address = address;

            Channels = new NewModuleChannels(channelCount);
        }
    #endregion

    #region Abstrsct Function
        public abstract void ReadModuleSettings();
        public abstract void ProgramModule();
        public abstract void ReadModule(Queue<NewChannelValue> queue, bool readOnlySelected = true);
        
        //public virtual void ReadModuleNoRead(Queue<NewChannelValue> queue, DateTime time)
        //{
        //    if (Address != null)
        //    {
        //        foreach (NewChannelAbstract channel in Channels)
        //        {
        //            var signalValue = new NewChannelValue(channel, time, NewEnumSignalStatus.NoRead);
        //            lock (queue) { queue.Enqueue(signalValue); }
        //        }
        //    }
        //}

        public virtual void ReadModuleNoRead(Queue<NewChannelValue> queue, DateTime time)
        {
            if (Address != null)
            {
                foreach (NewChannelAbstract channel in Channels)
                {
                    var signalValue = new NewChannelValue(channel, time, NewEnumSignalStatus.NoRead);
                    lock (queue) { queue.Enqueue(signalValue); }
                }
            }
        }
    #endregion
    }
    
    public class NewNetModules
    {
        private readonly List<NewModuleAbstract> _modules = new List<NewModuleAbstract>();
        
        public int Count { get { return _modules.Count; } }

        public NewModuleAbstract this[byte address10]
        {
            get
            {
                foreach (NewModuleAbstract module in _modules)
                    if (module.Address10 == address10)
                        return module;
                return null;
            }
        }

        public NewModuleAbstract this[string address]
        {
            get
            {
                foreach (NewModuleAbstract module in _modules)
                    if (module.Address == address)
                        return module;
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _modules.GetEnumerator();
        }

        public void Clear()
        {
            _modules.Clear();
        }

        public bool AddModule(byte address10, NewModuleAbstract module)
        {
            if (!ContainsAddress(address10))
            {
                _modules.Add(module);
                module.Address10 = address10;
                return true;
            }
            return false;
        }

        public bool AddModule(string address, NewModuleAbstract module)
        {
            byte? address10 = NewModuleAbstract.HexToByte(address);
            if (address10 != null)
                return AddModule((byte) address10, module);
            return false;
        }

        public bool DeleteModule(byte address10)
        {
            foreach (NewModuleAbstract module in _modules)
                if (module.Address10 == address10)
                {
                    _modules.Remove(module);
                    return true;
                }
            return false;
        }

        public bool DeleteModule(string address)
        {
            byte? address10 = NewModuleAbstract.HexToByte(address);
            if (address10 != null)
                return DeleteModule((byte)address10);
            return false;
        } 

        public bool ContainsAddress(byte address10)
        {
            foreach (NewModuleAbstract module in _modules)
                if (module.Address10 == address10)
                    return true;
            return false;
        }

        public bool ContainsAddress(string address)
        {
            byte? address10 = NewModuleAbstract.HexToByte(address);
            if (address10 != null)
                return ContainsAddress((byte) address10);
            return false;
        }

        public NewChannelAbstract Channel(string code)
        {
            foreach (var module in _modules)
                foreach (NewChannelAbstract channel in module.Channels)
                    if (channel.Code == code)
                        return channel;
            return null;
        }

        public bool ContainsCode(string code)
        {
            foreach (var module in _modules)
                foreach (NewChannelAbstract channel in module.Channels)
                    if (channel.Code == code)
                        return true;
            return false;
        }
    }
}
