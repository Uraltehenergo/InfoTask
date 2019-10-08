using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaseLibrary;
using GraphicLibrary;
using GraphicLibrary.Params;

namespace _4._0
{
    public partial class ArhAnalyzerTest : Form
    {
        public ArhAnalyzerTest()
        {
            InitializeComponent();
        }

        private void ArhAnalyzerTest_Load(object sender, EventArgs e)
        {
            tbArhAnalyzer.Text = @"C:\InfoTask\Debug\Analyzer\ArhAnalyzer.accdb";
            tbArhAnalyzerArchive.Text = @"C:\InfoTask\Debug\Analyzer\ArhAnalyzerArchive.accdb";
            tbArhAnalyzerProject.Text = @"C:\InfoTask\Debug\Analyzer\ArhAnalyzerProject.accdb";
        }

        private void butArhAnalyzerTest_Click(object sender, EventArgs e)
        {
            try
            {
                string dbFile = tbArhAnalyzer.Text;
                string archiveFile = tbArhAnalyzerArchive.Text;
                string projectFile = tbArhAnalyzerProject.Text;
                string task = cbTask.Text;
                byte decPlacesDefault = 4;
                
                DateTime beginTime;
                DateTime endTime;

                int projectId = -1;
                int intervalId = -1;

                string stSql = "SELECT ProjectId FROM Projects WHERE (Project='ArhAnalyzer');";
                using (var reader = new ReaderAdo(archiveFile, stSql))
                {
                    projectId = reader.GetInt("ProjectId");
                }

                stSql = "SELECT IntervalId, TimeBegin, TimeEnd FROM NamedIntervals WHERE (IntervalName='" + task + "');";
                using (var reader = new ReaderAdo(archiveFile, stSql))
                {
                    intervalId = reader.GetInt("IntervalId");
                    beginTime = reader.GetTime("TimeBegin");
                    endTime = reader.GetTime("TimeEnd");
                }

                GraphicForm form = new GraphicForm();
                form.SetDatabase("Access", dbFile);
                form.SetTimerMode(0);
                form.TimeBegin = beginTime;
                form.TimeEnd = endTime;
                form.SetDecPlacesDefault(1);
                form.Show();

                if ((projectId > -1) && (intervalId > -1))
                {
                    stSql = "SELECT * FROM CalcParams WHERE (Task='" + task + "') AND (CalcOtm=True) ORDER BY Code;";
                    using (var readerParam = new ReaderAdo(projectFile, stSql))
                    {
                        while (readerParam.Read())
                        {
                            var code = readerParam.GetString("Code");
                            var name = readerParam.GetString("Name");
                            var resultType = readerParam.GetString("ResultType");
                            var min1 = readerParam.GetDoubleNull("Min");
                            double min = min1 ?? 0;
                            var max1 = readerParam.GetDoubleNull("Max");
                            double max = max1 ?? 0;
                            var units = readerParam.GetString("Units");
                            var decPlaces1 = readerParam.GetIntNull("DecPlaces");
                            int decPlaces = decPlaces1 ?? decPlacesDefault;

                            var errMess = form.AddParam(code, name, "", resultType, min, max, units, decPlaces);

                            if (errMess != "")
                            {
                                errMess = form.ErrorMessage + "\n" + form.ErrorParams;
                                MessageBox.Show(errMess);
                                return;
                            }

                            GraphicParam temp = form.GetParamByCode(code);

                            stSql = "SELECT ParamId FROM Params WHERE (ProjectId=" + projectId + ") AND (Task='" + 
                                    task + "') AND (Code='" + temp.Code + "')";

                            int paramId = -1;
                            using (var reader = new ReaderAdo(archiveFile, stSql))
                            {
                                paramId = reader.GetInt("ParamId");
                                temp.Id = paramId;
                            }

                            if (paramId > -1)
                            {
                                stSql = "SELECT '" + temp.Code + "' AS Code, Null AS Id, Time, Value AS Val, Nd " +
                                        "FROM [" + archiveFile + "].NamedValues WHERE (IntervalId=" + intervalId +
                                        ") AND (ParamId=" + paramId + ") ORDER BY Time;";

                                errMess = form.LoadValues(stSql, beginTime, endTime);
                                //errMess = form.LoadValues(stSql);
                                if (errMess != "")
                                {
                                    //errMess = form.ErrorMessage + "\n" + form.ErrorParams;
                                    //MessageBox.Show(errMess);
                                    return;
                                }
                            }

                            form.Repaint(temp);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        private void butArhAnalyzerTestNew_Click(object sender, EventArgs e)
        {
            try
            {
                string dbFile = tbArhAnalyzer.Text;
                string archiveFile = tbArhAnalyzerArchive.Text;
                string projectFile = tbArhAnalyzerProject.Text;
                string task = cbTask.Text;
                byte decPlacesDefault = 4;

                DateTime beginTime;
                DateTime endTime;

                int projectId = -1;
                int intervalId = -1;

                string stSql = "SELECT ProjectId FROM Projects WHERE (Project='ArhAnalyzer');";
                using (var reader = new ReaderAdo(archiveFile, stSql))
                {
                    projectId = reader.GetInt("ProjectId");
                }

                stSql = "SELECT IntervalId, TimeBegin, TimeEnd FROM NamedIntervals WHERE (IntervalName='" + task + "');";
                using (var reader = new ReaderAdo(archiveFile, stSql))
                {
                    intervalId = reader.GetInt("IntervalId");
                    beginTime = reader.GetTime("TimeBegin");
                    endTime = reader.GetTime("TimeEnd");
                }

                var form = new FormGraphic();
                form.Init(beginTime, endTime);
                form.SetDatabase("Access", dbFile);
                //form.SetDecPlacesDefault(1);
                form.Show();

                if ((projectId > -1) && (intervalId > -1))
                {
                    stSql = "SELECT * FROM CalcParams WHERE (Task='" + task + "') AND (CalcOtm=True) ORDER BY Code;";
                    using (var readerParam = new ReaderAdo(projectFile, stSql))
                    {
                        while (readerParam.Read())
                        {
                            var code = readerParam.GetString("Code");
                            var name = readerParam.GetString("Name");
                            var resultType = readerParam.GetString("ResultType");
                            var min1 = readerParam.GetDoubleNull("Min");
                            double min = min1 ?? 0;
                            var max1 = readerParam.GetDoubleNull("Max");
                            double max = max1 ?? 0;
                            var units = readerParam.GetString("Units");
                            var decPlaces1 = readerParam.GetIntNull("DecPlaces");
                            int decPlaces = decPlaces1 ?? decPlacesDefault;
                            
                            stSql = "SELECT ParamId FROM Params WHERE (ProjectId=" + projectId + ") AND (Task='" +
                                    task + "') AND (Code='" + code + "')";

                            int paramId = -1;
                            using (var reader = new ReaderAdo(archiveFile, stSql))
                            {
                                paramId = reader.GetInt("ParamId");
                            }

                            Graphic gr;
                            if ((resultType.ToDataType() == DataType.Integer) || (resultType.ToDataType() == DataType.Real))
                                gr = form.AddAnalogGraphic(code, paramId, name, "", resultType, min, max, units, decPlaces);
                            else
                                gr = form.AddDiscretGraphic(code, paramId, name, "", resultType);

                            if (gr == null) return;
                            
                            if (paramId > -1)
                            {
                                stSql = "SELECT '" + gr.Param.Code + "' AS Code, Null AS Id, Time, Value AS Val, Nd " +
                                        "FROM [" + archiveFile + "].NamedValues WHERE (IntervalId=" + intervalId +
                                        ") AND (ParamId=" + paramId + ") ORDER BY Time;";

                                string errMess = form.LoadValues(stSql, beginTime, endTime);
                                //errMess = form.LoadValues(stSql);
                                if (errMess != "")
                                {
                                    //errMess = form.ErrorMessage + "\n" + form.ErrorParams;
                                    //MessageBox.Show(errMess);
                                    return;
                                }
                            }

                            form.RepaintGraphic(gr);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + "\n=========\n" + exception.StackTrace);
            }
        }

        private void tbArhAnalyzerProject_TextChanged(object sender, EventArgs e)
        {
            UpdateTasks();
        }

        private void butSelectArhAnalyzer_Click(object sender, EventArgs e)
        {
            const string title = @"Файл ArhAnalyzer";
            string initialDirectory = tbArhAnalyzer.Text != "" ? tbArhAnalyzer.Text : Application.StartupPath;
            const string filter = @"Базы данных Access (mdb, mde, accdb, accde)|*.mdb; *.mde; *.accdb; *.accde";

            SelectFile(title, initialDirectory, filter, tbArhAnalyzer);
        }

        private void butSelectArhAnalyzerArchive_Click(object sender, EventArgs e)
        {
            const string title = @"Файл ArhAnalyzerArchive";
            string initialDirectory = tbArhAnalyzerArchive.Text != "" ? tbArhAnalyzerArchive.Text : Application.StartupPath;
            const string filter = @"Базы данных Access (mdb, mde, accdb, accde)|*.mdb; *.mde; *.accdb; *.accde";

            SelectFile(title, initialDirectory, filter, tbArhAnalyzerArchive);
        }

        private void butSelectArhAnalyzerProject_Click(object sender, EventArgs e)
        {
            const string title = @"Файл ArhAnalyzerProject";
            string initialDirectory = tbArhAnalyzerProject.Text != "" ? tbArhAnalyzerProject.Text : Application.StartupPath;
            const string filter = @"Базы данных Access (mdb, mde, accdb, accde)|*.mdb; *.mde; *.accdb; *.accde";

            SelectFile(title, initialDirectory, filter, tbArhAnalyzerProject);
        }

        private void butSelectAll_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            string[] files = Directory.GetFiles(fbd.SelectedPath);

            tbArhAnalyzer.Text = "";
            tbArhAnalyzerArchive.Text = "";
            tbArhAnalyzerProject.Text = "";

            foreach(string f in files)
            {
                int i = f.LastIndexOf(@"\");
                string fName = f.Substring(i + 1);
                i = fName.LastIndexOf(".");
                string fEx = fName.Substring(i + 1);
                fName = fName.Substring(0, i);

                if ((fEx == "accdb") || (fEx == "accde"))
                {
                    if (fName == "ArhAnalyzer")
                        tbArhAnalyzer.Text = f;
                    else if (fName == "ArhAnalyzerArchive")
                        tbArhAnalyzerArchive.Text = f;
                    else if (fName == "ArhAnalyzerProject")
                        tbArhAnalyzerProject.Text = f;
                }
            }
        }
        
        private void UpdateTasks()
        {
            cbTask.Items.Clear();

            string projectFile = tbArhAnalyzerProject.Text;
            if (projectFile != "")
            {
                const string stSql = "SELECT [Task] FROM Tasks;";

                try
                {
                    using (var reader = new ReaderAdo(projectFile, stSql))
                    {
                        while (reader.Read())
                        {
                            cbTask.Items.Add(reader.GetString("Task"));
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }

        private void SelectFile(string title, string initialDirectory, string filter, TextBox textBox = null)
        {
            var fileDlg = new OpenFileDialog
            {
                Title = title,
                InitialDirectory = initialDirectory, //(tbArchiveFile.Text != "" ? tbArchiveFile.Text : Application.StartupPath /*typeof(fmMonitor).Assembly.Location*/),
                Filter = filter, //@"Базы данных Access (mdb, mde, accdb, accde)|*.mdb; *.mde; *.accdb; *.accde",
                ValidateNames = true,
                CheckFileExists = false,
                //ShowReadOnly = true
            };
            if (fileDlg.ShowDialog() == DialogResult.OK)
            {
                string fileName = fileDlg.FileName;
                if (textBox != null) textBox.Text = fileName;
            }
        }
    }
}
