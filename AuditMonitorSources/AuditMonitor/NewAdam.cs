using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AuditMonitor
{
    public abstract class NewChannelAdamAbstract : NewChannelAbstract
    {
    #region Fields/Properties
        //свойства модуля
        public virtual string ChannelRange { get; set; }              //тип канала (настройка модуля)
        public string DataFormat                                      //формат данных
        {
            get { return ((NewModuleAdamAbstract)Module).DataFormat; }
        }
    #endregion

    #region Constructors/Destructors
        protected NewChannelAdamAbstract(NewModuleAdamAbstract module, byte channel) : base(module, channel) { }
        protected NewChannelAdamAbstract(NewModuleAdamAbstract module, byte channel, string code) : base(module, channel, code) { }
    #endregion
    }

    public class NewChannelAdamCjc : NewChannelAdamAbstract
    {
        public NewChannelAdamCjc(NewModuleAdamAbstract module, byte channel) : base(module, channel)
        {
            Code = "M" + module.Address + "CJC";
            Name = "ТХС модуля " + module.Address;
        }

        public override bool IsCjc { get { return true; } }

        public override string ChannelType { get { return "AdamCJC"; } }
        public override string ChannelRange { get { return "CJC"; } }
        public override double? Aperture { get { return null; } }
        public override double? Min { get { return null; } }
        public override double? Max { get { return null; } }
        public override string Units { get { return "°C"; } }
        public override string InLevel { get { return null; } }
        public override string Conversion { get { return null; } }

        public override void CalcVal(string signal, ref NewEnumSignalStatus status, out double? val)
        {
            val = null;

            if (status == NewEnumSignalStatus.NoError)
            {
                if (signal != null)
                {
                    if ((signal != "+888888") && (signal != "888888"))
                    {
                        double value;
                        if (double.TryParse(signal, NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out value))
                            val = value;
                        else
                            status = NewEnumSignalStatus.SignalValueError;
                    }
                    else
                        status = NewEnumSignalStatus.SignalValueError;
                }
                else
                    status = NewEnumSignalStatus.ReadError;
            }
        }
    }

    public abstract class NewModuleAdamAbstract : NewModuleAbstract
    {
    #region Fields/AutoProperties
        public string Name { get; protected set; }             //имя модуля                            //унести в NewChannelAbstractAdam
        public string FirmwareVersion { get; protected set; }  //версия ПО модуля                      //унести в NewChannelAbstractAdam
        public abstract string DataFormat { get; set; }        //тип единиц измерения                  //унести в NewChannelAbstractAdam
        public abstract string InputRange { get; set; }        //входной диапазон канала ("FF")        //унести в NewChannelAbstractAdam
        public abstract string BaudRate { get; set; }          //скорость передачи данных ("06")       //унести в NewChannelAbstractAdam

        public abstract NewChannelAdamAbstract Cjc { get; }    //канал Cjc
    #endregion

    #region Constructors/Destructors
        protected NewModuleAdamAbstract(NewNetModules net, byte address10, byte channelCount)
            : base(net, address10, channelCount)
        {
            Name = null;
            FirmwareVersion = null;
            //Time = null;
        }

        protected NewModuleAdamAbstract(NewNetModules net, string address, byte channelCount)
            : base(net, address, channelCount)
        {
            Name = null;
            FirmwareVersion = null;
            //Time = null;
        }
    #endregion
    }
}
