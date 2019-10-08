using System.Threading;
using BaseLibrary;
using CommonTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TablikTest.Generator
{
    [TestClass]
    public class TablsTest
    {
        [TestMethod]
        public void TablsList()
        {
            var tdir = Different.GetTestInfoTaskDir() + @"CommonTypes\Tabls\TablsList\";
            var tdb = new DaoDb(tdir + "Project1GenData.accdb");
            {
                var tlist = new TablsList(tdb);
                Assert.IsNotNull(tlist.Tabls);
                Assert.IsTrue(tlist.Tabls.ContainsKey("T1"));
                var t = tlist.Tabls["T1"];
                Assert.AreEqual("T1", t.Code);
                Assert.AreEqual(1, t.MaxLevel);
                Assert.AreEqual(2, t.Fields.Count);
                Assert.AreEqual(2, t.Fields[0].Count);
                Assert.AreEqual(2, t.FieldsNums[0].Count);
                Assert.IsTrue(new TablField("StringField", 0, DataType.String).IsEquals(t.FieldsNums[0][0]));
                Assert.IsTrue(new TablField("IntField", 1, DataType.Integer).IsEquals(t.FieldsNums[0][1]));
                Assert.AreEqual(2, t.Fields[1].Count);
                Assert.AreEqual(2, t.FieldsNums[1].Count);
                Assert.IsTrue(new TablField("StringSub", 0, DataType.String).IsEquals(t.FieldsNums[1][0]));
                Assert.IsTrue(new TablField("IntSub", 1, DataType.Integer).IsEquals(t.FieldsNums[1][1]));

                Assert.IsTrue(tlist.Tabls.ContainsKey("T2"));
                t = tlist.Tabls["T2"];
                Assert.AreEqual("T2", t.Code);
                Assert.AreEqual(0, t.MaxLevel);
                Assert.AreEqual(1, t.Fields.Count);
                Assert.AreEqual(1, t.Fields[0].Count);
            }
        }

         [TestMethod]
         public void LoadValues()
         {
             var tdir = Different.GetTestInfoTaskDir() + @"CommonTypes\Tabls\LoadValues\";
             var tdb = new DaoDb(tdir + "Project2GenData.accdb");
             {
                 var tlist = new TablsList(tdb);
                 tlist.LoadValues();

                 var t = tlist.Tabls["T1"];
                 var rnums = t.TablValues.SubNums;
                 var rcodes = t.TablValues.SubCodes;
                 Assert.IsTrue(rnums.ContainsKey(1));
                 Assert.AreEqual("aa", rnums[1].Code);
                 Assert.AreEqual("sssa", rnums[1]["stringfield"].String);
                 Assert.AreEqual(10, rnums[1]["intfield"].Integer);
                 Assert.AreEqual(2, rnums[1].SubCodes.Count);
                 Assert.AreEqual(2, rnums[1].SubNums.Count);
                 Assert.AreEqual("a1", rnums[1].SubNums[1].Code);
                 Assert.AreEqual(2, rnums[1].SubCodes["a2"].Num);
                 Assert.AreEqual("sa1", rnums[1].SubNums[1]["StringSub"].String);
                 Assert.AreEqual("sa2", rnums[1].SubNums[2]["StringSub"].String);
                 Assert.AreEqual("sa1", rnums[1].SubNums[1][0].String);
                 Assert.AreEqual("sa2", rnums[1].SubNums[2][0].String);
                 Assert.AreEqual(11, rnums[1].SubNums[1][1].Integer);
                 Assert.AreEqual(12, rnums[1].SubNums[2][1].Integer);

                 Assert.IsTrue(rcodes.ContainsKey("aa"));
                 Assert.AreEqual(1, rcodes["aa"].Num);
                 Assert.AreEqual("sssa", rcodes["aa"]["stringfield"].String);
                 Assert.AreEqual("sa2", rcodes["aa"].SubNums[2]["StringSub"].String);

                 Assert.IsTrue(rnums.ContainsKey(2));
                 Assert.AreEqual("bb", rnums[2].Code);
                 Assert.AreEqual("Sssb", rnums[2]["StringField"].String);
                 Assert.AreEqual(20, rnums[2]["IntField"].Integer);
                 Assert.AreEqual(1, rnums[2].SubCodes.Count);
                 Assert.AreEqual(1, rnums[2].SubNums.Count);
                 Assert.AreEqual("b1", rnums[2].SubNums[1].Code);
                 Assert.AreEqual("sb1", rnums[2].SubNums[1]["StringSub"].String);

                 Assert.IsTrue(rnums.ContainsKey(3));
                 Assert.AreEqual("cc", rnums[3].Code);
                 Assert.AreEqual("SSSC", rnums[3]["StringField"].String);
                 Assert.AreEqual(30, rnums[3]["IntField"].Integer);
                 Assert.AreEqual(3, rnums[3].SubCodes.Count);
                 Assert.AreEqual(3, rnums[3].SubNums.Count);
                 Assert.AreEqual("c2", rnums[3].SubNums[2].Code);
                 Assert.AreEqual("sc3", rnums[3].SubNums[3]["StringSub"].String);
                 Assert.AreEqual(33, rnums[3].SubNums[3]["IntSub"].Integer);

                 t = tlist.Tabls["T2"];
                 Assert.AreEqual(0, t.TablValues.SubNums.Count);
                 Assert.AreEqual(0, t.TablValues.SubCodes.Count);
             }
         }
    }
}
