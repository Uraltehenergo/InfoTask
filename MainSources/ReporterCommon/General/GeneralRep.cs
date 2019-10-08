using System;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using Microsoft.Office.Interop.Excel;
using VersionSynch;
using Calculation;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace ReporterCommon
{
    //Объекты, общие для всех Project-ов
    public static class GeneralRep
    {
        //True, если надстройка уже инициализирована
        private static bool _isInitialized;
        
        //True, если активировано или время триала еще не вышло
        public static bool IsActivated { get; private set; }

        //Производится открытие новой книги
        public static bool IsOpening { get; set; }
        public static bool ProgrammOpening { get; set; }

        //Инициализация, version - версия Excel
        public static void Initialize(Application application, string version)
        {
            try
            {
                if (_isInitialized) return;
                IsActivated = new DbVersion().ACheck("Excel") >= 1;
                if (!IsActivated) return;

                General.Initialize();

                Application = application;
                CommonBook = new ReportBook("", null);
                CommonBook.OpenHistory(General.ReporterDir + @"ReporterHistory\" + version + "History.accdb", General.HistryTemplateFile);
                _isInitialized = true;
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка при инициализации построителя отчетов");
            }
        }

        //Закрывает все ресурсы
        public static void Close()
        {
            if (!IsActivated) return;
            CommonBook.UpdateHistory(false);
            CommonBook.CloseHistory();
            try { DaoDb.Compress(General.ReporterFile, 100000000);}
            catch {}
        }
        
        //True, пока идет обновление отчета из контрольного отчета
        public static bool IsUpdateFromReportServer { get; set; }

        //Словарь всех открытых книг
        private static readonly DicS<ReportBook> _books = new DicS<ReportBook>();
        public static DicS<ReportBook> Books { get { return _books; }}
        //Приложение
        public static Application Application { get; private set; }
        //Активная книга
        public static ReportBook ActiveBook{ get; private set;}
        
        //Установить текущую книгу активной, вызывается при смене книги или при открытии новой
        public static void ChangeActiveBook()
        {
            if (!IsActivated) return;
            try
            {
                if (!SysPage.IsInfoTask()) HideBook();
                else
                {
                    var wb = Application.ActiveWorkbook;
                    var s = new SysPage(wb).GetValue("Report");
                    using (CommonBook.StartLog("Смена активной книги", "", s))
                    {
                        try
                        {
                            if (IsOpening && !ProgrammOpening && !s.IsEmpty())
                            {
                                try //Чтобы не вылазила загадочная ошибка
                                {
                                    if (Books.ContainsKey(s) && Books[s].Workbook.FullName != wb.FullName)
                                        Different.MessageError("Файл отчета с кодом " + s + " уже открыт");
                                }
                                catch { }
                            }
                            if (s.IsEmpty()) ActiveBook = null;
                            else if (ActiveBook == null || ActiveBook.Code.ToLower() != s.ToLower())
                            {
                                HideBook();
                                if (Books.ContainsKey(s))
                                {
                                    ActiveBook = Books[s];
                                    foreach (var f in ActiveBook.Forms.Values)
                                        f.Show();
                                }
                                else
                                {
                                    ActiveBook = new ReportBook(s, wb);
                                    ActiveBook.OpenHistory(General.ReporterDir + @"ReporterHistory\" + s + "History.accdb", General.HistryTemplateFile);
                                    Books.Add(s, ActiveBook);
                                    ActiveBook.LoadSetup();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            CommonBook.AddError("Ошибка при открытии отчета", ex);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.MessageError("Ошибка при открытии отчета", "Системная ошибка построителя отчетов");
            }
            IsOpening = false;
            ProgrammOpening = false;
        }

        //Спрятать активную книгу со всеми формами
        private static void HideBook()
        {
            if (ActiveBook != null)
            {
                foreach (var f in ActiveBook.Forms.Values)
                    f.Hide();
                ActiveBook = null;    
            }
        }

        //Удаляет закрываемую книгу из списка
        public static void CloseBook()
        {
            if (!IsActivated) return;
            try
            {
                if (SysPage.IsInfoTask())
                {
                    var s = ActiveBook.SysPage.GetValue("Report");
                    using (CommonBook.StartLog("Закрытие книги", "", s))
                        if (Books.ContainsKey(s) && Books[s].Workbook.FullName == Application.ActiveWorkbook.FullName)
                        {
                            Books[s].CloseBook();
                            Books.Remove(s);
                        }
                }
            }
            catch { }
        }

        //Набор форм общих для всех книг - фиктивная книга
        public static ReportBook CommonBook { get; set; }

        //Запускает комманду и возвращает новую заданную форму, если такая есть
        public static Form RunReporterCommand(ReporterCommand c)
        {
            if (!IsActivated) return null;
            if (c.OneForAllBooks() || ActiveBook == null)
                return CommonBook.RunCommandReporter(c);
            return ActiveBook.RunCommandReporter(c);    
        }

        //Закрывает заданную форму, close - закрывать форму прямо здесь в процедуре
        public static void CloseForm(ReporterCommand form, bool close = false)
        {
            if (!IsActivated) return;
            if (form.OneForAllBooks())
                CommonBook.CloseForm(form, close);
            else if (ActiveBook != null) 
                ActiveBook.CloseForm(form, close);
        }

        //Добавляет ошибку в лог и выводит сообщение
        public static void ShowError(string errText, Exception ex = null)
        {
            try { CommonBook.AddError(errText, ex); } catch { }
            if (ex != null) ex.MessageError(errText, "Системная ошибка построителя отчетов");
        }

        //Выводит ошибку, если выделено более одного листа
        public static bool CheckOneSheet(bool isLinks)
        {
            var sheets = Application.ActiveWindow.SelectedSheets;
            string s = isLinks ? "Ссылки могут быть установлены" : "Отчет может быть сформирован";
            if (sheets.Count > 1)
            {
                Different.MessageError(s + ", только если выделено не более одного листа книги. Листы выделяются щелчком мыши при нажатой клавише \"Ctrl\" по вкладкам с именами листов в нижней части окна." , "Снимите выделение");
                return false;
            }
            if (sheets.Count == 1 && !(sheets[1] is Worksheet))
            {
                Different.MessageError(s + ", только если выделен обычный лист (не лист с графиком)");
                return false;
            }
            return true;
        }

        //Активация книги, делает книгу активным окном, работает и в Excel 2013
        public static void ActivateBook(this Workbook wb)
        {
            wb.Application.ScreenUpdating = true;
            wb.Activate();
            wb.Application.ScreenUpdating = true;
        }
    }
}