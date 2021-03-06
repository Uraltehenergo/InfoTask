﻿using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //База данных Access, проверки, запросы
    public class DaoDb : IDisposable
    {
        public DaoDb(string file)
        {
            File = file;
        }

        //DBEngine для экземпляра и статическое
        private DBEngine _engine;
        public DBEngine Engine
        {
            get
            {
                return _engine ?? (_engine = new DBEngine());
            }
        }
        private static DBEngine _engineStatic;
        public static DBEngine EngineStatic
        {
            get
            {
                return _engineStatic ?? (_engineStatic = new DBEngine());
            }
        }

        //Путь к файлу
        internal string File { get; set; }
        //Соединение DAO с базой данных
        public Database Database { get; private set; }
        //Соединение ADO с базой данных
        public OleDbConnection Connection { get; private set; }

        //Установить соединение по Ado или Dao
        public DaoDb ConnectDao()
        {
            if (Database == null)
            {
                if (!new FileInfo(File).Exists)
                    throw new FileNotFoundException("Файл базы данных не найден", File);
                Database = Engine.OpenDatabase(File);
            }
            return this;
        }
        public DaoDb ConnectAdo()
        {
            if (Connection == null)
            {
                if (!new FileInfo(File).Exists)
                    throw new FileNotFoundException("Файл базы данных не найден", File);
                Connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + File);
                Connection.Open();
            }
            return this;
        }

        public void Dispose()
        {
            try
            {
                if (Database != null) Database.Close();
                Database = null;
            } catch { }
            try
            {
                if (_engine != null)
                {
                    _engine.FreeLocks();
                    _engine = null;
                } 
            }
            catch { }
            try
            {
                if (Connection != null) Connection.Close();
                Connection = null;
            } catch { }
        }

        //Выполнить запрос StSql - строка запроса, options - опции запроса
        public void Execute(string stSql, object options = null)
        {
            if (Database == null)
                Database = Engine.OpenDatabase(File);
            if (options == null) Database.Execute(stSql);
            else Database.Execute(stSql, options);
        }

        public void ExecuteAdo(string stSql)
        {
            if (Connection == null)
            {
                Connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + File);
                Connection.Open();
            }
            var command = new OleDbCommand(stSql,Connection);
            command.ExecuteNonQuery();
        }

        //Работа с таблицами

        //Проверка на наличие поля в таблице
        private bool ColumnExists(string tableName, string columnName)
        {
            ConnectDao();
            //foreach (Field c in Database.TableDefs[tableName].Fields)
            //    if (c.Name.ToLower().Equals(columnName.ToLower()))
            //        return true;
            //return false;
            //далее то же самое
            return Database.TableDefs[tableName].Fields.Cast<Field>().Any(c => c.Name.ToLower().Equals(columnName.ToLower()));
        }
        //Проверка на наличие таблицы в БД
        private bool TableExists(string tableName)
        {
            ConnectDao();
            return Database.TableDefs.Cast<TableDef>().Any(t => t.Name.ToLower().Equals(tableName.ToLower()));
        }
        //Проверка на наличие индекса в БД
        private bool IndexExists(string tableName, string indexName)
        {
            ConnectDao();
            return Database.TableDefs[tableName].Indexes.Cast<Index>().Any(i => i.Name.ToLower().Equals(indexName.ToLower()));
        }

        //Добавление логического поля (если существует, просто навешивание комбобоксы на ячейки)
        public void SetColumnBool(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            bool? defaultValue = null)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] YESNO");
                Dispose();
            }
            else
            {
                ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] YESNO");
                Dispose();
            }
            ConnectDao();
            try
            {
                Database.TableDefs[tableName].Fields[columnName].Properties.Append(
                    Database.TableDefs[tableName].Fields[columnName].CreateProperty("DisplayControl", 3, 106, false));    
            }
            catch {}

            SetColumnIndex(tableName, columnName, indexMode);
            ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] SET DEFAULT " + defaultValue);
        }

        //Добавление поля в таблицу
        public void SetColumnDouble(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            double? defaultValue = null, bool required = false)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] DOUBLE");
                Dispose();
            }
            else
            {
                ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] DOUBLE");
                Dispose();
            }
            SetColumnIndex(tableName, columnName, indexMode);

            //ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] SET DEFAULT " + defaultValue);

            ConnectDao();
            Database.TableDefs[tableName].Fields[columnName].Required = required;
            string defValS = defaultValue.HasValue ? defaultValue.ToString() : "";
            Database.TableDefs[tableName].Fields[columnName].DefaultValue = defValS;
        }

        public void SetColumnLong(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            long? defaultValue = null, bool required = false)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] LONG");
                Dispose();
            }
            else
            {
                ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] LONG");
            }
            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            Database.TableDefs[tableName].Fields[columnName].Required = required;

            string defValS = defaultValue.HasValue ? defaultValue.ToString() : "";
            Database.TableDefs[tableName].Fields[columnName].DefaultValue = defValS;
        }

        public void SetColumnString(string tableName, string columnName, int length = 255,
            IndexModes indexMode = IndexModes.WithoutChange, string defaultValue = null, bool required = false, bool emptyStrings = true)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] TEXT(" + length + ")");
                Dispose();
            }
            ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] TEXT(" + length + ") WITH COMPRESSION");
            Dispose();

            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            Database.TableDefs[tableName].Fields[columnName].Required = required;
            Database.TableDefs[tableName].Fields[columnName].AllowZeroLength = emptyStrings;
            string defValS = defaultValue ?? "";
            Database.TableDefs[tableName].Fields[columnName].DefaultValue = defValS;
        }

        public void SetColumnMemo(string tableName, string columnName, string defaultValue = null, bool required = false, bool emptyStrings = true)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] MEMO");
                Dispose();
                ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] MEMO WITH COMPRESSION");
            }
            else ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] MEMO WITH COMPRESSION");

            ConnectDao();
            Database.TableDefs[tableName].Fields[columnName].Required = required;
            Database.TableDefs[tableName].Fields[columnName].AllowZeroLength = emptyStrings;
            string defValS = defaultValue ?? "";
            Database.TableDefs[tableName].Fields[columnName].DefaultValue = defValS;
        }

        public void SetColumnDatetime(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            DateTime? defaultValue = null, bool required = false)
        {
            if (!ColumnExists(tableName, columnName))
            {
                Execute("ALTER TABLE " + tableName + " ADD COLUMN [" + columnName + "] DATETIME");
                Dispose();
            }
            else
            {
                ExecuteAdo("ALTER TABLE " + tableName + " ALTER COLUMN [" + columnName + "] DATETIME");
                Dispose();
            }
            SetColumnIndex(tableName, columnName, indexMode);

            ConnectDao();
            Database.TableDefs[tableName].Fields[columnName].Required = required;
            string defValS = defaultValue.HasValue ? defaultValue.ToString() : "";
            Database.TableDefs[tableName].Fields[columnName].DefaultValue = defValS;
        }

        public void SetColumn(string tableName, string columnName, DataType dtype)
        {
            switch (dtype)
            {
                case DataType.String:
                    SetColumnString(tableName, columnName);
                    break;
                case DataType.Real:
                    SetColumnDouble(tableName, columnName);
                    break;
                case DataType.Integer:
                    SetColumnLong(tableName, columnName);
                    break;
                case DataType.Boolean:
                    SetColumnBool(tableName, columnName);
                    break;
                case DataType.Time:
                    SetColumnDatetime(tableName, columnName);
                    break;
            }
        }

        //Удаление поля из таблицы
        public void DeleteColumn(string tableName, string columnName)
        {
            if (ColumnExists(tableName, columnName))
                Execute("ALTER TABLE " + tableName + " DROP COLUMN " + columnName);
        }

        //Добавление таблицы в БД
        public void AddTable(string tableName, string sourceTable)
        {
            if (!TableExists(tableName))
            {
                Execute("SELECT * INTO " + tableName + " FROM " + sourceTable);
            }
        }

        public void RenameTable(string tableNameOld, string tableNameNew)
        {
            if (TableExists(tableNameOld))
            {
                //ExecuteAdo("RENAME TABLE " + tableNameOld + " TO " + tableNameNew + ";");
                ConnectDao();
                Database.TableDefs[tableNameOld].Name = tableNameNew;
            }
        }

        //Связь многие к одному
        public void AddForeignLink(string tableName, string columnName, string linkedTable, string linkedColumn, bool cascade = true)
        {
            string cascadeS = cascade ? " ON DELETE CASCADE ON UPDATE CASCADE" : "";
            ExecuteAdo("ALTER TABLE " + tableName + " ADD CONSTRAINT " + linkedTable + tableName
                       + " FOREIGN KEY ([" + columnName + "]) REFERENCES " + linkedTable + "(" + linkedColumn + ")" + cascadeS);
        }

        //Добавление параметров в SysTabl
        public void AddSysParam(string templatePath, string paramName)
        {
            using (var rec = new RecDao(File, "SELECT ParamId, ParamName FROM SysTabl WHERE ParamName='" + paramName + "'"))
                if (rec.HasRows()) return;
            Execute("INSERT INTO SysTabl SELECT ParamName, ParamType, ParamValue, ParamDescription, ParamTag " +
                    "FROM [" + templatePath + "].SysTabl t1 WHERE t1.ParamName='" + paramName + "';");
        }

        //Добавление подпараметров в SysTabl
        public void AddSysSubParam(string templatePath, string paramName, string subParamName)
        {
            int paramId;
            using (var sysTablRS = new RecDao(templatePath, "SELECT ParamId,ParamName FROM SysTabl WHERE ParamName='" + paramName + "'"))
                paramId = sysTablRS.GetInt("ParamId");
            Execute("INSERT INTO SysSubTabl " +
                    "SELECT ParamId, SubParamNum, SubParamName,SubParamType, SubParamValue, SubParamDescription, SubParamTag, SubParamRowSource " +
                    "FROM [" + templatePath + "].SysSubTabl t1 WHERE t1.ParamId=" + paramId + " AND t1.SubParamName='" + subParamName +
                    "' AND NOT EXISTS(SELECT * FROM SysSubTabl t2 WHERE t1.SubParamName = t2.SubParamName)");
        }

        //Добавление индекса по одному полю
        public void SetColumnIndex(string tableName, string columnName, IndexModes indexMode = IndexModes.WithoutChange,
            string oldIndexName = null)
        {
            //важно, что в случае indexMode = EmptyIndex columnName на самом деле - название индекса, а не поля,
            //т.к. они могут и не совпадать
            switch (indexMode)
            {
                case IndexModes.CommonIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE INDEX [" + columnName + "] ON " + tableName + " ([" + columnName + "])");
                    break;
                case IndexModes.UniqueIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE UNIQUE INDEX [" + columnName + "] ON " + tableName + " ([" + columnName + "])");
                    break;
                case IndexModes.EmptyIndex:
                    if (IndexExists(tableName, columnName))
                        ExecuteAdo("DROP INDEX [" + columnName + "] ON " + tableName);
                    break;
            }
        }

        //Добавление индекса по двум полям
        public void SetColumnIndex(string tableName, string columnName, string columnName2, bool primary, IndexModes indexMode = IndexModes.WithoutChange, string oldIndexName = null)
        {
            switch (indexMode)
            {
                case IndexModes.CommonIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE INDEX [" + columnName + "] ON " + tableName + " ([" + columnName + "], [" + columnName2 + "])" + (primary ? " WITH PRIMARY" : ""));
                    break;
                case IndexModes.UniqueIndex:
                    if (oldIndexName != null) SetColumnIndex(tableName, oldIndexName, IndexModes.EmptyIndex);
                    if (!IndexExists(tableName, columnName))
                        ExecuteAdo("CREATE UNIQUE INDEX [" + (primary ? "PrimaryKey" : columnName) + "] ON " + tableName + " ([" + columnName + "], [" + columnName2 + "])" + (primary ? " WITH PRIMARY" : ""));
                    break;
            }
        }

        //Метод выдает список индексов данной таблицы или сообщает о том, есть ли в ней данный индекс
        public static bool RunOverIndexList(string dbFile, string table, string index = "")
        {
            try
            {
                string indexesS = "";
                bool fieldFinded = false;
                using (var db = new DaoDb(dbFile))
                {
                    db.ConnectDao();
                    foreach (Index ind in db.Database.TableDefs[table].Indexes)
                    {
                        indexesS += ind.Name + "\n";
                        if (ind.Name == index)
                        {
                            fieldFinded = true;
                        }
                    }
                }
                if (index == "") MessageBox.Show(indexesS);
                else return fieldFinded;
                return false;
            }
            catch (Exception)
            { return false; }
        }

        //Статические члены
        //Закрывает базу
        public static void CloseDatabase(Database db)
        {
            try { db.Close(); }
            catch { }
            try
            {
                if (_engineStatic != null)
                {
                    _engineStatic.FreeLocks();
                    _engineStatic = null;
                }
            }
            catch { }
        }


        //Выполнить запрос file - путь к файлу базы данных или сама база, StSql - строка запроса, options - опции запроса
        public static void Execute(string file, string stSql, object options = null)
        {
            if (file.IsEmpty() || stSql.IsEmpty())
                throw new NullReferenceException("Файл базы данных и строка запроса не могут быть пустыми или null");
            Database db = EngineStatic.OpenDatabase(file);
            try
            {
                if (options == null) db.Execute(stSql);
                else db.Execute(stSql, options);
            }
            finally { CloseDatabase(db);}
        }

        public static void ExecuteAdo(string file, string stSql, object options = null)
        {
            if (file.IsEmpty() || stSql.IsEmpty())
                throw new NullReferenceException("Файл базы данных и строка запроса не могут быть пустыми или null");
            using (var connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file))
            {
                connection.Open();
                var command = new OleDbCommand(stSql, connection);
                command.ExecuteNonQuery();
            }
        }

        //Проверка на содержание списка указанных таблиц tables в указанной базе данных file 
        //Возвращает true, если проверка прошла удачно
        public static bool Check(string file, IEnumerable<string> tables = null)
        {
            try
            {
                if (file.IsEmpty() || !new FileInfo(file).Exists)
                    return false;
                Database db = EngineStatic.OpenDatabase(file);
                try
                {
                    var missing = new SortedSet<string>();
                    if (tables != null)
                    {
                        foreach (var table in tables)
                            missing.Add(table);
                        foreach (var t in db.TableDefs)
                        {
                            string s = ((TableDef)t).Name;
                            if (missing.Contains(s)) missing.Remove(s);
                        }
                    }
                    return missing.Count == 0;
                }
                finally { CloseDatabase(db); }
            }
            catch { return false; }
        }
        
        //Сжатие базы данных, file - файл базы, size - размер а байтах, после которого нужно сжимать, 
        //tmpDir - каталог временных фалов, timeout - время ожидания после сжатия в мс
        //Возвращает nul если все хорошо или Exception
        public static void Compress(string file, int size, string tmpDir = null, int timeout = 0)
        {
            if (file.IsEmpty())
                throw new NullReferenceException("Файл сжимаемой базы данных не может быть пустой строкой или null");

            var fdb = new FileInfo(file);
            if (fdb.Length < size) return;
            string sdir = fdb.Directory.FullName;
            if (tmpDir != null)
            {
                var dir = new DirectoryInfo(tmpDir);
                if (!dir.Exists) dir.Create();
                sdir = tmpDir;
            }
            var ftmp = new FileInfo(sdir + @"\Tmp" + fdb.Name);
            if (ftmp.Exists) ftmp.Delete();
            fdb.MoveTo(ftmp.FullName);
            new FileInfo(file).Delete();
            EngineStatic.CompactDatabase(ftmp.FullName, file);
            EngineStatic.FreeLocks();
            _engineStatic = null;
            if (timeout > 0) Thread.Sleep(timeout);
        }

        //Создает новый файл из шаблона, если его еще нет или версия не совпадает с шаблоном и checkVersion = true
        //template - путь к шаблону, file - путь к создаваемому файлу, 
        //saveOld - если true, то копирует старый файл в текущий каталог, добавляя на конце _1, _2 и т. д.
        //Возвращает true, если файл был скопирован
        public static bool FromTemplate(string template, string file, ReplaceByTemplate replace = ReplaceByTemplate.IfNewVersion, bool saveOld = false)
        {
            var f = new FileInfo(file);
            if (!f.Directory.Exists) f.Directory.Create();
            bool needCopy = !f.Exists;
            needCopy |= replace == ReplaceByTemplate.Always;
            if (!needCopy && replace == ReplaceByTemplate.IfNewVersion)
            {
                string st = null, sf = null;
                try { st = SysTabl.SubValueS(template, "AppOptions", "AppVersion"); }  
                catch {}
                try { sf = SysTabl.SubValueS(file, "AppOptions", "AppVersion"); }  
                catch { }
                needCopy |= st != sf;
            }
            if (needCopy)
            {
                if (saveOld && f.Exists)
                {
                    bool b = true;
                    string s = file.Substring(0, file.Length - 6);
                    int i = 1;
                    string ss = "";
                    while (b && i < 10000)
                    {
                        ss = s + "_" + (i++) + ".accdb";
                        b = new FileInfo(ss).Exists;
                    }
                    if (i <= 10000) new FileInfo(file).MoveTo(ss);
                    else new FileInfo(file).Delete();
                    Thread.Sleep(2000);
                }
                new FileInfo(template).CopyTo(file, true);
                Thread.Sleep(500);
            }
            return needCopy;
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //Как проверять, нужно ли копировать файл из шаблона
    public enum ReplaceByTemplate
    {
        Always,
        IfNewVersion,
        IfNotExists
    }
    
    //Перечисление состояний индексов в SetColumn
    public enum IndexModes
    {
        UniqueIndex, CommonIndex, EmptyIndex, WithoutChange
    }
}
