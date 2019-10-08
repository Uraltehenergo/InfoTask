using BaseLibrary;

namespace Calculation
{
    //Тип источника данных колонки
    internal enum ColumnSourceType
    {
        ParamValue,
        SubValue,
        ParamChar,
        SignalChar,
        ResultValue,
        System,
        Reserve,
        Error
    }

    //------------------------------------------------------------------------------------------------------
    //Как отображать в линейной или перекрестной ведомости
    internal enum VedView
    {
        Visible,
        Hiden,
        None
    }
    
    //------------------------------------------------------------------------------------------------------
    //Колонка ведомости анализатора
    internal class VedColumn
    {
        //Тип источника данных
        public ColumnSourceType SourceType { get; private set; }
        //Код источника данных 
        public string Code { get; private set; }
        //Тип данных
        public DataType DataType { get; private set; }
        //Способ отображения в различных ведомостях
        public VedView LinVedView { get; private set; }
        public VedView GroupVedView { get; private set; }
        //Прочие данные для отображения ведомостей
        private readonly int _id;
        private readonly bool _otm;
        private readonly int _num;
        private readonly string _caption;
        private readonly string _graphicView;
        private readonly VedView _krestVedView;
        private readonly string _format;
        private readonly int? _decimalPlaces;
        private readonly int? _textAlign;
        private readonly int? _columnWidth;
        private readonly int? _reportColumnWidth;
        private readonly int? _columnOrder;

        //Чтение из рекордсета из проекта
        public VedColumn(IRecordRead rec)
        {
            _id = rec.GetInt("ColumnId");
            _otm = rec.GetBool("Otm");
            _num = rec.GetInt("ColumnNum");
            SourceType = GetSourceType(rec.GetString("DataSourceType"));
            Code = rec.GetString("DataSourceCode");
            _caption = rec.GetString("Caption");
            DataType = rec.GetString("DataType").ToDataType();
            LinVedView = GetVedView(rec.GetString("LinVedView"));
            GroupVedView = GetVedView(rec.GetString("GroupVedView"));
            _krestVedView = GetVedView(rec.GetString("KrestVedView"));
            _graphicView = rec.GetString("GraphicView");
            _format = rec.GetString("Format");
            _decimalPlaces = rec.GetIntNull("DecimalPlaces");
            _textAlign = rec.GetIntNull("TextAlign");
            _columnWidth = rec.GetIntNull("ColumnWidth");
            _reportColumnWidth = rec.GetIntNull("ReportColumnWidth");
            _columnOrder = rec.GetIntNull("ColumnOrder");
        }

        //Преобразования значений таблиц в enum
        private ColumnSourceType GetSourceType(string s)
        {
            switch (s)
            {
                case "Значения параметра":        
                    return ColumnSourceType.ParamValue;
                case "Характеристика параметра":
                    return ColumnSourceType.ParamChar;
                case "Значения подпараметра":
                    return ColumnSourceType.SubValue;
                case "Характеристика сигнала":
                    return ColumnSourceType.SignalChar;
                case "Итоговое значение":
                    return ColumnSourceType.ResultValue;
                case "Служебное поле":
                    return ColumnSourceType.System;
                case "Резерв":
                    return ColumnSourceType.Reserve;
            }
            return ColumnSourceType.Error;
        }

        private VedView GetVedView(string s)
        {
            switch (s)
            {
                case "Видимая":
                    return VedView.Visible;
                case "Скрытая":
                    return VedView.Hiden;
                default:
                    return VedView.None;
            }
        }
        
        //Запись в рекордсет ведомости
        public void ToRecordset(RecDao rec)
        {
            rec.AddNew();
            rec.Put("ColumnId", _id);
            rec.Put("Otm", _otm);
            rec.Put("ColumnNum", _num);
            rec.Put("DataSourceType", SourceTypeToString(SourceType));
            rec.Put("DataSourceCode", Code);
            rec.Put("Caption", _caption);
            rec.Put("DataType", DataType.ToRussian());
            rec.Put("LinVedView", VedViewToString(LinVedView));
            rec.Put("KrestVedView", VedViewToString(_krestVedView));
            rec.Put("GroupVedView", VedViewToString(GroupVedView));
            rec.Put("GraphicView", _graphicView);
            rec.Put("Format", _format);
            rec.Put("DecimalPlaces", _decimalPlaces);
            rec.Put("TextAlign", _textAlign);
            rec.Put("ColumnWidth", _columnWidth);
            rec.Put("ReportColumnWidth", _reportColumnWidth);
            rec.Put("ColumnOrder", _columnOrder);
        }

        //Запись в рекордсет различных enum
        private string SourceTypeToString(ColumnSourceType t)
        {
            switch (t)
            {
                case ColumnSourceType.ParamValue:
                    return "Значения параметра";
                case ColumnSourceType.ParamChar:
                    return "Характеристика параметра";
                case ColumnSourceType.SubValue:
                    return "Значения подпараметра";
                case ColumnSourceType.SignalChar:
                    return "Характеристика сигнала";
                case ColumnSourceType.System:
                    return "Служебное поле";
                case ColumnSourceType.ResultValue:
                    return "Итоговое значение";
                case ColumnSourceType.Reserve:
                    return "Резерв";
                default:
                    return null;
            }
        }

        private string VedViewToString(VedView t)
        {
            switch (t)
            {
                case VedView.Visible:
                    return "Видимая";
                case VedView.Hiden:
                    return "Скрытая";
                default:
                    return null;
            }
        }
    }
}