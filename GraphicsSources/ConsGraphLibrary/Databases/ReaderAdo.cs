using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Обертка для reader из ADO.NET
    public class ReaderAdo : IRecordRead
    {
        //Тип базы данных
        public DatabaseType DatabaseType { get; private set; }
        //True - конструктор вызван с указанием соединения, False - с указанием фала Access или списка свойств SQL Server
        private readonly bool _useDb;

        //Ссылка на OleDbDataReader или SqlDataReader
        public IDataReader Reader { get; private set; }
        //Ссылка на Command
        public DbCommand Command { get; private set; }
        //Ссылка на базу данных
        public DaoDb DaoDb { get; private set; }
        //Свойства соединения с SQL
        private readonly SqlProps _props;
        private readonly SqlConnection _connection;
        //Конец
        public bool EOF { get; private set; }
        //True, если курсор передвигался
        private bool _isMove;

        //db - база Access, stSql - запрос
        public ReaderAdo(DaoDb db, string stSql)
        {
            _useDb = true;
            DatabaseType = DatabaseType.Access;
            DaoDb = db.ConnectAdo();
            OpenReader(stSql, db.Connection);
        }

        //db - файл accdb, stSql - запрос
        public ReaderAdo(string db, string stSql)
        {
            _useDb = false;
            DatabaseType = DatabaseType.Access;
            if (stSql.IsEmpty() || db.IsEmpty())
                throw new NullReferenceException("Путь к файлу базы данных не может быть пустой строкой или null");
            DaoDb = new DaoDb(db).ConnectAdo();
            OpenReader(stSql, DaoDb.Connection);
        }

        //Ридер для SQL Server
        public ReaderAdo(SqlProps props, string stSql)
        {
            _useDb = false;
            _props = props;
            DatabaseType = DatabaseType.SqlServer;
            _connection = SqlDb.Connect(props);
            OpenReader(stSql, _connection);
        }

        private void OpenReader(string stSql, IDbConnection con)
        {
            if (stSql.IsEmpty() || con == null)
                throw new NullReferenceException("Строка запроса и соединение не могут быть пустыми строками или null");
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    var cmdo = new OleDbCommand(stSql, (OleDbConnection)con);
                    var ro = cmdo.ExecuteReader();
                    Command = cmdo;
                    Reader = ro;
                    EOF = !ro.HasRows;
                    break;
                case DatabaseType.SqlServer:
                    var cmds = new SqlCommand(stSql, (SqlConnection)con) {CommandTimeout = 120};
                    var rs = cmds.ExecuteReader();
                    Command = cmds;
                    Reader = rs;
                    EOF = !rs.HasRows;
                    break;
            }
            if (!EOF) Reader.Read();
            _isMove = false;
        }

        //True - если есть записи
        public bool HasRows()
        {
            if (Reader is OleDbDataReader)
                return ((OleDbDataReader)Reader).HasRows;
            if (Reader is SqlDataReader)
                return ((SqlDataReader)Reader).HasRows;
            return false;
        }

        //Количество записей, на входе строка запроса-комманды Count
        public int RecordCount(string stSql)
        {
            if (stSql.IsEmpty())
                throw new NullReferenceException("Строка запроса не может быть пустой строкой или null");
            switch (DatabaseType)
            {
                case DatabaseType.Access:
                    var cmdo = new OleDbCommand(stSql, DaoDb.Connection);
                    return (int)cmdo.ExecuteScalar();
                case DatabaseType.SqlServer:
                    var cmds = new SqlCommand(stSql, SqlDb.Connect(_props)) {CommandTimeout = 120 };
                    return (int)cmds.ExecuteScalar();
            }
            return 0;
        }

        //Закрытие рекордсета, полная очистка ресурсов
        public void Dispose()
        {
            try { Command.Dispose(); } catch { }
            try
            {
                Reader.Close();
                Reader.Dispose();
            } catch { }
            try
            {
                if (DatabaseType == DatabaseType.SqlServer)
                    _connection.Close();
            }
            catch { }
            if (!_useDb && DatabaseType == DatabaseType.Access) 
                DaoDb.Dispose();
        }
        
        //Переход на следующую запись
        public bool Read()
        {
            if (_isMove) EOF = !Reader.Read();
            else _isMove = true;
            return !EOF;
        }

        //Переводит значение поля field таблицы rec в строку, DbNull переводит в nullValue
        public string GetString(string field, string nullValue = null)
        {
            var v = Reader[field];
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToString(v);
        }

        //Переводит значение поля field таблицы rec в число, DbNull переводит в nullValue
        public double GetDouble(string field, double nullValue = 0)
        {
            var v = Reader[field];
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToDouble(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в null
        public double? GetDoubleNull(string field)
        {
            var v = Reader[field];
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToDouble(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в nullValue
        public int GetInt(string field, int nullValue = 0)
        {
            var v = Reader[field];
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToInt32(v);
        }
        //Переводит значение поля field таблицы rec в число, DbNull переводит в null
        public int? GetIntNull(string field)
        {
            var v = Reader[field];
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToInt32(v);
        }
        //Переводит значение поля field таблицы rec в дату, DbNull переводит в DateTime.MinValue
        public DateTime GetTime(string field)
        {
            var v = Reader[field];
            return DBNull.Value.Equals(v) ? Different.MinDate : Convert.ToDateTime(v);
        }
        //Переводит значение поля field таблицы rec в дату, DbNull переводит в null
        public DateTime? GetTimeNull(string field)
        {
            var v = Reader[field];
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToDateTime(v);
        }
        //Переводит значение поля field таблицы rec в bool, DbNull переводит в nullValue
        public bool GetBool(string field, bool nullValue = false)
        {
            var v = Reader[field];
            return DBNull.Value.Equals(v) ? nullValue : Convert.ToBoolean(v);
        }
        //Переводит значение поля field таблицы rec в bool, DbNull переводит в null
        public bool? GetBoolNull(string field)
        {
            var v = Reader[field];
            if (DBNull.Value.Equals(v)) return null;
            return Convert.ToBoolean(v);
        }

        //Копирует строковое значение поля field таблицы rec в ячейку gridField, строки cells, датагрида WinForms
        public void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            cells[gridField ?? field].Value = Reader[field];
        }
    }
}
