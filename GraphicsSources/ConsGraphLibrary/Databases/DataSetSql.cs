﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Датасет SQL Server
    public class DataSetSql : IRecordSet
    {
        //stSql - строка запроса, props - свойства соединения
        public DataSetSql(SqlProps props, string stSql)
        {
            _con = SqlDb.Connect(props);
            _adapter = new SqlDataAdapter(stSql, _con) 
                { SelectCommand = {CommandTimeout = 100000} };
            new SqlCommandBuilder(_adapter);
            Reload();
        }

        //Соединение 
        private readonly SqlConnection _con;
        public SqlConnection Connection { get { return _con; } }
        //Адаптер
        private readonly SqlDataAdapter _adapter;
        //Датасет
        private DataSet _dataSet;
        public DataSet DataSet { get { return _dataSet; } }
        //Таблица
        private DataTable _table;
        //Коллекция рядов таблицы
        private DataRowCollection _rows;
        //Текущий ряд и его номер
        private DataRow _row;
        private int _rowNum;
        //True, если курсор передвигался
        private bool _isMove;
        //False, если был изменен, но не обновлен
        private bool _isChanged;

        //Повторно загружает данные из таблицы
        public void Reload()
        {
            _dataSet = new DataSet();
            _adapter.Fill(_dataSet);
            _table = _dataSet.Tables[0];
            _rows = _table.Rows;
            _isMove = false;
            _rowNum = 0;
            _row = HasRows() ? _rows[0] : null;
        }

        public bool Read()
        {
            if (_isMove)
            {
                _rowNum++;
                _row = !EOF ? _rows[_rowNum] : null;
            }
            _isMove = true;
            return !EOF;
        }

        public bool EOF { get { return _rowNum >= _rows.Count; }}

        public bool BOF { get { return _rowNum < 0; } }
        
        public void Dispose()
        {
            try { if (_isChanged) Update(); }
            catch { }
            try { _rows.Clear(); } catch {}
            try { _dataSet.Dispose();} catch {}
            try { _adapter.Dispose(); } catch {}
            try { _con.Close(); } catch { }
        }
        
        public bool HasRows()
        {
            return _rows.Count > 0;
        }

        public string GetString(string field, string nullValue = null)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (string)_row[field];
        }

        public double GetDouble(string field, double nullValue = 0)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (double)_row[field];
        }

        public double? GetDoubleNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return  (double)_row[field];
        }

        public int GetInt(string field, int nullValue = 0)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (int)_row[field];
        }

        public int? GetIntNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return (int)_row[field];
        }

        public DateTime GetTime(string field)
        {
            return DBNull.Value.Equals(_row[field]) ? Different.MinDate : (DateTime)_row[field];
        }

        public DateTime? GetTimeNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return (DateTime)_row[field];
        }

        public bool GetBool(string field, bool nullValue = false)
        {
            return DBNull.Value.Equals(_row[field]) ? nullValue : (bool)_row[field];
        }

        public bool? GetBoolNull(string field)
        {
            if (DBNull.Value.Equals(_row[field])) return null;
            return (bool)_row[field];
        }

        public void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            cells[gridField ?? field].Value = _row[field];
        }

        public void Update()
        {
            _adapter.Update(_dataSet);
            _isChanged = false;
        }

        public void AddNew()
        {
            _isChanged = true;
            _row = _table.NewRow();
            _rows.Add(_row);
            _rowNum = _rows.Count - 1;
        }
        
        public void Put(string field, string val, bool cut = false)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else
            {
                if (val.Length <= _table.Columns[field].MaxLength || !cut) _row[field] = val;
                else _row[field] = val.Substring(0, _table.Columns[field].MaxLength);
            } 
        }

        public void Put(string field, double val)
        {
            _isChanged = true;
            _row[field] = val;
        }

        public void Put(string field, double? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else _row[field] = (double)val;
        }

        public void Put(string field, int val)
        {
            _isChanged = true;
            _row[field] = val;
        }

        public void Put(string field, int? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else _row[field] = (int)val;
        }

        public void Put(string field, DateTime val)
        {
            _isChanged = true;
            if (val < Different.MinDate) _row[field] = Different.MinDate;
            else if (val > Different.MaxDate) _row[field] = Different.MaxDate;
            else _row[field] = val;
        }

        public void Put(string field, DateTime? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else if ((DateTime)val < Different.MinDate) _row[field] = _row[field] = Different.MinDate;
            else if ((DateTime)val > Different.MaxDate) _row[field] = _row[field] = Different.MaxDate;
            else Put(field, (DateTime)val);
        }

        public void Put(string field, bool val)
        {
            _isChanged = true;
            _row[field] = val;
        }

        public void Put(string field, bool? val)
        {
            _isChanged = true;
            if (val == null) _row[field] = DBNull.Value;
            else _row[field] = (bool)val;
        }

        public void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null)
        {
            _isChanged = true;
            _row[field] = cells[gridField ?? field].Value;
        }

        public int RecordCount { get { return _rows.Count; } }

        public bool MoveFirst()
        {
            _rowNum = 0;
            _row = HasRows() ? _rows[0] : null;
            _isMove = true;
            return _row != null;
        }

        public bool MoveLast()
        {
            _rowNum = _rows.Count - 1;
            _row = HasRows() ? _rows[_rows.Count - 1] : null;
            _isMove = true;
            return _row != null;
        }

        public bool MoveNext()
        {
            if (EOF) throw new Exception("Текущая запись отсутствует");
            _rowNum++;
            _row = EOF ? null : _rows[_rowNum];
            _isMove = true;
            return _row != null;
        }

        public bool MovePrevious()
        {
            if (BOF) throw new Exception("Текущая запись отсутствует");
            _rowNum--;
            _row = BOF ? null : _rows[_rowNum];
            _isMove = true;
            return _row != null;
        }

        public bool Find(int key)
        {
            try
            {
                _isMove = true;
                _row = _rows.Find(key);
                _rowNum = _rows.IndexOf(_row);
                return true;
            }
            catch { return false;}
        }

        public bool Find(string key)
        {
            try
            {
                _isMove = true;
                _row = _rows.Find(key);
                _rowNum = _rows.IndexOf(_row);
                return true;
            }
            catch { return false; }
        }

        private bool StrEqual(int num, string val, string field)
        {
            return (val == null && DBNull.Value.Equals(_rows[num])) || (val != null && (string) _rows[num][field] == val);
        }

        private bool IntEqual(int num, int val, string field)
        {
            return !DBNull.Value.Equals(_rows[num][field]) && (int)_rows[num][field] == val;
        }

        public bool FindFirst(string field, string val)
        {
            _rowNum = -1;
            return FindNext(field, val);
        }

        public bool FindFirst(string field, int val)
        {
            _rowNum = -1;
            return FindNext(field, val);
        }

        public bool FindLast(string field, string val)
        {
            _rowNum = _rows.Count;
            return FindPrevious(field, val);
        }

        public bool FindLast(string field, int val)
        {
            _rowNum = _rows.Count;
            return FindPrevious(field, val);
        }

        public bool FindNext(string field, string val)
        {
            _isMove = true;
            if (!HasRows()) return false;
            while (Read() && !StrEqual(_rowNum, val, field)) { }
            _row = EOF ? null : _rows[_rowNum];
            return !EOF;
        }

        public bool FindNext(string field, int val)
        {
            _isMove = true;
            if (!HasRows()) return false;
            while (Read() && !IntEqual(_rowNum, val, field)) { }
            _row = EOF ? null : _rows[_rowNum];
            return !EOF;
        }

        public bool FindPrevious(string field, string val)
        {
            _isMove = true;
            if (!HasRows()) return false;
            while (MovePrevious() && !StrEqual(_rowNum, val, field)) { }
            _row = BOF ? null : _rows[_rowNum];
            return !BOF;
        }

        public bool FindPrevious(string field, int val)
        {
            _isMove = true;
            if (!HasRows()) return false;
            while (MovePrevious() && !IntEqual(_rowNum, val, field)) { }
            _row = BOF ? null : _rows[_rowNum];
            return !BOF;
        }
    }
}