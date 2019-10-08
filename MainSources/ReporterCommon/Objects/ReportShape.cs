using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Core;
using Shape = Microsoft.Office.Interop.Excel.Shape;

namespace ReporterCommon
{
    //Интерфейс для ReportShape
    internal interface IReportShape
    {
        Shape Shape { get; }
        string Formula { get; }
        ProgNode Node { get; }
        DicS<ShapeVar> Vars { get; }
        void SetShape(Shape shape);
    }

    //-------------------------------------------------------------------------------------------------
    //Одна фигура со ссылкой для отладочного отображения
    internal class ReportShapeDebug : IReportShape
    {
        public ReportShapeDebug(Shape shape, string formula)
        {
            Formula = formula;
            SetShape(shape);
        }

        public Shape Shape { get; private set; }
        public string Formula { get; private set; }
        public ProgNode Node { get; private set; }
        public string ErrMess { get; private set; }

        private readonly DicS<ShapeVar> _vars = new DicS<ShapeVar>();
        public DicS<ShapeVar> Vars { get { return _vars; } }

        public void SetShape(Shape shape)
        {
            Shape = shape;
            if (shape.Type == MsoShapeType.msoGroup)
            {
                var parsing = new ShapeViewParsing();
                Node = (ProgNode)parsing.Parse(Shape, Formula);
                ErrMess = parsing.Keeper.ErrMess;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    //Одна фигура со ссылкой
    internal class ReportShape : ReportObject, IReportShape
    {
        public ReportShape(IRecordRead rec) : base(rec)
        {
            Id = rec.GetInt("ShapeId");
            Formula = rec.GetString("Formula");
        }

        //Фигура 
        public Shape Shape { get; private set; }
        //Формула
        public string Formula { get; private set; }
        //Разобранная формула
        public ProgNode Node { get; private set; }
        //Словарь переменных
        private readonly DicS<ShapeVar> _vars  = new DicS<ShapeVar>();
        public DicS<ShapeVar> Vars { get { return _vars; } }
        //Ошибка синтаксиса
        public string ErrMess { get; private set; }

        //Задать фигуру
        public void SetShape(Shape shape)
        {
            Shape = shape;
            if (shape.Type == MsoShapeType.msoGroup)
            {
                var parsing = new ShapeViewParsing();
                Node = (ProgNode)parsing.Parse(Shape, Formula);
                ErrMess = parsing.Keeper.ErrMess;
            }
        }
    }

    //-------------------------------------------------------------------------------------------------
    //Переменная из формулы фигуры
    internal class ShapeVar
    {
        public ShapeVar(string code)
        {
            Code = code;
        }

        //Код
        public string Code { get; private set; }
        //Значение
        public Mean Value { get; set; }
    }
}