using System;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;

namespace ReporterCommon
{
    //Узел - операция или функция
    internal abstract class OperNode : Node, ICalcNode
    {
        protected OperNode(ITerminalNode terminal, params ICalcNode[] pars) : base(terminal)
        {
            Params = pars;
        }

        protected override string NodeType
        {
            get { return "Oper"; }
        }
        
        protected Mean M0;
        protected Mean M1;
        protected Mean M2;
        protected abstract Mean CalcMean();

        public Mean GetMean(IReportShape rshape, int value)
        {
            if (Params.Length > 0) M0 = Params[0].GetMean(rshape, value);
            if (Params.Length > 1) M1 = Params[1].GetMean(rshape, value);
            if (Params.Length > 2) M2 = Params[2].GetMean(rshape, value);

            return CalcMean();
        }
        public ICalcNode[] Params { get; private set; }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Params);
        }
    }
    
    //------------------------------------------------------------------------------------------
    //Узлы различных функций
    internal class BitNode : OperNode
    {
        public BitNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0.Integer.GetBit(M1.Integer)); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class RoundNode : OperNode
    {
        public RoundNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanReal(Math.Round(M0.Real, M1.Integer)); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class RgbNode : OperNode
    {
        public RgbNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1, ICalcNode par2)
            : base(terminal, par0, par1, par2) { }

        protected override Mean CalcMean()
        {
            return new MeanInteger(M0.Integer + 256 * M1.Integer + 256 * 256 * M2.Integer);
        }
    }
    
    //------------------------------------------------------------------------------------------
    internal class OrNode : OperNode
    {
        public OrNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1) 
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0.Bool || M1.Bool); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class AndNode : OperNode
    {
        public AndNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1) 
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0.Bool && M1.Bool); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class XorNode : OperNode
    {
        public XorNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0.Bool ^ M1.Bool); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class NotNode : OperNode
    {
        public NotNode(ITerminalNode terminal, ICalcNode par0)
            : base(terminal, par0) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(!M0.Bool); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class UnMinusNode : OperNode
    {
        public UnMinusNode(ITerminalNode terminal, ICalcNode par0)
            : base(terminal, par0) { }

        protected override Mean CalcMean()
        {
            if (M0.DataType.LessOrEquals(DataType.Integer))
                return new MeanInteger(-M0.Integer);
            return new MeanReal(-M0.Real);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class MinusNode : OperNode
    {
        public MinusNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            if (M1.DataType.Add(M0.DataType).LessOrEquals(DataType.Integer))
                return new MeanInteger(M0.Integer - M1.Integer);
            return new MeanReal(M0.Real - M1.Real);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class PlusNode : OperNode
    {
        public PlusNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            var dt = M1.DataType.Add(M0.DataType);
            if (dt.LessOrEquals(DataType.Integer))
                return new MeanInteger(M0.Integer + M1.Integer);
            if (dt == DataType.Real)
                return new MeanReal(M0.Real + M1.Real);
            return new MeanString(M0.String + M1.String);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class MultiplyNode : OperNode
    {
        public MultiplyNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            if (M1.DataType.Add(M0.DataType).LessOrEquals(DataType.Integer))
                return new MeanInteger(M0.Integer * M1.Integer);
            return new MeanReal(M0.Real * M1.Real);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class DivideNode : OperNode
    {
        public DivideNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanReal(M0.Real / M1.Real);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class DivNode : OperNode
    {
        public DivNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanInteger(M0.Integer / M1.Integer);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class ModNode : OperNode
    {
        public ModNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanInteger(M0.Integer % M1.Integer);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class EqualsNode : OperNode
    {
        public EqualsNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0 == M1); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class NotEqualsNode : OperNode
    {
        public NotEqualsNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0 != M1);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class LessNode : OperNode
    {
        public LessNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0 < M1);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class LessEqualsNode : OperNode
    {
        public LessEqualsNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0 <= M1); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class GreaterNode : OperNode
    {
        public GreaterNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0 > M1);
        }
    }

    //------------------------------------------------------------------------------------------
    internal class GreaterEqualsNode : OperNode
    {
        public GreaterEqualsNode(ITerminalNode terminal, ICalcNode par0, ICalcNode par1)
            : base(terminal, par0, par1) { }

        protected override Mean CalcMean()
        {
            return new MeanBool(M0 >= M1); 
        }
    }

    //------------------------------------------------------------------------------------------
    internal class IfNode : Node, ICalcNode
    {
        public IfNode(ITerminalNode terminal, params ICalcNode[] pars)
            : base(terminal)
        {
            Params = pars;
        }

        public ICalcNode[] Params { get; private set; }

        public Mean GetMean(IReportShape rshape, int value)
        {
            for (int i = 0; i < Params.Length - 1; i += 2)
                if (Params[i].GetMean(rshape, value).Bool)
                    return Params[i + 1].GetMean(rshape, value);
            return Params[Params.Length - 1].GetMean(rshape, value);
        }

        protected override string NodeType { get { return "If"; } }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(Params);
        }
    }
}