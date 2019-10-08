using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Узел с возможностью задания условия и перехода к подтаблице
    internal class NodeCProp : Node
    {
        public NodeCProp(ITerminalNode terminal, NodeCond cond = null, NodeCChildren children = null)
            : base(terminal)
        {
            Cond = cond;
            Children = children;
        }

        protected override string NodeType { get { return "Prop"; } }
        //Условие для добавления параметра
        public NodeCond Cond { get; private set; }
        //Переход к подчиненной подтаблице
        public NodeCChildren Children { get; private set; }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Cond, Children);
        }
    }

    //----------------------------------------------------------------------------------------------------
    //Базовый класс для узлов перечисляющих ряды таблиц (Tabl, Parent и Empty)
    internal abstract class NodeIter : NodeCProp
    {
        protected NodeIter(ITerminalNode terminal, NodeCond cond = null, NodeCChildren children = null) 
            : base(terminal, cond, children) { }

        //Перечисление строк таблицы
        internal abstract void Generate(GenParamBase par, TablsList tabls);
    }

    //----------------------------------------------------------------------------------------------------
    //Базовый класс для узлов перечисляющих ряды подтаблицы заданного ряда (Children и SubEmpty)
    internal abstract class NodeSubIter : NodeCProp
    {
        protected NodeSubIter(ITerminalNode terminal, NodeCond cond = null, NodeCChildren children = null)
            : base(terminal, cond, children) { }

        //Генерация по строке таблицы
        internal abstract void Generate(GenParamBase par, SubRows row);
    }

    //---------------------------------------------------------------------------------------------------
    //Узел, формруемый если условие не задано
    internal class NodeEmpty : NodeIter
    {
        public NodeEmpty() : base(null) { }
        protected override string NodeType { get { return "Empty"; } }
        
        //Генерация по строке таблицы
        internal override void Generate(GenParamBase par, TablsList tabls)
        {
            par.Generate(null);
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Узел, формруемый если условие не задано
    internal class NodeSubEmpty : NodeSubIter
    {
        public NodeSubEmpty() : base(null) { }
        protected override string NodeType { get { return "Empty"; }}

        //Генерация по строке таблицы
        internal override void Generate(GenParamBase par, SubRows row)
        {
            par.Generate(row);
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Узел - имя таблицы + переход к родителю
    internal class NodeCParent : NodeIter
    {
        public NodeCParent(ITerminalNode terminal): base(terminal)
        {
            _tablName = Token.Text;
        }

        //Тип узла
        protected override string NodeType { get { return "Parent"; } }
        //Имя таблицы
        private readonly string _tablName;

        //Генерация для надтаблицы
        internal override void Generate(GenParamBase par, TablsList tabls)
        {
            if (!tabls.Tabls.ContainsKey(_tablName))
                par.ErrMess += "Не найдена таблица " + _tablName + " (GenConditions); ";
            else 
                par.Generate(tabls.Tabls[_tablName].TablValues);
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Узел - имя таблицы
    internal class NodeCTabl : NodeIter
    {
        public NodeCTabl(ITerminalNode terminal, NodeCond cond = null, NodeCChildren children = null)
            : base(terminal, cond, children)
        {
            _tablName = Token.Text;
        }

        //Тип узла
        protected override string NodeType { get { return "Tabl"; } }
        //Имя таблицы
        private readonly string _tablName;

        //Перечисление строк таблицы
        internal override void Generate(GenParamBase par, TablsList tabls)
        {
            if (!tabls.Tabls.ContainsKey(_tablName))
                par.ErrMess += "Не найдена таблица " + _tablName + " (GenConditions); ";
            else
                foreach (TablRow row in tabls.Tabls[_tablName].TablValues.Subs)
                    if (Cond == null || Cond.CalculateBool(row))
                    {
                        if (Children == null) par.Generate(row);
                        else Children.Generate(par, row);
                    }
        }
    }

    //---------------------------------------------------------------------------------------------------
    //Узел - переход к подтаблице
    internal class NodeCChildren : NodeSubIter
    {
        public NodeCChildren(ITerminalNode terminal, NodeCond cond = null, NodeCChildren children = null)
            : base(terminal, cond, children)
        {
        }

        //Тип узла
        protected override string NodeType { get { return "Children"; } }
        
        //Генерация по строке таблицы
        internal override void Generate(GenParamBase par, SubRows row)
        {
            foreach (TablRow r in row.Subs)
                if (Cond == null || Cond.CalculateBool(r))
                {
                    if (Children == null) par.Generate(r);
                    else Children.Generate(par, r);
                }
        }
    }
}