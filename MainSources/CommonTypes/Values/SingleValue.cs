using System.Collections.Generic;
using System.Windows.Forms;
using BaseLibrary;

namespace CommonTypes
{
    //Тип для SingleValue
    public enum SingleType
    {
        Void,
        Moment,
        List,
        Segments
    }

    //--------------------------------------------------------------------------------------
    //Мгновенное значение или список мгновенных значений
    public class SingleValue
    {
        public SingleValue(SingleType type = SingleType.Void)
        {
            Type = type;
            if (type == SingleType.List) Moments = new List<Moment>();
            if (type == SingleType.Segments) Segments = new List<Segment>();
        }

        public SingleValue(Moment m)
        {
            Type = SingleType.Moment;
            Moment = m;
        }

        public SingleValue(List<Moment> moments)
        {
            Type = SingleType.List;
            Moments = moments;
        }

        public SingleValue(List<Segment> segments)
        {
            Type = SingleType.Segments;
            Segments = segments;
        }

        //Тип
        public SingleType Type { get; private set; }
        //Мгновенное значение
        private Moment _moment;
        public Moment Moment
        {
            get { return _moment; } 
            set { _moment = value; Type = SingleType.Moment;}
        }
        //Список мгновенных значений по времени
        private List<Moment> _moments; 
        public List<Moment> Moments
        {
            get { return _moments; }
            set { _moments = value; Type = SingleType.List; }
        }
        //Сегменты
        private List<Segment> _segments;
        public List<Segment> Segments
        {
            get { return _segments; }
            set { _segments = value; Type = SingleType.Segments; }
        }

        //Последнее или единственное мгновенное значение
        public Moment LastMoment
        {
            get
            {
                switch(Type)
                {
                    case SingleType.Moment: 
                        return Moment ?? new Moment();
                    case SingleType.List:
                        return Moments.Count == 0 ? new Moment() : Moments[Moments.Count - 1];
                }
                return new Moment();
            }
        }

        //True, если есть хотя бы одно мгновенное значение
        public bool HasLastMoment
        {
            get { return Moment != null || (Moments != null && Moments.Count > 0);}
        }

        //Ошибка
        public ErrorCalc Error { get; set; }
        //Итоговая ошибка для сохранения в Result
        public string LastError
        {
            get
            {
                if (Error != null) 
                    return Error.Text;
                if (Segments != null && Segments.Count > 0 && Segments[Segments.Count - 1].Error != null) 
                    return Segments[Segments.Count - 1].Error.Text;
                if (LastMoment.Error != null) 
                    return LastMoment.Error.Text;
                return null;
            }
        }

        //Тип данных
        public DataType DataType
        {
            get
            {
                switch (Type)
                {
                    case SingleType.Void:
                        return DataType.Value;
                    case SingleType.Moment:
                        return Moment == null ? DataType.Value : Moment.DataType;
                    case SingleType.List:
                        return (Moments.Count == 0 || Moments[0] == null) ? DataType.Value : Moments[0].DataType;
                    case SingleType.Segments:
                        return DataType.Segments;
                }
                return DataType.Error;
            }   
        }

        //Изменяет тип данных всех мгновенных значений на dt
        public SingleValue ChangeDataType(DataType dt)
        {
            if (Type == SingleType.Moment && Moment != null && !dt.LessOrEquals(Moment.DataType))
                return new SingleValue(Moment.Clone(dt));
            if (Type == SingleType.List && Moments != null && !dt.LessOrEquals(DataType))
            {
                var sv = new SingleValue(SingleType.List);
                foreach (var m in Moments)
                    sv.AddMoment(m.Clone(dt));
                return sv;
            }
            return this;
        }

        //Добавляет в Momnets мгновенное значение mv, сохраняя упорядоченность sv по времени
        public void AddMoment(Moment mv)
        {
            if (Moments == null) Moments = new List<Moment>();
            if (Moments.Count == 0 || mv.Time > Moments[Moments.Count - 1].Time)
                Moments.Add(mv);
            else if (mv.Time == Moments[Moments.Count - 1].Time)
                Moments.Add(mv.Clone(mv.Time.AddMilliseconds(0.6)));
            else 
            {
                int i = Moments.Count - 1;
                while (i >= 0 && Moments[i].Time >= mv.Time) i--;
                if (mv.Time != Moments[i + 1].Time) Moments.Insert(i + 1, mv);
            }
        }

        //Возвращает значение как список мгновенных
        public List<Moment> ToMomList()
        {
            if (Moments != null) return Moments;
            if (Moment != null) return new List<Moment> {Moment};
            return new List<Moment>();
        }
    }
}