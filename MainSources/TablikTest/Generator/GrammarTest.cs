using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tablik.Generator;

namespace TablikTest.Generator
{
     [TestClass]
    public class GrammarTest
    {
        //Разбор грамматики Generator
        private string Parse(string text)
        {
            var parsing = new ParsingGenerator();
            return parsing.Parse(text).ToTestString();
        }

        [TestMethod]
        public void Text()
        {
            Assert.AreEqual("Combined: (Text: text)", 
                Parse("text"));
            Assert.AreEqual("Combined: (Text: SubTabl UpTabl Or And Not И Или Не)",
                Parse("SubTabl UpTabl Or And Not И Или Не"));
            Assert.AreEqual("Combined: (Text: 2+ 3*4.5 -67/(8.1^0))",
                Parse("2+ 3*4.5 -67/(8.1^0)"));
            Assert.AreEqual("Combined: (Text: aa'строка'dd)",
                Parse("aa'строка'dd"));
            Assert.AreEqual("Combined: (Text: {сигнал}{сигнал})",
                Parse("{сигнал}{сигнал}"));
            Assert.AreEqual("Combined: (Text: ввв\n/*комментарий*/\ndff)",
                Parse("ввв\n/*комментарий*/\ndff"));
            Assert.AreEqual("Combined: (Text: 'd{ww}d')",
                Parse("'d{ww}d'"));
            Assert.AreEqual("Combined: (Text: 'd{w/*qq*/w}d')",
                Parse("'d{w/*qq*/w}d'"));
            Assert.AreEqual("Combined: (Text: {'d{w/*qq*/w}.d'}')",
                Parse("{'d{w/*qq*/w}.d'}'"));
            Assert.AreEqual("Combined: (Text: 'sss' 'ttt' 'uuu')",
                Parse("'sss' 'ttt' 'uuu'"));
            Assert.AreEqual("Combined: (Text: /* [жизнь] */.)",
                Parse("/* [жизнь] */."));
            Assert.AreEqual("Combined: (Text: aa/* [Подтабл [ddd]] */bb)",
                Parse("aa/* [Подтабл [ddd]] */bb"));
            Assert.AreEqual(@"Combined: (Text: {Сигнал})",
                Parse(@"\{Сигнал\}"));
            Assert.AreEqual(@"Combined: (Text: 'Строка')",
                Parse(@"\'Строка\'"));
            Assert.AreEqual(@"Combined: (Text: aa/*Комментарий*/bb)",
                Parse(@"aa\/*Комментарий\*/bb"));
        }

         [TestMethod]
         public void Field()
         {
             Assert.AreEqual("Combined: (Field: field)",
                Parse("[field]"));
             Assert.AreEqual("Combined: (Text: {, Field: field, Text: })",
                Parse(@"\{[field]\}"));
             Assert.AreEqual("Combined: (Field: field)",
                Parse("[  field]"));
             Assert.AreEqual("Combined: (Text: aa/*вв*/,, Field: field, Text:  sd {d}s)",
                Parse("aa/*вв*/,[field] sd {d}s"));
             Assert.AreEqual("Combined: (Field: aa, Text:  , Field: bb)",
                Parse("[ aa] [bb]"));
             Assert.AreEqual("Combined: (Fun: ToIdent (Field: Field))",
                Parse("[Field.ToIdent]"));
         }

          [TestMethod]
          public void Parent()
          {
              Assert.AreEqual("Combined: (Parent: Надтабл (Combined: (Text: sss)))",
                  Parse("[Надтабл [sss]]"));
              Assert.AreEqual("Combined: (Text: bbb, Parent: Uptabl (Combined: (Text: sss, Field: field)), Text: eee)",
                  Parse("bbb[Uptabl[sss[field]]]eee"));
              Assert.AreEqual("Combined: (Text: bbb, Parent: НадТабл (Combined: (Text: sss, Field: field, Text: ttt)), Text: eee)",
                  Parse("bbb[ НадТабл [sss[field]ttt] ]eee"));
              Assert.AreEqual("Combined: (Text: bbb, Parent: Надтабл (Combined: (Text: sss, Field: field, Text: ttt)), Text:  fff , Field: field, Text: eee )",
                  Parse("bbb[ Надтабл [sss[ field]ttt] ] fff [ field]eee "));
          }

          [TestMethod]
          public void Children()
          {
              Assert.AreEqual("Combined: (Children: ПодТабл (Combined: (Text: aa), Combined: (Text: bb)))",
                Parse("[ПодТабл[aa][bb]]"));
              Assert.AreEqual("Combined: (Children: ПодТабл (Combined: (Text: a, Field: field, Text: a), Combined: (Text: bb)))",
                Parse("[ПодТабл[a[field]a][bb]]"));
              Assert.AreEqual("Combined: (Text: f=Fun(, Children: ПодТабл (Combined: (Text: x), Combined: (Text: , )), Text: ):f)",
                Parse("f=Fun([ПодТабл[x][, ]]):f"));
              Assert.AreEqual("Combined: (Text: Fun(, Children: ПодТабл (Combined: (Text: x, Children: ПодТабл (Combined: (Text: y), Combined: (Text: , )), Text:  ), Combined: (Text: ; )), Text: ))",
                Parse("Fun([ ПодТабл [x[ ПодТабл [y] [, ] ] ] [; ] ])"));
              Assert.AreEqual("Combined: (Text: Fun(, Children: ПодТабл (Combined: (Text: x, Children: ПодТабл (Combined: (Text: y, Field: ind), Combined: (Text: , )), Text:  ), Combined: (Text: ; )), Text: ))",
                Parse("Fun([ ПодТабл [x[ ПодТабл [y[ind]] [, ] ] ] [; ] ])"));
          }

         [TestMethod]
         public void Cond()
         {
             Assert.AreEqual("Combined: (Cond: ПодТаблУсл (Operation: == (Field: type, String: 'aa'), Combined: (Text: ss), Combined: (Text: .)))",
                Parse("[ПодТаблУсл [[type]=='aa'] [ss] [.] ]"));
             Assert.AreEqual("Combined: (Text: aa, Cond: ПодТаблУсл (Operation: == (Field: type, String: 'aa'), Combined: (Text: ss, Field: Firld), Combined: (Text: ., Field: Field)))",
                Parse("aa[ПодТаблУсл [[type]=='aa'] [ss[Firld]] [.[Field]] ]"));
             Assert.AreEqual("Combined: (Text: aa, Cond: SubTablCond (Operation: == (Field: type, String: 'aa'), Combined: (Text: ss, Cond: SubTablCond (Operation: == (Field: ind, Number: 32), Combined: (Text: ss), Combined: (Text: tt)), Text: ss), Combined: (Text: sep)))",
                Parse(@"aa[
                            SubTablCond
                                [[type]=='aa'] 
                                [ss[ 
                                    SubTablCond 
                                        [ [ind] == 32 ]
                                        [ss] 
                                        [tt]
                                    ]ss] 
                                [sep] 
                            ]"));
             Assert.AreEqual("Combined: (Text: aa, Cond: SubTablCond (Operation: == (Field: type, String: 'aa'), Combined: (Text: ss, Children: SubTabl (Combined: (Text: ss), Combined: (Text: tt)), Text: ss), Combined: (Text: sep)))",
                Parse(@"aa[
                            SubTablCond
                                [[type]=='aa'] 
                                [ss[ 
                                    SubTabl 
                                        [ss] 
                                        [tt]
                                    ]ss] 
                                [sep] 
                            ]"));
             Assert.AreEqual("Combined: (Text: text, Cond: SubTablCond (Operation: <> (Field: type, String: 'aa'), Combined: (Text: text), Combined: (Text: sep)))",
                Parse(@"text[ 
                            SubTablCond
                                [[type]<>'aa'] 
                                [text]
                                [sep]       
                            ]"));
             Assert.AreEqual("Combined: (Text: text, Cond: SubTablCond (And: and (Or: or (Operation: <> (Field: type, String: 'aa'), Operation: == (Field: code, String: 'ss')), And: and (Not: not (Operation: < (Field: num, Number: 3)), Operation: >= (Field: id, Number: 10))), Combined: (Text: text), Combined: (Text: sep)))",
                Parse(@"text[ 
                            SubTablCond
                                [(( [type]<>'aa' ) or  [code] == 'ss' ) and (not [num] < 3 and [id] >= 10)] 
                                [text]
                                [sep]       
                            ]"));
         }
    }
}