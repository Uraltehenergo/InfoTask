using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using System.Windows.Forms;
using BaseLibrary;

namespace AuditMonitor
{
//    public class ClassArchive
//    {
//        public static event Action<int> EventArchiveUpdated;
        
//        private OleDbConnection _accConnection;
//        private DataSet _accDataSet;
//        private OleDbDataAdapter _accAdapterArchive;
//        private OleDbCommandBuilder _accBuilderArchive;
//        private OleDbDataAdapter _accAdapterWorkLog;
//        private OleDbCommandBuilder _accBuilderWorkLog;
        
//        public int Connect(string archiveFile, bool showAlerts = true)
//        {
//            int res = 0;
//            int step = 0;

//            if (File.Exists(archiveFile))
//            {
//                try
//                {
//                    _accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + archiveFile);
//                    _accDataSet = new DataSet();
//                    _accAdapterArchive = new OleDbDataAdapter(@"SELECT [ID], [Address], [Channel], [Code], [SignalValue], [TagValue], [SignalStatus], [TagTime] FROM Archive;", _accConnection);
//                    _accBuilderArchive = new OleDbCommandBuilder(_accAdapterArchive);
//                    _accAdapterWorkLog = new OleDbDataAdapter(@"SELECT [ID], [Address], [Channel], [Code], [LogTime], [Log] FROM WorkLog;", _accConnection);
//                    _accBuilderWorkLog = new OleDbCommandBuilder(_accAdapterWorkLog);

//                    step = 1;
//                    _accAdapterArchive.Fill(_accDataSet, "Archive");
//                    step = 2;
//                    _accAdapterWorkLog.Fill(_accDataSet, "WorkLog");
//                }
//                catch (OleDbException e)
//                {
//                    if (_accConnection != null) _accConnection.Close();

//                    if(e.ErrorCode == -2147467259)
//                    {
//                        if (showAlerts) MessageBox.Show(e.ErrorCode + "\n" + e.Message);
//                        //if (showAlerts) MessageBox.Show(@"Файл " + archiveFile + @" не найден");
//                        res = 1;
//                    }
//                    else
//                    {
//                        if (showAlerts)
//                        {
//                            string strMessage = "Неверная структура файла архива " + archiveFile;

//                            switch (e.ErrorCode)
//                            {
//                                case -2147217865:
//                                    strMessage += "\nНе найдена таблица ";
//                                    break;
//                                case -2147217904:
//                                    strMessage += "\nНеверная структура таблицы ";
//                                    break;
//                                default:
//                                    step = 0;
//                                    break;
//                            }

//                            switch (step)
//                            {
//                                case 1:
//                                    strMessage += "Archive";
//                                    break;
//                                case 2:
//                                    strMessage += "WorkLog";
//                                    break;
//                                default:
//                                    strMessage = e.ErrorCode + "\n" + e.Message;
//                                    break;
//                            }

//                            MessageBox.Show(strMessage);
//                        }
//                        res = 2;
//                    }
//                }
//                catch (Exception e)
//                {
//                    _accConnection.Close();
//                    MessageBox.Show(e.GetType() + "\n" + e.Message);
//                    res = -1;
//                }
//            }
//            else
//            {
//                //предлагать создать новый
//                if (showAlerts) MessageBox.Show(@"Файл " + archiveFile + @" не найден");
//                res = 1;
//            }

//            return res;
//        }

//        public int Disconnect()
//        {
//            if (_accConnection != null) _accConnection.Close();
//            return 0;
//        }

//        public int WriteValue(long? id, string module, string channel, string code, string signalValue,
//                              double? value, byte signalStatus, DateTime? time, out Exception exception)
//        {
//            int res=0;
            
//            try
//            {
//                var newRow = _accDataSet.Tables["Archive"].NewRow();
//                if (id != null) {newRow["id"] = id;} else newRow["id"] = DBNull.Value;
//                newRow["Address"] = module;
//                newRow["Channel"] = channel;
//                newRow["Code"] = code;
//                newRow["SignalValue"] = signalValue;
//                if (value != null) newRow["TagValue"] = value; else newRow["TagValue"] = DBNull.Value;
//                newRow["SignalStatus"] = signalStatus;
//                newRow["TagTime"] = time;
//                _accDataSet.Tables["Archive"].Rows.Add(newRow);
//                //_accAdapteArchive.Update(_accDataSet, "Archive");
//                exception = null;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                res = -1;
//            }
            
//            return res;
//        }

//        public int UpdateArchive(out Exception exception)
//        {
//            int res = 0;

//            try
//            {
//                _accAdapterArchive.Update(_accDataSet, "Archive");
//                exception = null;
//                if (EventArchiveUpdated != null) EventArchiveUpdated(_accDataSet.Tables["Archive"].Rows.Count);
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                res = -1;
//            }

//            return res;
//        }
        
//        public int WriteWorkLog(long? id, string module, string channel, string code, DateTime time,
//                                string message, out Exception exception)
//        {
//            int res = 0;

//            try
//            {
//                var newRow = _accDataSet.Tables["WorkLog"].NewRow();
//                if (id != null) { newRow["id"] = id; } else newRow["id"] = DBNull.Value;
//                newRow["Address"] = module;
//                newRow["Channel"] = channel;
//                newRow["Code"] = code;
//                newRow["LogTime"] = time;
//                newRow["Log"] = message;

//                _accDataSet.Tables["WorkLog"].Rows.Add(newRow);
//                _accAdapterWorkLog.Update(_accDataSet, "WorkLog");
//                exception = null;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                res = -1;
//            }

//            return res;
//        }

//        public long? RecordCount
//        {
//            get
//            {
//                if (_accConnection != null) return _accDataSet.Tables["Archive"].Rows.Count; 
//                return null;
//            }
//        }

//        public bool IsConnected()
//        {
//            return (_accConnection != null);
//        }

//        public string DataSource()
//        {
//            return (_accConnection != null) ? _accConnection.DataSource : null;
//        }

//        public bool BeforeRead()
//        {
//            return true;
//        }

//        public bool ChangeChannelReadFlag()
//        {
//            return true;
//        }

//        public static DataRowCollection ReadFromArchive(string archiveFileName, string codes, DateTime beginTime, DateTime endTime)
//        {
//            OleDbConnection accConnection;
//            DataSet accDataSet;
//            //OleDbCommandBuilder accBuilder;

//            if (File.Exists(archiveFileName))
//            {
//                try
//                {
//                    accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + archiveFileName);
//                    accDataSet = new DataSet();
//                    var culture = new System.Globalization.CultureInfo("");
//                    var codeArr = codes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//                    for (int i = 0; i < codeArr.Length; i++) codeArr[i] = "'" + codeArr[i].Trim() + "'";
//                    var accAdapter = new OleDbDataAdapter(@"SELECT [ID], [Address], [Channel], [Code], [SignalValue], [TagValue], [SignalStatus], [TagTime] " +
//                                                           "FROM Archive " +
//                                                           "WHERE ([Code] IN (" + string.Join(", ", codeArr) + ")) " +
//                                                           "AND ([TagTime] >= #" + Convert.ToString(beginTime, culture) + "#) " +
//                                                           "AND ([TagTime] <= #" + Convert.ToString(endTime, culture) + "#) " +
//                                                           "ORDER BY [Code], [TagTime];",
//                                                          accConnection);
//                    //accBuilder = new OleDbCommandBuilder(accAdapter);
//                    accAdapter.Fill(accDataSet, "Archive");
//                }
//                catch/*(Exception e)*/
//                {
//                    //MessageBox.Show(e.Message);
//                    return null;
//                }
//            }
//            else{ return null; }
            
//            accConnection.Close();
//            return accDataSet.Tables["Archive"].Rows;
//        }

//        public static DataRowCollection ReadFromArchive(string archiveFileName, string address, string channel, DateTime beginTime, DateTime endTime)
//        {
//            OleDbConnection accConnection;
//            DataSet accDataSet;
//            //OleDbCommandBuilder accBuilder;

//            if (File.Exists(archiveFileName))
//            {
//                try
//                {
//                    accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + archiveFileName);
//                    accDataSet = new DataSet();
//                    var culture = new System.Globalization.CultureInfo("");
//                    var accAdapter = new OleDbDataAdapter(@"SELECT [ID], [Address], [Channel], [Code], [SignalValue], [TagValue], [SignalStatus], [TagTime] " +
//                                                           "FROM Archive " +
//                                                           "WHERE ([Address] = '" + address + "') AND ([Channel] = '" + channel + "')" +
//                                                           "AND ([TagTime] >= #" + Convert.ToString(beginTime, culture) + "#) " +
//                                                           "AND ([TagTime] <= #" + Convert.ToString(endTime, culture) + "#) " +
//                                                           "ORDER BY [Address], [Channel], [TagTime];",
//                                                          accConnection);
//                    //accBuilder = new OleDbCommandBuilder(accAdapter);
//                    accAdapter.Fill(accDataSet, "Archive");
//                }
//                catch { return null; }
//            }
//            else { return null; }

//            accConnection.Close();
//            return accDataSet.Tables["Archive"].Rows;
//        }
//    }
    
//    public class ClassAuditArchive
//    {
//        private DaoDb _daoDb;
//        private RecDao _rsValue;
        
//        public int? RecordCount
//        {
//            get
//            {
//                if(_rsValue != null) return _rsValue.RecordCount;
//                return null;
//            }
//        }
        
//        public bool Connect(string fileName, bool showAlerts = true)
//        {
//            if (DaoDb.Check(fileName, new[] { "Params", "Values" }))
//            {
//                try
//                {
//                    _daoDb = new DaoDb(fileName);

//                    byte step = 0;

//                    try
//                    {
//                        step = 1;
//                        string stSql =
//                            @"SELECT [ParamId], [Code], [Name], [Module], [Channel], [Min], [Max], [Units], [Aperture], 
//                                     [DataFormat], [ChannelRange], [SignalType], [Conversion], [ConversionType],
//                                     [ConvCoef0], [ConvCoef1], [ConvCoef2], [ConvCoef3], [ConvCoef4], [ConvCoef5],
//                                     [Active] , [TimeAdd], [TimeChange] FROM Params;";

//                        using (new RecDao(_daoDb, stSql))
//                        {
//                        }

//                        step = 2;
//                        stSql = @"SELECT [ParamId], [Time], [Signal], [Value], [Status] FROM [Values];";
//                        _rsValue = new RecDao(_daoDb, stSql);

//                        return true;
//                    }
//                    catch (Exception e)
//                    {
//                        if (showAlerts)
//                        {
//                            string strMessage = "Ошибка доступа к архиву " + fileName;
                            
//                            switch (step)
//                            {
//                                case 1:
//                                    strMessage += ":\nНеверная структура таблицы Params";
//                                    break;
//                                case 2:
//                                    strMessage += ":\nНеверная структура таблицы Values";
//                                    break;
//                                default:
//                                    strMessage += "\n" + e.Message;
//                                    break;
//                            }

//                            MessageBox.Show(strMessage);
//                        }
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (showAlerts) MessageBox.Show(@"Ошибка открытия Базы Данных (" + e.GetType() + @")" + "\n" + e.Message);
//                }
//            }
//            else
//            {
//                if (showAlerts) MessageBox.Show(@"Ошибка открытия Базы Данных:" + "\n" + @"Недопустимый файл архива");
//            }

//            return false;
//        }

//        public void Disconnect()
//        {
//            _rsValue.Dispose();
//            _daoDb.Dispose();
//        }
        
//        public bool UpdateParams(ClassAdamModules modules, out Exception exception)
//        {
//            try
//            {
//                _daoDb.Execute("UPDATE Params SET [Active] = False");
                
//                const string stSql =
//                    @"SELECT [ParamId], [Code], [Name], [Module], [Channel], [Min], [Max], [Units], [Aperture], 
//                             [DataFormat], [ChannelRange], [SignalType], [Conversion], [ConversionType],
//                             [ConvCoef0], [ConvCoef1], [ConvCoef2], [ConvCoef3], [ConvCoef4], [ConvCoef5],
//                             [Active] , [TimeAdd], [TimeChange] FROM Params;";

//                using (var rsParams = new RecDao(_daoDb, stSql))
//                {
//                    foreach (ClassAbstractAdamModule module in modules)
//                        foreach (ClassAbstractAdamChannel channel in module.Channel)
//                            if (rsParams.FindFirst("Code", channel.Code))
//                            {
//                                bool isChange = false;
                                
//                                if (channel.Name != rsParams.GetString("Name"))
//                                {
//                                    rsParams.Put("Name", channel.Name); 
//                                    isChange = true;
//                                }

//                                if (channel.Module.Address != rsParams.GetString("Module"))
//                                {   
//                                    rsParams.Put("Module", channel.Module.Address);
//                                    isChange = true;
//                                }

//                                if (channel.Channel != rsParams.GetIntNull("Channel"))
//                                { 
//                                    rsParams.Put("Channel", channel.Channel);
//                                    isChange = true;
//                                }

//                                if (channel.Min != rsParams.GetDoubleNull("Min"))
//                                {
//                                    rsParams.Put("Min", channel.Min);
//                                    isChange = true;
//                                }

//                                if (channel.Max != rsParams.GetDoubleNull("Max"))
//                                {
//                                    rsParams.Put("Max", channel.Max);
//                                    isChange = true;
//                                }

//                                if (channel.Units != rsParams.GetString("Units"))
//                                {
//                                    rsParams.Put("Units", channel.Units);
//                                    isChange = true;
//                                }

//                                if (channel.Aperture != rsParams.GetDoubleNull("Aperture"))
//                                {
//                                    rsParams.Put("Aperture", channel.Aperture);
//                                    isChange = true;
//                                }

//                                if (channel.DataFormat != rsParams.GetString("DataFormat"))
//                                {
//                                    rsParams.Put("DataFormat", channel.DataFormat);
//                                    isChange = true;
//                                }

//                                if (channel.ChannelRange != rsParams.GetString("ChannelRange"))
//                                {
//                                    rsParams.Put("ChannelRange", channel.ChannelType);
//                                    isChange = true;
//                                }

//                                if (channel.InLevel != rsParams.GetString("SignalType"))
//                                {
//                                    rsParams.Put("SignalType", channel.InLevel);
//                                    isChange = true;
//                                }

//                                if (channel.Conversion != rsParams.GetString("Conversion"))
//                                {
//                                    rsParams.Put("Conversion", channel.Conversion);
//                                    isChange = true;
//                                }

//                                if (channel.ConversionType != rsParams.GetIntNull("ConversionType"))
//                                {
//                                    rsParams.Put("ConversionType", channel.ConversionType);
//                                    isChange = true;
//                                }

//                                if (channel.ConvCoef0 != rsParams.GetDoubleNull("ConvCoef0"))
//                                {
//                                    rsParams.Put("ConvCoef0", channel.ConvCoef0);
//                                    isChange = true;
//                                }

//                                if (channel.ConvCoef1 != rsParams.GetDoubleNull("ConvCoef1"))
//                                {
//                                    rsParams.Put("ConvCoef1", channel.ConvCoef1);
//                                    isChange = true;
//                                }

//                                if (channel.ConvCoef2 != rsParams.GetDoubleNull("ConvCoef2"))
//                                {
//                                    rsParams.Put("ConvCoef2", channel.ConvCoef2);
//                                    isChange = true;
//                                }

//                                if (channel.ConvCoef3 != rsParams.GetDoubleNull("ConvCoef3"))
//                                {
//                                    rsParams.Put("ConvCoef3", channel.ConvCoef3);
//                                    isChange = true;
//                                }

//                                if (channel.ConvCoef4 != rsParams.GetDoubleNull("ConvCoef4"))
//                                {
//                                    rsParams.Put("ConvCoef4", channel.ConvCoef4);
//                                    isChange = true;
//                                }

//                                if (channel.ConvCoef5 != rsParams.GetDoubleNull("ConvCoef5"))
//                                {
//                                    rsParams.Put("ConvCoef5", channel.ConvCoef5);
//                                    isChange = true;
//                                }

//                                if (isChange) rsParams.Put("TimeChange", DateTime.Now);

//                                rsParams.Put("Active", true);
//                                rsParams.Update();

//                                channel.ArchiveId = rsParams.GetInt("ParamId");
//                            }
//                            else
//                            {
//                                rsParams.AddNew();
//                                rsParams.Put("Code", channel.Code);
//                                rsParams.Put("Name", channel.Name);
//                                rsParams.Put("Module", channel.Module.Address);
//                                rsParams.Put("Channel", channel.Channel);
//                                rsParams.Put("Min", channel.Min);
//                                rsParams.Put("Max", channel.Max);
//                                rsParams.Put("Units", channel.Units);
//                                rsParams.Put("Aperture", channel.Aperture);
//                                rsParams.Put("DataFormat", channel.DataFormat);
//                                rsParams.Put("ChannelRange", channel.ChannelRange);
//                                rsParams.Put("SignalType", channel.InLevel);
//                                rsParams.Put("Conversion", channel.Conversion);
//                                rsParams.Put("ConversionType", channel.ConversionType);
//                                rsParams.Put("ConvCoef0", channel.ConvCoef0);
//                                rsParams.Put("ConvCoef1", channel.ConvCoef1);
//                                rsParams.Put("ConvCoef2", channel.ConvCoef2);
//                                rsParams.Put("ConvCoef3", channel.ConvCoef3);
//                                rsParams.Put("ConvCoef4", channel.ConvCoef4);
//                                rsParams.Put("ConvCoef5", channel.ConvCoef5);

//                                rsParams.Put("Active", true);
//                                rsParams.Put("TimeAdd", DateTime.Now);
                                
//                                channel.ArchiveId = rsParams.GetInt("ParamId");
//                                rsParams.Update();
//                            }
//                }
                
//                exception = null;
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }
        
//        public bool WriteValue(int paramId, DateTime? time, string signal, double? value, byte status, out Exception exception)
//        {
//            try
//            {
//                _rsValue.AddNew();
//                _rsValue.Put("ParamId", paramId);
//                _rsValue.Put("Time", time);
//                _rsValue.Put("Signal", signal);
//                _rsValue.Put("Value", value);
//                _rsValue.Put("Status", status);
//                _rsValue.Update();

//                exception = null;
//                return true;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public bool WriteValue(ClassAbstractAdamChannel channel, out Exception exception)
//        {
//            try
//            {
//                if (channel.ArchiveId != null)
//                {
//                    _rsValue.AddNew();
//                    _rsValue.Put("ParamId", channel.ArchiveId);
//                    _rsValue.Put("Time", channel.Time);
//                    _rsValue.Put("Signal", channel.Signal);
//                    _rsValue.Put("Value", channel.Value);

//                    byte? status;
//                    if (channel.Status != null)
//                        status = Convert.ToByte(channel.Status);
//                    else
//                        status = null;
                    
//                    _rsValue.Put("Status", status);
//                    _rsValue.Update();

//                    exception = null;
//                    return true;
//                }

//                exception = null;
//                return false;
//            }
//            catch (Exception e)
//            {
//                exception = e;
//                return false;
//            }
//        }

//        public bool WriteValue(ClassChannelValue channelValue, out Exception exception)
//        {
//            return WriteValue(channelValue.Channel, out exception);
//        }

//        public void WriteStopValues(out Exception exception)
//        {
//            exception = null;
            
//            try
//            {
//                _daoDb.Execute(
//                    "INSERT INTO [Values] ( ParamId, [Time], Signal, [Value], Status ) " +
//                    "SELECT Values.ParamId, Now(), Null, Null, 1 " +
//                    "FROM [Values] INNER JOIN " +
//                    "(SELECT Values.ParamId, Last(Values.Time) AS [Last-Time] FROM [Values] GROUP BY Values.ParamId) AS A " +
//                    "ON (Values.Time = A.[Last-Time]) AND (Values.ParamId = A.ParamId) " +
//                    "WHERE (Values.Status<>1) " +
//                    "ORDER BY Values.ParamId;");
//            }
//            catch (Exception e)
//            {
//                exception = e;
//            }
//        }

//        public static DataRowCollection ReadFromArchive(string archiveFileName, string codes, DateTime beginTime, DateTime endTime, 
//                                                        out Exception exception)
//        {
//            if (File.Exists(archiveFileName))
//            {
//                try
//                {
//                    var accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + archiveFileName);
//                    var accDataSet = new DataSet();
                    
//                    var culture = new System.Globalization.CultureInfo("");
//                    var codeArr = codes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//                    for (int i = 0; i < codeArr.Length; i++) codeArr[i] = "'" + codeArr[i].Trim() + "'";
//                    var accAdapter = new OleDbDataAdapter(@"SELECT [Params].[ParamId], [Params].[Module] AS [Address], [Params].[Channel], [Params].[Code], 
//                                                            [Values].[Signal] AS [SignalValue], [Values].[Value] AS [TagValue], [Values].[Status] AS [SignalStatus], [Values].[Time] AS [TagTime]
//                                                            FROM Params INNER JOIN Values ON [Params].[ParamId] = [Values].[ParamId]
//                                                            WHERE ([Code] IN (" + string.Join(", ", codeArr) + ")) " +
//                                                           "AND ([Time] >= #" + Convert.ToString(beginTime, culture) + "#) " +
//                                                           "AND ([Time] <= #" + Convert.ToString(endTime, culture) + "#) " +
//                                                           "ORDER BY [Code], [Time];",
//                                                          accConnection);
//                    //accBuilder = new OleDbCommandBuilder(accAdapter);
//                    accAdapter.Fill(accDataSet, "Archive");
                    
//                    accConnection.Close();

//                    exception = null;
//                    return accDataSet.Tables["Archive"].Rows;
//                }
//                catch(Exception e)
//                {
//                    exception = e;
//                    return null;
//                }
//            }
//            else
//            {
//                exception = new Exception(@"Ошибка открытия Базы Данных:" + "\n" + @"Недопустимый файл архива");
//                return null;
//            }
//        }
//    }

    public class NewAuditArchive
    {
        private DaoDb _daoDb;
        //private RecDao _rsObjects;
        //private RecDao _rsSignals;
        private RecDao _rsIntervals;
        private RecDao _rsValue;
        private bool _isConnected;
        private DicS<int> _channelIds;

        public static bool TryConnect(string fileName, out int valueRecordCount, out Exception exception)
        {
            valueRecordCount = 0;
            
            if (DaoDb.Check(fileName, new[] { "Objects", "Signals", "Intervals", "MomentsValues" }))
            {
                try
                {
                    using (var daoDb = new DaoDb(fileName))
                    {
                        byte step = 0;

                        try
                        {
                            step = 1;
                            string stSql = @"SELECT [ObjectId], [TypeObject], [CodeObject], [NameObject], [TagObject], [SysField], [Otm], [NumObject], [Comment], [CommName], [ErrMess],
                                                    [Prop0], [Prop1], [Prop2], [Prop3], [Prop4], [Prop5], [Prop6] , [Prop7], [Prop8], [Prop9] 
                                             FROM Objects;";
                            using (new RecDao(daoDb, stSql)) { }

                            step = 2;
                            stSql = @"SELECT [SignalId], [ObjectId], [Otm], [CodeSignal], [NameSignal], [FullCode], [Default], [DataType], [Units], [Min], [Max],
                                             [ConstValue], [Active], [TagSignal], [Inf], [SysField], [NumSignal], [CommentSignal] , [ErrMess], [SourceName], [ReceiverName] 
                                      FROM Signals;";
                            using (new RecDao(daoDb, stSql)) { }

                            step = 3;
                            stSql = @"SELECT [IntervalId], [TimeBegin], [TimeEnd], [IntervalName], [SysField] 
                                      FROM Intervals;";
                            using (new RecDao(daoDb, stSql)) { }
                            
                            step = 4;
                            stSql = @"SELECT [SignalId], [IntervalId], [Value], [Time], [Nd] 
                                      FROM MomentsValues;";
                            using (var rsValue = new RecDao(daoDb, stSql))
                            {
                                valueRecordCount = rsValue.RecordCount;
                            }
                            
                            exception = null;
                            return true;
                        }
                        catch (Exception e)
                        {
                            string strMessage = "Ошибка доступа к архиву " + fileName;

                            switch (step)
                            {
                                case 1:
                                    strMessage += ": Неверная структура таблицы Objects";
                                    break;
                                case 2:
                                    strMessage += ": Неверная структура таблицы Signals";
                                    break;
                                case 3:
                                    strMessage += ": Неверная структура таблицы Intervals";
                                    break;
                                case 4:
                                    strMessage += ": Неверная структура таблицы MomentsValues";
                                    break;
                                default:
                                    strMessage += ": " + e.Message;
                                    break;
                            }

                            exception = new Exception(strMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    exception = new Exception("Ошибка открытия Базы Данных (" + e.GetType() + @")" + ": " + e.Message);
                }
            }
            else
            {
                exception = new Exception("Ошибка открытия Базы Данных: Недопустимый файл архива");
            }

            return false;
        }

        public bool Connect(string fileName, out Exception exception)
        {
            if (DaoDb.Check(fileName, new[] {"Objects", "Signals", "Intervals", "MomentsValues"}))
            {
                try
                {
                    _daoDb = new DaoDb(fileName);

                    byte step = 0;

                    try
                    {
                        step = 1;
                        string stSql =
                            @"SELECT [ObjectId], [TypeObject], [CodeObject], [NameObject], [TagObject], [SysField], [Otm], [NumObject], [Comment], [CommName], [ErrMess],
                                     [Prop0], [Prop1], [Prop2], [Prop3], [Prop4], [Prop5], [Prop6] , [Prop7], [Prop8], [Prop9] 
                              FROM Objects;";
                        using (new RecDao(_daoDb, stSql))
                        {
                        }

                        step = 2;
                        stSql =
                            @"SELECT [SignalId], [ObjectId], [Otm], [CodeSignal], [NameSignal], [FullCode], [Default], [DataType], [Units], [Min], [Max],
                                     [ConstValue], [Active], [TagSignal], [Inf], [SysField], [NumSignal], [CommentSignal] , [ErrMess], [SourceName], [ReceiverName] 
                              FROM Signals;";
                        using (new RecDao(_daoDb, stSql))
                        {
                        }

                        step = 3;
                        stSql = @"SELECT [IntervalId], [TimeBegin], [TimeEnd], [IntervalName], [SysField] 
                                  FROM Intervals;";
                        //using (_rsIntervals = new RecDao(_daoDb, stSql)){}
                        _rsIntervals = new RecDao(_daoDb, stSql);

                        step = 4;
                        stSql = @"SELECT [SignalId], [IntervalId], [Value], [Time], [Nd] 
                                  FROM MomentsValues;";
                        //using (_rsValue = new RecDao(_daoDb, stSql)){}
                        _rsValue = new RecDao(_daoDb, stSql);

                        _isConnected = true;

                        exception = null;
                        return true;
                    }
                    catch (Exception e)
                    {
                        string strMessage = "Ошибка доступа к архиву " + fileName;

                        switch (step)
                        {
                            case 1:
                                strMessage += ": Неверная структура таблицы Objects";
                                break;
                            case 2:
                                strMessage += ": Неверная структура таблицы Signals";
                                break;
                            case 3:
                                strMessage += ": Неверная структура таблицы Intervals";
                                break;
                            case 4:
                                strMessage += ": Неверная структура таблицы MomentsValues";
                                break;
                            default:
                                strMessage += ": " + e.Message;
                                break;
                        }

                        exception = new Exception(strMessage);
                    }

                }
                catch (Exception e)
                {
                    exception = new Exception("Ошибка открытия Базы Данных (" + e.GetType() + @")" + ": " + e.Message);
                }
            }
            else
            {
                exception = new Exception("Ошибка открытия Базы Данных: Недопустимый файл архива");
            }

            return false;
        }

        public void Disconnect()
        {
            if (_isConnected)
            {
                _rsIntervals.Dispose();
                _rsValue.Dispose();
                _daoDb.Dispose();

                _channelIds = null;

                _isConnected = false;

                DaoDb.CloseEngine();
            }
        }

        public int ValuesRecordCount
        {
            get { return _rsValue.RecordCount; }
        }

        public bool UpdateParams(NewNetModules modules, out Exception exception)
        {
            if (!_isConnected)
            {
                exception = new Exception("Связь с БД не установлена");
                return false;
            }

            try
            {
                _daoDb.Execute("UPDATE Signals SET [Active] = False");

                string stSql =
                    @"SELECT [ObjectId], [TypeObject], [CodeObject], [NameObject], [TagObject], [SysField], [Otm], [NumObject], [Comment], [CommName], [ErrMess],
                                        [Prop0], [Prop1], [Prop2], [Prop3], [Prop4], [Prop5], [Prop6] , [Prop7], [Prop8], [Prop9] 
                                 FROM Objects
                                 ORDER BY [ObjectId];";

                using (var rsObjects = new RecDao(_daoDb, stSql))
                {
                    stSql =
                        @"SELECT [ObjectId], [SignalId], [Otm], [CodeSignal], [NameSignal], [FullCode], [Default], [DataType], [Units], [Min], [Max],
                                     [ConstValue], [Active], [TagSignal], [Inf], Signals.[SysField], [NumSignal], [CommentSignal] , Signals.[ErrMess], [SourceName], [ReceiverName]
                              FROM Signals
                              ORDER BY [ObjectId], [CodeSignal];";

                    using (var rsSignals = new RecDao(_daoDb, stSql))
                    {
                        var dicChannels = new DicS<NewChannelAbstract>();
                        var channelUsed = new HashSet<string>();
                        var disNoSignalPar = new DicS<int>();

                        _channelIds = new DicS<int>();

                        foreach (NewModuleAbstract module in modules)
                            foreach (NewChannelAbstract channel in module.Channels)
                                dicChannels.Add(channel.Code, channel);

                        while (rsObjects.Read())
                        {
                            var channel = dicChannels[rsObjects.GetString("CodeObject")];
                            if (channel != null)
                            {
                                bool upd = false;

                                //ObjectId
                                if (rsObjects.GetString("TypeObject") != "AI")
                                {
                                    rsObjects.Put("TypeObject", "AI");
                                    upd = true;
                                }
                                //CodeObject
                                if (rsObjects.GetString("NameObject") != channel.Name)
                                {
                                    rsObjects.Put("NameObject", channel.Name);
                                    upd = true;
                                }
                                //TagObject
                                //SysField
                                //Otm
                                //NumObject
                                //Comment
                                //CommName
                                //ErrMess
                                if (rsObjects.GetString("Prop0") != channel.Module.Address)
                                {
                                    rsObjects.Put("Prop0", channel.Module.Address);
                                    upd = true;
                                }
                                if (rsObjects.GetInt("Prop1") != channel.Channel)
                                {
                                    rsObjects.Put("Prop1", channel.Channel);
                                    upd = true;
                                }
                                if (rsObjects.GetDoubleNull("Prop2") != channel.Aperture)
                                {
                                    rsObjects.Put("Prop2", channel.Aperture);
                                    upd = true;
                                }

                                switch (channel.ChannelType)
                                {
                                    case "Adam4019+":
                                        if (rsObjects.GetString("Prop3") !=
                                            ((NewChannelAdam4019Plus) channel).DataFormat)
                                        {
                                            rsObjects.Put("Prop3", ((NewChannelAdam4019Plus) channel).DataFormat);
                                            upd = true;
                                        }
                                        if (rsObjects.GetString("Prop4") !=
                                            ((NewChannelAdam4019Plus) channel).ChannelRange)
                                        {
                                            rsObjects.Put("Prop4", ((NewChannelAdam4019Plus) channel).ChannelRange);
                                            upd = true;
                                        }
                                        break;

                                    default:
                                        if (rsObjects.GetString("Prop3") != null)
                                        {
                                            rsObjects.Put("Prop3", (string) null);
                                            upd = true;
                                        }
                                        if (rsObjects.GetString("Prop4") != null)
                                        {
                                            rsObjects.Put("Prop4", (string) null);
                                            upd = true;
                                        }
                                        break;
                                }


                                if (rsObjects.GetString("Prop5") != channel.InLevel)
                                {
                                    rsObjects.Put("Prop5", channel.InLevel);
                                    upd = true;
                                }
                                if (rsObjects.GetString("Prop6") != channel.Conversion)
                                {
                                    rsObjects.Put("Prop6", channel.Conversion);
                                    upd = true;
                                }
                                if (rsObjects.GetInt("Prop7") != channel.ConversionType)
                                {
                                    rsObjects.Put("Prop7", channel.ConversionType);
                                    upd = true;
                                }
                                string prop8 = channel.ConvCoef0 + ";" + channel.ConvCoef1 + ";" + channel.ConvCoef2 +
                                               ";" + channel.ConvCoef3 + ";" + channel.ConvCoef4 + ";" +
                                               channel.ConvCoef5;
                                if (rsObjects.GetString("Prop8") != prop8)
                                {
                                    rsObjects.Put("Prop8", prop8);
                                    upd = true;
                                }
                                if (upd) rsObjects.Update();

                                while ((!rsSignals.EOF) && (rsSignals.GetInt("ObjectId") < rsObjects.GetInt("ObjectId")))
                                    rsSignals.MoveNext();

                                bool fgPar = false;

                                while ((!rsSignals.EOF) &&
                                       (rsSignals.GetInt("ObjectId") == rsObjects.GetInt("ObjectId")))
                                {
                                    if (rsSignals.GetString("CodeSignal") == "Пар")
                                    {
                                        fgPar = true;
                                        upd = false;

                                        //SignalId
                                        //ObjectId
                                        //Otm
                                        //CodeSignal
                                        if (rsSignals.GetString("NameSignal") != "Параметр")
                                        {
                                            rsSignals.Put("NameSignal", "Параметр");
                                            upd = true;
                                        }
                                        var fullCode = rsObjects.GetString("CodeObject") + ".Пар";
                                        if (rsSignals.GetString("FullCode") != fullCode)
                                        {
                                            rsSignals.Put("FullCode", fullCode);
                                            upd = true;
                                        }
                                        if (rsSignals.GetBool("Default") != true)
                                        {
                                            rsSignals.Put("Default", true);
                                            upd = true;
                                        }
                                        if (rsSignals.GetString("DataType") != "действ")
                                        {
                                            rsSignals.Put("DataType", "действ");
                                            upd = true;
                                        }
                                        if (rsSignals.GetString("Units") != channel.Units)
                                        {
                                            rsSignals.Put("Units", channel.Units);
                                            upd = true;
                                        }
                                        if (rsSignals.GetDoubleNull("Min") != channel.Min)
                                        {
                                            rsSignals.Put("Min", channel.Min);
                                            upd = true;
                                        }
                                        if (rsSignals.GetDoubleNull("Max") != channel.Max)
                                        {
                                            rsSignals.Put("Max", channel.Max);
                                            upd = true;
                                        }
                                        //ConstValue
                                        if (rsSignals.GetBool("Active") != true)
                                        {
                                            rsSignals.Put("Active", true);
                                            upd = true;
                                        } //всегда верно
                                        //TagSignal
                                        //Inf
                                        //SysField
                                        //NumSignal
                                        //CommentSignal
                                        //ErrMess
                                        //SourceName
                                        //ReceiverName
                                        if (upd) rsSignals.Update();

                                        _channelIds.Add(channel.Code, rsSignals.GetInt("SignalId"));
                                    }
                                    else
                                    {
                                        upd = false;

                                        //SignalId
                                        //ObjectId
                                        //Otm
                                        //CodeSignal
                                        //NameSignal
                                        //CodeObject
                                        //FullCode
                                        //Default
                                        //DataType
                                        //Units
                                        //Min
                                        //Max
                                        //ConstValue
                                        if (rsSignals.GetBool("Active"))
                                        {
                                            rsSignals.Put("Active", false);
                                            upd = true;
                                        } //всегда верно
                                        //TagSignal
                                        //Inf
                                        //SysField
                                        //NumSignla
                                        //CommentSignal
                                        //ErrMess
                                        //SourceName
                                        //ReceiverName
                                        if (upd) rsSignals.Update();
                                    }

                                    rsSignals.MoveNext();
                                }

                                if (!fgPar)
                                    disNoSignalPar.Add(rsObjects.GetString("CodeObject"), rsObjects.GetInt("ObjectId"));

                                channelUsed.Add(rsObjects.GetString("CodeObject"));
                            }
                        }

                        foreach (var channel in dicChannels.Values)
                        {
                            int objectId = 0;

                            if (!channelUsed.Contains(channel.Code))
                            {
                                rsObjects.AddNew();
                                //ObjectId
                                rsObjects.Put("TypeObject", "AI");
                                rsObjects.Put("CodeObject", channel.Code);
                                rsObjects.Put("NameObject", channel.Name);
                                //TagObject
                                //SysField
                                //Otm
                                //NumObject
                                //Comment
                                //CommName
                                //ErrMess
                                rsObjects.Put("Prop0", channel.Module.Address);
                                rsObjects.Put("Prop1", channel.Channel);
                                rsObjects.Put("Prop2", channel.Aperture);

                                switch (channel.ChannelType)
                                {
                                    case "Adam4019+":
                                        rsObjects.Put("Prop3", ((NewChannelAdam4019Plus) channel).DataFormat);
                                        rsObjects.Put("Prop4", ((NewChannelAdam4019Plus) channel).ChannelRange);
                                        break;
                                    default:
                                        rsObjects.Put("Prop3", (string) null);
                                        rsObjects.Put("Prop4", (string) null);
                                        break;
                                }

                                rsObjects.Put("Prop5", channel.InLevel);
                                rsObjects.Put("Prop6", channel.Conversion);
                                rsObjects.Put("Prop7", channel.ConversionType);
                                rsObjects.Put("Prop8",
                                              channel.ConvCoef0 + ";" + channel.ConvCoef1 + ";" + channel.ConvCoef2 +
                                              ";" + channel.ConvCoef3 + ";" + channel.ConvCoef4 + ";" +
                                              channel.ConvCoef5);

                                objectId = rsObjects.GetInt("ObjectId");
                                rsObjects.Update();
                            }

                            if ((!channelUsed.Contains(channel.Code)) || (disNoSignalPar.ContainsKey(channel.Code)))
                            {
                                if (disNoSignalPar.ContainsKey(channel.Code)) objectId = disNoSignalPar[channel.Code];

                                rsSignals.AddNew();
                                //SignalId
                                rsSignals.Put("ObjectId", objectId);
                                //Otm
                                rsSignals.Put("CodeSignal", "Пар");
                                rsSignals.Put("NameSignal", "Параметр");
                                rsSignals.Put("FullCode", channel.Code + ".Пар");
                                rsSignals.Put("Default", true);
                                rsSignals.Put("DataType", "действ");
                                rsSignals.Put("Units", channel.Units);
                                rsSignals.Put("Min", channel.Min);
                                rsSignals.Put("Max", channel.Max);
                                //ConstValue
                                rsSignals.Put("Active", true);
                                //TagSignal
                                //Inf
                                //SysField
                                //NumSignal
                                //CommentSignal
                                //ErrMess
                                //SourceName
                                //ReceiverName
                                int signalId = rsSignals.GetInt("SignalId");
                                rsSignals.Update();

                                _channelIds.Add(channel.Code, signalId);
                            }
                        }
                    }
                }

                exception = null;
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public int BeginInterval(DateTime timeBegin, string intervalName, out Exception exception)
        {
            try
            {
                _rsIntervals.AddNew();
                _rsIntervals.Put("TimeBegin", timeBegin);
                _rsIntervals.Put("TimeEnd", timeBegin);
                _rsIntervals.Put("IntervalName", intervalName);
                int i = _rsIntervals.GetInt("IntervalId");
                _rsIntervals.Update();

                exception = null;
                return i;
            }
            catch (Exception e)
            {
                exception = e;
                return 0;
            }
        }

        public bool EndInterval(int intervalId, DateTime timeEnd, out Exception exception)
        {
            try
            {
                //_daoDb.Execute("UPDATE Intervals SET [TimeEnd] = " + timeEnd.ToAccessString() + " WHERE [IntervalId] = " + intervalId + ";");
                _rsIntervals.FindFirst("IntervalId", intervalId);
                _rsIntervals.Put("TimeEnd", timeEnd);
                _rsIntervals.Update();

                exception = null;
                return true;
            }
            catch (Exception e)
            {
                exception = e;
                return false;
            }
        }

        public bool WriteValue(string code, int intervalId, double? value, DateTime? time, byte status, out Exception exception)
        {
            if (_channelIds.ContainsKey(code))
            {
                var paramId = _channelIds[code];

                try
                {
                    _rsValue.AddNew();
                    _rsValue.Put("SignalId", paramId);
                    _rsValue.Put("IntervalId", intervalId);
                    _rsValue.Put("Value", value);
                    _rsValue.Put("Time", time);
                    //_rsValue.Put("Signal", signal);
                    _rsValue.Put("Nd", status);
                    _rsValue.Update();

                    exception = null;
                    return true;
                }
                catch (Exception e)
                {
                    exception = e;
                    return false;
                }
            }

            exception = new Exception("Неизвестный код");
            return false;
        }

        public void WriteEndIntervalValues(int intervalId, DateTime time, out Exception exception)
        {
            exception = null;

            try
            {
                //string stTime = "IIF([MomentsValues].[Time] < " + time.ToAccessString() + ", " + time.ToAccessString() +
                //                ", " + time.AddSeconds(1).ToAccessString() + ")";

                //string stTime = time.ToAccessString();

                //_daoDb.Execute(
                //    "INSERT INTO [MomentsValues] ( [SignalId], [IntervalId], [Value], [Time], [Nd] ) " +
                //    "SELECT [MomentsValues].[SignalId], [MomentsValues].[IntervalId], Null, " + stTime + ", 1 " +
                //    "FROM [MomentsValues] INNER JOIN " + "(SELECT [SignalId], [IntervalId], Max([Time]) AS [LastTime] " +
                //    "FROM [MomentsValues] " +
                //    "WHERE ([IntervalId] = " + intervalId + ") " +
                //    "GROUP BY [SignalId], [IntervalId]) AS qA " +
                //    "ON ([MomentsValues].[SignalId] = [qA].[SignalId]) AND ([MomentsValues].[IntervalId] = [qA].[IntervalId]) AND ([MomentsValues].[Time] = [qA].[LastTime]) " +
                //    "WHERE ([MomentsValues].[Nd] <> 1) " +
                //    "ORDER BY [MomentsValues].[SignalId];");

                string stSql = "SELECT [MomentsValues].[SignalId], [MomentsValues].[IntervalId] " +
                               "FROM [MomentsValues] INNER JOIN " +
                               "(SELECT [SignalId], [IntervalId], Max([Time]) AS [LastTime] " +
                               "FROM [MomentsValues] " +
                               "WHERE ([IntervalId] = " + intervalId + ") " +
                               "GROUP BY [SignalId], [IntervalId]) AS qA " +
                               "ON ([MomentsValues].[SignalId] = [qA].[SignalId]) AND ([MomentsValues].[IntervalId] = [qA].[IntervalId]) AND ([MomentsValues].[Time] = [qA].[LastTime]) " +
                               "WHERE ([MomentsValues].[Nd] <> 1) " +
                               "ORDER BY [MomentsValues].[SignalId];";

                using (RecDao rsEndValues = new RecDao(_daoDb, stSql))
                {
                    while(rsEndValues.Read())
                    {
                        _rsValue.AddNew();
                        _rsValue.Put("SignalId", rsEndValues.GetInt("SignalId"));
                        _rsValue.Put("IntervalId", rsEndValues.GetInt("IntervalId"));
                        _rsValue.Put("Value", (double?) null);
                        _rsValue.Put("Time", time);
                        _rsValue.Put("Nd", 1);
                        _rsValue.Update();
                    }
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
        }

    #region Static
//        public static DataRowCollection ReadFromArchive(string archiveFileName, string codes, DateTime beginTime, DateTime endTime, out Exception exception)
//        {
//            if (File.Exists(archiveFileName))
//            {
//                try
//                {
//                    var accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + archiveFileName);
//                    var accDataSet = new DataSet();

//                    var culture = new System.Globalization.CultureInfo("");
//                    var codeArr = codes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
//                    for (int i = 0; i < codeArr.Length; i++) codeArr[i] = "'" + codeArr[i].Trim() + "'";
//                    var accAdapter = new OleDbDataAdapter(@"SELECT [Signals].[SignalId], [Objects].[Prop0] AS [Address], [Objects].[Prop1] AS [Channel], [Objects].[CodeObject] AS [Code], 
//                                                                   [MomentsValues].[Value], [MomentsValues].[Nd] AS [Status], [MomentsValues].[Time]
//                                                            FROM ([Objects] INNER JOIN [Signals] ON [Objects].[ObjectId] = [Signals].[ObjectId]) 
//                                                                            INNER JOIN [MomentsValues] ON [Signals].[SignalId] = [MomentsValues].[SignalId]
//                                                            WHERE ([Signals].[CodeSignal] = 'Пар') AND ([Objects].[CodeObject] IN (" + string.Join(", ", codeArr) + ")) " +
//                                                           "AND ([Time] >= #" + Convert.ToString(beginTime, culture) + "#) " +
//                                                           "AND ([Time] <= #" + Convert.ToString(endTime, culture) + "#) " +
//                                                           "ORDER BY [Objects].[CodeObject], [MomentsValues].[Time];",
//                                                          accConnection);
//                    //accBuilder = new OleDbCommandBuilder(accAdapter);
//                    accAdapter.Fill(accDataSet, "Archive");

//                    accConnection.Close();

//                    exception = null;
//                    return accDataSet.Tables["Archive"].Rows;
//                }
//                catch (Exception e)
//                {
//                    exception = e;
//                    return null;
//                }
//            }

//            exception = new Exception(@"Ошибка открытия Базы Данных:" + "\n" + @"Недопустимый файл архива");
//            return null;
//        }

        public static RecDao ReadFromArchive(string archiveFileName, string codes, DateTime beginTime, DateTime endTime, out Exception exception)
        {
            if (File.Exists(archiveFileName))
            {
                try
                {
                    var daoDb = new DaoDb(archiveFileName);
                    var culture = new System.Globalization.CultureInfo("");
                    var codeArr = codes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < codeArr.Length; i++) codeArr[i] = "'" + codeArr[i].Trim() + "'";
                    
                    string stSql = @"SELECT [Signals].[SignalId], [Objects].[Prop0] AS [Address], [Objects].[Prop1] AS [Channel], [Objects].[CodeObject] AS [Code], 
                                            [MomentsValues].[Value], [MomentsValues].[Nd] AS [Status], [MomentsValues].[Time]
                                    FROM ([Objects] INNER JOIN [Signals] ON [Objects].[ObjectId] = [Signals].[ObjectId]) 
                                                    INNER JOIN [MomentsValues] ON [Signals].[SignalId] = [MomentsValues].[SignalId]
                                    WHERE ([Signals].[CodeSignal] = 'Пар') AND ([Objects].[CodeObject] IN (" + string.Join(", ", codeArr) + ")) " +
                                   "AND ([Time] >= #" + Convert.ToString(beginTime, culture) + "#) AND ([Time] <= #" + Convert.ToString(endTime, culture) + "#) " +
                                   "ORDER BY [Objects].[CodeObject], [MomentsValues].[Time];";

                    exception = null;
                    return new RecDao(daoDb, stSql);

                }
                catch (Exception e)
                {
                    exception = e;
                    return null;
                }
            }

            exception = new Exception(@"Ошибка открытия Базы Данных:" + "\n" + @"Недопустимый файл архива");
            return null;
        }
    #endregion
    }
}
