﻿using System;
using System.Data.SqlClient;
using System.Threading;

namespace BaseLibrary
{
    //Свойства соединения с SQL
    public struct SqlProps
    {
        public SqlProps(string serverName, string databaseName, bool sqlIdent = false, string login = "", string password = "")
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            SqlIdent = sqlIdent;
            Login = login;
            Password = password;
        }

        //Имя сервера
        public string ServerName;
        //Имя базы данных
        public string DatabaseName;
        //True, если идентификация SQL Server
        public bool SqlIdent;
        //Пользователь
        public string Login;
        //Пароль
        public string Password;

        public override string ToString()
        {
            return "Server=" + (ServerName ?? "") + ";Database=" + (DatabaseName ?? "") + ";SqlIdent=" + SqlIdent + ";Login=" + (Login ?? "") + ";Password=" + (Password ?? "");
        }
    }

    //---------------------------------------------------------------------------------------
    //База данных SQL
    public static class SqlDb
    {
        //Открывает соединение без соединения с базой данных
        public static SqlConnection ConnectServer(string server, bool sqlIdent = false, string user = "", string password = "")
        {
            var st = "Data Source=" + (server ?? "") + ";Integrated Security=";
            if (!sqlIdent) st += "True";
            else st += "False;User=" + (user ?? "") + "; Password=" + (password ?? "");
            var con = new SqlConnection(st);
            con.Open();
            return con;
        }

        //Открывает соединение, исключения не обрабатывает
        public static SqlConnection Connect(string server, string database, bool sqlIdent = false, string user = "", string password = "")
        {
            var st = "Data Source=" + (server ?? "") + ";Initial Catalog=" + (database ?? "") + ";Integrated Security=";
            if (!sqlIdent) st += "True";
            else st += "False;User=" + (user ?? "") + "; Password=" + (password ?? "");
            var con = new SqlConnection(st);
            con.Open();
            return con;
        }

        //sqlIdent=true - идентификация SqlServer иначе Windows 
        public static SqlConnection Connect(SqlProps props)
        {
            var st = "Data Source=" + (props.ServerName ?? "") + ";Initial Catalog=" + (props.DatabaseName ?? "") + ";Integrated Security=";
            if (!props.SqlIdent) st += "True";
            else st += "False;User=" + (props.Login ?? "") + "; Password=" + (props.Password ?? "");
            var con = new SqlConnection(st);
            con.Open();
            return con;
        } 

        //Выполняет запрос на изменение
        public static void Execute(SqlProps props, string stSql)
        {
            var con = Connect(props);
            var com = new SqlCommand(stSql, con) { CommandTimeout = 500 };
            bool b = false;
            int i = 0;
            Exception exc = null;
            while (!b && i++<3)
            {
                try
                {
                    com.ExecuteNonQuery(); 
                    b = true;
                }
                catch (Exception ex)
                {
                    exc = ex;
                    Thread.Sleep(500);
                }
            }
            try {con.Close();} catch {}
            if (!b) throw exc;
        }
    }
}
