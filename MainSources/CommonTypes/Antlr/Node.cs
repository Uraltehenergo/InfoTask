using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace CommonTypes
{
    //Интерфейс для узлов различных типов 
    public interface INode
    {
        //Токен
        IToken Token { get; }
        //Вывод в строку
        string ToTestString();
    }

    //----------------------------------------------------------------------------------------------------------------------
    //Базовый класс для узлов различных типов 
    public abstract class Node : INode
    {
        protected Node(ITerminalNode terminal)
        {
            if (terminal != null)
                Token = terminal.Symbol;
        }

        //Ссылка на токен
        public IToken Token { get; private set; }
        //Тип узла, для записи в строку
        protected abstract string NodeType { get; }

        //Запись в строку
        public virtual string ToTestString()
        {
            return ToTestWithChildren();
        }

        //Запись в строку, с указанием списка детей
        protected string ToTestWithChildren(params INode[] children)
        {
            var sb = new StringBuilder(NodeType + ": ");
            if (Token != null) sb.Append(Token.Text);
            if (children.Length != 0)
            {
                if (Token != null) sb.Append(" ");
                sb.Append("(");
                for (int i = 0; i < children.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    if (children[i] != null) sb.Append(children[i].ToTestString());
                }
                sb.Append(")");
            }
            return sb.ToString();
        }

        //Обработка одной вершины дерева разбора при парсинге
        public static INode Visit(IParseTreeVisitor<INode> visitor, //Visitor
                                 ITerminalNode terminal,  //Терминал с главным токеном вершины
                                 Func<List<INode>, INode> fun, //Функция обработки вершины
                                 string errMess = "Ошибка разбора выражения", //Строка ошибки
                                 params IParseTree[] children) //Дети в дереве разбора
        {
            return Visit(visitor, true, terminal, fun, errMess, children);
        }
        
        //Обработка вершины без указания терминала
        public static INode Visit(IParseTreeVisitor<INode> visitor, Func<List<INode>, INode> fun, params IParseTree[] children)
        {
            return Visit(visitor, false, null, fun, null, children);
        }

        private static INode Visit(IParseTreeVisitor<INode> visitor, bool hasTerminal, ITerminalNode terminal, Func<List<INode>, INode> fun, string errMess = "Ошибка разбора выражения", params IParseTree[] children)
        {
            try
            {
                if (hasTerminal && terminal == null)
                    return new NodeError();

                var subParse = new List<INode>();
                foreach (var node in children)
                {
                    if (node == null)
                    {
                        if (terminal == null)
                            return new NodeError();
                        return new NodeError(terminal, errMess);
                    }
                    if (!(node is ITerminalNode))
                    {
                        var res = visitor.Visit(node);
                        subParse.Add(res);
                        if (res == null)
                        {
                            if (terminal == null)
                                return new NodeError();
                            return new NodeError(terminal, errMess);
                        }
                        if (res is NodeError)
                        {
                            if (terminal == null || res.Token != null || ((NodeError)res).ErrMess != null)
                                return res;
                            return new NodeError(terminal, errMess);
                        }
                    }
                }
                return fun(subParse);
            }
            catch { return new NodeError(); }
        }
    }

    //----------------------------------------------------------------------------------------------------------------------
    //Узел содержит ошибку парсинга 
    public class NodeError : Node
    {
        //Если не удалось распознать терминальный узел
        public NodeError() : base(null) {}

        //Если не удалось распознать узел в поддереве (с указанием ошибочного токена и ошибки)
        public NodeError(ITerminalNode terminal, string errMess) : base(terminal)
        {
            ErrMess = errMess;
        }

        //Тип узла
        protected override string NodeType { get { return "Error"; } }

        //Сообщение ошибки
        public string ErrMess { get; private set; }

        //Запись в строку
        public override string ToTestString()
        {
            if (Token == null) return ErrMess;
            return ErrMess + " (" + Token.Text + ", строка: " + Token.Line + ", позиция: " + (Token.Column + 1) + ")";
        }
    }
   
    //----------------------------------------------------------------------------------------------------------------------
    //Интерфейс для парсинга одной строки
    public interface IParsing
    {
        INode Parse(string text);
    }
}