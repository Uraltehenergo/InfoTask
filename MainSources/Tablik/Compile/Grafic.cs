using BaseLibrary;

namespace Tablik
{
    //График из проекта
    internal class Grafic
    {
        public Grafic(string code, int dim)
        {
            Code = code;
            Dimension = dim;
        }

        //Код 
        public string Code { get; private set; }
        //Размерность графика
        public int Dimension { get; private set; }
        //Параметры, использующие этот график
        private DicS<CalcParam> _usingParams;
        public DicS<CalcParam> UsingParams { get { return _usingParams ?? (_usingParams = new DicS<CalcParam>()); } }
    }
}