using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CommonTypes;
using Microsoft.Office.Interop.Excel;

namespace ReporterCommon
{
    public class ShapeViewParsing
    {
        internal ParsingKeeper Keeper { get; private set; }

        public INode Parse(Shape shape, string text)
        {
            Keeper = new ParsingKeeper();
            Keeper.SetFieldName("фигура");
            var reader = new StringReader(text);
            var input = new AntlrInputStream(reader.ReadToEnd());
            var lexer = new ShapeViewLexer(input);
            var errorLexerListener = new LexerErrorListener(Keeper, text);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(errorLexerListener);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ShapeViewParser(tokens);
            var errorParserListener = new ParserErrorListener(Keeper);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorParserListener);
            IParseTree tree = parser.prog();
            if (tree == null)
            {
                Keeper.AddError("Недопустимое выражение", (IToken)null);
                return new NodeError(null, "Недопустимое выражение");
            }
            var visitor = new ShapeViewVisitor { GroupShape = shape, Keeper = Keeper};
            var n = Node.Visit(visitor, subParse => subParse[0], tree);
            if (n is NodeError && n.Token == null)
            {
                Keeper.AddError("Недопустимое выражение", (IToken)null);
                return new NodeError(null, "Недопустимое выражение");
            }
            return n;
        }
    }
}