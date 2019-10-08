using BaseLibrary;
using Calculation;
using CommonTypes;

namespace ReporterCommon
{
    //Базовый класс для ReportCell и ReportShape
    public abstract class ReportObject
    {
        protected ReportObject(IRecordRead rec)
        {
            ParamId = rec.GetInt("ParamId");
            Page = rec.GetString("Page");
            LinkField = rec.GetString("LinkField").ToLinkField();
            LinkType = rec.GetString("LinkType").ToLinkType();
            IntervalType = rec.GetString("IntervalType").ToIntervalType();
            Properties = rec.GetString("Properties").ToPropertyDicS();
        }

        //Свойства ссылки
        public int Id { get; protected set; }
        public string Page { get; private set; }
        public int ParamId { get; private set; }
        public LinkField LinkField { get; private set; }
        public LinkType LinkType { get; private set; }

        public DicS<string> Properties { get; private set; }
        public IntervalType IntervalType { get; private set; }
        //Ссылка на параметр отчета
        public ReportParam Param { get; set; }

        //Значение или список значений для заполнения ячейки или сохранения в архив
        public SingleValue SingleValue { get; set; }
        //Ссылка на интервал заказа из архива
        public ArchiveQueryValues ArchiveQueryValues { get; set; }

        //Ссылка на архивный параметр для сохранения
        public ArchiveParam ArchiveParam { get; set; }
        //Тип данных
        private DataType _dataType = DataType.Error;
        public DataType DataType
        {
            get
            {
                if (_dataType == DataType.Error)
                {
                    if (!LinkField.IsValueField()) _dataType = DataType.String;
                    else if (LinkField == LinkField.Time) _dataType = DataType.Time;
                    else if (LinkField == LinkField.Nd || LinkField == LinkField.Number) _dataType = DataType.Integer;
                    else if (Param == null) _dataType = DataType.Error;
                    else _dataType = Param.ArchiveParam.DataType;
                }
                return _dataType;
            }
        }
        //Единицы измерения
        public string Units { get { return LinkField == LinkField.Value && Param != null ? Param.ArchiveParam.Units : null; } }
    }
}