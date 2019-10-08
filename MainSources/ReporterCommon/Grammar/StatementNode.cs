using System;
using Antlr4.Runtime.Tree;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Core;

namespace ReporterCommon
{
    internal interface IStatementNode : INode
    {
        void Apply(IReportShape rshape, int value);
    }

    //---------------------------------------------------------------------------------------
    //Одно присвоение свойства
    internal class StatementNode : Node, IStatementNode
    {
        public StatementNode(ITerminalNode terminal, string shapeCode, ICalcNode expr) 
            : base(terminal)
        {
            _expr = expr;
            _shapeCode = shapeCode;
        }

        //Код фигуры
        private readonly string _shapeCode;
        //Узел, вычисляющий значение
        private readonly ICalcNode _expr;

        protected override string NodeType { get { return "Statement"; } }

        //Установка свойства фигуры
        public void Apply(IReportShape rshape, int value) //Значение привязанного параметра
        {
            try
            {
                var shape = rshape.Shape.GroupItems.Item(_shapeCode);
                if (Token.Text == null || shape == null) return;
                var mean = _expr.GetMean(rshape, value);
                switch (Token.Text.ToLower())
                {
                    case "visible":
                        shape.Visible = mean.Bool ? MsoTriState.msoTrue : MsoTriState.msoFalse;
                        break;
                    case "text":
                        shape.TextFrame.Characters().Text = mean.String;
                        break;
                    case "decimalplaces":
                        shape.TextFrame.Characters().Text = Math.Round(shape.TextFrame.Characters().Text.ToDouble(), mean.Integer).ToString();
                        break;
                    case "color":
                        shape.Fill.ForeColor.RGB = mean.Integer;
                        break;
                    case "backcolor":
                        shape.Fill.BackColor.RGB = mean.Integer;
                        break;
                    case "linecolor":
                        shape.Line.ForeColor.RGB = mean.Integer;
                        break;
                    case "fontcolor":
                        shape.DrawingObject.Font.Color = mean.Integer;
                        break;
                    case "pattern":
                        switch (mean.Integer)
                        {
                            case 0: shape.Fill.Solid(); break;
                            case 1: shape.Fill.Patterned(MsoPatternType.msoPatternDarkHorizontal); break;
                            case 2: shape.Fill.Patterned(MsoPatternType.msoPatternDarkVertical); break;
                        }
                        break;
                    case "transparent":
                        shape.Fill.Transparency = (float)mean.Real;
                        break;
                    case "linetransparent":
                        shape.Line.Transparency = (float)mean.Real;
                        break;
                }
            }
            catch { }
        }

        //Запись в строку
        public override string ToTestString()
        {
            return "Statement: (" +_shapeCode + ", " + Token.Text + ", " + _expr.ToTestString() + ")";
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //Использование переменной
       internal class VarNode : Node, ICalcNode
    {
        public VarNode(ITerminalNode terminal)
            : base(terminal)
        {
            Params = new ICalcNode[0];
        }

        protected override string NodeType
        {
            get { return "Var"; }
        }

        public Mean GetMean(IReportShape rshape, int value)
        {
            return rshape.Vars[Token.Text].Value;
        }

        public ICalcNode[] Params { get; private set; }
    }

    //---------------------------------------------------------------------------------------------------------
    //Присвоение переменной
    internal class SetVarNode : Node, IStatementNode
    {
        public SetVarNode(ITerminalNode terminal, ICalcNode expr)
            : base(terminal)
        {
            _expr = expr;
        }

        protected override string NodeType { get { return "SetVar"; } }

        //Узел, вычисляющий значение
        private readonly ICalcNode _expr;

        //Присвоение значения
        public void Apply(IReportShape rshape, int value)
        {
            try
            {
                var code = Token.Text;
                if (!rshape.Vars.ContainsKey(code))
                    rshape.Vars.Add(code, new ShapeVar(code));
                rshape.Vars[code].Value = _expr.GetMean(rshape, value);
            }
            catch { }
        }

        //Запись в строку
        public override string ToTestString()
        {
            return "SetVar: (" + Token.Text + ", " + _expr.ToTestString() + ")";
        }
    }

    //---------------------------------------------------------------------------------------------------------
    //Главный узел формулы фигуры
    internal class ProgNode : Node
    {
        public ProgNode(IStatementNode[] statements) : base(null)
        {
            _statements = statements;
        }

        private readonly IStatementNode[] _statements;

        protected override string NodeType
        {
            get { return "Prog"; }
        }

        //Установка свойств фигуры
        public void Apply(IReportShape rshape, int value) //Значение привязанного параметра
        {
            foreach (var statement in _statements)
            {
                statement.Apply(rshape, value);
            }
        }

        //Запись в строку
        public override string ToTestString()
        {
            return ToTestWithChildren(_statements);
        }
    }
}