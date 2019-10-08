using System.Text;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;

namespace Tablik.Generator
{
    //Базовый класс для NodeNumber и NodeString
    internal interface INodeMean : INode
    {
        //Значение 
        Mean GetMean(TablRow row);
    }

    //------------------------------------------------------------------------------
    //Узел - числовая константа
    internal class NodeNumber : Node, INodeMean
    {
        public NodeNumber(ITerminalNode terminal)
            : base(terminal)
        {
            _mean = new MeanInteger(Token.Text.ToInt());
        }

        //Тип узла
        protected override string NodeType { get { return "Number"; }}
        //Значение числа
        private readonly Mean _mean;
        public Mean GetMean(TablRow row)
        {
            return _mean;
        } 
    }

    //------------------------------------------------------------------------------
    //Узел - строковая константа
    internal class NodeString : Node, INodeMean
    {
        public NodeString(ITerminalNode terminal)
            : base(terminal)
        {
            _mean = new MeanString(Token.Text.Substring(1, Token.Text.Length-2));
        }

        //Тип узла
        protected override string NodeType { get { return "String"; } }
        //Значение строки
        private readonly Mean _mean;
        public Mean GetMean(TablRow row)
        {
            return _mean;
        } 
    }

    //------------------------------------------------------------------------------
    //Узел - имя поля
    internal class NodeField : NodeFormText, INodeMean
    {
        public NodeField(ITerminalNode terminal) : base(terminal)
        {
            _fieldName = Token.Text;
        }

        //Тип узла
        protected override string NodeType { get { return "Field"; } }
        //Имя поля
        private readonly string _fieldName;
        //Значение поля
        public Mean GetMean(TablRow row)
        {
            if (_fieldName.ToLower() == "code")
                return new MeanString(row.Code);
            if (_fieldName.ToLower() == "num")
                return new MeanInteger(row.Num);
            if (row == null || !(row is TablRow) || !row.Contains(_fieldName))
                return new MeanString("Не найдено поле " + _fieldName + " ");
            return row[_fieldName];
        }

        //Запись в строку
        public override string GetText(SubRows row)
        {
            return GetMean((TablRow)row).String;
        }
    }

    //---------------------------------------------------------------------------------
    //Узел - функция
    internal class NodeFun : NodeFormText, INodeMean
    {
        //Тип узла
        public NodeFun(ITerminalNode terminal, INodeMean arg) 
            : base(terminal)
        {
            _funName = Token.Text.ToLower();
            _arg = arg;
        }

        //Имя функции
        private readonly string _funName;
        //Аргумент
        private readonly INodeMean _arg;

        protected override string NodeType { get { return "Fun"; } }

        const string Leters = "0123456789abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя_";

        //Получить значение
        public Mean GetMean(TablRow row)
        {
            string s = _arg.GetMean(row).String;
            if (s.IsEmpty()) return null;
            switch (_funName)
            {
                case "toident":
                    s = s.Trim();
                    var sb = new StringBuilder();
                    foreach (char c in s)
                        sb.Append(char.IsLetterOrDigit(c) ? c : '_');
                    return new MeanString(sb.ToString());
            }
            return null;
        }
        public override string GetText(SubRows row)
        {
            return GetMean((TablRow)row).String;
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_arg);
        }
    }
}