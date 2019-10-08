using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Обход выражения GenConditions
    internal class ConditionVisitor : ConditionBaseVisitor<INode>
    {
        public override INode VisitTextExpr(ConditionParser.TextExprContext context)
        {
            return Node.Visit(this, subParce => subParce[0], context.expr());
        }

        public override INode VisitTextPartial(ConditionParser.TextPartialContext context)
        {
            return new NodeError(null, "Ошибка в конце выражения");
        }

        public override INode VisitSubTextExpr(ConditionParser.SubTextExprContext context)
        {
            return Node.Visit(this, subParce => subParce[0], context.children());
        }

        public override INode VisitSubTextPartial(ConditionParser.SubTextPartialContext context)
        {
            return new NodeError(null, "Ошибка в конце выражения");
        }

        public override INode VisitExprIdent(ConditionParser.ExprIdentContext context)
        {
            return Node.Visit(this, context.IDENT(), subParse => new NodeCTabl(context.IDENT()));
        }

        public override INode VisitExprParent(ConditionParser.ExprParentContext context)
        {
            return Node.Visit(this, context.IDENT(), subParse => new NodeCParent(context.IDENT()));
        }

        public override INode VisitExprProps(ConditionParser.ExprPropsContext context)
        {
            return Node.Visit(this, context.IDENT(), subParse =>
                {
                    var props = (NodeCProp)subParse[0];
                    return new NodeCTabl(context.IDENT(), props.Cond, props.Children);
                }, "Недопустимое условие или ссылка на подтаблицу", context.props());
        }

        public override INode VisitPropsCond(ConditionParser.PropsCondContext context)
        {
            return Node.Visit(this, subParse => new NodeCProp(null, (NodeCond)subParse[0]), context.cond());
        }

        public override INode VisitPropsChildren(ConditionParser.PropsChildrenContext context)
        {
            return Node.Visit(this, subParse => new NodeCProp(null, null, (NodeCChildren)subParse[0]), context.children());
        }

        public override INode VisitPropsCondChildren(ConditionParser.PropsCondChildrenContext context)
        {
            return Node.Visit(this, subParse => new NodeCProp(null, (NodeCond)subParse[0], (NodeCChildren)subParse[1]), context.cond(), context.children());
        }

        public override INode VisitGetChildren(ConditionParser.GetChildrenContext context)
        {
            return Node.Visit(this, context.CHILDREN(), subParse => new NodeCChildren(context.CHILDREN()));
        }

        public override INode VisitGetChildrenProps(ConditionParser.GetChildrenPropsContext context)
        {
            return Node.Visit(this, context.CHILDREN(), subParse =>
            {
                var props = (NodeCProp)subParse[0];
                return new NodeCChildren(context.CHILDREN(), props.Cond, props.Children);
            }, "Недопустимое условие или ссылка на подтаблицу", context.props());
        }

        public override INode VisitCondMean(ConditionParser.CondMeanContext context)
        {
            return Node.Visit(this, context.OPER(), subParse =>
                new NodeOper(context.OPER(), (INodeMean)subParse[0], (INodeMean)subParse[1]),
                "Недопустимые параметры операции", context.mean(0), context.mean(1));
        }

        public override INode VisitCondParen(ConditionParser.CondParenContext context)
        {
            return Node.Visit(this, subParse => subParse[0], context.cond());
        }

        public override INode VisitCondNot(ConditionParser.CondNotContext context)
        {
            return Node.Visit(this, context.NOT(), subParse => new NodeNot(context.NOT(), (NodeCond) Visit(context.cond())), 
                "Недопустимые параметры операции", context.cond());
        }

        public override INode VisitCondOr(ConditionParser.CondOrContext context)
        {
            return Node.Visit(this, context.OR(), subParse => new NodeOr(context.OR(), (NodeCond)subParse[0], (NodeCond)subParse[1]), 
                "Недопустимые параметры операции", context.cond(0), context.cond(1));
        }

        public override INode VisitCondAnd(ConditionParser.CondAndContext context)
        {
            return Node.Visit(this, context.AND(), subParse => new NodeAnd(context.AND(), (NodeCond)subParse[0], (NodeCond)subParse[1]), 
                "Недопустимые параметры операции", context.cond(0), context.cond(1));
        }

        public override INode VisitMeanField(ConditionParser.MeanFieldContext context)
        {
            return Node.Visit(this, context.IDENT(), subParse => new NodeField(context.IDENT()));
        }

        public override INode VisitMeanNumber(ConditionParser.MeanNumberContext context)
        {
            return Node.Visit(this, context.NUMBER(), subParse => new NodeNumber(context.NUMBER()));
        }

        public override INode VisitMeanString(ConditionParser.MeanStringContext context)
        {
            return Node.Visit(this, context.STRING(), subParse => new NodeString(context.STRING()));
        }
    }
}