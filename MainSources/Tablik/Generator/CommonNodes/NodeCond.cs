using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Базовый класс для узлов - условий
    internal abstract class NodeCond : Node
    {
        protected NodeCond(ITerminalNode terminal) : base(terminal) { }

        //Вычислить значение 
        public abstract bool CalculateBool(TablRow row);
    }

    //------------------------------------------------------------------------------
    //Узел - операция сравнения
    internal class NodeOper : NodeCond
    {
        public NodeOper(ITerminalNode terminal, INodeMean arg1, INodeMean arg2)
            : base(terminal)
        {
            _operation = Token.Text;
            _arg1 = arg1;
            _arg2 = arg2;
        }

        //Тип узла
        protected override string NodeType { get { return "Operation"; } }
        //Операция
        private readonly string _operation;
        //Аргументы
        private readonly INodeMean _arg1;
        private readonly INodeMean _arg2;

        //Вычислить значение 
        public override bool CalculateBool(TablRow row)
        {
            var a1 = _arg1.GetMean(row);
            var a2 = _arg2.GetMean(row);
            switch (_operation)
            {
                case "==":
                    return a1 == a2;
                case "<>":
                    return a1 != a2;
                case "<":
                    return a1 < a2;
                case ">":
                    return a1 > a2;
                case "<=":
                    return a1 <= a2;
                case ">=":
                    return a1 >= a2;
            }
            return false;
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_arg1, _arg2);
        }
    }

    //------------------------------------------------------------------------------
    //Узел - операция OR
    internal class NodeOr : NodeCond
    {
        public NodeOr(ITerminalNode terminal, NodeCond op1, NodeCond op2)
            : base(terminal)
        {
            _op1 = op1;
            _op2 = op2;
        }

        //Тип узла
        protected override string NodeType { get { return "Or"; } }
        //Операнды
        private readonly NodeCond _op1;
        private readonly NodeCond _op2;

        //Вычислить значение 
        public override bool CalculateBool(TablRow row)
        {
            return _op1.CalculateBool(row) || _op2.CalculateBool(row);
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_op1, _op2);
        }
    }

    //------------------------------------------------------------------------------
    //Узел - операция AND
    internal class NodeAnd : NodeCond
    {
        public NodeAnd(ITerminalNode terminal, NodeCond op1, NodeCond op2)
            : base(terminal)
        {
            _op1 = op1;
            _op2 = op2;
        }

        //Тип узла
        protected override string NodeType { get { return "And"; } }
        //Операнды
        private readonly NodeCond _op1;
        private readonly NodeCond _op2;

        //Вычислить значение 
        public override bool CalculateBool(TablRow row)
        {
            return _op1.CalculateBool(row) && _op2.CalculateBool(row);
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_op1, _op2);
        }
    }

    //------------------------------------------------------------------------------
    //Узел - операция NOT
    internal class NodeNot : NodeCond
    {
        public NodeNot(ITerminalNode terminal, NodeCond op)
            : base(terminal)
        {
            _op = op;
        }

        //Тип узла
        protected override string NodeType { get { return "Not"; } }
        //Операнд
        private readonly NodeCond _op;

        //Вычислить значение 
        public override bool CalculateBool(TablRow row)
        {
            return !_op.CalculateBool(row);
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_op);
        }
    }
}