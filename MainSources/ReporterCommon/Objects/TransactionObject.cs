namespace ReporterCommon
{
    //Базовый класс для TransactionCell и TransactionShape
    public abstract class TransactionObject
    {
        //Старое и новое значение ячейки
        public string OldValue { get; private set; }
        public string NewValue { get; set; }
        //Старое и новое примечание к ячейке
        public string OldLink { get; private set; }
        public string NewLink { get; set; }

        //Запись и чтение значения 
        public abstract string Value { get; set; }
        //Запись и чтение комментария  
        public abstract string Link { get; set; }

        //Установка ссылки
        protected void SaveOldValue()
        {
            try { NewLink = OldLink = Link; } catch {}
            try { NewValue = OldValue = Value; } catch { }
        }

        //Откат транзакции
        public void Undo()
        {
            try { if (OldLink != NewLink) Link = OldLink; } catch { }
            try { if (OldValue != NewValue) Value = OldValue; } catch { }
        }

        //Обратный накат транзакции
        public void Redo()
        {
            try { if (OldLink != NewLink) Link = NewLink; } catch { }
            try { if (OldValue != NewValue) Value = NewValue; } catch { }
        }
    }
}