using BaseLibrary;

namespace ReporterCommon
{
    //Одна ячейка из файла данных
    public class ReportCell : ReportObject
    {
        public ReportCell(IRecordRead rec) : base(rec)
        {
            Id = rec.GetInt("CellId");
            X = rec.GetInt("X");
            Y = rec.GetInt("Y");
            SaveCode = rec.GetString("SaveCode");
            if (SaveCode.IsEmpty()) SaveCode = Page + "_" + Y + "_" + X;
            AllowEdit = rec.GetBool("AllowEdit");
        }

        //Коодрдинаты
        public int X { get; private set; }
        public int Y { get; private set; }
        //Разрешено редактирование
        public bool AllowEdit { get; private set; }
        //Количество значений при последнем заполнении
        public int NumValues { get; set; }
        //Код архивного параметра
        public string SaveCode { get; private set; }
    }
}