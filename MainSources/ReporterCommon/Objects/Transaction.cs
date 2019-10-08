using System.Collections.Generic;
using BaseLibrary;

namespace ReporterCommon
{
    //Одна транзакция
    public class Transaction
    {
        //Словарь ячеек c ссылками
        private readonly Dictionary<string, TransactionCell> _cellsLink = new Dictionary<string, TransactionCell>();
        //Словарь ячеек cо значениями
        private readonly Dictionary<string, TransactionCell> _cellsValue = new Dictionary<string, TransactionCell>();
        //Ячейки, входящие в транзакцию
        private readonly List<TransactionCell> _cells = new List<TransactionCell>();
        public List<TransactionCell> Cells { get { return _cells; } }

        //Фигура
        public TransactionShape TrShape { get; private set; }

        //Множество не найденных параметров
        private SetS _notFound;
        public SetS NotFound { get { return _notFound ?? (_notFound = new SetS()); } }
        //Множество повторяющихся ячеек
        private SetS _repeat;
        public SetS Repeat { get { return _repeat ?? (_repeat = new SetS()); } }

        //Добавление ячейки в транзакцию
        public void AddCell(TransactionCell cell)
        {
            string code = cell.Cell.MergeArea.Address.Replace("$", "");
            bool add = false;
            if (cell.NewLink != cell.OldLink)
            {
                if (!_cellsLink.ContainsKey(code))
                {
                    _cellsLink.Add(code, cell);
                    Cells.Add(cell);
                    add = true;
                }
                else Repeat.Add(code);
            }
            if (cell.NewValue != cell.OldValue)
            {
                if (!_cellsValue.ContainsKey(code))
                {
                    _cellsValue.Add(code, cell);
                    if (!add) Cells.Add(cell);
                }
                else Repeat.Add(code);
            }
        }

        //Добавление фигуры в транзакцию
        public void AddShape(TransactionShape shape)
        {
            if (shape.NewLink != shape.OldLink || shape.NewValue != shape.OldValue)
                TrShape = shape;
        }

        //Сообщение об ошибке
        public string ErrMess()
        {
            if ((NotFound == null || NotFound.Count == 0) && (Repeat == null || Repeat.Count == 0)) return "";
            string s = "";
            if (NotFound != null && NotFound.Count > 0)
            {
                s += "Не найдены параметры или недопустимые типы ссылок для параметров: ";
                int i = 0;
                foreach (var c in NotFound.Values)
                {
                    if (i > 0) s += ", ";
                    s += c;
                    if (i++ >= 20)
                    {
                        s += " и другие";
                        break;
                    }
                }
                s += '\n';
            }
            if (Repeat != null && Repeat.Count > 0)
            {
                s += "Попытка установки более одной ссылки в некоторые ячейки: ";
                int i = 0;
                foreach (var c in Repeat.Values)
                {
                    if (i > 0) s += ", ";
                    s += c;
                    if (i++ >= 20)
                    {
                        s += " и другие";
                        break;
                    }
                }
            }
            return s;
        }
    }
}