using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GraphicLibrary;
using CommonTypes;

namespace AuditMonitor
{
    public partial class FormGraphicsNew : Form
    {
        private static readonly OuterColorUseList Colors = new OuterColorUseList();
        private static readonly List<bool> Numbers = new List<bool>() ;
        
        public FormGraphicsNew()
        {
            InitializeComponent();
            
            //NewNet.EventNetRead += UpdateGraphic;
            ucGraphic.NeedAPieceOfArchiveSoMuch += AddPieceOfArchive;
        }

        private void FormGraphics_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var gr in ucGraphic.DictionaryOfGraphics)
            {
                Colors.FreeColor(gr.Value.Series.Color);
            }
        }

        //public void AddGraphic(string code)
        //{
        //    var chn = Program.Net.Modules.Channel(code);

        //    NewEnumSignalStatus status;
        //    double? min = (chn.Min == null) || (chn.Conversion == null) ? chn.Min : chn.SignalConversion((double)chn.Min, out status);
        //    double? max = (chn.Max == null) || (chn.Conversion == null) ? chn.Max : chn.SignalConversion((double)chn.Max, out status);

        //    string dataFormat = null;
        //    switch (chn.Module.ModuleType)
        //    {
        //        case "Adam4019+":
        //            dataFormat = ((NewModuleAdam4019Plus)chn.Module).DataFormat;
        //            break;
        //    }

        //    var graphicSeries = new Graphic(code, chn.Name, "", dataFormat, DataType.Real, min, max);

        //    DateTime time = chn.Time ?? DateTime.Now;
        //    double value = chn.Value ?? 0;
        //    int stat = chn.Status != null ? (int)chn.Status : 1;

        //    var dot = new MomentReal(time, value, 0, stat, 0);
        //    graphicSeries.IsUpdated = true;
        //    graphicSeries.Add(dot);
        //    graphicSeries.IsUpdated = false;

        //    ucGraphic.DictionaryOfGraphics.Add(code, graphicSeries);

        //    graphicSeries.Series.Color = Colors.GetColor();
        //    graphicSeries.Number = 1;

        //    ucGraphic.Draw();
        //}

        private bool _fg = true;
        private DateTime _time0;

        public void AddGraphic(string code, string name, string comment, string units, DataType dataType, double? min, double? max)
        {
            var graphicSeries = new Graphic(code, name, "", units, DataType.Real, min, max);
            
            if (_fg) _time0 = DateTime.Now;

            var dot = new MomentReal(_time0, 0.6 * (max ?? 0 + min ?? 0), 0, 1, 0);
            graphicSeries.IsUpdated = true;
            graphicSeries.Add(dot);
            graphicSeries.IsUpdated = false;

            ucGraphic.DictionaryOfGraphics.Add(code, graphicSeries);

            graphicSeries.Series.Color = Colors.GetColor();

            int i;
            for (i = 0; i < Numbers.Count; i++)
                if (Numbers[i] == false) Numbers[i] = true;
            if (i == Numbers.Count) Numbers.Add(true);
            graphicSeries.Number = ++i;

            if (_fg)
            {
                ucGraphic.Draw();
                _fg = false;
            }
        }

        public void DeleteGraphic(string code)
        {
            var gr = ucGraphic.DictionaryOfGraphics[code];
            Colors.FreeColor(gr.Series.Color);
            Numbers[gr.Number - 1] = false;

            ucGraphic.DictionaryOfGraphics.Remove(code);
        }

        private void AddPieceOfArchive(DateTime beginTime, DateTime endTime)
        {
            Exception ex;

            var rsArch = NewAuditArchive.ReadFromArchive(Program.FmMonitor.ArchiveFileName, Text, beginTime, endTime, out ex);
            if (ex == null)
            {
                var dots = new List<MomentValue>();
                while (rsArch.Read())
                {
                    try
                    {
                        DateTime? time = rsArch.GetTimeNull("Time");
                        double value = rsArch.GetDoubleNull("Value") ?? 0;
                        int status = rsArch.GetIntNull("Status") ?? 1;

                        if (time != null) dots.Add(new MomentReal((DateTime) time, value, 0, status, 0));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                
                try
                {
                    foreach (string cd in ucGraphic.DictionaryOfGraphics.Keys)
                        ucGraphic.DictionaryOfGraphics[cd].Add(dots);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        public void AddValue(string code, DateTime? time, double? value, int? status)
        {
            if (ucGraphic.DictionaryOfGraphics.ContainsKey(code))
            {
                var dot = new MomentReal(time ?? DateTime.Now, value ?? 0, 0, status ?? 1, 0);
                ucGraphic.DictionaryOfGraphics[code].Add(dot);
            }
        }
    }

    public class GraphicList
    {
        private long _count;
        private FormGraphicsNew _fmGraphic;

        public void AddGraphic(string code, string name, string comment, string units, DataType dataType, double? min, double? max)
        {
            if (_count == 0)
            {
                _fmGraphic = new FormGraphicsNew();
                _fmGraphic.AddGraphic(code, name, comment, units, dataType, min, max);
                _fmGraphic.Text = "Графики";
                _fmGraphic.Show();
            }
            else
                _fmGraphic.AddGraphic(code, name, comment, units, dataType, min, max);
            
            _count++;
        }
        
        public void DeleteGraphic(string code)
        {
            _fmGraphic.DeleteGraphic(code);
            _count--;
        }

        public void AddValue(string code, DateTime? time, double? value, int? status)
        {
            _fmGraphic.AddValue(code, time, value, status);
        }
    }
}