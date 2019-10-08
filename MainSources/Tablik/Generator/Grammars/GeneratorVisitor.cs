
using System.Linq;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    internal class GeneratorVisitor : GeneratorBaseVisitor<INode>
    {
        public override INode VisitCombinedExpr(GeneratorParser.CombinedExprContext context)
        {
            var ch = new IParseTree[context.ChildCount-1]; 
             for (int i = 0; i < context.ChildCount-1; i++)
                ch[i] = context.GetChild(i);
            return Node.Visit(this, subParse => new NodeCombined(subParse.Select(n => (NodeFormText)n)), ch);
        }

        public override INode VisitElementTag(GeneratorParser.ElementTagContext context)
        {
            return Node.Visit(this, subParse => subParse[0], context.tag());
        }

        public override INode VisitElementText(GeneratorParser.ElementTextContext context)
        {
            return Node.Visit(this, subParse => subParse[0], context.combtext());
        }

        public override INode VisitCombinedText(GeneratorParser.CombinedTextContext context)
        {
            var ch = new IParseTree[context.ChildCount];
            for (int i = 0; i < context.ChildCount; i++)
                ch[i] = context.GetChild(i);
            return Node.Visit(this, subParse => new NodeCombText(subParse.Select(n => (NodeText)n)), ch);
        }

        public override INode VisitGetText(GeneratorParser.GetTextContext context)
        {
            var term = (ITerminalNode) context.GetChild(0);
            return Node.Visit(this, term, subParse => new NodeText(term));
        }

        public override INode VisitGetSpecial(GeneratorParser.GetSpecialContext context)
        {
            var term = (ITerminalNode)context.GetChild(0);
            return Node.Visit(this, term, subParse => new NodeText(term.Symbol.Text.Substring(1)));
        }

        public override INode VisitTagParent(GeneratorParser.TagParentContext context)
        {
            return Node.Visit(this, context.PARENT(),
                              subParse => new NodeParent(context.PARENT(), (NodeCombined) subParse[0]),
                              "Недопустимое выражение для вышестоящей таблицы", context.expr());
        }

        //public override INode VisitTagChildren(GeneratorParser.TagChildrenContext context)
        //{
        //    return Node.Visit(this, context.CHILDREN(),
        //                      subParse => new NodeChildren(context.CHILDREN(), (NodeCombined)subParse[0]),
        //                      "Недопустимое выражение для подтаблицы", context.expr());
        //}

        public override INode VisitTagChildrenSep(GeneratorParser.TagChildrenSepContext context)
        {
            return Node.Visit(this, context.CHILDREN(),
                              subParse => new NodeChildren(context.CHILDREN(), (NodeCombined)subParse[0], (NodeCombined)subParse[1]),
                              "Недопустимое выражение для подтаблицы", context.expr(0), context.expr(1));
        }

        //public override INode VisitTagCond(GeneratorParser.TagCondContext context)
        //{
        //    return Node.Visit(this, context.COND(),
        //                      subParse => new NodeChildrenCond(context.COND(), (NodeCond)subParse[0], (NodeCombined)subParse[1]),
        //                      "Недопустимое условие или выражение для подтаблицы", context.cond(), context.expr());
        //}

        public override INode VisitTagCondSep(GeneratorParser.TagCondSepContext context)
        {
            return Node.Visit(this, context.COND(),
                              subParse => new NodeChildrenCond(context.COND(), (NodeCond)subParse[0], (NodeCombined)subParse[1], (NodeCombined)subParse[2]),
                              "Недопустимое условие или выражение для подтаблицы", context.cond(), context.expr(0), context.expr(1));
        }

        public override INode VisitTagField(GeneratorParser.TagFieldContext context)
        {
            return Node.Visit(this, subParse => subParse[0], context.field());
        }

        public override INode VisitCondMean(GeneratorParser.CondMeanContext context)
        {
            return Node.Visit(this, context.OPER(), subParse => 
                new NodeOper(context.OPER(), (INodeMean)subParse[0], (INodeMean)subParse[1]), 
                "Недопустимые параметры операции", context.mean(0), context.mean(1));
        }

        public override INode VisitCondParen(GeneratorParser.CondParenContext context)
        {
            return Node.Visit(this, subParse => subParse[0], context.cond());
        }

        public override INode VisitCondNot(GeneratorParser.CondNotContext context)
        {
            return Node.Visit(this, context.NOT(), subParse => new NodeNot(context.NOT(), (NodeCond)Visit(context.cond())),
                "Недопустимые параметры операции", context.cond());
        }

        public override INode VisitCondOr(GeneratorParser.CondOrContext context)
        {
            return Node.Visit(this, context.OR(), subParse => new NodeOr(context.OR(), (NodeCond)subParse[0], (NodeCond)subParse[1]),
                "Недопустимые параметры операции", context.cond(0), context.cond(1));
        }

        public override INode VisitCondAnd(GeneratorParser.CondAndContext context)
        {
            return Node.Visit(this, context.AND(), subParse => new NodeAnd(context.AND(), (NodeCond)subParse[0], (NodeCond)subParse[1]),
                "Недопустимые параметры операции", context.cond(0), context.cond(1));
        }

        public override INode VisitFieldSimple(GeneratorParser.FieldSimpleContext context)
        {
            return Node.Visit(this, context.IDENT(), subParse => new NodeField(context.IDENT()));
        }

        public override INode VisitFieldToIdent(GeneratorParser.FieldToIdentContext context)
        {
            return Node.Visit(this, context.IDENT(), subParse => new NodeFun(context.TOIDENT(), new NodeField(context.IDENT())),
                "Недопустимые параметры операции", context.IDENT(), context.TOIDENT());
        }

        public override INode VisitMeanField(GeneratorParser.MeanFieldContext context)
        {
            return Node.Visit(this, subParse => subParse[0], context.field());
        }

        public override INode VisitMeanNumber(GeneratorParser.MeanNumberContext context)
        {
            return Node.Visit(this, context.NUMBER(), subParse => new NodeNumber(context.NUMBER()));
        }

        public override INode VisitMeanString(GeneratorParser.MeanStringContext context)
        {
            return Node.Visit(this, context.STRING(), subParse => new NodeString(context.STRING()));
        }
    }
}