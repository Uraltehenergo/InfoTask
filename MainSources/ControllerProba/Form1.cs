using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ControllerClient;
using BaseLibrary;
using CommonTypes;
using Microsoft.Office.Interop.Access.Dao;
using Tablik;

namespace ControllerProba
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Controller _cont;

        private void butRetro_Click(object sender, EventArgs e)//Retro
        {
            _cont = new Controller();
            _cont.OpenLocal(6);
            _cont.GetSourcesTime();
            _cont.SetCalcOperations(true, true, false);
            _cont.SetDebugOperations(true, true, true, true, false, true);
            _cont.Calc(new DateTime(2016, 10, 8, 0, 00, 00), new DateTime(2016, 10, 12, 0, 00, 00));
            _cont.Close();
        }

        private void butRepeat_Click(object sender, EventArgs e)
        {
            var controller = new Controller();
            controller.OpenLocal(67);
            controller.LoadSetup();
            controller.GetSourcesTime();
            //MessageBox.Show(controller.SourcesBegin.ToString());
            //MessageBox.Show(controller.SourcesEnd.ToString());
            controller.SetCalcOperations(true, true, false);
            controller.SetDebugOperations(true, true, true, true, false, true);
            controller.Calc(new DateTime(2013, 03, 03, 0, 0, 0), new DateTime(2013, 03, 03, 0, 0, 0), "NoImit", "1", "Periodic", 15);
            controller.Close();
        }

        private void butTest_Click(object sender, EventArgs e)//Test
        {
            var controller = new Controller();
            controller.OpenLocal(21);
            controller.LoadSetup();
            controller.GetSourcesTime();
            //MessageBox.Show(controller.SourcesBegin.ToString());
            //MessageBox.Show(controller.SourcesEnd.ToString());
            controller.SetCalcOperations(true, true, true);
            controller.SetDebugOperations(true, true, true, true, true, true);
            //controller.Calc(new DateTime(2013, 06, 01, 8, 0, 0), new DateTime(2013, 06, 03, 8, 0, 0), "NoImit", "ConstructorInterval", "NamedAdd");
            controller.Calc(new DateTime(2010, 6, 1, 0, 0, 0), new DateTime(2010, 6, 1, 1, 0, 0), "NoImit");
            controller.Close();
        }

        private void ButClone_Click(object sender, EventArgs e)
        {
            var cloner = new Cloner();
            cloner.OpenLocal("OvationComm");
            //cloner.MakeClone(@"d:\InfoTask\Debug\Clones\TestCloneClone.accdb", new DateTime(2010, 06, 01, 0, 5, 0), new DateTime(2010, 06, 01, 0, 30, 0));
            //cloner.MakeClone(@"f:\InfoTask\Debug\Clones\TestCloneOvation.accdb", new DateTime(2013, 08, 25, 15, 40, 0), new DateTime(2013, 08, 25, 15, 50, 0));
            cloner.MakeClone(@"C:\InfoTask\Debug\Clones\Clone1.accdb", new DateTime(2014, 07, 11, 0, 0, 0), new DateTime(2014, 07, 12, 0, 0, 0));
            cloner.Close();
        }

        private void SqlButton_Click(object sender, EventArgs e)
        {
            var props = new SqlProps("200.0.1.20", "CalcArchiveTemplate", true, "sa", "1");
            SqlDb.Execute(props, "DELETE FROM Proba");
            using (var rec = new BulkSql(props, "Proba" ))
            {
                for (int i = 1; i <= 5; i++)
                {
                    rec.AddNew();
                    rec.Put("IntF", 10 * i);
                    double? d = 1.1 * i;
                    if (i % 2 == 0) d = null;
                    rec.Put("DoubleF", d);
                    string s = i + "abcdefghijklmnopqrstuvwxyzabcdefghijklmnopqrstuvwxyz";
                    if (i % 2 == 0) s = null;
                    rec.Put("StrF", s, true);
                    rec.Put("BoolF", i % 2 == 0);
                    rec.Put("TimeF", DateTime.Now);
                }
                rec.Update();    
            }
            //SqlDb.Execute(con, "DELETE FROM Projects");
            //var d = DateTime.Now;
            //var bulk = new BulkSql("Projects", con);
            //for (int i = 0; i < 100000; i++)
            //{
            //    bulk.AddNew();
            //    bulk.Put("Project", "Pr" + i);
            //    bulk.Put("ProjectName", "Name" + i);
            //    bulk.Put("SourceChange", DateTime.Now);
            //    bulk.Put("TimeChange", DateTime.Now);
            //    bulk.Put("TimeAdd", DateTime.UtcNow);
            //}
            //bulk.Update();
            //bulk.Close();
            //MessageBox.Show(DateTime.Now.Subtract(d).TotalSeconds.ToString());
        }

        private void DataSetButton_Click(object sender, EventArgs e)
        {
            var props = new SqlProps("200.0.1.20", "CalcArchiveTemplate", true, "sa", "1");
            SqlDb.Execute(props, "DELETE FROM Proba");
            using (var rec = new DataSetSql(props, "SELECT * FROM Proba"))
            {
                rec.AddNew();
                rec.Put("IntF", 10);
                rec.Put("DoubleF", 20.5);
                rec.Put("StrF", "sss");
                rec.Put("BoolF", true);
                rec.Put("TimeF", DateTime.Now);
                rec.Update();
                rec.AddNew();
                rec.Put("IntF", 20);
                double? d = null;
                rec.Put("DoubleF", d);
                rec.Put("StrF", "");
                rec.Put("BoolF", false);
                rec.Put("TimeF", DateTime.UtcNow);
                rec.AddNew();
                rec.Put("IntF", 30);
                rec.Update();
                rec.AddNew();
                rec.MoveFirst();
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                rec.MoveNext();
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                rec.MoveNext();
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                rec.MovePrevious();
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                rec.MoveLast();
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                MessageBox.Show(rec.FindLast("IntF", 10).ToString());
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                MessageBox.Show(rec.MoveLast() + " " + rec.MoveNext());
                MessageBox.Show(rec.EOF + " " + rec.BOF);
                MessageBox.Show(rec.MoveFirst() + " " + rec.MovePrevious());
                MessageBox.Show(rec.EOF + " " + rec.BOF);
                MessageBox.Show(rec.RecordCount.ToString());
                rec.MoveLast();
                rec.Put("IntF", 40);
                rec.Put("DoubleF", 200);
                rec.Put("StrF", "ooo");
                bool? b = null;
                rec.Put("BoolF", b);
                rec.Put("TimeF", DateTime.Now);
                MessageBox.Show(rec.GetBool("BoolF") + " " + rec.GetBoolNull("BoolF") + " " + rec.GetInt("IntF") + " " + rec.GetIntNull("IntF")
                    + " " + rec.GetDouble("DoubleF") + " " + rec.GetDoubleNull("DoubleF") + " " + rec.GetString("StrF") + " " + rec.GetTime("TimeF") + " " + rec.GetTimeNull("TimeF"));
                rec.Update();    
            }

            //SqlDb.Execute(con, "DELETE FROM Projects");
            //var d = DateTime.Now;
            //var bulk = new DataSetSql("SELECT * FROM Projects", con);
            //for (int i = 0; i < 1000; i++)
            //{
            //    bulk.AddNew();
            //    bulk.Put("Project", "Pr" + i);
            //    bulk.Put("ProjectName", "Name" + i);
            //    bulk.Put("SourceChange", DateTime.Now);
            //    bulk.Put("TimeChange", DateTime.Now);
            //    bulk.Put("TimeAdd", DateTime.UtcNow);
            //}
            //bulk.Update();
            //bulk.Close();
            //MessageBox.Show(DateTime.Now.Subtract(d).TotalSeconds.ToString());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime d = DateTime.Now;
            for(int i=0; i < 100; i++)
            {
                //SqlDb.Connect("200.0.1.20", "CalcArchive", true, "sa", "1").Close();
                DaoDb.Check(@"\\Ute02\ute\Проекты\CalcProjects\Курган\SmenVed.accdb");
                //DaoDb.Check(@"d:\InfoTask\Debug\Constructor\Constructor.accdb ");
            }
            MessageBox.Show(DateTime.Now.Subtract(d).TotalSeconds.ToString());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var props = new SqlProps("200.0.1.20", "CalcArchive", true, "sa", "1");
            SqlDb.Execute(props, "DELETE FROM SingleValues");
            SqlDb.Execute(props, "DELETE FROM SingleStrValues");

            SqlDb.Execute(props, "DELETE FROM BaseValues");
            SqlDb.Execute(props, "DELETE FROM HourValues");
            SqlDb.Execute(props, "DELETE FROM DayValues");
            SqlDb.Execute(props, "DELETE FROM PeriodicStrValues");
            SqlDb.Execute(props, "DELETE FROM AbsoluteValues");
            SqlDb.Execute(props, "DELETE FROM AbsoluteStrValues");
            SqlDb.Execute(props, "DELETE FROM AbsoluteDayValues");
            SqlDb.Execute(props, "DELETE FROM MomentsValues");

            SqlDb.Execute(props, "DELETE FROM Intervals");
            SqlDb.Execute(props, "DELETE FROM Ranges");
            SqlDb.Execute(props, "DELETE FROM Params");
            SqlDb.Execute(props, "DELETE FROM Projects");

            SqlDb.Execute(props, "DELETE FROM ReportParams");
            SqlDb.Execute(props, "DELETE FROM Reports");

            SqlDb.Execute(props, "DELETE FROM HandInputValues");
            SqlDb.Execute(props, "DELETE FROM AbsoluteEditValues");
            SqlDb.Execute(props, "DELETE FROM HandInputProjects");
        }

        private void butCompile_Click(object sender, EventArgs e)
        {
            string itd = Different.GetInfoTaskDir();
            var t = new TablikCompiler { ShowIndicator = true };
            t.SetHistoryFile(itd + @"Constructor\TablikHistory\TablikHistory.accdb");
            t.SetCompiledDir(itd + @"Compiled");
            //t.LoadProject(itd + @"ProjectsReal\Тутаев\TutElectro.accdb");
            //t.LoadProject(itd + @"ProjectsReal\Сургут2-3\Electro3.accdb");
            //t.LoadProject(itd + @"ProjectsReal\Курган\Ktec2_Tep.accdb");
            //t.LoadProject(itd + @"Projects\PrSignals.accdb");
            //t.LoadProject(itd + @"Analyzer\ArhAnalyzerProject.accdb");
            t.LoadProject(itd + @"Projects\ClassTest.accdb");
            //t.LoadProject(itd + @"ProjectsReal\СУГРЭС-11\Sugres11_PN.accdb");
            //t.LoadProject(itd + @"Projects\ApdControl.accdb");
            //MessageBox.Show(t.ProjectFile + " " + t.ProjectCode + " " + t.ProjectName);
            t.LoadSignals();
            string res = t.CompileProject();
            if (!res.IsEmpty()) MessageBox.Show(res);
            t.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string s = @"d:\InfoTask\Debug\ProjectsReal\Курган\Electro.accdb";
            string sql = "SELECT Objects.*, Signals.* FROM Objects INNER JOIN Signals ON Objects.ObjectId = Signals.ObjectId";
            var db = new DaoDb(s);
            var recd = new RecDao(db, sql);
            DateTime d = DateTime.Now;
            var reca = new ReaderAdo(db, sql);
            while (reca.Read())
            {
                string f = reca.GetString("FullCode");
                f = reca.GetString("CodeSignal");
                f = reca.GetString("NameSignal");
            }
            reca.Dispose();
            MessageBox.Show(DateTime.Now.Subtract(d).ToString());
            recd.Dispose();
            db.Dispose();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string s = @"d:\Calc\Projects\Курган\Electro.accdb";
            string sql = "SELECT Objects.*, Signals.* FROM Objects INNER JOIN Signals ON Objects.ObjectId = Signals.ObjectId";
            var db = new DaoDb(s);
            var recd = new RecDao(db, sql);
            var reca = new ReaderAdo(db, sql);
            DateTime d = DateTime.Now;
            while (recd.Read())
            {
                string f = recd.GetString("FullCode");
                f = recd.GetString("CodeSignal");
                f = recd.GetString("NameSignal");
            }
            recd.Dispose();
            MessageBox.Show(DateTime.Now.Subtract(d).ToString());
            reca.Dispose();
            db.Dispose();
        }

        private void ButCheckSignals_Click(object sender, EventArgs e)
        {
            string itd = Different.GetInfoTaskDir();
            var t = new TablikCompiler { ShowIndicator = true };
            t.SetHistoryFile(itd + @"Constructor\TablikHistory\TablikHistory.accdb");
            t.SetCompiledDir(itd + @"Compiled");
            //t.LoadProject(itd + @"ProjectsReal\Тутаев\TutElectro.accdb");
            //t.LoadProject(itd + @"Projects\CalcTest.accdb");
            //t.LoadProject(itd + @"ProjectsReal\Сургут2-3\Electro3.accdb");
            t.LoadProject(itd + @"ProjectsTest\Super2.accdb");
            //t.LoadProject(itd + @"Projects\P1.accdb");
            MessageBox.Show(t.CheckSignals());
            t.Close();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Regex.IsMatch(@"aa bb aa cc", @"^.*bb.*cc").ToString());
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //Crypted.Text = Crypting.Encrypt("true");
           //Crypted.Text = Crypting.Decrypt("t«2WиZоэc");
            Crypted.Text = Crypting.Encrypt("false");
        }

        private void butSubstring_Click(object sender, EventArgs e)
        {
            var t = DateTime.Now;
            string st = "sejghlsafkЛПШЛПВываиыарРВJH:ЖЛРАЖЛмфм фафоа:J уыпЗЖ:JOFK|:KO{UHT^(&^(*JDC;jv;kshzdvkhasdvkhaвртвыржоывДОПРАДОпрдффпиыпджsvОВАлыk;ZKHVALSVJVJZ;VHF;ZKHVALSVJ;K))}";
            var r = new Random();
            for (int i = 0; i < 1000000; i++)
                st.Substring(r.Next(1, 50), r.Next(52, 100));
            MessageBox.Show(DateTime.Now.Subtract(t).ToString());
        }

        private void button10_Click(object sender, EventArgs e)
        {
            double? b = null;
            MessageBox.Show(b.ToString());
            MessageBox.Show(b.ToString().IsEmpty().ToString());
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show((1 << 31).ToString());
        }

        private void ButSourceReader_Click(object sender, EventArgs e)
        {
            var r = new SourceReader();
            r.RunSource("KosmotronikaRetroSource", "Source", "RetroServerName=RetroServerM");
            MessageBox.Show(r.SourceBegin.ToString() + " - " + r.SourceEnd.ToString());
            r.ClearSignals();
            r.AddSignal("K1HHG01CP003XQ01.1_Пар", "действ", "NumType=1;NumOut=1;Appartment=0;SysNum=10028;");
            //r.ReadValues(r.SourceBegin.AddHours(5));
            string s = "";
            for (int i = 0; i < 100; i++)
            {
                r.ReadValues(DateTime.Now);
                s += r.RealValue("K1HHG01CP003XQ01.1_Пар") + "  ";
                Thread.Sleep(10);
            }
            MessageBox.Show(s);
            r.Close();
        }

        private void ButAnalyzerVed_Click(object sender, EventArgs e)
        {
            _cont = new Controller();
            //_cont.OpenLocal(84);
            //_cont.AnalyzerCalc(new DateTime(2010, 06, 01, 0, 0, 0), new DateTime(2010, 06, 01, 0, 30, 0), @"d:\InfoTask\Debug\Tmp\Ved.accdb", "Task1");
            _cont.OpenLocal(107);
            _cont.SetDebugOperations(true, true, true, true, false, true);
            //_cont.AnalyzerCalc(new DateTime(2013, 10, 19, 0, 0, 0), new DateTime(2013, 10, 19, 3, 0, 0), @"d:\InfoTask\Debug\Projects\RasVedVTZ.accdb", "ВТЗ");
            //_cont.AnalyzerCalc(new DateTime(2015, 09, 20, 12, 0, 0), new DateTime(2015, 09, 20, 15, 0, 0), @"d:\Calc\Проекты\Сургут2-1\RAS\Ved\ReviewVtz.accdb", "ВТЗ");
            _cont.AnalyzerCalc(new DateTime(2015, 09, 20, 12, 0, 0), new DateTime(2015, 09, 20, 12, 20, 0), @"d:\Calc\Проекты\Сургут2-1\RAS\Ved\VedAnalog.accdb", "Аналоговые сигналы");
            _cont.Close();
        }

        private void GenGrammars_Click(object sender, EventArgs e)
        {
            var f = new GrammarsForm();
            f.Show();
        }

        private void ButIndex_Click(object sender, EventArgs e)
        {
            DaoDb.RunOverIndexList(@"d:\InfoTask\Debug\ProjectsReal\Сургут2-1\RAS_NEW\RasProject.accdb", "SysSubTabl");
            //DaoDb.RunOverIndexList(@"d:\InfoTask\Debug\ProjectsReal\Курган\RAS_NEW\RasProject.accdb", "SysSubTabl");
        }

        private void button12_Click(object sender, EventArgs e)
        {
            var en = new DBEngine();
            var db = new DaoDb(@"d:\Calc\Projects\Курган\Electro.accdb");
            var rec = new RecDao(db, "CalcParams");
        }

        private void Crypted_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
