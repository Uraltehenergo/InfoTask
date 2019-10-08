using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.IO;

namespace AuditMonitor
{
    public class ClassParams
    {
        private const string ParamsFileName = @"AuditMonitorParams.txt";
        
        public bool? SaveNetOnClose { get; set; }
        public byte? FirstScanAddress { get; set; }
        public byte? LastScanAddress { get; set; }
        
        public int SetParam(string paramName, string paramValue)
        {
            int res;
            switch (paramName)
            {
                //добавить в case'ах проверку на ParamValue (ошибка 2)
                case @"ModuleTimeOut":
                    int n;
                    if (int.TryParse(paramValue, out n)) { Program.Net.ModuleTimeOut = n; res = 0; }
                    else res = 1;
                    break;
                case @"ReadPeriod":
                    if (int.TryParse(paramValue, out n)) { Program.Net.ReadPeriod = n; res = 0; }
                    else res = 1;
                    break;
                case "WriteArchiveByRead":
                    bool b;
                    if (bool.TryParse(paramValue, out b)) { Program.Net.WriteArchiveByRead = b; res = 0; }
                    else res = 1;
                    break;
                case "SaveNetOnClose":
                    if (bool.TryParse(paramValue, out b)) { SaveNetOnClose = b; res = 0; }
                    else res = 1;
                    break;
                case @"ArchiveFileName":
                    Program.FmMonitor.ArchiveFileName = paramValue;
                    res = 0;
                    break;
                case @"WriteArchive":
                    if (bool.TryParse(paramValue, out b)) { Program.FmMonitor.WriteArchive = b; res = 0; }
                    else res = 1;
                    break;
                case @"Port":
                    byte k;
                    if (byte.TryParse(paramValue, out k)) { Program.Net.OpenPort(k); res = 0; }
                    else res = 1;
                    break;
                case @"FirstScanAddress":
                    if (byte.TryParse(paramValue, out k)) { FirstScanAddress = k; res = 0; }
                    else res = 1;
                    break;
                case @"LastScanAddress":
                    if (byte.TryParse(paramValue, out k)) { LastScanAddress = k; res = 0; }
                    else res = 1;
                    break;
                case "VirtualModules":
                    Program.Net.InitVirtNet(paramValue);
                    res = 0;
                    break;
                case @"FmMonitorWidth":
                    if (int.TryParse(paramValue, out n)) { Program.FmMonitor.Width = n; res = 0; } 
                    else res = 1;
                    break;
                case @"FmMonitorHeight":
                    if (int.TryParse(paramValue, out n)) { Program.FmMonitor.Height = n; res = 0; } 
                    else res = 1;
                    break;
                case @"FmMonitorTop":
                    if (int.TryParse(paramValue, out n)) { Program.FmMonitor.Top = n; res = 0; } 
                    else res = 1;
                    break;
                case @"FmMonitorLeft":
                    if (int.TryParse(paramValue, out n)) { Program.FmMonitor.Left = n; res = 0; } 
                    else res = 1;
                    break;
                case@"SplitterDistanceListsLog":
                    if (int.TryParse(paramValue, out n)) { Program.FmMonitor.SplitterDistanceListsLog = n; res = 0; } 
                    else res = 1;
                    break;
                case @"SplitterDistanceLists":
                    if (int.TryParse(paramValue, out n)) { Program.FmMonitor.SplitterDistanceLists = n; res = 0; } 
                    else res = 1;
                    break;
                case @"LvColumnDisplayIndex":
                    Program.FmMonitor.SignalListColumnDisplayIndex = paramValue; 
                    res = 0;
                    break;
                case @"LvColumnWidths":
                    Program.FmMonitor.SignalListColumnWidths = paramValue; 
                    res = 0;
                    break;
                case @"LvFont":
                    string[] lvFont = paramValue.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                    float fontSize;
                    bool fontBold, fontItalic;
                    if ((lvFont.GetLength(0) >= 4) && (float.TryParse(lvFont[1], out fontSize)) && (bool.TryParse(lvFont[2], out fontBold)) && (bool.TryParse(lvFont[3], out fontItalic)))
                    {
                        Program.FmMonitor.SetlvSignalListFontParams(lvFont[0], fontSize, fontBold, fontItalic);
                        res = 0;
                    }
                    else res = 1;
                    break;
                default:
                    res = -1;
                    break;
            }
            return res;
        }

        public void Write()
        {
            try
            {
                string fileName = System.Windows.Forms.Application.StartupPath + @"\" + ParamsFileName;
                var sw = new StreamWriter(fileName, false);
                sw.WriteLine(@"ModuleTimeOut = {0};", Program.Net.ModuleTimeOut);
                sw.WriteLine(@"ReadPeriod = {0};", Program.Net.ReadPeriod);
                sw.WriteLine(@"WriteArchiveByRead = {0};", Program.Net.WriteArchiveByRead);
                sw.WriteLine(@"SaveNetOnClose = {0};", SaveNetOnClose);
                sw.WriteLine(@"ArchiveFileName = {0};", Program.FmMonitor.ArchiveFileName);
                sw.WriteLine(@"WriteArchive = {0};", Program.FmMonitor.WriteArchive);
                sw.WriteLine(@"Port = {0};", Program.Net.Port);
                sw.WriteLine(@"FirstScanAddress = {0};", FirstScanAddress);
                sw.WriteLine(@"LastScanAddress = {0};", LastScanAddress);
                sw.WriteLine(@"VirtualModules = {0};", Program.Net.VirtualModules);
                sw.WriteLine(@"FmMonitorWidth = {0};", Program.FmMonitor.Width);
                sw.WriteLine(@"FmMonitorHeight = {0};", Program.FmMonitor.Height);
                sw.WriteLine(@"FmMonitorTop = {0};", Program.FmMonitor.Top);
                sw.WriteLine(@"FmMonitorLeft = {0};", Program.FmMonitor.Left);
                sw.WriteLine(@"SplitterDistanceListsLog = {0};", Program.FmMonitor.SplitterDistanceListsLog);
                sw.WriteLine(@"SplitterDistanceLists = {0};", Program.FmMonitor.SplitterDistanceLists);
                sw.WriteLine(@"LvColumnDisplayIndex = {0};", Program.FmMonitor.SignalListColumnDisplayIndex);
                sw.WriteLine(@"LvColumnWidths = {0};", Program.FmMonitor.SignalListColumnWidths);
                sw.WriteLine(@"LvFont = {0};", Program.FmMonitor.LvSignalList.Font.Name + ", " + Program.FmMonitor.LvSignalList.Font.Size +
                             ", " + Program.FmMonitor.LvSignalList.Font.Bold + ", " + Program.FmMonitor.LvSignalList.Font.Italic);
                sw.Close();
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.GetType() + "\n" + e.Message);
            }
        }

        public bool Read()
        {
            try
            {
                string fileName = System.Windows.Forms.Application.StartupPath + @"\" + ParamsFileName;

                if (File.Exists(fileName))
                {
                    var sr = new StreamReader(fileName, false);
                    var str = sr.ReadToEnd();
                    sr.Close();
                    //var separator = new char[] {';'};
                    var separator = new [] {";\r\n"};
                    string[] paramsArr = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i <= paramsArr.GetUpperBound(0); i++) 
                    {
                        if(paramsArr[i].Length>0)
                        {
                            var j = paramsArr[i].IndexOf(" = ");
                            if (j > 0)
                            {
                                //System.Windows.Forms.MessageBox.Show(paramsArr[i].Substring(0, j) + "\n" + paramsArr[i].Substring(j + 3));
                                SetParam(paramsArr[i].Substring(0, j), paramsArr[i].Substring(j + 3));
                            }
                        }
                    }

                    /*var str = sr.ReadLine();
                    while (str != null)
                    {
                        int i = str.IndexOf("=");
                        System.Windows.Forms.MessageBox.Show(str.Substring(0, i) + "\n" + str.Substring(i + 1));
                        SetBootupParam(str.Substring(0, i), str.Substring(i + 1));
                        str = sr.ReadLine();
                    }
                    sr.Close();*/
                    
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.GetType() + "\n" + e.Message);
                return false;
            }
        }
    }
}
