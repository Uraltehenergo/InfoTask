namespace CommonTypes
{
    //Ошибка в расчете
    public class ErrorCalc
    {
        public ErrorCalc(string text, string address)
        {
            Text = text;
            Address = address;
        }
        
        public ErrorCalc(string address, ErrorCalc parent)
        {
            Address = address;
            Parent = parent;
        }

        //Текст ошибки
        public string Text { get; private set; }
        //Адрес ошибки (параметр и функция)
        public string Address { get; private set; }
        //Ссылка на ошибку, породившую данную (в другом параметре)
        public ErrorCalc Parent { get; private set; }

        //Полное опиание ошибки
        public new string ToString()
        {
            string s = "";
            var er = this;
            while (er.Parent != null)
            {
                er = er.Parent;
                if (s != "") s += ", ";
                s += er.Address;        
            }
            return (s =="" ? "" : "(" + s + ") ") + er.Text;
        }

        public static ErrorCalc operator | (ErrorCalc x, ErrorCalc y)
        {
            if (x == null) return y;
            return x;
        }
    }
}