using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace AuditMonitor
{
    //public enum EChannelStatus : byte //список ошибок канала
    //{
    //    NoError = 0,                  //нет ошибки
    //    NoRead = 1,                   //модуль не читался (по указанию прользователя)
    //    ReadError = 2,                //ошибка чтения
    //    SignalError = 3,              //ошибка значения сигнала
    //    ThermocoupleOutOfRange = 4,   //сигнал вне диапазона (для термопар)
    //    TransformError = 11,          //ошибка при преобразовании
    //    TransformOutOfRange = 12,     //недопустимое значение сигнала для преобразования
    //    TransformCjcError = 13,       //ошибка при преобразовании - ошибка сигнала ТХС
    //    TransformCjcOutOfRange = 14,  //ТХС вне допустимого диапозона для преобразования
    //    ConversionSyntaxError = 21,   //ошибка синтаксиса в Conversion
    //    ConversionCalcError = 22,     //вычислительная ошибка в Conversion
    //    UnknownError = 253            //неизвестная ошибка
    //}

    //public class ClassChannelValue
    //{
    //    #region Properties
    //    public DateTime Time { get; private set; }          //время опроса
    //    public string Signal { get; private set; }          //сигнал - строка, приходящая от модуля
    //    public double? Value { get; private set; }          //значение (после преобразования) с учетом апертуры
    //    public EChannelStatus Status { get; private set; }  //код ошибки сигнала или его обработки
    //    public double? Val { get; private set; }            //значение после первичной обработки (до преобразования)
    //    public double? NewValue { get; private set; }       //значение (после преобразования), меняется не смотря на апертуру
    //    public bool ValueChanged { get; private set; }      //измененилось ли значение Value или Status

    //    public ClassAbstractAdamChannel Channel { get; private set; }
    //    #endregion

    //    #region Constructors/Destructors
    //    public ClassChannelValue(ClassAbstractAdamChannel channel, DateTime time)
    //    {
    //        Channel = channel;

    //        Time = time;
    //        Signal = null;
    //        Value = null;
    //        Status = EChannelStatus.NoRead;
    //        Val = null;
    //        NewValue = null;
    //        ValueChanged = false; //true;
    //    }

    //    public ClassChannelValue(ClassAbstractAdamChannel channel, DateTime time, string signal)
    //    {
    //        Channel = channel;

    //        Time = time;
    //        Signal = signal;
    //        Status = EChannelStatus.NoError;
    //        Val = null;
    //        NewValue = null;
    //        ValueChanged = false; //true;

    //        //? подать сигнал расчета потоку расчета
    //    }

    //    public ClassChannelValue(ClassAbstractAdamChannel channel, DateTime time, EChannelStatus status)
    //    {
    //        Channel = channel;

    //        Time = time;
    //        Signal = null;
    //        Status = status;
    //        Val = null;
    //        NewValue = null;
    //        ValueChanged = false; //true;

    //        //? подать сигнал расчета потоку расчета
    //    }
    //    #endregion

    //    public void CalcValue()
    //    {
    //        if (Channel != null)
    //        {
    //            var status = Status;
    //            double? val;

    //            Channel.CalcVal(Signal, ref status, out val);
    //            Val = val;

    //            if ((status == EChannelStatus.NoError) && (val != null))
    //                NewValue = Channel.SignalConversion((double) val, out status);

    //            Status = status;

    //            if (Status == EChannelStatus.NoError)
    //            {
    //                if (Channel.Status == EChannelStatus.NoError)
    //                {
    //                    if (Channel.Aperture != null)
    //                    {
    //                        if (Math.Abs((double) NewValue - (double) Channel.Value) > Channel.Aperture)
    //                        {
    //                            ValueChanged = true;
    //                            Value = NewValue;
    //                        }
    //                        else
    //                        {
    //                            ValueChanged = false;
    //                            Value = Channel.Value;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        ValueChanged = (NewValue != Channel.Value);
    //                        Value = NewValue;
    //                    }
    //                }
    //                else
    //                {
    //                    ValueChanged = true;
    //                    Value = NewValue;
    //                }
    //            }
    //            else if (Status != Channel.Status)
    //            {
    //                ValueChanged = true;
    //                Value = (Status != EChannelStatus.NoRead) ? NewValue : Channel.Value;
    //            }
    //            else
    //            {
    //                ValueChanged = false;
    //                Value = Channel.Value;
    //            }

    //            Channel.CurrentValue = this;
    //        }
    //        else
    //        {
    //            ValueChanged = true;
    //            Value = NewValue;
    //        }
    //    }
    //}
}