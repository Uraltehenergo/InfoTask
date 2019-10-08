using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    //Одна операция установки или удаления ссылки в ячейку
    public class TransactionCell : TransactionObject
    {
        public TransactionCell(Range cell)
        {
            Cell = cell;
            SaveOldValue();
        }

        public TransactionCell(Worksheet sheet, int x, int y) 
        {
            Cell = sheet.Cells[y, x];
            SaveOldValue();
        }
        
        //Ячейка
        public Range Cell { get; private set; }

        //Запись и чтение значения 
        public override string Value
        {
            get { return Cell.Value2; }
            set { Cell.Value2 = value; }
        }
        //Запись и чтение ссылки  
        public override string Link
        {
            get { return Cell.Comment == null ? null : Cell.Comment.Text(); }
            set
            {
                Cell.ClearComments();
                if (value != null) Cell.AddComment(value);
            }
        }
    }


}