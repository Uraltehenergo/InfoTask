using BaseLibrary;

namespace Tablik
{
    //Предыдущие значения по параметру
    public class Prev
    {
        public Prev(string code)
        {
            Code = code;
        }

        //Код параметра или подпараметра
        public string Code { get; private set; }
        //Id в таблице PrevParams
        public int Id { get; set; }
        //Используется ПредАбс
        public bool PrevAbs { get; set; }
        //Используемые функции Пред за последний интервал
        public bool LastBase { get; set; }
        public bool LastHour { get; set; }
        public bool LastDay { get; set; }
        //Используемые функции Пред за много интервалов
        public bool ManyBase { get; set; }
        public bool ManyHour { get; set; }
        public bool ManyDay { get; set; }
        public bool ManyMoments { get; set; }

        //Копирует Prev в Prev par с кодом code, если par = null, то создает новый
        public Prev Clone(string code, Prev par = null)
        {
            var p = par ?? new Prev(code);
            p.PrevAbs |= PrevAbs;
            p.LastBase |= LastBase;
            p.LastHour |= LastHour;
            p.LastDay |= LastDay;
            p.ManyBase |= ManyBase;
            p.ManyHour |= ManyHour;
            p.ManyDay |= ManyDay;
            p.ManyMoments |= ManyMoments;
            return p;
        }

        //Записывает в рекордсет PrevParams
        public void ToRecordset(RecDao rec, bool addnew)
        {
            if (Id != 0)
            {
                if (addnew) rec.AddNew();
                rec.Put("ArchiveParamId", Id);
                rec.Put("FullCode", Code);
                rec.Put("PrevAbs", PrevAbs);
                rec.Put("LastBase", LastBase);
                rec.Put("LastHour", LastHour);
                rec.Put("LastDay", LastDay);
                rec.Put("ManyBase", ManyBase);
                rec.Put("ManyHour", ManyHour);
                rec.Put("ManyDay", ManyDay);
                rec.Put("ManyMoments", ManyMoments);
            }
        }
    }
}
