﻿using System;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //Класс для работы с SysTabl и SysSubTabl
    public class SysTabl : IDisposable 
    {
        //True - конструктор вызван с указанием соединения, False - с указанием файла Access
        private readonly bool _useDb;

        //dbFile - файл базы данных, useSubTabl - использовать SysSubTabl
        public SysTabl(string dbFile, bool useSubTabl = true)
        {
            _db = new DaoDb(dbFile);
            _useDb = false;
            OpenTables(useSubTabl);
        }

        // db - соединение с базой данных, useSubTabl - использовать SysSubTabl
        public SysTabl(DaoDb db, bool useSubTabl = true)
        {
            _db = db;
            _useDb = true;
            OpenTables(useSubTabl);
        }

        private void OpenTables(bool useSubTabl)
        {
            _tabl = new RecDao(_db, "SELECT * FROM SysTabl");
            if (useSubTabl)
                _sub = new RecDao(_db, "SELECT SysSubTabl.*, SysTabl.ParamName FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId");
        }

        //База данных, содержащяя SysTabl
        private readonly DaoDb _db;
        //Рекордсеты SysTabl и  SysSubTabl
        private RecDao _tabl;
        private RecDao _sub;

        //Закрытие
        public void Dispose()
        {
            if (_sub != null) _sub.Dispose();
            if (_tabl != null) _tabl.Dispose();
            if (_db != null && !_useDb) _db.Dispose();
        }

        //Получение значения параметра из SysTabl
        public string Value(string param)
        {
            _tabl.FindFirst("ParamName", param);
            return _tabl.GetString("ParamValue");
        }

        //Запись значения параметра в SysTabl
        public void PutValue(string param, string value, string tag = null)
        {
            _tabl.FindFirst("ParamName", param);
            _tabl.Put("ParamValue", value);
            if (tag != null) _tabl.Put("ParamTag", tag);
            _tabl.Update();
        }

        //Получение Tag параметра из SysTabl
        public string Tag(string param)
        {
            _tabl.FindFirst("ParamName", param);
            return _tabl.GetString("ParamTag");
        }

        //Запись Tag параметра в SysTabl
        public void PutTag(string param, string tag, string value = null)
        {
            _tabl.FindFirst("ParamName", param);
            _tabl.Put("ParamTag", tag);
            if (value != null) _tabl.Put("ParamValue", value);
            _tabl.Update();
        }

        //Получение значения подпараметра из SysSubTabl
        public string SubValue(string param, string subparam)
        {
            _sub.FindFirst("(ParamName='" + param + "') And (SubParamName='" + subparam + "')");
            return _sub.GetString("SubParamValue");
        }

        //Запись значения подпараметра в SysSubTabl
        public void PutSubValue(string param, string subparam, string value, string tag = null)
        {
            _sub.FindFirst("(ParamName='" + param + "') And (SubParamName='" + subparam + "')");
            _sub.Put("SubParamValue", value);
            if (tag != null) _sub.Put("SubParamTag", tag);  
            _sub.Update();
        }

        //Получение Tag подпараметра из SysSubTabl
        public string SubTag(string param, string subparam)
        {
            _sub.FindFirst("(ParamName='" + param + "') And (SubParamName='" + subparam + "')");
            return _sub.GetString("SubParamTag");
        }

        //Запись Tag подпараметра в SysSubTabl
        public void PutSubTag(string param, string subparam, string tag, string value = null)
        {
            _sub.FindFirst("(ParamName='" + param + "') And (SubParamName='" + subparam + "')");
            _sub.Put("SubParamTag", tag);
            if (value != null) _sub.Put("SubParamValue", value);    
            _sub.Update();
        }

        //Статические методы, 
        //Отличаются от обычных тем, что указан файл содержаший SysTabl, после чтения значения файл закрывается

        //Получение значения параметра из SysTabl
        public static string ValueS(string file, string param)
        {
            string stSql = "SELECT SysTabl.ParamValue FROM SysTabl WHERE (SysTabl.ParamName='" + param + "')";
            using (var rec = new RecDao(file, stSql, RecordsetTypeEnum.dbOpenSnapshot, RecordsetOptionEnum.dbReadOnly))
                return rec.GetString("ParamValue");
        }

        //Запись значения параметра в SysTabl
        public static void PutValueS(string file, string param, string value, string tag = null)
        {
            string stSql = "SELECT SysTabl.ParamValue, SysTabl.ParamTag FROM SysTabl WHERE (SysTabl.ParamName='" + param + "')";
            using (var rec = new RecDao(file, stSql))
            {
                rec.Put("ParamValue", value);
                if (tag != null) rec.Put("ParamTag", tag);
                rec.Update();
            }
        }

        //Получение Tag параметра из SysTabl
        public static string TagS(string file, string param)
        {
            string stSql = "SELECT SysTabl.ParamTag FROM SysTabl WHERE (SysTabl.ParamName='" + param + "')";
            using (var rec = new RecDao(file, stSql, RecordsetTypeEnum.dbOpenSnapshot, RecordsetOptionEnum.dbReadOnly))
                return rec.GetString("ParamTag");
        }

        //Запись Tag параметра в SysTabl
        public static void PutTagS(string file, string param, string tag, string value = null)
        {
            string stSql = "SELECT SysTabl.ParamValue, SysTabl.ParamTag FROM SysTabl WHERE (SysTabl.ParamName='" + param + "')";
            using (var rec = new RecDao(file, stSql))
            {
                rec.Put("ParamTag", tag);
                if (value != null) rec.Put("ParamValue", value);
                rec.Update();
            }
        }

        //Получение значения подпараметра из SysSubTabl
        public static string SubValueS(string file, string param, string subparam)
        {
            string stSql = "SELECT SysSubTabl.SubParamValue FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId " +
                                 " WHERE (SysTabl.ParamName='" + param + "') AND (SysSubTabl.SubParamName='" + subparam + "')";
            using (var rec = new RecDao(file, stSql, RecordsetTypeEnum.dbOpenSnapshot, RecordsetOptionEnum.dbReadOnly))
                return rec.GetString("SubParamValue");
        }

        //Запись значения подпараметра в SysSubTabl
        public void PutSubValueS(string file, string param, string subparam, string value, string tag = null)
        {
            string stSql = "SELECT SysSubTabl.SubParamValue, SysSubTabl.SubParamTag FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId " +
                                 " WHERE (SysTabl.ParamName='" + param + "') AND (SysSubTabl.SubParamName='" + subparam + "')";
            using (var rec = new RecDao(file, stSql))
            {
                rec.Put("SubParamValue", value);
                if (tag != null) rec.Put("SubParamTag", tag);
                rec.Update();
            }
        }

        //Получение Tag подпараметра из SysSubTabl
        public static string SubTagS(string file, string param, string subparam)
        {
            string stSql = "SELECT SysSubTabl.SubParamTag FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId " +
                                 " WHERE (SysTabl.ParamName='" + param + "') AND (SysSubTabl.SubParamName='" + subparam + "')";
            using (var rec = new RecDao(file, stSql, RecordsetTypeEnum.dbOpenSnapshot, RecordsetOptionEnum.dbReadOnly))
                return rec.GetString("SubParamTag");
        }

        //Запись Tag подпараметра в SysSubTabl
        public static void PutSubTagS(string file, string param, string subparam, string tag, string value = null)
        {
            string stSql = "SELECT SysSubTabl.SubParamValue, SysSubTabl.SubParamTag FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId = SysSubTabl.ParamId " +
                                 " WHERE (SysTabl.ParamName='" + param + "') AND (SysSubTabl.SubParamName='" + subparam + "')";
            using (var rec = new RecDao(file, stSql, RecordsetTypeEnum.dbOpenDynaset))
            {
                rec.Put("SubParamTag", tag);
                if (value != null) rec.Put("SubParamValue", value);
                rec.Update();
            }
        }

    }
}
