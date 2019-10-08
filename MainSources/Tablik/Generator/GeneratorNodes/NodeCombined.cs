using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Базовый класс для всех узлов, которые формируют итоговый текст
    internal abstract class NodeFormText : Node
    {
        protected NodeFormText(ITerminalNode terminal) : base(terminal) { }

        //Сформировать текст для записи в поле
        public abstract string GetText(SubRows row);
    }

    //--------------------------------------------------------------------------------------------------

    //Узел - комбинация нескольких элементов выражения
    internal class NodeCombined : NodeFormText
    {
        public NodeCombined(IEnumerable<NodeFormText> children) : base(null)
        {
            _children = children;
        }
        
        //Тип узла
        protected override string NodeType { get { return "Combined"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_children.ToArray());
        }

        //Подчиненные узлы
        private readonly IEnumerable<NodeFormText> _children;

        //Сформировать текст для записи в поле
        public override string GetText(SubRows row)
        {
            var sb = new StringBuilder("");
            foreach (var n in _children)
                sb.Append(n.GetText(row));
            return sb.ToString();
        }
    }
}