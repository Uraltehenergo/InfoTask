using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuditMonitor
{
    public enum NewEnumSignalStatus : byte //список возможных ошибок канала
    {
        NoError = 0,                       //нет ошибки
        //ошибки чтения
        NoRead = 1,                        //модуль не читался (по указанию прользователя)
        ReadError = 2,                     //ошибка чтения
        //ошибки занчений сигнала
        SignalValueError = 10,             //ошибка значения сигнала
        SignalOutOfRange = 11,             //сигнал вне диапазона (например, для термопар)
        //ошибки первичного преобразования
        TransformError = 20,               //ошибка при преобразовании
        TransformOutOfRange = 21,          //недопустимое значение сигнала для преобразования
        TransformCjcError = 22,            //ошибка при преобразовании - ошибка сигнала ТХС
        TransformCjcOutOfRange = 23,       //ТХС вне допустимого диапозона для преобразования
        //ошибки преобразования
        ConversionSyntaxError = 31,        //ошибка синтаксиса в Conversion
        ConversionCalcError = 32,          //вычислительная ошибка в Conversion
        ConversionOutOfRange = 33,         //неправильное значение значения в Conversion
        //флаги
        //-FlagEndCicle = 240,                //флаг - новый опрос
        //-FlagNewArchiveInterval = 241,      //флаг - начать новый интервал архива
        //-FlagArchiveStop = 242,             //флаг - остановить запись в архив
        //-FlagArchiveStopError = 243,        //флаг - остановить запись в архив по ошибке инициализации (подключения к архиву, обновления списка параметров, начала нового интервала)
        //другие ошибки
        UnknownError = 255                 //неизвестная ошибка
    }

    public enum NewEnumSerieStatus : byte //список статусов серии (опроса) сети
    {
        Empty = 0,                   //пустой
        WriteArchive = 1,            //запись в архив
        NewArchiveInterval = 2,      //начало интервал архива (первая серия опроса)
        EndArchiveInterval = 4       //конец интервала архива (последняя серия опроса)
    }

    public class NewValue
    {
    #region Properties
        public String Code { get; protected set; }                 //код сигнала
        public DateTime Time { get; protected set; }               //время опроса
        public string Signal { get; protected set; }               //сигнал - строка, приходящая от модуля
        public NewEnumSignalStatus Status { get; protected set; }  //код ошибки сигнала или его обработки
    #endregion

    #region Constructors/Destructors
        public NewValue(String code, DateTime time)
        {
            Code = code;
            Time = time;
            Signal = null;
            Status = NewEnumSignalStatus.NoRead;
        }

        public NewValue(String code, DateTime time, string signal)
        {
            Code = code;
            Time = time;
            Signal = signal;
            Status = NewEnumSignalStatus.NoError;
        }

        public NewValue(String code, DateTime time, NewEnumSignalStatus status)
        {
            Code = code;
            Time = time;
            Signal = null;
            Status = status;
        }

        public NewValue(String code, DateTime time, string signal, NewEnumSignalStatus status)
        {
            Code = code;
            Time = time;
            Signal = signal;
            Status = status;
        }
    #endregion
    }

    public class NewChannelValue : NewValue
    {
    #region Properties
        //public DateTime Time { get; private set; }               //время опроса
        //public string Signal { get; private set; }               //сигнал - строка, приходящая от модуля
        public double? Val { get; private set; }                 //значение после первичной обработки (до преобразования)
        public double? CurrentValue { get; private set; }        //значение (после преобразования), меняется не смотря на апертуру
        public double? Value { get; private set; }               //значение (после преобразования) с учетом апертуры
        //public NewEnumSignalStatus Status { get; private set; }  //код ошибки сигнала или его обработки
        public bool ValueChanged { get; private set; }           //измененилось ли значение Value или Status

        public NewChannelAbstract Channel { get; private set; }
    #endregion

    #region Constructors/Destructors
        public NewChannelValue(NewChannelAbstract channel, DateTime time): base(channel.Code, time)
        {
            Channel = channel;

            //Time = time;
            //Signal = null;
            Val = null;
            CurrentValue = null;
            Value = null;
            //Status = NewEnumSignalStatus.NoRead;
            ValueChanged = false; //true;

            //заменить на рассчёт CalcValue
        }

        public NewChannelValue(NewChannelAbstract channel, DateTime time, string signal) : base(channel.Code, time, signal)
        {
            Channel = channel;

            //Time = time;
            //Signal = signal;
            Val = null;
            CurrentValue = null;
            //Status = NewEnumSignalStatus.NoError;
            ValueChanged = false; //true;

            //заменить на рассчёт CalcValue
        }

        public NewChannelValue(NewChannelAbstract channel, DateTime time, NewEnumSignalStatus status): base(channel.Code, time, status)
        {
            Channel = channel;

            //Time = time;
            //Signal = null;
            Val = null;
            CurrentValue = null;
            //Status = status;
            ValueChanged = false; //true;

            //заменить на рассчёт CalcValue
        }

        public NewChannelValue(NewChannelAbstract channel, DateTime time, string signal, NewEnumSignalStatus status): base(channel.Code, time, signal, status)
        {
            Channel = channel;
            
            //Time = time;
            //Signal = signal;
            Val = null;
            CurrentValue = null;
            //Status = status;
            ValueChanged = false; //true;

            //заменить на рассчёт CalcValue
        }
    #endregion

    #region Function
        public void CalcValue()
        {
            if (Channel != null)
            {
                var status = Status;
                double? val;

                Channel.CalcVal(Signal, ref status, out val);
                Val = val;

                if ((status == NewEnumSignalStatus.NoError) && (val != null))
                    CurrentValue = Channel.SignalConversion((double)val, out status);

                Status = status;

                if (Channel.ChannelValue != null)
                {
                    if (Status == NewEnumSignalStatus.NoError)
                    {
                        if (Channel.ChannelValue.Status == NewEnumSignalStatus.NoError)
                        {
                            if (Channel.Aperture != null)
                            {
                                if (Math.Abs((double) CurrentValue - (double) Channel.ChannelValue.Value) >
                                    Channel.Aperture)
                                {
                                    ValueChanged = true;
                                    Value = CurrentValue;
                                }
                                else
                                {
                                    ValueChanged = false;
                                    Value = Channel.ChannelValue.Value;
                                }
                            }
                            else
                            {
                                ValueChanged = (CurrentValue != Channel.ChannelValue.Value);
                                Value = CurrentValue;
                            }
                        }
                        else
                        {
                            ValueChanged = true;
                            Value = CurrentValue;
                        }
                    }
                    else if (Status != Channel.ChannelValue.Status)
                    {
                        ValueChanged = true;
                        Value = (Status != NewEnumSignalStatus.NoRead) ? CurrentValue : Channel.ChannelValue.Value;
                    }
                    else
                    {
                        ValueChanged = false;
                        Value = Channel.ChannelValue.Value;
                    }
                }
                else
                {
                    ValueChanged = true;
                    Value = CurrentValue;
                }

                Channel.ChannelValue = this;
            }
            else
            {
                ValueChanged = true;
                Value = CurrentValue;
            }
        }
    #endregion
    }

    public class NewChannelValueSerie
    {
        public NewEnumSerieStatus ArchiveStatus { get; set; }
        public string ArchiveFileName { get; set; }
        public DateTime Time { get; set; }

        public readonly Queue<NewChannelValue> QueueValues = new Queue<NewChannelValue>();

    #region Constructors/Destructors
        public NewChannelValueSerie()
        {
            ArchiveStatus = NewEnumSerieStatus.Empty;
        }

        public NewChannelValueSerie(NewEnumSerieStatus status)
        {
            ArchiveStatus = status;
        }

        public NewChannelValueSerie(NewEnumSerieStatus status, DateTime time) : this (status)
        {
            Time = time;
        }

        public bool InArchiveStatus(NewEnumSerieStatus status)
        {
            //проверяет, входит ли status в statusIn
            return ((byte)ArchiveStatus & (byte)status) != 0;
        }
    #endregion
    }
}
