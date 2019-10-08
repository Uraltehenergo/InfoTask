using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseLibrary
{
    //Комманда для вызова из других приложений
    public class CommandExternal : Command
    {
        internal CommandExternal(Logger logger) : base(logger, null, "")
        {
        }

        //Добавить в комманду ошибку error, isRepeated - ошибка произошла в повторяемой операции
        public override void AddError(ErrorCommand error, bool isRepeated)
        {
            ChangeQuality(error, isRepeated);
            if (!isRepeated)
            {
                bool isFound = false;
                foreach (var err in _errors)
                    if (err.EqualsTo(error))
                        isFound = true;
                if (!isFound) _errors.Add(error);
            }
        }

        //Список ошибок 
        private readonly List<ErrorCommand> _errors = new List<ErrorCommand>();

        //Совокупное сообщение об ошибках
        //Добавляются в описание: addContext - контекст ошибки, addParams - параметры, addErrType - Ошибка или Предупреждение
        public string ErrorMessage(bool addContext = true, bool addParams = true, bool addErrType = true)
        {
            var sb = new StringBuilder();
            bool isFirst = true;
            foreach (var e in _errors.Where(e => e.Quality == CommandQuality.Error))
            {
                if (!isFirst) sb.Append(Environment.NewLine);
                if (addErrType)  sb.Append("Ошибка: ");
                sb.Append(e.Text);
                if (addContext && !e.Context.IsEmpty()) 
                    sb.Append("; ").Append(e.Context);
                if (addParams && !e.Params.IsEmpty()) 
                    sb.Append("; ").Append(e.Params);
                isFirst = false;
            }
            foreach (var e in _errors.Where(e => e.Quality != CommandQuality.Error))
            {
                if (!isFirst) sb.Append(Environment.NewLine);
                if (addErrType) sb.Append("Предупреждение: ");
                sb.Append(e.Text);
                if (addContext && !e.Context.IsEmpty()) 
                    sb.Append("; ").Append(e.Context);
                if (addParams && !e.Params.IsEmpty()) 
                    sb.Append("; ").Append(e.Params);
                isFirst = false;
            }
            return sb.ToString();
        }
    }
}