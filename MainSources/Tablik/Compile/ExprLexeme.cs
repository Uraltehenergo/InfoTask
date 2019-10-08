using System.Collections.Generic;
using Ast;

namespace Tablik
{
    //Элемент выражения
    internal class ExprLexeme
    {
        public ExprLexeme(Lexeme lexeme, ExprType exprType)
        {
            Lexeme = lexeme;
            ExprType = exprType;
        }

        public ExprLexeme(Lexeme lexeme, CalcType type, ExprType exprType)
        {
            Lexeme = lexeme;
            ExprType = exprType;
            CalcTypeConst = type;
        }

        public ExprLexeme(Lexeme lexeme, FunClass fun, ExprType type = ExprType.Fun)
        {
            ExprType = type;
            Lexeme = lexeme;
            FunOverload = fun.Overloads[0];
        }

        public ExprLexeme(Lexeme lexeme, CalcParam calcParam, bool isPrev = false)
        {
            ExprType = isPrev ? ExprType.Prev : ExprType.Calc;
            Lexeme = lexeme;
            CalcParam = calcParam;
        }

        public ExprLexeme(Lexeme lexeme, Var v)
        {
            ExprType = ExprType.VarUse;
            Lexeme = lexeme;
            Var = v;
        }

        //Ссылка на лексему
        public Lexeme Lexeme { get; private set; }
        //Тип элемента
        public ExprType ExprType { get; internal set; }

        //Тип объекта, если задан сразу
        public CalcType CalcTypeConst { get; set; }
        //Расчетный параметр
        public CalcParam CalcParam { get; set; }
        //Функция
        public FunOverload FunOverload { get; set; }
        //Переменная (varuse)
        public Var Var { get; private set; }
        //Строка с откомпилированным списком типов параметров перегрузки функции
        //Имеет вид ,тип,тип.... Тип результата не включается
        public string ParamString = "";
        
        //Параметры, добавленные с значением по умолчанию
        public List<ExprLexeme> Defaults { get; set; }
        //Адрес перехода по невыполнению условия (if, while, else, ret, for)
        private int _linkAddress = -1;
        public int LinkAddress { get { return _linkAddress; } set { _linkAddress = value; } }

        //Тип данных
        public CalcType CalcType
        {
            get
            {
                switch (ExprType)
                {
                    case ExprType.Calc:
                    case ExprType.Met:
                    case ExprType.Prev:
                    case ExprType.PrevM:
                        return CalcParam != null ? CalcParam.CalcType : new CalcType(ClassType.Error);
                    case ExprType.VarUse:
                        return Var != null ? Var.CalcType : new CalcType(ClassType.Error);
                    default:
                        return CalcTypeConst;
                }
            }
        }
    }
}
