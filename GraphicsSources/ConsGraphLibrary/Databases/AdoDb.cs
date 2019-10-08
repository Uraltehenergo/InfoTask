using System;
using System.Data;
using System.Data.OleDb;

namespace BaseLibrary
{
    public class AdoDb : IDisposable 
    {
        public AdoDb(string file)
        {
            Connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file);
            Connection.Open();
        }

        //Ссылка на OleDbConnection или SqlConnection
        public IDbConnection Connection { get; private set; }

        public void Dispose()
        {
            try
            {
                Connection.Close();
                Connection.Dispose();
            }
            catch { }
        }
    }
}