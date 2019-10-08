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
    public partial class FormGraphics : Form
    {
        private static readonly OuterColorUseList Colors = new OuterColorUseList();
        
        public FormGraphics()
        {
            InitializeComponent();

            //-ClassAdamNet.EventNetRead += UpdateGraphic;
            NewNet.EventNetRead += UpdateGraphic;
            ucGraphic.NeedAPieceOfArchiveSoMuch += AddPieceOfArchive;
        }

        private void FormGraphics_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var gr in ucGraphic.DictionaryOfGraphics)
            {
                Colors.FreeColor(gr.Value.Series.Color);
            }
        }

        //public void AddGraphic(string moduleAddress, string channel)
        //{
        //    var addr = Convert.ToString(int.Parse(moduleAddress), 16).ToUpper();
        //    if (addr.Length == 1) addr = "0" + addr;

        //    //-int ch = int.Parse(channel);
        //    byte ch = byte.Parse(channel);

        //    var code = Program.Net.Modules[addr].Channels[ch].Code ?? addr + "." + channel;
        //    //var code = addr + "." + channel;

        //    //-var chn = channel != "CJC" ? Program.Net.Modules[addr].Channels[ch] : Program.Net.Modules[addr]).Cjc;
        //    var chn = (channel != "CJC") ? Program.Net.Modules[addr].Channels[ch] : ((NewModuleAdam4019Plus)Program.Net.Modules[addr]).Cjc;

        //    //-EChannelStatus status;
        //    NewEnumSignalStatus status;

        //    /*
        //    double? min = (chn.Min == null) || (chn.Conversion == null)
        //                ? chn.Min : ClassAbstractAdamChannel.SignalConversion((double) chn.Min, chn.Conversion, out err);

        //    double? max = (chn.Max == null) || (chn.Conversion == null)
        //                ? chn.Max : ClassAbstractAdamChannel.SignalConversion((double) chn.Max, chn.Conversion, out err);
        //    */

        //    double? min = (chn.Min == null) || (chn.Conversion == null)
        //                ? chn.Min : chn.SignalConversion((double)chn.Min, out status);

        //    double? max = (chn.Max == null) || (chn.Conversion == null)
        //                ? chn.Max : chn.SignalConversion((double)chn.Max, out status);

        //    //-var graphicSeries = new Graphic(code, chn.Name, "", chn.DataFormat, DataType.Real, min, max);

        //    string dataFormat = null;
            
        //    switch(chn.Module.ModuleType)
        //    {
        //        case "Adam4019+":
        //            dataFormat = ((NewModuleAdam4019Plus) chn.Module).DataFormat;
        //            break;
        //    }

        //    //-var graphicSeries = new Graphic(code, chn.Name, "", ((NewModuleAdam4019Plus)chn.Module).DataFormat, DataType.Real, min, max);
        //    var graphicSeries = new Graphic(code, chn.Name, "", dataFormat, DataType.Real, min, max);
            
        //    //if ((chn.Time != null) && (chn.Status != null))
        //    //{
        //    //    double value = chn.Value ?? 0;
                
        //    //    var dot = new MomentReal((DateTime) chn.Time, value, 0, (int) chn.Status, 0);
        //    //    graphicSeries.IsUpdated = true;
        //    //    graphicSeries.Add(dot);
        //    //    graphicSeries.IsUpdated = false;
        //    //}

        //    DateTime time = chn.Time ?? DateTime.Now;
        //    double value = chn.Value ?? 0;
        //    int stat = chn.Status != null ? (int) chn.Status : 1;
            
        //    var dot = new MomentReal(time, value, 0, stat, 0);
        //    graphicSeries.IsUpdated = true;
        //    graphicSeries.Add(dot);
        //    graphicSeries.IsUpdated = false;

        //    ucGraphic.DictionaryOfGraphics.Add(code, graphicSeries);

        //    graphicSeries.Series.Color = Colors.GetColor();
        //    graphicSeries.Number = 1;

        //    ucGraphic.Draw();
        //}

        public void AddGraphic(string code)
        {
            var chn = Program.Net.Modules.Channel(code);

            NewEnumSignalStatus status;
            double? min = (chn.Min == null) || (chn.Conversion == null) ? chn.Min : chn.SignalConversion((double)chn.Min, out status);
            double? max = (chn.Max == null) || (chn.Conversion == null) ? chn.Max : chn.SignalConversion((double)chn.Max, out status);
            
            string dataFormat = null;
            switch (chn.Module.ModuleType)
            {
                case "Adam4019+":
                    dataFormat = ((NewModuleAdam4019Plus)chn.Module).DataFormat;
                    break;
            }
            
            var graphicSeries = new Graphic(code, chn.Name, "", dataFormat, DataType.Real, min, max);
            
            DateTime time = chn.Time ?? DateTime.Now;
            double value = chn.Value ?? 0;
            int stat = chn.Status != null ? (int)chn.Status : 1;

            var dot = new MomentReal(time, value, 0, stat, 0);
            graphicSeries.IsUpdated = true;
            graphicSeries.Add(dot);
            graphicSeries.IsUpdated = false;

            ucGraphic.DictionaryOfGraphics.Add(code, graphicSeries);

            graphicSeries.Series.Color = Colors.GetColor();
            graphicSeries.Number = 1;

            ucGraphic.Draw();
        }

        //public void TryAddGraphicPoint(string code, DateTime time, Double? value, EChannelStatus status)
        //{
        //    if (ucGraphic.DictionaryOfGraphics.ContainsKey(code))
        //    {
        //        var dot = new MomentReal(time, value ?? 0, 0, (int)status, 0);
        //        ucGraphic.DictionaryOfGraphics[code].Add(dot);
        //    }
        //}

        //private void AddPieceOfArchive(DateTime beginTime, DateTime endTime)
        //{
        //    ////MessageBox.Show("Keine Katastrofe: \nZeit: " + beginning + "\n" + ending, "Achtung!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //    ////string codes = "";
        //    ////foreach (Graphic graphic in ucGraphic.DictionaryOfGraphics.Values) codes += graphic.Param.Code + ",";
        //    ////var datas = ClassArchive.ReadFromArchive(Program.FmMonitor.ArchiveFileName, codes, beginTime, endTime);

        //    Exception ex;
            
        //    var datas = NewAuditArchive.ReadFromArchive(Program.FmMonitor.ArchiveFileName, this.Text, beginTime, endTime, out ex);
        //    if (ex == null)
        //    {

        //        var dots = new List<MomentValue>();
        //        //string curCode = null;

        //        if (datas != null)
        //        {
        //            foreach (DataRow row in datas)
        //            {
        //                //if (row["Code"] != curCode)
        //                //{
        //                //    if (curCode != null) ucGraphic.DictionaryOfGraphics[curCode].Add(dots);
        //                //    curCode = (string)row["Code"];
        //                //}

        //                try
        //                {
        //                    DateTime? time = NewTagList.DbDateTime(row["TagTime"]);
        //                    double? value = NewTagList.DbDouble(row["TagValue"]);
        //                    byte? status = NewTagList.DbByte(row["SignalStatus"]);

        //                    if ((time != null) && (value != null) && (status != null))
        //                        dots.Add(new MomentReal((DateTime) time, (double) value, 0, (int) status, 0));
        //                }
        //                catch (Exception e)
        //                {
        //                    MessageBox.Show(e.Message);
        //                }
        //            }
        //            //if (curCode != null) ucGraphic.DictionaryOfGraphics[curCode].Add(dots);

        //            try
        //            {
        //                foreach (string cd in ucGraphic.DictionaryOfGraphics.Keys)
        //                    ucGraphic.DictionaryOfGraphics[cd].Add(dots);
        //            }
        //            catch (Exception e)
        //            {
        //                MessageBox.Show(e.Message);
        //            }
        //        }
        //    }
        //}

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

        private void UpdateGraphic(DateTime upgTime)
        {
            foreach (NewModuleAdamAbstract module in Program.Net.Modules)
                foreach(NewChannelAbstract channel in module.Channels)
                    if (ucGraphic.DictionaryOfGraphics.ContainsKey(channel.Code))
                    {
                        DateTime time = channel.Time ?? DateTime.Now;
                        double value = channel.Value ?? 0;
                        int status = channel.Status != null ? (int) channel.Status : 1;

                        var dot = new MomentReal(time, value, 0, status, 0);
                        ucGraphic.DictionaryOfGraphics[channel.Code].Add(dot);
                    }
        }
    }
}

//public class Splasher<T> where T:SplashParent, new()
//    {
//        private static Thread splashThread;
//        private static T splash;
//        static void ShowThread()
//        {            
//            splash = new T();  
//            splash.Show();
//            Application.Run(splash);            
//        }
//        /// <summary>
//        /// Отображение
//        /// </summary>
//        static public void Show()
//        {
//            if (splashThread != null)
//                return;
//            splashThread = new Thread(ShowThread) {IsBackground = true};
//            //splashThread.ApartmentState = ApartmentState.MTA;
//            splashThread.Start();
//        }
//        /// <summary>
//        /// Скрытие
//        /// </summary>
//        static public void Close()
//        {
//            if (splashThread == null) return;
//            if (splash == null) return;
//            try
//            {                
//                splash.Invoke(new MethodInvoker(splash.Close));
//            }
//            catch (Exception ex)
//            {
//                log.Error(ex.Message, ex);
//            }
//            splashThread = null;
//            splash = null;
//        }
//        /// <summary>
//        /// Задание Статуса(лейбла)
//        /// </summary>
//        static public string Status
//        {
//            set
//            {
//                if (splash == null)
//                {
//                    Thread.CurrentThread.Join(600);
//                }
//                try
//                {
//                    splash.StatusInfo = value;
//                }
//                catch (NullReferenceException)
//                { }
//            }
//            get
//            {
//                if (splash == null)
//                {
//                   return ""; 
//                }
//                return splash.StatusInfo;
//            }
//        }

    //public class Splasher<T> where T:SplashParent, new()
    //{
    //    private static Thread splashThread;
    //    private static T splash;
    //    static void ShowThread()
    //    {            
    //        splash = new T();  
    //        splash.Show();
    //        Application.Run(splash);            
    //    }
    //    /// <summary>
    //    /// Отображение
    //    /// </summary>
    //    static public void Show()
    //    {
    //        if (splashThread != null)
    //            return;
    //        splashThread = new Thread(ShowThread) {IsBackground = true};
    //        //splashThread.ApartmentState = ApartmentState.MTA;
    //        splashThread.Start();
    //    }
    //    /// <summary>
    //    /// Скрытие
    //    /// </summary>
    //    static public void Close()
    //    {
    //        if (splashThread == null) return;
    //        if (splash == null) return;
    //        try
    //        {                
    //            splash.Invoke(new MethodInvoker(splash.Close));
    //        }
    //        catch (Exception ex)
    //        {
    //            log.Error(ex.Message, ex);
    //        }
    //        splashThread = null;
    //        splash = null;
    //    }
    //    /// <summary>
    //    /// Задание Статуса(лейбла)
    //    /// </summary>
    //    static public string Status
    //    {
    //        set
    //        {
    //            if (splash == null)
    //            {
    //                Thread.CurrentThread.Join(600);
    //            }
    //            try
    //            {
    //                splash.StatusInfo = value;
    //            }
    //            catch (NullReferenceException)
    //            { }
    //        }
    //        get
    //        {
    //            if (splash == null)
    //            {
    //               return ""; 
    //            }
    //            return splash.StatusInfo;
    //        }
    //    }

//Вызов осуществляется посредством
//Splasher<Splash>.Show();
//            Splasher<Splash>.Status = "Проверка, что продукт не запущен";
//SplashParent и Splash соотв. классы формочки. Год назад делал, и после года не нравиться строка с задержкой потока. Она была необходима, так как объект мог ещё создаться в другом потоке. Думаю необходимо сделать stumb, который проверяет создан ли объект. 