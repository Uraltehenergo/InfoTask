using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Узел - переход к родителю
    internal class NodeParent : NodeFormText
    {
        public NodeParent(ITerminalNode terminal, NodeCombined expr) 
            : base(terminal)
        {
            _expr = expr;
        }

        //Выражение для формирования строки по родителю
        private readonly NodeCombined _expr;

        //Сформировать текст для записи в поле
        public override string GetText(SubRows row)
        {
            if (row == null || row.Parent == null)
                return "Надтаблица не задана ";
            return _expr.GetText(row.Parent);
        }

        //Тип узла
        protected override string NodeType { get { return "Parent"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_expr);
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Узел - переход к подтаблице
    internal class NodeChildren : NodeFormText
    {
        public NodeChildren(ITerminalNode terminal, NodeCombined expr, NodeCombined separator = null) 
            : base(terminal)
        {
            _expr = expr;
            _separator = separator;
        }

        //Выражение для формирования строки по детям
        private readonly NodeCombined _expr;
        //Выражение - разделитель
        private readonly NodeCombined _separator;

        //Сформировать текст для записи в поле
        public override string GetText(SubRows row)
        {
            if (row == null) return "Недопустимое обращение к подтаблице ";
            string s = "";
            for (int i = 0; i < row.Subs.Count; i++)
            {
                var r = row.Subs[i];
                s += _expr.GetText(r);
                if (i != row.Subs.Count - 1)
                    s += _separator.GetText(r);
            }
            return s;
        }

        //Тип узла
        protected override string NodeType { get { return "Children"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_expr, _separator);
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Узел - переход к подтаблице с условием
    internal class NodeChildrenCond : NodeFormText
    {
        public NodeChildrenCond(ITerminalNode terminal, NodeCond cond, NodeCombined expr, NodeCombined separator = null) 
            : base(terminal)
        {
            _cond = cond;
            _expr = expr;
            _separator = separator;
        }

        //Условие
        private readonly NodeCond _cond;
        //Выражение для формирования строки по детям
        private readonly NodeCombined _expr;
        //Выражение - разделитель
        private readonly NodeCombined _separator;

        //Сформировать текст для записи в поле
        public override string GetText(SubRows row)
        {
            if (row == null) return "Недопустимое обращение к подтаблице ";
            string s = "";
            bool isFirst = true;
            for (int i = 0; i < row.Subs.Count; i++)
            {
                var r = row.Subs[i];
                if (_cond.CalculateBool(r))
                {
                    if (!isFirst) s += _separator.GetText(r);
                    s += _expr.GetText(r);
                    isFirst = false;
                }
            }
            return s;
        }

        //Тип узла
        protected override string NodeType { get { return "Cond"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_cond, _expr, _separator);
        }
    }
}