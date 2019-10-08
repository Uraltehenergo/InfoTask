using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Узел - текст, непосредственно добавляемый в поле
    internal class NodeText : NodeFormText
    {
        public NodeText(ITerminalNode terminal) : base(terminal)
        {
            Text = terminal.Symbol.Text;
        }

        public NodeText(string text) : base(null)
        {
            Text = text;
        }

        protected NodeText() : base(null) { } 

        //Тип узла
        protected override string NodeType { get { return "Text"; }}
        //Текст
        public string Text { get; protected set; }
        //Запись в строку
        public override string ToTestString()
        {
            return "Text: " + Text ;
        }
        
        //Сформировать текст для записи в поле
        public override string GetText(SubRows row)
        {
            return Text;
        }
    }

    //--------------------------------------------------------------------------------------------------
    //Узел - комбинация нескольких частей текста
    internal class NodeCombText : NodeText
    {
        public NodeCombText(IEnumerable<NodeText> children) 
        {
            var sb = new StringBuilder();
            foreach (var node in children)
                sb.Append(node.Text);
            Text = sb.ToString();
        }
    }
}