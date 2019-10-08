using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;

namespace ReporterCommon
{
    //Узлы для вычисления отображения фигур

    //Константа
    internal abstract class ConstNode : Node, ICalcNode
    {
        protected ConstNode(ITerminalNode terminal)
            : base(terminal)
        {
            Params = new ICalcNode[0];
        }

        protected override string NodeType
        {
            get { return "Const"; }
        }

        protected Mean Mean { get; set; }
        public Mean GetMean(IReportShape rshape, int value)
        {
            return Mean;
        }

        public ICalcNode[] Params { get; private set; }
    }

    //-------------------------------------------------------------------------------------------------------------------------
    //Константы разных типов
    internal class IntNode : ConstNode
    {
        public IntNode(ITerminalNode terminal) : base(terminal)
        {
            Mean = new MeanInteger(terminal.Symbol.Text.ToInt());
        }

        public override string ToTestString()
        {
            return "Int: " + Mean.Integer;
        }
    }

    internal class RealNode : ConstNode
    {
        public RealNode(ITerminalNode terminal) : base(terminal)
        {
            Mean = new MeanReal(terminal.Symbol.Text.ToDouble());
        }

        public override string ToTestString()
        {
            return "Real: " + Mean.Real;
        }
    }

    internal class StringNode : ConstNode
    {
        public StringNode(ITerminalNode terminal)
            : base(terminal)
        {
            Mean = new MeanString(terminal.Symbol.Text);
        }

        public override string ToTestString()
        {
            return "String: " + Mean.String;
        }
    }

    //--------------------------------------------------------------------------------------------------------------------------
    //Значение привязанного параметра
    internal class StateNode : Node, ICalcNode
    {
        public StateNode(ITerminalNode terminal)
            : base(terminal)
        {
            Params = new ICalcNode[0];
        }

        protected override string NodeType
        {
            get { return "Value"; }
        }

        public Mean GetMean(IReportShape rshape, int value)
        {
            return new MeanInteger(value);
        }

        public ICalcNode[] Params { get; private set; }
    }
}