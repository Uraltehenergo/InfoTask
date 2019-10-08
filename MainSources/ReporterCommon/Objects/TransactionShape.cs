using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    //Одна операция установки или удаления ссылки в фигуру
    public class TransactionShape : TransactionObject
    {
        public TransactionShape(Shape shape)
        {
            Shape = shape;
            SaveOldValue();
        }

        //Фигура
        public Shape Shape { get; private set; }

        //Запись и чтение значения 
        public override string Value 
        {
            get
            {
                if (Shape.TextFrame == null || Shape.TextFrame.Characters() == null) return null;
                return Shape.TextFrame.Characters().Text;
            }
            set
            {
                if (Shape.TextFrame != null && Shape.TextFrame.Characters() != null)
                    Shape.TextFrame.Characters().Text = Value;
            } 
        }

        //Запись и чтение ссылки  
        public override string Link
        {
            get { return Shape.Title; }
            set { Shape.Title = value; }
        }
    }
}