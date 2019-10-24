using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace GrafeoLibrary
{
    internal class Printer
    {
        [DllImport("gdi32.dll")]
        public static extern long BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc,
                                         int nXSrc, int nYSrc, int dwRop);
        private Bitmap _memoryImage;

        private readonly PrintDocument _printDoc = new PrintDocument();
        private void PrintDocInit()
        {
            _printDoc.DefaultPageSettings.Landscape = true;
        }

        //private void Print()
        //{
        //    try
        //    {
        //        //var pd1 = new PrintDialog {Document = chart1.Printing.PrintDocument};
        //        //var ppd1 = new PrintPreviewDialog {Document = chart1.Printing.PrintDocument};
        //        //var psd1 = new PageSetupDialog {Document = chart1.Printing.PrintDocument};
        //        //chart1.Printing.PrintDocument.DefaultPageSettings.Landscape = true;
        //        //chart1.Printing.PrintDocument.DefaultPageSettings.Margins = new Margins(100,100,100,100);
        //        //if (ppd1.ShowDialog() == DialogResult.OK) chart1.Printing.Print(false);
        //        var s = new Size(Size.Width, Size.Height);
        //        //DialogResult d = MessageBox.Show("Включить метки графиков?", "Печать", MessageBoxButtons.YesNo,
        //        //                                 MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
        //        //Visible = false;
        //        Size = new Size(2048, 1736);

        //        Refresh();
        //        Label[,] indexLabels = new Label[AnalogGraphics.Count, 4];
        //        Label capL = new Label { Text = Caption, AutoSize = true, BackColor = Color.Transparent };
        //        Label dateL = new Label { Text = DateTime.Now.ToString(), AutoSize = true, BackColor = Color.Transparent };
        //        if (chbPrintLabels.Checked)
        //        {
        //            //for (int indexP = 0; indexP < AnalogGraphics.Count - 1; indexP++)
        //            int indexP = -1;
        //            foreach(var aGr in AnalogGraphics)
        //            {
        //                indexP++;
        //                if (aGr.IsVisible)
        //                    for (int indexN = 0; indexN < 4; indexN++)
        //                    {
        //                        indexLabels[indexP, indexN] = new Label
        //                        {
        //                            Text = aGr.Num.ToString(),
        //                            BackColor = Color.Transparent,
        //                            AutoSize = true,
        //                            Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((204)))
        //                        };
        //                        chartMain.Controls.Add(indexLabels[indexP, indexN]);
        //                        double labelXPosition;
        //                        if (indexN < 3)
        //                            labelXPosition = ((aGr.Area.Position.X * chartMain.Width / 100) +
        //                                              (indexN + .2) * (aGr.Area.Position.Width * chartMain.Width / 330)
        //                                              + (-3 + indexP) * 15);
        //                        else
        //                            labelXPosition = ((aGr.Area.Position.X * chartMain.Width / 100) +
        //                                              (indexN) * (aGr.Area.Position.Width * chartMain.Width / 320)
        //                                              + (-2 - AnalogGraphics.Count + indexP) * 15);
        //                        try
        //                        {
        //                            double labelXValue = aGr.Area.AxisX.PixelPositionToValue(labelXPosition);
        //                            MomentValue xValue = aGr.Dots.FindLast(x => (x.Time.ToOADate() <= labelXValue));
        //                            var m = xValue == null ? 0 : xValue.ToMomentReal().Mean;
        //                            double labelYPosition = aGr.Area.AxisY.ValueToPixelPosition(m);
        //                            indexLabels[indexP, indexN].Top = (int)(Math.Round(labelYPosition)) - 5;
        //                            indexLabels[indexP, indexN].Left = (int)Math.Round(labelXPosition) - 8;
        //                            if (labelYPosition + 35 > aGr.Area.Position.Bottom * chartMain.Height / 100 ||
        //                                labelYPosition < aGr.Area.Position.Y * chartMain.Height / 100)
        //                                indexLabels[indexP, indexN].Text = "";
        //                            //indexLabels[indexP, indexN].Text = (DateTime.FromOADate(
        //                            //    curP.Area.AxisX.PixelPositionToValue(0))).ToString();        
        //                        }
        //                        catch { }
        //                    }
        //            }
        //        }

        //        chartMain.Controls.Add(dateL);
        //        dateL.Location = new Point((int)((BackGround.Area.Position.X * chartMain.Width / 100) +
        //                                          (BackGround.Area.Position.Width * chartMain.Width / 100) - 150), 0);
        //        chartMain.Controls.Add(capL);
        //        capL.Location = new Point((int)((BackGround.Area.Position.X * chartMain.Width / 100) + 20), 0);

        //        CaptureP();
        //        if (chbPrintLabels.Checked)
        //        {
        //            foreach (var l in indexLabels)
        //            {
        //                if (l != null) l.Dispose();
        //            }
        //        }
        //        dateL.Dispose();
        //        capL.Dispose();
        //        Size = s;
        //        //Visible = true;
        //        //var ppd = new PrintPreviewDialog { Document = new PrintDocument() };
        //        //var ppd = new PageSetupDialog { Document = chart1.Printing.PrintDocument };
        //        //var ppd = new PrintDocument();
        //        _printDoc.PrintPage += PrintDocument_PrintPage;
        //        //ppd.Document.PrintPage += PrintDocument1PrintPage;
        //        //ppd.Document.DefaultPageSettings.Landscape = true;

        //        //_ppd.DefaultPageSettings.Landscape = true;
        //        //_ppd.DefaultPageSettings.PrinterSettings.PrinterName = cbPrinters.Text;

        //        //ppd.ShowDialog();
        //        TopMost = false;
        //        //ppd.Show();
        //        //ppd.TopMost = true;
        //        //if (ppd.DialogResult == DialogResult.OK)
        //        //{
        //        //    ppd.Document.Print();
        //        //}
        //        //ppd.Closing += PpdClosing;
        //        //if (ppd.ShowDialog() == DialogResult.OK) chart1.Printing.Print(false);

        //        //var prdlg = new PrintDialog { Document = ppd };
        //        //prdlg.ShowDialog();
        //        //var pgstdlg = new PageSetupDialog {Document = ppd};
        //        //pgstdlg.ShowDialog();

        //        var prewdlg = new PrintPreviewDialog {Document = _printDoc};
        //        prewdlg.ShowDialog();
        //        //ppd.Print();
        //    }
        //    catch (Exception ex)
        //    {
        //        //~ Error = new ErrorCommand("Ошибка печати (Button3Click)", ex);
        //        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
        //    }
        //}

        //private void CaptureP()
        //{
        //    //Graphics mygraphics = CreateGraphics();
        //    //_memoryImage = new Bitmap(splitContainer1.Width, splitContainer1.Height, mygraphics);
        //    //Graphics memoryGraphics = Graphics.FromImage(_memoryImage);
        //    //IntPtr dc1 = mygraphics.GetHdc();
        //    //IntPtr dc2 = memoryGraphics.GetHdc();
        //    //BitBlt(dc2, 0, 0, ClientRectangle.Width, ClientRectangle.Height, dc1, 7,
        //    //       7, 13369376);
        //    //mygraphics.ReleaseHdc(dc1);
        //    //memoryGraphics.ReleaseHdc(dc2);
        //    Graphics mGraphics = splitContainerV.CreateGraphics();
        //    _memoryImage = new Bitmap(splitContainerV.Width, splitContainerV.Height, mGraphics);
        //    splitContainerV.DrawToBitmap(_memoryImage, new Rectangle(0, 0, splitContainerV.Width, splitContainerV.Height));
        //}

        //private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //    e.Graphics.DrawImage(_memoryImage, 40, 40, e.PageSettings.PrintableArea.Height - 70,
        //                         e.PageSettings.PrintableArea.Width - 70);
        //}

        //private void InitPrinters()
        //{
        //    PrinterSettings.StringCollection sc = PrinterSettings.InstalledPrinters;
        //    foreach (var pr in sc)
        //    {
        //        var it = cbPrinters.Items.Add(pr.ToString());
        //        if (new PrinterSettings { PrinterName = pr.ToString() }.IsDefaultPrinter)
        //            cbPrinters.SelectedIndex = it;
        //    }
        //}

        public static List<string> InstalledPrinters
        {
            get
            {
                PrinterSettings.StringCollection installedPrinters = PrinterSettings.InstalledPrinters;
                var printers = new List<string>();
                foreach (var printer in installedPrinters)
                {
                    printers.Add(printer.ToString());
                    //if (new PrinterSettings {PrinterName = pr.ToString()}.IsDefaultPrinter)
                    //    cbPrinters.SelectedIndex = it;
                }
                return printers;
            }
        }

        public static string DefaultPrinet
        {
            get
            {
                PrinterSettings.StringCollection installedPrinters = PrinterSettings.InstalledPrinters;
                foreach (var printer in installedPrinters)
                {
                    if (new PrinterSettings { PrinterName = printer.ToString() }.IsDefaultPrinter)
                        return printer.ToString();
                }
                return null;
            }
        }

        public static void FillPrintersComboBox(ComboBox comboBox)
        {
            comboBox.Items.Clear();

            PrinterSettings.StringCollection installedPrinters = PrinterSettings.InstalledPrinters;
            foreach (var priner in installedPrinters)
            {
                var it = comboBox.Items.Add(priner.ToString());
                if (new PrinterSettings { PrinterName = priner.ToString() }.IsDefaultPrinter)
                    comboBox.SelectedIndex = it;
            }
        }

        //private void ShowPrinterSettings()
        //{
        //    var prdlg = new PrintDialog { Document = _printDoc };
        //    prdlg.ShowDialog();

        //    cbPrinters.Text = _printDoc.DefaultPageSettings.PrinterSettings.PrinterName;
        //}

        public void ShowPrintPageSetup()
        {
            var pgstdlg = new PageSetupDialog { Document = _printDoc };
            pgstdlg.ShowDialog();
        }
    }
}
