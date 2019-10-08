namespace ReporterCommon
{
    //Парамтр ручного ввода для отчета
    public class ReportHandParam
    {
        public ReportHandParam(ReportParam reportParam, string stringValue)
        {
            ReportParam = reportParam;
            StringValue = stringValue;
        }

        //Ссылка на параметр отчета
        public ReportParam ReportParam { get; private set; }
        //Значение из ячейки
        public string StringValue { get; private set; }
        //Шв в таблицах объектов и сигналов
        public int ObjectId { get; set; }
        public int SignalId { get; set; }
    }
}