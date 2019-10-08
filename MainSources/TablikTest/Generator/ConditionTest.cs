using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tablik.Generator;

namespace TablikTest.Generator
{
    [TestClass]
    public class ConditionTest
    {
        //Разбор грамматики Condition для параметра
        private string Parse(string text)
        {
            var parsing = new ParsingCondition();
            return parsing.Parse(text).ToTestString();
        }

        //Разбор грамматики Condition для подпараметра
        private string SubParse(string text)
        {
            var parsing = new ParsingCondition();
            return parsing.SubParse(text).ToTestString();
        }

        [TestMethod]
        public void GrammarSimple()
        {
            Assert.AreEqual("Tabl: Tabl (, )",
                Parse("Tabl"));

            Assert.AreEqual("Parent: Tabl (, )",
                Parse("Tabl [Надтабл]"));

            Assert.AreEqual("Parent: Tabl (, )",
                Parse("Tabl [UPTABL]"));

            Assert.AreEqual("Tabl: Tabl (, Children: SubTabl (, ))",
                Parse("Tabl [SubTabl]"));

            Assert.AreEqual("Tabl: Tabl (, Children: ПодТабл (, ))",
                Parse("Tabl [ПодТабл]"));

            Assert.AreEqual("Ошибка в конце выражения",
                Parse("Бред какой-то"));

            Assert.AreEqual("Ошибка в конце выражения",
                Parse("Tabl [Подтабл] фцв"));

            Assert.AreEqual("Недопустимое выражение",
                Parse("Tabl [Подтабл or  ]"));

            Assert.AreEqual("Недопустимое выражение",
                Parse("Tabl [Надтабл or  ]"));

            Assert.AreEqual("Недопустимое выражение",
                Parse("Табл [НадтаблБред]"));
        }

        [TestMethod]
        public void GrammarOper()
        {
            Assert.AreEqual("Tabl: Tabl (Operation: == (Field: type, String: 'aa'), )",
                Parse("Tabl \n [[type]=='aa']"));

            Assert.AreEqual("Tabl: Tabl (Operation: == (Field: num, Number: 3), )",
                Parse("Tabl [[num]==3]"));

            Assert.AreEqual("Tabl: Tabl (Operation: <> (Field: num, Number: 6877), )",
                Parse("Tabl \n [ [num] <> 6877 ]"));

            Assert.AreEqual("Tabl: Tabl (Operation: < (Field: num, Number: 6877), )",
                Parse("Tabl[[num] < 6877]"));

            Assert.AreEqual("Tabl: Tabl (Operation: > (Field: n, Number: 5), )",
                Parse("Tabl[ [n] > 5 ]"));

            Assert.AreEqual("Tabl: Tabl (Operation: >= (Field: n, Number: 5), )",
                Parse("Tabl [([n] >= 5)]"));

            Assert.AreEqual("Tabl: Com_Tabl (Operation: <= (Field: n, Number: 5), )",
                Parse("Com_Tabl [ ( ( [n] <= 5 ) ) ]"));

            Assert.AreEqual("Недопустимое выражение",
                Parse("Tabl [([Type]='sss') ]"));
        }

        [TestMethod]
        public void GrammarLogic()
        {
            Assert.AreEqual("Tabl: Tabl (Or: or (Operation: == (Field: code, String: 'aa'), Operation: == (Field: code, String: 'bb')), )",
                Parse("Tabl [ ([code] == 'aa') or ([code] == 'bb')]"));

            Assert.AreEqual("Tabl: Tabl (And: And (Operation: == (Field: code, String: 'aa'), Operation: == (Field: code, String: 'bb')), )",
                Parse("Tabl [ ([code] == 'aa') And ([code] == 'bb')]"));

            Assert.AreEqual("Tabl: Tabl (Not: not (Operation: == (Field: code, String: 'aa')), )",
                Parse("Tabl [not ([code] == 'aa')]"));

            Assert.AreEqual("Tabl: Tabl (Or: or (Not: not (Operation: == (Field: code, String: 'aa')), Not: not (Operation: == (Field: code, String: 'bb'))), )",
                Parse("Tabl [not ([code] == 'aa') or not ([code] == 'bb') ]"));

            Assert.AreEqual("Tabl: Tabl (And: and (Or: or (Operation: == (Field: code, String: 'aa'), Operation: == (Field: num, Number: 3)), Operation: <> (Field: type, String: 'dd')), )",
                Parse("Tabl [(([code] == 'aa') or ([num] == 3)) and ([type] <> 'dd') ]"));
        }

        [TestMethod]
        public void GrammarComplex()
        {
            Assert.AreEqual("Tabl: Tabl (Operation: == (Field: code, String: 'aa'), Children: Подтабл (, ))",
                Parse("Tabl  [[code] == 'aa' ] [Подтабл]"));

            Assert.AreEqual("Tabl: Tabl (Operation: == (Field: code, String: 'aa'), Children: Подтабл (Operation: == (Field: num, Number: 3), Children: Подтабл (, )))",
                Parse("Tabl  [[code] == 'aa' ] [Подтабл] [[num] == 3] [Подтабл]"));

            Assert.AreEqual("Tabl: Tabl (Operation: == (Field: code, String: 'aa'), Children: Подтабл (Operation: == (Field: num, Number: 3), Children: Подтабл (, Children: Подтабл (, ))))",
                Parse("Tabl  [[code] == 'aa' ]  [Подтабл] [[num] == 3] [Подтабл] [Подтабл]"));

            Assert.AreEqual("Недопустимое выражение",
                Parse("Tabl [Подтабл] [[aa]==22 s] [Подтабл]"));

            Assert.AreEqual("Недопустимое выражение",
                Parse("Tabl [[aa]==22] [Подтабл a]"));
        }

        [TestMethod]
        public void SubCondition()
        {
            Assert.AreEqual("Children: SubTabl (, )",
                SubParse("[SubTabl]"));
            Assert.AreEqual("Children: ПодТабл (, )",
                SubParse("[ПодТабл]"));
            //Assert.AreEqual("Ошибка в конце выражения",
            //    SubParse("[Подтабл] фцв"));
            Assert.AreEqual("Недопустимое выражение",
                SubParse("[Подтабл or  ]"));
            Assert.AreEqual("Недопустимое выражение",
                SubParse("Tabl [Подтабл]"));
            Assert.AreEqual("Children: Подтабл (Or: or (Operation: == (Field: code, String: 'aa'), Operation: == (Field: code, String: 'bb')), )",
                SubParse("[Подтабл] [ ([code] == 'aa') or ([code] == 'bb')]"));
            Assert.AreEqual("Children: ПодТабл (And: and (Or: or (Operation: == (Field: code, String: 'aa'), Operation: == (Field: num, Number: 3)), Operation: <> (Field: type, String: 'dd')), )",
                SubParse("[ПодТабл] [(([code] == 'aa') or ([num] == 3)) and ([type] <> 'dd') ]"));
        }
    }
}
