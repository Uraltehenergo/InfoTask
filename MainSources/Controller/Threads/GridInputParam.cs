using System;
using System.Collections.ObjectModel;
using BaseLibrary;
using Calculation;
using CommonTypes;

namespace Controller
{
    //Один параметр для редактирования ручного ввода или абсолютных значений
    public class GridInputParam : HandInputValue
    {
        //Загрузка из проекта, isAbsolute=false - как параметр ручного ввода, иначе как параметр для абсолютного накопления
        public GridInputParam(string project, IRecordRead rec, bool isAbsolute)
        {
            Project = project;
            Code = rec.GetString("Code");
            Name = rec.GetString("Name");
            Units = rec.GetString("Units");
            Comment = rec.GetString("Comment");
            Task = rec.GetString("Task");
            if (!isAbsolute)
            {
                DataType = rec.GetString("CalcParamType").ToCalcParamType().HandDataType().ToRussian();
                DefaultValue = rec.GetString("DefaultValue");
            }
            else DataType = rec.GetString("DataType");
        }

        //Загрузка значений из источника ручного ввода
        public void ValuesFromClone(IProvider source)
        {
            bool isSql = source is HandInputSqlSource;
            if (isSql) _sqlProps = ((HandInputSqlSource) source).SqlProps;
            else _cloneFile = ((CloneSource)source).CloneFile;
            HandInputValues.Clear();
            var dt = DataType.ToDataType();
            string stSql = dt.LessOrEquals(BaseLibrary.DataType.Real)
                               ? "SELECT MomentsValues.* FROM (Objects INNER JOIN Signals ON Objects.ObjectId = Signals.ObjectId) " +
                                 "INNER JOIN MomentsValues ON Signals.SignalId = MomentsValues.SignalId WHERE Objects.CodeObject='" + Code + "'"
                               : "SELECT MomentsStrValues.* FROM (Objects INNER JOIN Signals ON Objects.ObjectId = Signals.ObjectId) " +
                                 "INNER JOIN MomentsStrValues ON Signals.SignalId = MomentsStrValues.SignalId WHERE Objects.CodeObject='" + Code + "'";
            using (var rec = isSql ? (IRecordSet)new DataSetSql(_sqlProps, stSql) : new RecDao(_cloneFile, stSql))
                while (rec.Read())
                    HandInputValues.Add(new HandInputValue(rec, dt));
        }

        //Сохранение значений ввода
        public void SaveHandValues()
        {
            if (_cloneFile != null)
                ValuesToClone();
            else ValuesToSql();
        }

        //Сохранение в файл клона
        public void ValuesToClone()
        {
            using (var db = new DaoDb(_cloneFile))
            {
                int id;
                using (var rec = new RecDao(db, "SELECT * FROM Objects WHERE CodeObject='" + Code + "'"))
                {
                    if (!rec.HasRows()) rec.AddNew();
                    rec.Put("CodeObject", Code);
                    rec.Put("NameObject", Name);
                    rec.Put("Comment", Comment);
                    id = rec.GetInt("ObjectId");
                }
                using (var rec = new RecDao(db, "SELECT * FROM Signals WHERE ObjectId=" + id))
                {
                    if (!rec.HasRows()) rec.AddNew();
                    rec.Put("ObjectId", id);
                    rec.Put("CodeSignal", "Руч");
                    rec.Put("NameSignal", "Ручной ввод");
                    rec.Put("FullCode", Code + ".Руч");
                    rec.Put("Default", true);
                    rec.Put("DataType", DataType);
                    rec.Put("Units", Units);
                    rec.Put("Active", true);
                    id = rec.GetInt("SignalId");
                    rec.Update();
                }
                var dt = DataType.ToDataType();
                DateTime mind = Different.MaxDate, maxd = Different.MinDate;
                RecDao recv;
                if (dt.LessOrEquals(BaseLibrary.DataType.Real))
                {
                    db.Execute("DELETE * FROM MomentsValues WHERE SignalId=" + id);
                    recv = new RecDao(db, "SELECT * FROM MomentsValues WHERE SignalId=" + id);
                }
                else
                {
                    db.Execute("DELETE * FROM MomentsStrValues WHERE SignalId=" + id);
                    recv = new RecDao(db, "SELECT * FROM MomentsStrValues WHERE SignalId=" + id);
                }
                using (recv)
                    foreach (var v in HandInputValues)
                    {
                        v.ToRecordset(recv, id, dt);
                        if (v.TimeCorrect && v.Time.ToDateTime() < mind) mind = v.Time.ToDateTime();
                        if (v.TimeCorrect && v.Time.ToDateTime() > maxd) maxd = v.Time.ToDateTime();
                    }
                
                if (mind != Different.MaxDate)
                    using (var sys = new SysTabl(db, false))
                    {
                        var d = sys.Value("BeginInterval").ToDateTime();
                        if (d == Different.MinDate || d > mind) sys.PutValue("BeginInterval", mind.ToString());
                        d = sys.Value("EndInterval").ToDateTime();
                        if (d < maxd) sys.PutValue("EndInterval", maxd.ToString());
                    }
            }
        }

        //Сохранение в SQL
        public void ValuesToSql()
        {
            int id;
            using (var rec = new DataSetSql(_sqlProps, "SELECT * FROM Objects WHERE CodeObject='" + Code + "'"))
            {
                if (!rec.HasRows()) rec.AddNew();
                rec.Put("CodeObject", Code);
                rec.Put("NameObject", Name);
                rec.Put("Comment", Comment);
                rec.Update();
                rec.Reload();
                id = rec.GetInt("ObjectId");
            }
            using (var rec = new DataSetSql(_sqlProps, "SELECT * FROM Signals WHERE ObjectId=" + id))
            {
                if (!rec.HasRows()) rec.AddNew();
                rec.Put("ObjectId", id);
                rec.Put("CodeSignal", "Руч");
                rec.Put("NameSignal", "Ручной ввод");
                rec.Put("FullCode", Code + ".Руч");
                rec.Put("Default", true);
                rec.Put("DataType", DataType);
                rec.Put("Units", Units);
                rec.Put("Active", true);
                rec.Update();
                rec.Reload();
                id = rec.GetInt("SignalId");
            }
            var dt = DataType.ToDataType();
            DateTime mind = Different.MaxDate, maxd = Different.MinDate;
            DataSetSql recv;
            if (dt.LessOrEquals(BaseLibrary.DataType.Real))
            {
                SqlDb.Execute(_sqlProps, "DELETE MomentsValues FROM MomentsValues WHERE SignalId=" + id);
                recv = new DataSetSql(_sqlProps, "SELECT * FROM MomentsValues WHERE SignalId=" + id);
            }
            else
            {
                SqlDb.Execute(_sqlProps, "DELETE MomentsStrValues FROM MomentsStrValues WHERE SignalId=" + id);
                recv = new DataSetSql(_sqlProps, "SELECT * FROM MomentsStrValues WHERE SignalId=" + id);
            }
            using (recv)
                foreach (var v in HandInputValues)
                {
                    v.ToRecordset(recv, id, dt);
                    if (v.TimeCorrect && v.Time.ToDateTime() < mind) mind = v.Time.ToDateTime();
                    if (v.TimeCorrect && v.Time.ToDateTime() > maxd) maxd = v.Time.ToDateTime();
                }
            
            if (mind != Different.MaxDate)
                using (var rec = new DataSetSql(_sqlProps, "SELECT * FROM SysTabl"))
                {
                    rec.FindFirst("ParamName", "BeginInterval");
                    var d = rec.GetString("ParamValue").ToDateTime();
                    if (d == Different.MinDate || d > mind)
                        rec.Put("ParamValue", mind.ToString());
                    rec.FindFirst("ParamName", "EndInterval");
                    d = rec.GetString("ParamValue").ToDateTime();
                    if (d < maxd) rec.Put("ParamValue", mind.ToString());
                }
        }

        //Файл клона, где лежат значения по данному параметру
        private string _cloneFile;
        //Ссылка на базу SQL, где лежат значения по данному параметру
        private SqlProps _sqlProps;

        //Код проекта
        private string _project;
        public string Project
        {
            get { return _project; }
            set
            {
                if (value == _project) return;
                _project = value;
                OnPropertyChanged("Project");
            }
        }
        //Полный код параметра
        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                if (value == _code) return;
                _code = value;
                OnPropertyChanged("Code");
            }
        }
        //Имя
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        //Единицы измерения
        private string _units;
        public string Units
        {
            get { return _units; }
            set
            {
                if (value == _units) return;
                _units = value;
                OnPropertyChanged("Units");
            }
        }
        //Комментарий
        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value == _comment) return;
                _comment = value;
                OnPropertyChanged("Comment");
            }
        }
        //Задача
        private string _task;
        public string Task
        {
            get { return _task; }
            set
            {
                if (value == _task) return;
                _task = value;
                OnPropertyChanged("Task");
            }
        }
        //Значение по умолчанию
        private string _defaultValue;
        public string DefaultValue
        {
            get { return _defaultValue; }
            set
            {
                if (value == _defaultValue) return;
                _defaultValue = value;
                OnPropertyChanged("DefaultValue");
            }
        }
        //Старое значение
        private string _oldValue;
        public string OldValue
        {
            get { return _oldValue; }
            set
            {
                if (value == _oldValue) return;
                _oldValue = value;
                OnPropertyChanged("OldValue");
            }
        }
        //Время начала отсчета абсолютного значения
        private string _oldTime;
        public string OldTime
        {
            get { return _oldTime; }
            set
            {
                if (value == _oldTime) return;
                _oldTime = value;
                OnPropertyChanged("OldTime");
            }
        }

        //Список значений
        private readonly ObservableCollection<HandInputValue> _handInputValues = new ObservableCollection<HandInputValue>();
        public ObservableCollection<HandInputValue> HandInputValues { get { return _handInputValues; } }
    }
}