using System;

namespace BaseLibrary
{
    //Одна возвращаемая ошибка
    public class ErrorCommand
    {
        public ErrorCommand() { }

        public ErrorCommand(string text, Exception ex = null, string par = "", string context = "", bool isFatal = true)
        {
            Text = text;
            Params = par;
            Exeption = ex;
            Context = context;
            IsFatal = isFatal;
        }

        //Текст сообщения
        public string Text { get; private set; }
        //Ошибка или предупреждение
        public bool IsFatal { get; private set; }
        //Системная ошибка, вызвавшая данную
        public Exception Exeption { get; set; }
        //Параметры ошибки
        public string Params { get; set; }
        //Контекст в котором выполняется комманда (проект, провайдер и т.п.)
        public string Context { get; internal set; }

        //Строка для отображения ошибки в сообщениях
        public override string ToString()
        {
            return Text + (Params.IsEmpty() ? "" : (";\n" + Params));
        }
        //Строка для записи в лог
        public string ToLog()
        {
            if (Exeption == null) return Params ?? "";
            string s = Exeption.Message + ";" + Different.NewLine + Exeption.StackTrace;
            if (!Params.IsEmpty()) s = Params + ";" + Different.NewLine + s;
            return s;
        }
    }
}
