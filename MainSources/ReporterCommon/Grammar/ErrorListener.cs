using System;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;

namespace ReporterCommon
{
    //Прослушиватель ошибок лексера
    internal class LexerErrorListener : IAntlrErrorListener<int>
    {
        public LexerErrorListener(ParsingKeeper keeper, string fieldValue)
        {
            _fieldValue = fieldValue;
            _keeper = keeper;
        }

        //Разбираемая строка
        private readonly string _fieldValue;
        //Накопитель ошибок
        private readonly ParsingKeeper _keeper;

        public void SyntaxError(IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var lexer = (Lexer)recognizer;
            const string mess = "Недопустимая последовательность символов";
            string last = "";
            if (lexer.CharIndex < _fieldValue.Length)
                last += _fieldValue[lexer.CharIndex];
            if (lexer.Token != null)
            {
                var t = lexer.Token;
                _keeper.AddError(mess, t.Text.Trim() + last, t.Line, t.Column, t);
            }
            else _keeper.AddError(mess, lexer.Text.Trim() + last, line, charPositionInLine);
        }
    }

    //-------------------------------------------------------------------------------------------------
    //Прослушиватель ошибок парсера
    internal class ParserErrorListener : IAntlrErrorListener<IToken>
    {
        public ParserErrorListener(ParsingKeeper keeper)
        {
            _keeper = keeper;
        }

        //Накопитель ошибок
        private readonly ParsingKeeper _keeper;

        public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            string mess = "Ошибка разбора выражения";
            IToken token = offendingSymbol;
            if (offendingSymbol != null)
            {
                if (offendingSymbol.Text == "<EOF>")
                {
                    mess = "Ошибка в конце выражения";
                    token = null;
                }
                else mess = "Недопустимое использование лексемы";
            }
            if (token != null) _keeper.AddError(mess, token);
            else _keeper.AddError(mess, null, line, charPositionInLine);
        }
    }

    //---------------------------------------------------------------------------------------------------------------
    //Накопление данных в процессе разбора (ошибки и т.п.)
    internal class ParsingKeeper
    {
        //Имя разбираемого поля
        public string FieldName { get; protected set; }
        //Установить разбираемое поле
        public virtual void SetFieldName(string fieldName)
        {
            FieldName = fieldName;
        }

        //Словарь ошибок по одному на поле таблицы, ключи - имена полей
        private readonly DicS<ParsingError> _errors = new DicS<ParsingError>();
        public DicS<ParsingError> Errors { get { return _errors; } }

        //Добавить ошибку в список
        public void AddError(string errMess, string lexeme, int line, int pos, IToken token = null)
        {
            if (!Errors.ContainsKey(FieldName))
                Errors.Add(FieldName, new ParsingError(FieldName, errMess, lexeme, line, pos, token));
        }
        public void AddError(string errMess, IToken token)
        {
            if (!Errors.ContainsKey(FieldName))
                Errors.Add(FieldName, new ParsingError(FieldName, errMess, token));
        }
        public void AddError(string errMess, ITerminalNode terminal)
        {
            if (terminal == null)
                AddError(errMess, (IToken)null);
            else AddError(errMess, terminal.Symbol);
        }
        public void AddError(string errMess, INode node)
        {
            AddError(errMess, node.Token);
        }

        //Накапливаемое сообщение об ошибке
        public string ErrMess
        {
            get
            {
                var s = "";
                foreach (var err in Errors.Values)
                {
                    if (s != "") s += Environment.NewLine;
                    s += err.ToString();
                }
                return s;
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------------------------------------------
    //Ошибка разбора выражения
    public class ParsingError
    {
        public ParsingError(string fieldName, string errMess, IToken token)
        {
            FieldName = fieldName;
            ErrMess = errMess;
            Token = token;
            if (token != null)
            {
                Lexeme = Token.Text;
                Line = Token.Line;
                Pos = Token.Column;
            }
        }

        public ParsingError(string fieldName, string errMess, string lexeme, int line, int pos, IToken token = null)
        {
            FieldName = fieldName;
            ErrMess = errMess;
            Lexeme = lexeme;
            Line = line;
            Pos = pos;
            Token = token;
        }

        //Сообщение
        public string ErrMess { get; private set; }
        //Токен
        internal IToken Token { get; private set; }
        //Текст токена 
        public string Lexeme { get; private set; }
        //Имя разбираемого поля
        public string FieldName { get; private set; }
        //Номер строки
        public int Line { get; private set; }
        //Положение в строке
        public int Pos { get; private set; }

        //Запись в строку
        public override string ToString()
        {
            return ErrMess + (Lexeme.IsEmpty() ? "" : (", '" + Lexeme.Trim() + "'")) + (FieldName.IsEmpty() ? "" : (" (" + FieldName + ", строка: " + Line + ", позиция: " + (Pos + 1) + ")"));
        }
    }
}