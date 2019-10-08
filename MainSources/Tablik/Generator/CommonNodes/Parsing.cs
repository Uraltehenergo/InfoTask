using System.IO;
using System.Windows.Forms;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CommonTypes;

namespace Tablik.Generator
{
    //Класс для запуска парсинга строки условий генерации
    internal class ParsingCondition : IParsing
    {
        //Разбор выражения для параметра
        public INode Parse(string text)
        {
            return ParseText(OpenParser(text).text());
        }

        //Разбор выражения для подпараметра
        public INode SubParse(string text)
        {
            return ParseText(OpenParser(text).children());
        }

        //Открыть парсер
        private static ConditionParser OpenParser(string text)
        {
            var reader = new StringReader(text);
            var input = new AntlrInputStream(reader.ReadToEnd());
            var lexer = new ConditionLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new ConditionParser(tokens);
            return parser;
        }

        //Вызвать парсинг
        private static INode ParseText(IParseTree tree)
        {
            if (tree == null) return new NodeError(null, "Недопустимое выражение");
            var visitor = new ConditionVisitor();
            var n = Node.Visit(visitor, subParse => subParse[0], tree);
            if (n is NodeError && ((NodeError) n).ErrMess == null)
                return new NodeError(null, "Недопустимое выражение");
            return n;
        }
    }

    //-------------------------------------------------------------------------------------------------------------
    //Класс для запуска парсинга строки условий генерации
    internal class ParsingGenerator : IParsing
    {
        public INode Parse(string text)
        {
            var reader = new StringReader(text);
            var input = new AntlrInputStream(reader.ReadToEnd());
            var lexer = new GeneratorLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new GeneratorParser(tokens);
            IParseTree tree = parser.expr();
            if (tree == null) return new NodeError(null, "Недопустимое выражение");
            var visitor = new GeneratorVisitor();
            var n = Node.Visit(visitor, subParse => subParse[0], tree);
            if (n is NodeError && n.Token == null)
                return new NodeError(null, "Недопустимое выражение");
            return n;
        }
    }
}