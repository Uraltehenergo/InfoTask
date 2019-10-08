using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Antlr4.Runtime.Tree;
using CommonTypes;
using Microsoft.Office.Interop.Excel;
using P = ReporterCommon.ShapeViewParser;

namespace ReporterCommon
{
    internal class ShapeViewVisitor : ShapeViewBaseVisitor<INode>
    {
        public ParsingKeeper Keeper { get; set; }

        //Обход дерева разбора
        private INode Go(IParseTree tree)
        {
            return tree == null ? null : Visit(tree);
        }
        private ICalcNode GoCalc(IParseTree tree)
        {
            return (ICalcNode)(tree == null ? null : Visit(tree));
        }

        //Текущий группирований Shape
        public Shape GroupShape { private get; set; }

        public override INode VisitProg(P.ProgContext context)
        {
            return new ProgNode(context.statement().Select(Go).Cast<IStatementNode>().ToArray());
        }

        private readonly HashSet<string> _allowedProps = new HashSet<string>
        {
            "visible",
            "text",
            "decimalplaces",
            "color",
            "backcolor",
            "linecolor",
            "fontcolor",
            "pattern",
            "transparent",
            "linetransparent"
        };

        public override INode VisitStatementAction(P.StatementActionContext context)
        {
            var code = context.IDENT(0).Symbol.Text;
            var sn = new StatementNode(context.IDENT(1), 
                                                     code,
                                                     GoCalc(context.expr()));
            try { var it = GroupShape.GroupItems.Item(code);}
            catch { Keeper.AddError("Не найдена фигура",  context.IDENT(0)); }
            if (sn.Token.Text == null || !_allowedProps.Contains(sn.Token.Text.ToLower()))
                Keeper.AddError("Недопустимое имя свойства", sn.Token);
            return sn;
        }

        public override INode VisitStatementVar(P.StatementVarContext context)
        {
            return new SetVarNode(context.IDENT(), GoCalc(context.expr()));
        }

        public override INode VisitExprCons(P.ExprConsContext context)
        {
            return GoCalc(context.cons());
        }

        public override INode VisitExprState(P.ExprStateContext context)
        {
            return new StateNode(context.STATE());
        }

        public override INode VisitExprVar(P.ExprVarContext context)
        {
            return new VarNode(context.IDENT());
        }

        public override INode VisitExprParen(P.ExprParenContext context)
        {
            return GoCalc(context.expr());
        }

        public override INode VisitExprFun(P.ExprFunContext context)
        {
            var f = context.FUN();
            switch (f.Symbol.Text.ToLower())
            {
                case "bit":
                case "бит":
                    return new BitNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "rgb":
                    return new RgbNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)), GoCalc(context.expr(2)));
                case "round":
                    return new RoundNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
            }
            return null;
        }

        public override INode VisitExprIf(P.ExprIfContext context)
        {
            return new IfNode(context.IF(), context.expr().Select(GoCalc).ToArray());
        }

        public override INode VisitExprUnary(P.ExprUnaryContext context)
        {
            var f = (ITerminalNode)context.children[0];
            if (f.Symbol.Text.ToLower() == "not")
                return new NotNode(f, GoCalc(context.expr()));
            return new UnMinusNode(f, GoCalc(context.expr()));
        }

        public override INode VisitExprOper1(P.ExprOper1Context context)
        {
            var f = context.OPER1();
            switch (f.Symbol.Text.ToLower())
            {
                case "or":
                case "или":
                    return new OrNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "and":
                case "и":
                    return new AndNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "xor":
                case "исклили":
                    return new XorNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
            }
            return null;
        }

        public override INode VisitExprOper2(P.ExprOper2Context context)
        {
            var f = context.OPER2();
            switch (f.Symbol.Text)
            {
                case "==":
                    return new EqualsNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "<>":
                    return new NotEqualsNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "<":
                    return new LessNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "<=":
                    return new LessEqualsNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case ">":
                    return new GreaterNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case ">=":
                    return new GreaterEqualsNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
            }
            return null;
        }

        public override INode VisitExprOper3(P.ExprOper3Context context)
        {
            var f = (ITerminalNode)context.children[1];
            switch (f.Symbol.Text)
            {
                case "+":
                    return new PlusNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "-":
                    return new MinusNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
            }
            return null;
        }

        public override INode VisitExprOper4(P.ExprOper4Context context)
        {
            var f = context.OPER4();
            switch (f.Symbol.Text.ToLower())
            {
                case "*":
                    return new MultiplyNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "/":
                    return new DivideNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "div":
                    return new DivNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
                case "mod":
                    return new ModNode(f, GoCalc(context.expr(0)), GoCalc(context.expr(1)));
            }
            return null;
        }

        public override INode VisitConsInt(P.ConsIntContext context)
        {
            return new IntNode(context.INT());
        }

        public override INode VisitConsReal(P.ConsRealContext context)
        {
            return new RealNode(context.REAL());
        }

        public override INode VisitConsString(P.ConsStringContext context)
        {
            return new StringNode(context.STRING());
        }
    }
}