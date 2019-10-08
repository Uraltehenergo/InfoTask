using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Provider
{
    internal class SimaticArchive
    {
        //Одна база данных: основная или дублирующая
        internal SimaticArchive(SimaticSource source, string serverName, bool isReserve)
        {
            Source = source;
            ServerName = serverName;
            HasArchive = !ServerName.IsEmpty();
            IsReserve = isReserve;
            Hash = !HasArchive ? "" : ((!isReserve ? "SQLServer=" : "SQLReserveServer=") + ServerName);
            SuccessTime = Different.MinDate;
        }

        //Источник
        internal SimaticSource Source { get; private set; }
        //Архив задан
        internal bool HasArchive { get; private set; }
        //Является резервным
        internal bool IsReserve { get; private set; }
        //Имя сервера
        internal string ServerName { get; private set; }
        //Хэш
        internal string Hash { get; private set; }

        //Соединение с базой
        internal OleDbConnection Connection { get; private set; }
        //Время последнего успешного соединения
        internal DateTime SuccessTime { get; set; }

        //Установить соединение с архивом
        internal OleDbConnection Connnect()
        {
            if (!HasArchive) return Connection = null;
            try
            {
                var list = SqlDb.SqlDatabasesList(ServerName);
                var dbName = "";
                foreach (var db in list)
                    if (db.StartsWith("CC_") && db.EndsWith("R"))
                        dbName = db;
                if (dbName.IsEmpty()) return null;
                SqlDb.Connect(ServerName, dbName).GetSchema();//Проверка
                var dic = new Dictionary<string, string>
                    {
                        {"Provider", "WinCCOLEDBProvider.1"},
                        {"Catalog", dbName},
                        {"Data Source", ServerName}
                    };
                Connection = new OleDbConnection(dic.ToPropertyString());
                Connection.Open();
                return Connection;
            }
            catch (Exception ex)
            {
                Source.Logger.AddError("Соединение с сервером не установлено", ex, ServerName, Source.Context, false);
                return null;
            }
        }

        //Закрытие соединений
        internal void Disconnect()
        {
            if (Connection == null) return;
            try {Connection.Close();} catch {}
            Connection = null;
        }

        //Проверка соединения
        internal bool CheckConnection()
        {
            string s = " с " + (IsReserve ? "резервным" : "основным") + " архивом " + ServerName;
            if (!HasArchive)
                Source.CheckConnectionMessage += "Не задано соединение" + s;
            else
            {
                try
                {
                    if (Connnect().State == ConnectionState.Open)
                    {
                        Source.CheckConnectionMessage += "Успешное соединение" + s;
                        return true;
                    }
                    Source.CheckConnectionMessage += "Не удалось соединиться" + s;
                }
                catch { Source.CheckConnectionMessage += "Не удалось соединиться" + s; }
            }
            return false;
        }
    }
}