using System;
using System.Windows.Forms;

namespace BaseLibrary
{
    //Тип базы данных
    public enum DatabaseType
    {
        Access,
        SqlServer
    }

    //---------------------------------------------------------------------------------------
    //Общий интерфейс для всех рекордсетов читающих данные
    public interface IRecordRead : IDisposable
    {
        bool Read();
        bool EOF { get;}
        bool HasRows();

        string GetString(string field, string nullValue = null);
        double GetDouble(string field, double nullValue = 0);
        double? GetDoubleNull(string field);
        int GetInt(string field, int nullValue = 0);
        int? GetIntNull(string field);
        DateTime GetTime(string field);
        DateTime? GetTimeNull(string field);
        bool GetBool(string field, bool nullValue = false);
        bool? GetBoolNull(string field);

        void GetToDataGrid(string field, DataGridViewCellCollection cells, string gridField = null);
    }

    //---------------------------------------------------------------------------------------------
    //Интерфейс добавления данных в таблицу
    public interface IRecordAdd : IDisposable
    {
        void AddNew();
        void Update();

        void Put(string field, string val, bool cut = false);
        void Put(string field, double val);
        void Put(string field, double? val);
        void Put(string field, int val);
        void Put(string field, int? val);
        void Put(string field, DateTime val);
        void Put(string field, DateTime? val);
        void Put(string field, bool val);
        void Put(string field, bool? val);
        void PutFromDataGrid(string field, DataGridViewCellCollection cells, string gridField = null);
    }


    //---------------------------------------------------------------------------------------------
    //Общий интерфейс для всех рекордсетов позволяющих запись, чтение и поиск данных
    public interface IRecordSet : IRecordRead, IRecordAdd
    {
        int RecordCount { get; }
        bool MoveFirst();
        bool MoveLast();
        bool MoveNext();
        bool MovePrevious();
        bool BOF { get; }

        bool FindFirst(string field, string val);
        bool FindFirst(string field, int val);
        bool FindLast(string field, string val);
        bool FindLast(string field, int val);
        bool FindNext(string field, string val);
        bool FindNext(string field, int val);
        bool FindPrevious(string field, string val);
        bool FindPrevious(string field, int val);
    }
}
