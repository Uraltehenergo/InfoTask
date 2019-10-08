using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb; 
using System.Windows.Forms;

namespace AuditMonitor
{
//    static class ClassTagListTable
//    {
//        private const string FileName = "TagList.mdb";
//        private static OleDbConnection _accConnection;
//        private static DataSet _accDataSet;
//        private static OleDbDataAdapter _accAdapter;
//        private static OleDbCommandBuilder _accBuilder;

//        public static string DbString(object val)
//        {
//            return (DBNull.Value.Equals(val)) ? null : (string) val;
//        }

//        public static byte? DbByte(object val)
//        {
//            return (DBNull.Value.Equals(val)) ? null : (byte?)val;
//        }

//        public static int? DbInt(object val)
//        {
//            return (DBNull.Value.Equals(val)) ? null : (int?) val;
//        }

//        public static double? DbDouble(object val)
//        {
//            return (DBNull.Value.Equals(val)) ? null : (double?) val;
//        }

//        public static DateTime? DbDateTime(object val)
//        {
//            return (DBNull.Value.Equals(val)) ? null : (DateTime?)val;
//        }

//        //public static byte LoadChannelConfiguration(ClassAdamNet net, byte loadMode = (byte) 0)
//        {
//            //LoadMode = 0 - если канала нет в ObjetsPTK, очищает ввсе поля
//            //         = 1 - если канала нет в ObjetsPTK, оставляет все как было

//            string tagListFileName = Application.StartupPath + @"\" + FileName;

//            if (File.Exists(tagListFileName))
//            {
//                try
//                {
//                    _accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
//                    _accDataSet = new DataSet();

//                    _accAdapter = new OleDbDataAdapter(@"SELECT [SbCabinet], [SbBlockNum], [KKS], [Name], 
//                                                       [DELTA_VALUE], [Min], [max], [Units], [FINALELEM], [In_Level], [Conversion]
//                                                       FROM ObjectsPTK WHERE (SbCabinet IS NOT NULL) AND (SbBlockNum IS NOT NULL) 
//                                                       ORDER BY SbCabinet, SbBlockNum",
//                                                       _accConnection);
//                    _accBuilder = new OleDbCommandBuilder(_accAdapter);
//                    _accAdapter.Fill(_accDataSet, "TagList");
//                }
//                catch (OleDbException e)
//                {
//                    if (_accConnection != null) _accConnection.Close();

//                    if(e.ErrorCode == -2147467259)
//                    {
//                        MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                        return 1;
//                    }
                    
//                    string strMessage = "Неверная структура файла архива " + tagListFileName;
//                    switch (e.ErrorCode)
//                    {
//                        case -2147217865:
//                            strMessage += "\nНе найдена таблица ObjectsPTK";
//                            break;
//                        case -2147217904:
//                            strMessage += "\nНеверная структура таблицы ObjectsPTK";
//                            break;
//                        default:
//                            strMessage = e.ErrorCode + "\n" + e.Message;
//                            break;
//                    }
                        
//                    MessageBox.Show(strMessage);
//                    return 2;
//                }
//                catch (Exception e)
//                {
//                    _accConnection.Close();
//                    MessageBox.Show(e.GetType() + "\n" + e.Message);
//                    return 255;
//                }
//            }
//            else
//            {
//                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                return 1;
//            }
            
//            int i = 0;
//            DataRow row = (_accDataSet.Tables["TagList"].Rows.Count > i) ? _accDataSet.Tables["TagList"].Rows[i] : null;

//            foreach(ClassAbstractAdamModule module in net.Modules)
//            {
//                foreach(ClassAbstractAdamChannel channel in module.Channel)
//                {
//                    while ((row != null) && ((module.Address.CompareTo((string) row["SbCabinet"]) > 0) || ((module.Address.CompareTo((string) row["SbCabinet"]) == 0) && (channel.Channel > (int) row["SbBlockNum"]))))
//                        row = (_accDataSet.Tables["TagList"].Rows.Count > ++i) ? _accDataSet.Tables["TagList"].Rows[i] : null;
                    
//                    if ((row != null) && ((module.Address == (string) row["SbCabinet"]) && (channel.Channel == (int) row["SbBlockNum"]))) 
//                    {
//                        channel.Code = DbString(row["KKS"]);
//                        channel.Name = DbString(row["Name"]);
                            
//                        double val;
                        
//                        if (! DBNull.Value.Equals(row["DELTA_VALUE"]))
//                            if (double.TryParse(DbString(row["DELTA_VALUE"]).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out val))
//                                channel.Aperture = val;
//                            else
//                                channel.Aperture = null;
//                        else
//                            channel.Aperture = null;

//                        if (! DBNull.Value.Equals(row["Min"]))
//                            if (double.TryParse(DbString(row["Min"]).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out val))
//                                channel.Min = val;
//                            else
//                                channel.Min = null;
//                        else
//                            channel.Aperture = null;

//                        if (! DBNull.Value.Equals(row["Max"]))
//                            if (double.TryParse(DbString(row["Max"]).Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out val))
//                                channel.Max = val;
//                            else
//                                channel.Max = null;
//                        else
//                            channel.Aperture = null;

//                        channel.Units = DbString(row["Units"]);
//                        channel.ChannelRange = DbString(row["FINALELEM"]);
//                        channel.InLevel = DbString(row["In_Level"]);
//                        channel.Conversion = DbString(row["Conversion"]);
//                    }
//                    else
//                    {
//                        channel.Code = "M" + channel.Module.Address + (!channel.IsCjcChannel ? "CH" + (channel.Channel + 1).ToString() : "CJC");
//                        channel.Name = null;
//                        channel.Aperture = null;
//                        channel.Min = null;
//                        channel.Max = null;
//                        channel.Units = null;
//                        channel.ChannelRange = null;
//                        channel.InLevel = null;
//                        channel.Conversion = null;
//                    }
//                }
//            }
            
//            _accConnection.Close();
//            return 0;
//        }

//    #region TagList.TagList
//        public static byte LoadNetFromTagList(ClassAdamNet net)
//        {
//            string tagListFileName = Application.StartupPath + @"\" + FileName;

//            if (File.Exists(tagListFileName))
//            {
//                try
//                {
//                    _accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
//                    _accDataSet = new DataSet();

//                    _accAdapter = new OleDbDataAdapter(@"SELECT [Selected], [TagId], [Code], [Name], [Module], [Channel], 
//                                                       [Aperture], [Min], [Max], [Units], [DataFormat], [ChannelRange], [In_Level], [Conversion] 
//                                                       FROM TagList WHERE ([Module] IS NOT NULL) AND ([Channel] IS NOT NULL) 
//                                                       ORDER BY [Module], [Channel]",
//                                                       _accConnection);

//                    _accBuilder = new OleDbCommandBuilder(_accAdapter);
//                    _accAdapter.Fill(_accDataSet, "TagList");
//                }
//                catch (OleDbException e)
//                {
//                    if (_accConnection != null) _accConnection.Close();

//                    if (e.ErrorCode == -2147467259)
//                    {
//                        MessageBox.Show(e.ErrorCode + "\n" + e.Message);

//                        //MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                        return 1;
//                    }

//                    string strMessage = "Неверная структура файла архива " + tagListFileName;
//                    switch (e.ErrorCode)
//                    {
//                        case -2147217865:
//                            strMessage += "\nНе найдена таблица TagLit";
//                            break;
//                        case -2147217904:
//                            strMessage += "\nНеверная структура таблицы TagList";
//                            break;
//                        default:
//                            strMessage = e.ErrorCode + "\n" + e.Message;
//                            break;
//                    }

//                    MessageBox.Show(strMessage);
//                    return 2;
//                }
//                catch (Exception e)
//                {
//                    _accConnection.Close();
//                    MessageBox.Show(e.GetType() + "\n" + e.Message);
//                    return 255;
//                }
//            }
//            else
//            {
//                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                return 1;
//            }

//            if (Program.FmMonitor != null)
//            {
//                Program.FmMonitor.ClearModulesTreeView();
//                Program.FmMonitor.ClearSignalListView();
//            }

//            if (_accDataSet.Tables["TagList"].Rows.Count > 0)
//            {
//                int i = 0;
//                DataRow row = _accDataSet.Tables["TagList"].Rows[i];
//                while (row != null)
//                {
//                    var moduleAddress = DbString(row["Module"]);
//                    var newModule = new ClassModuleAdam4019Plus(moduleAddress)
//                                        {
//                                            DataFormat = DbString(row["DataFormat"])
//                                        };


//                    while ((row != null) && (DbString(row["Module"]) == moduleAddress))
//                    {
//                        string chn = DbString(row["Channel"]);
//                        int intChannel;

//                        ClassAbstractAdamChannel channel = null;
//                        if (int.TryParse(chn, out intChannel)) channel = newModule.Channel[intChannel];
//                        else if (chn.ToUpper() == "CJC") channel = newModule.Cjc;

//                        if (channel != null)
//                        {
//                            channel.Selected = (bool)row["Selected"];
//                            channel.Code = DbString(row["Code"]);
//                            channel.Name = DbString(row["Name"]);
//                            channel.Aperture = DbDouble(row["Aperture"]);
//                            channel.Min = DbDouble(row["Min"]);
//                            channel.Max = DbDouble(row["Max"]);
//                            channel.Units = DbString(row["Units"]);
//                            channel.ChannelRange = DbString(row["ChannelRange"]);
//                            channel.InLevel = DbString(row["In_Level"]);
//                            channel.Conversion = DbString(row["Conversion"]);
//                        }

//                        row = (++i < _accDataSet.Tables["TagList"].Rows.Count)
//                                  ? _accDataSet.Tables["TagList"].Rows[i]
//                                  : null;
//                    }
//                    net.Modules.AddModule(moduleAddress, newModule);
//                }
//                Program.Net.ProgramChannels();
//                Program.Net.ReadModulesSettings();
//            }

//            _accConnection.Close();
//            return 0;
//        }

//        public static byte SaveNetToTagList(ClassAdamNet net)
//        {
//            string tagListFileName = Application.StartupPath + @"\" + FileName;
            
//            if (File.Exists(tagListFileName))
//            {
//                try
//                {
//                    _accConnection =
//                        new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
//                    _accDataSet = new DataSet();

//                    _accAdapter =
//                        new OleDbDataAdapter(@"SELECT [Selected], [TagId], [Code], [Name], [Module], [Channel], 
//                                             [Aperture], [Min], [Max], [Units], [DataFormat], [ChannelRange], [In_Level], [Conversion] 
//                                             FROM TagList",
//                            _accConnection);

//                    _accBuilder = new OleDbCommandBuilder(_accAdapter) {QuotePrefix = "[", QuoteSuffix = "]"};

//                    _accAdapter.Fill(_accDataSet, "TagList");
//                }
//                catch (OleDbException e)
//                {
//                    if (_accConnection != null) _accConnection.Close();

//                    if (e.ErrorCode == -2147467259)
//                    {
//                        MessageBox.Show(e.ErrorCode + "\n" + e.Message);
//                        //MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                        return 1;
//                    }

//                    string strMessage = "Неверная структура файла архива " + tagListFileName;
//                    switch (e.ErrorCode)
//                    {
//                        case -2147217865:
//                            strMessage += "\nНе найдена таблица TagLit";
//                            break;
//                        case -2147217904:
//                            strMessage += "\nНеверная структура таблицы TagList";
//                            break;
//                        default:
//                            strMessage = e.ErrorCode + "\n" + e.Message;
//                            break;
//                    }

//                    MessageBox.Show(strMessage);
//                    return 2;
//                }
//                catch (Exception e)
//                {
//                    _accConnection.Close();
//                    MessageBox.Show(e.GetType() + "\n" + e.Message);
//                    return 255;
//                }
//            }
//            else
//            {
//                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                return 1;
//            }

//            try
//            {
//                //int i;
//                //for (i = 0; i < _accDataSet.Tables["TagList"].Rows.Count; i++)
//                //{
//                //    _accDataSet.Tables["TagList"].Rows[i].Delete();
//                //}

//                var oleDbCommand = new OleDbCommand("DELETE * FROM TagList", _accConnection);
//                _accConnection.Open();
//                oleDbCommand.ExecuteNonQuery();

//                //_accDataSet.Tables["TagList"].Rows.Clear(); //так не работает
                
//                int i = 0;
//                foreach (ClassAbstractAdamModule module in Program.Net.Modules)
//                {
//                    foreach (ClassAbstractAdamChannel channel in module.Channel)
//                    {
//                        var newRow = _accDataSet.Tables["TagList"].NewRow();
//                        newRow["Selected"] = channel.Selected;
//                        newRow["TagId"] = ++i;
//                        if (channel.Code != null) newRow["Code"] = channel.Code; else newRow["Code"] = DBNull.Value;
//                        if (channel.Name != null) newRow["Name"] = channel.Name; else newRow["Name"] = DBNull.Value;
//                        newRow["Module"] = module.Address;
//                        newRow["Channel"] = channel.Channel;
//                        if (channel.Aperture != null) newRow["Aperture"] = channel.Aperture; else newRow["Aperture"] = DBNull.Value;
//                        if (channel.Min != null) newRow["Min"] = channel.Min; else newRow["Min"] = DBNull.Value;
//                        if (channel.Max != null) newRow["Max"] = channel.Max; else newRow["Max"] = DBNull.Value;
//                        if (channel.Units != null) newRow["Units"] = channel.Units; else newRow["Units"] = DBNull.Value;
//                        if (channel.DataFormat != null) newRow["DataFormat"] = channel.DataFormat; else newRow["DataFormat"] = DBNull.Value;
//                        if (channel.ChannelRange != null) newRow["ChannelRange"] = channel.ChannelRange; else newRow["ChannelRange"] = DBNull.Value;
//                        if (channel.InLevel != null) newRow["In_Level"] = channel.InLevel; else newRow["In_Level"] = DBNull.Value;
//                        if (channel.Conversion != null) newRow["Conversion"] = channel.Conversion; else newRow["Conversion"] = DBNull.Value;

//                        _accDataSet.Tables["TagList"].Rows.Add(newRow);
//                    }
//                }

//                _accAdapter.Update(_accDataSet, "TagList");
//                _accConnection.Close();
//                return 0;
//            }
//            catch(OleDbException e)
//            {
//                MessageBox.Show(e.ErrorCode + "\n" + e.Message);
//                return 3;
//            }
//        }
//    #endregion

//    #region NewNet
//        public static byte LoadNetFromTagList(NewNet net)
//        {
//            string tagListFileName = Application.StartupPath + @"\" + FileName;

//            if (File.Exists(tagListFileName))
//            {
//                try
//                {
//                    _accConnection = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
//                    _accDataSet = new DataSet();

//                    _accAdapter = new OleDbDataAdapter(@"SELECT [Selected], [TagId], [Code], [Name], [Module], [Channel], 
//                                                       [Aperture], [Min], [Max], [Units], [DataFormat], [ChannelRange], [In_Level], [Conversion] 
//                                                       FROM TagList WHERE ([Module] IS NOT NULL) AND ([Channel] IS NOT NULL) 
//                                                       ORDER BY [Module], [Channel]",
//                                                       _accConnection);

//                    _accBuilder = new OleDbCommandBuilder(_accAdapter);
//                    _accAdapter.Fill(_accDataSet, "TagList");
//                }
//                catch (OleDbException e)
//                {
//                    if (_accConnection != null) _accConnection.Close();

//                    if (e.ErrorCode == -2147467259)
//                    {
//                        MessageBox.Show(e.ErrorCode + "\n" + e.Message);

//                        //MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                        return 1;
//                    }

//                    string strMessage = "Неверная структура файла архива " + tagListFileName;
//                    switch (e.ErrorCode)
//                    {
//                        case -2147217865:
//                            strMessage += "\nНе найдена таблица TagLit";
//                            break;
//                        case -2147217904:
//                            strMessage += "\nНеверная структура таблицы TagList";
//                            break;
//                        default:
//                            strMessage = e.ErrorCode + "\n" + e.Message;
//                            break;
//                    }

//                    MessageBox.Show(strMessage);
//                    return 2;
//                }
//                catch (Exception e)
//                {
//                    _accConnection.Close();
//                    MessageBox.Show(e.GetType() + "\n" + e.Message);
//                    return 255;
//                }
//            }
//            else
//            {
//                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                return 1;
//            }

//            if (Program.FmMonitor != null)
//            {
//                Program.FmMonitor.ClearModulesTreeView();
//                Program.FmMonitor.ClearSignalListView();
//            }

//            if (_accDataSet.Tables["TagList"].Rows.Count > 0)
//            {
//                int i = 0;
//                DataRow row = _accDataSet.Tables["TagList"].Rows[i];
//                while (row != null)
//                {
//                    var moduleAddress = DbString(row["Module"]);
//                    var newModule = new NewModuleAdam4019Plus(net.Modules, moduleAddress)
//                    {
//                        DataFormat = DbString(row["DataFormat"])
//                    };


//                    while ((row != null) && (DbString(row["Module"]) == moduleAddress))
//                    {
//                        string chn = DbString(row["Channel"]);
//                        byte intChannel;

//                        NewChannelAbstract channel = null;
//                        if (byte.TryParse(chn, out intChannel)) channel = newModule.Channels[intChannel];
//                        else if (chn.ToUpper() == "CJC") channel = newModule.Cjc;

//                        if (channel != null)
//                        {
//                            channel.Selected = (bool)row["Selected"];
//                            channel.Code = DbString(row["Code"]);
//                            channel.Name = DbString(row["Name"]);
//                            channel.Aperture = DbDouble(row["Aperture"]);
//                            channel.Min = DbDouble(row["Min"]);
//                            channel.Max = DbDouble(row["Max"]);
//                            channel.Units = DbString(row["Units"]);
//                            ((NewChannelAdamAbstract)channel).ChannelRange = DbString(row["ChannelRange"]);
//                            channel.InLevel = DbString(row["In_Level"]);
//                            channel.Conversion = DbString(row["Conversion"]);
//                        }

//                        row = (++i < _accDataSet.Tables["TagList"].Rows.Count)
//                                  ? _accDataSet.Tables["TagList"].Rows[i]
//                                  : null;
//                    }
//                    net.Modules.AddModule(moduleAddress, newModule);
//                }
//                Program.Net.ProgramChannels();
//                Program.Net.ReadModulesSettings();
//            }

//            _accConnection.Close();
//            return 0;
//        }

//        public static byte SaveNetToTagList(NewNet net)
//        {
//            string tagListFileName = Application.StartupPath + @"\" + FileName;

//            if (File.Exists(tagListFileName))
//            {
//                try
//                {
//                    _accConnection =
//                        new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
//                    _accDataSet = new DataSet();

//                    _accAdapter =
//                        new OleDbDataAdapter(@"SELECT [Selected], [TagId], [Code], [Name], [Module], [Channel], 
//                                             [Aperture], [Min], [Max], [Units], [DataFormat], [ChannelRange], [In_Level], [Conversion] 
//                                             FROM TagList",
//                            _accConnection);

//                    _accBuilder = new OleDbCommandBuilder(_accAdapter) { QuotePrefix = "[", QuoteSuffix = "]" };

//                    _accAdapter.Fill(_accDataSet, "TagList");
//                }
//                catch (OleDbException e)
//                {
//                    if (_accConnection != null) _accConnection.Close();

//                    if (e.ErrorCode == -2147467259)
//                    {
//                        MessageBox.Show(e.ErrorCode + "\n" + e.Message);
//                        //MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                        return 1;
//                    }

//                    string strMessage = "Неверная структура файла архива " + tagListFileName;
//                    switch (e.ErrorCode)
//                    {
//                        case -2147217865:
//                            strMessage += "\nНе найдена таблица TagLit";
//                            break;
//                        case -2147217904:
//                            strMessage += "\nНеверная структура таблицы TagList";
//                            break;
//                        default:
//                            strMessage = e.ErrorCode + "\n" + e.Message;
//                            break;
//                    }

//                    MessageBox.Show(strMessage);
//                    return 2;
//                }
//                catch (Exception e)
//                {
//                    _accConnection.Close();
//                    MessageBox.Show(e.GetType() + "\n" + e.Message);
//                    return 255;
//                }
//            }
//            else
//            {
//                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
//                return 1;
//            }

//            try
//            {
//                //int i;
//                //for (i = 0; i < _accDataSet.Tables["TagList"].Rows.Count; i++)
//                //{
//                //    _accDataSet.Tables["TagList"].Rows[i].Delete();
//                //}

//                var oleDbCommand = new OleDbCommand("DELETE * FROM TagList", _accConnection);
//                _accConnection.Open();
//                oleDbCommand.ExecuteNonQuery();

//                //_accDataSet.Tables["TagList"].Rows.Clear(); //так не работает

//                int i = 0;
//                foreach (ClassAbstractAdamModule module in Program.Net.Modules)
//                {
//                    foreach (ClassAbstractAdamChannel channel in module.Channel)
//                    {
//                        var newRow = _accDataSet.Tables["TagList"].NewRow();
//                        newRow["Selected"] = channel.Selected;
//                        newRow["TagId"] = ++i;
//                        if (channel.Code != null) newRow["Code"] = channel.Code; else newRow["Code"] = DBNull.Value;
//                        if (channel.Name != null) newRow["Name"] = channel.Name; else newRow["Name"] = DBNull.Value;
//                        newRow["Module"] = module.Address;
//                        newRow["Channel"] = channel.Channel;
//                        if (channel.Aperture != null) newRow["Aperture"] = channel.Aperture; else newRow["Aperture"] = DBNull.Value;
//                        if (channel.Min != null) newRow["Min"] = channel.Min; else newRow["Min"] = DBNull.Value;
//                        if (channel.Max != null) newRow["Max"] = channel.Max; else newRow["Max"] = DBNull.Value;
//                        if (channel.Units != null) newRow["Units"] = channel.Units; else newRow["Units"] = DBNull.Value;
//                        if (channel.DataFormat != null) newRow["DataFormat"] = channel.DataFormat; else newRow["DataFormat"] = DBNull.Value;
//                        if (channel.ChannelRange != null) newRow["ChannelRange"] = channel.ChannelRange; else newRow["ChannelRange"] = DBNull.Value;
//                        if (channel.InLevel != null) newRow["In_Level"] = channel.InLevel; else newRow["In_Level"] = DBNull.Value;
//                        if (channel.Conversion != null) newRow["Conversion"] = channel.Conversion; else newRow["Conversion"] = DBNull.Value;

//                        _accDataSet.Tables["TagList"].Rows.Add(newRow);
//                    }
//                }

//                _accAdapter.Update(_accDataSet, "TagList");
//                _accConnection.Close();
//                return 0;
//            }
//            catch (OleDbException e)
//            {
//                MessageBox.Show(e.ErrorCode + "\n" + e.Message);
//                return 3;
//            }
//        }
//    #endregion
//    }

    internal static class NewTagList
    {
        private const string FileName = "TagList.mdb";
        private static OleDbConnection _accConnection;
        private static DataSet _accDataSet;
        private static OleDbDataAdapter _accAdapter;
        private static OleDbCommandBuilder _accBuilder;

    #region DBNull
        public static string DbString(object val)
        {
            return (DBNull.Value.Equals(val)) ? null : (string) val;            
        }

        public static byte? DbByte(object val)
        {
            return (DBNull.Value.Equals(val)) ? null : (byte?) val;
        }

        public static int? DbInt(object val)
        {
            return (DBNull.Value.Equals(val)) ? null : (int?) val;
        }

        public static double? DbDouble(object val)
        {
            return (DBNull.Value.Equals(val)) ? null : (double?) val;
        }

        public static DateTime? DbDateTime(object val)
        {
            return (DBNull.Value.Equals(val)) ? null : (DateTime?) val;
        }
    #endregion

    #region TagList
        public static byte LoadChannelConfiguration(NewNet net, byte loadMode = 0)
        {
            //LoadMode = 0 - если канала нет в ObjetsPTK, очищает ввсе поля
            //         = 1 - если канала нет в ObjetsPTK, оставляет все как было

            string tagListFileName = Application.StartupPath + @"\" + FileName;

            if (File.Exists(tagListFileName))
            {
                try
                {
                    _accConnection =
                        new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
                    _accDataSet = new DataSet();

                    _accAdapter =
                        new OleDbDataAdapter(
                            @"SELECT [SbCabinet], [SbBlockNum], [KKS], [Name], 
                                                       [DELTA_VALUE], [Min], [max], [Units], [FINALELEM], [In_Level], [Conversion]
                                                       FROM ObjectsPTK WHERE (SbCabinet IS NOT NULL) AND (SbBlockNum IS NOT NULL) 
                                                       ORDER BY SbCabinet, SbBlockNum",
                            _accConnection);
                    _accBuilder = new OleDbCommandBuilder(_accAdapter);
                    _accAdapter.Fill(_accDataSet, "TagList");
                }
                catch (OleDbException e)
                {
                    if (_accConnection != null) _accConnection.Close();

                    if (e.ErrorCode == -2147467259)
                    {
                        MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
                        return 1;
                    }

                    string strMessage = "Неверная структура файла архива " + tagListFileName;
                    switch (e.ErrorCode)
                    {
                        case -2147217865:
                            strMessage += "\nНе найдена таблица ObjectsPTK";
                            break;
                        case -2147217904:
                            strMessage += "\nНеверная структура таблицы ObjectsPTK";
                            break;
                        default:
                            strMessage = e.ErrorCode + "\n" + e.Message;
                            break;
                    }

                    MessageBox.Show(strMessage);
                    return 2;
                }
                catch (Exception e)
                {
                    _accConnection.Close();
                    MessageBox.Show(e.GetType() + "\n" + e.Message);
                    return 255;
                }
            }
            else
            {
                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
                return 1;
            }

            int i = 0;
            DataRow row = (_accDataSet.Tables["TagList"].Rows.Count > i) ? _accDataSet.Tables["TagList"].Rows[i] : null;

            foreach (NewModuleAbstract module in net.Modules)
                foreach (NewChannelAbstract channel in module.Channels)
                {
                    while ((row != null) &&
                           ((module.Address.CompareTo((string) row["SbCabinet"]) > 0) ||
                            ((module.Address.CompareTo((string) row["SbCabinet"]) == 0) &&
                             (channel.Channel > (int) row["SbBlockNum"]))))
                        row = (_accDataSet.Tables["TagList"].Rows.Count > ++i)
                                  ? _accDataSet.Tables["TagList"].Rows[i]
                                  : null;

                    if ((row != null) &&
                        ((module.Address == (string) row["SbCabinet"]) && (channel.Channel == (int) row["SbBlockNum"])))
                    {
                        channel.Code = DbString(row["KKS"]);
                        channel.Name = DbString(row["Name"]);

                        double val;

                        if (!DBNull.Value.Equals(row["DELTA_VALUE"]))
                            if (double.TryParse(DbString(row["DELTA_VALUE"]).Replace(",", "."), NumberStyles.Any,
                                                CultureInfo.InvariantCulture.NumberFormat, out val))
                                channel.Aperture = val;
                            else
                                channel.Aperture = null;
                        else
                            channel.Aperture = null;

                        if (!DBNull.Value.Equals(row["Min"]))
                            if (double.TryParse(DbString(row["Min"]).Replace(",", "."), NumberStyles.Any,
                                                CultureInfo.InvariantCulture.NumberFormat, out val))
                                channel.Min = val;
                            else
                                channel.Min = null;
                        else
                            channel.Aperture = null;

                        if (!DBNull.Value.Equals(row["Max"]))
                            if (double.TryParse(DbString(row["Max"]).Replace(",", "."), NumberStyles.Any,
                                                CultureInfo.InvariantCulture.NumberFormat, out val))
                                channel.Max = val;
                            else
                                channel.Max = null;
                        else
                            channel.Aperture = null;

                        channel.Units = DbString(row["Units"]);
                        channel.InLevel = DbString(row["In_Level"]);
                        channel.Conversion = DbString(row["Conversion"]);
                        
                        switch (channel.ChannelType)
                        {
                            case "Adam4019+":
                                ((NewChannelAdamAbstract)channel).ChannelRange = DbString(row["FINALELEM"]);
                                break;
                        }
                    }
                    else
                    {
                        channel.Code = "M" + channel.Module.Address +
                                       (!channel.IsCjc ? "CH" + (channel.Channel + 1).ToString() : "CJC");
                        channel.Name = null;
                        channel.Aperture = null;
                        channel.Min = null;
                        channel.Max = null;
                        channel.Units = null;
                        channel.InLevel = null;
                        channel.Conversion = null;

                        switch (channel.ChannelType)
                        {
                            case "Adam4019+":
                                ((NewChannelAdamAbstract)channel).ChannelRange = null;
                                break;
                        }
                    }
            }

            _accConnection.Close();
            return 0;
        }
    
        public static byte LoadNetFromTagList(NewNet net)
        {
            string tagListFileName = Application.StartupPath + @"\" + FileName;

            if (File.Exists(tagListFileName))
            {
                try
                {
                    _accConnection =
                        new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
                    _accDataSet = new DataSet();

                    _accAdapter =
                        new OleDbDataAdapter(
                            @"SELECT [Selected], [TagId], [Code], [Name], [Module], [Channel], 
                                                       [Aperture], [Min], [Max], [Units], [DataFormat], [ChannelRange], [In_Level], [Conversion] 
                                                       FROM TagList WHERE ([Module] IS NOT NULL) AND ([Channel] IS NOT NULL) 
                                                       ORDER BY [Module], [Channel]",
                            _accConnection);

                    _accBuilder = new OleDbCommandBuilder(_accAdapter);
                    _accAdapter.Fill(_accDataSet, "TagList");
                }
                catch (OleDbException e)
                {
                    if (_accConnection != null) _accConnection.Close();

                    if (e.ErrorCode == -2147467259)
                    {
                        MessageBox.Show(e.ErrorCode + "\n" + e.Message);

                        //MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
                        return 1;
                    }

                    string strMessage = "Неверная структура файла архива " + tagListFileName;
                    switch (e.ErrorCode)
                    {
                        case -2147217865:
                            strMessage += "\nНе найдена таблица TagLit";
                            break;
                        case -2147217904:
                            strMessage += "\nНеверная структура таблицы TagList";
                            break;
                        default:
                            strMessage = e.ErrorCode + "\n" + e.Message;
                            break;
                    }

                    MessageBox.Show(strMessage);
                    return 2;
                }
                catch (Exception e)
                {
                    _accConnection.Close();
                    MessageBox.Show(e.GetType() + "\n" + e.Message);
                    return 255;
                }
            }
            else
            {
                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
                return 1;
            }

            if (Program.FmMonitor != null)
            {
                Program.FmMonitor.ClearModulesTreeView();
                Program.FmMonitor.ClearSignalListView();
            }

            if (_accDataSet.Tables["TagList"].Rows.Count > 0)
            {
                int i = 0;
                DataRow row = _accDataSet.Tables["TagList"].Rows[i];
                while (row != null)
                {
                    var moduleAddress = DbString(row["Module"]);

                    NewModuleAbstract newModule;

                    const string moduleType = "Adam4019+";
                    
                    switch (moduleType)
                    {
                        case "Adam4019+":
                            newModule = new NewModuleAdam4019Plus(net.Modules, moduleAddress) { DataFormat = DbString(row["DataFormat"]) };
                            break;
                    }

                    if (newModule != null)
                    {
                        while ((row != null) && (DbString(row["Module"]) == moduleAddress))
                        {
                            string chn = DbString(row["Channel"]);
                            byte intChannel;

                            NewChannelAbstract channel = null;
                            if (byte.TryParse(chn, out intChannel)) channel = newModule.Channels[intChannel];
                            else if (chn.ToUpper() == "CJC")
                            {
                                switch (moduleType)
                                {
                                    case "Adam4019+":
                                        channel = ((NewModuleAdamAbstract)newModule).Cjc;
                                        break;
                                }
                            }

                            if (channel != null)
                            {
                                channel.Selected = (bool) row["Selected"];
                                channel.Code = DbString(row["Code"]);
                                channel.Name = DbString(row["Name"]);
                                channel.Aperture = DbDouble(row["Aperture"]);
                                channel.Min = DbDouble(row["Min"]);
                                channel.Max = DbDouble(row["Max"]);
                                channel.Units = DbString(row["Units"]);

                                switch (moduleType)
                                {
                                    case "Adam4019+":
                                        ((NewChannelAdamAbstract)channel).ChannelRange = DbString(row["ChannelRange"]);
                                        break;
                                }

                                channel.InLevel = DbString(row["In_Level"]);
                                channel.Conversion = DbString(row["Conversion"]);
                            }

                            row = (++i < _accDataSet.Tables["TagList"].Rows.Count)
                                      ? _accDataSet.Tables["TagList"].Rows[i]
                                      : null;
                        }
                        net.Modules.AddModule(moduleAddress, newModule);
                    }
                }
                Program.Net.ProgramChannels();
                Program.Net.ReadModulesSettings();
            }

            _accConnection.Close();
            return 0;
        }

        public static byte SaveNetToTagList(NewNet net)
        {
            string tagListFileName = Application.StartupPath + @"\" + FileName;

            if (File.Exists(tagListFileName))
            {
                try
                {
                    _accConnection =
                        new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + tagListFileName);
                    _accDataSet = new DataSet();

                    _accAdapter =
                        new OleDbDataAdapter(
                            @"SELECT [Selected], [TagId], [Code], [Name], [Module], [Channel], 
                                             [Aperture], [Min], [Max], [Units], [DataFormat], [ChannelRange], [In_Level], [Conversion] 
                                             FROM TagList",
                            _accConnection);

                    _accBuilder = new OleDbCommandBuilder(_accAdapter) {QuotePrefix = "[", QuoteSuffix = "]"};

                    _accAdapter.Fill(_accDataSet, "TagList");
                }
                catch (OleDbException e)
                {
                    if (_accConnection != null) _accConnection.Close();

                    if (e.ErrorCode == -2147467259)
                    {
                        MessageBox.Show(e.ErrorCode + "\n" + e.Message);
                        return 1;
                    }

                    string strMessage = "Неверная структура файла архива " + tagListFileName;
                    switch (e.ErrorCode)
                    {
                        case -2147217865:
                            strMessage += "\nНе найдена таблица TagLit";
                            break;
                        case -2147217904:
                            strMessage += "\nНеверная структура таблицы TagList";
                            break;
                        default:
                            strMessage = e.ErrorCode + "\n" + e.Message;
                            break;
                    }

                    MessageBox.Show(strMessage);
                    return 2;
                }
                catch (Exception e)
                {
                    _accConnection.Close();
                    MessageBox.Show(e.GetType() + "\n" + e.Message);
                    return 255;
                }
            }
            else
            {
                MessageBox.Show(@"Файл " + tagListFileName + @" не найден");
                return 1;
            }

            try
            {
                var oleDbCommand = new OleDbCommand("DELETE * FROM TagList", _accConnection);
                _accConnection.Open();
                oleDbCommand.ExecuteNonQuery();

                //_accDataSet.Tables["TagList"].Rows.Clear(); //так не работает

                int i = 0;
                foreach (NewModuleAbstract module in Program.Net.Modules)
                    foreach (NewChannelAbstract channel in module.Channels)
                    {
                        var newRow = _accDataSet.Tables["TagList"].NewRow();
                        newRow["Selected"] = channel.Selected;
                        newRow["TagId"] = ++i;
                        if (channel.Code != null) newRow["Code"] = channel.Code;
                        else newRow["Code"] = DBNull.Value;
                        if (channel.Name != null) newRow["Name"] = channel.Name;
                        else newRow["Name"] = DBNull.Value;
                        newRow["Module"] = module.Address;
                        newRow["Channel"] = channel.Channel;
                        if (channel.Aperture != null) newRow["Aperture"] = channel.Aperture;
                        else newRow["Aperture"] = DBNull.Value;
                        if (channel.Min != null) newRow["Min"] = channel.Min;
                        else newRow["Min"] = DBNull.Value;
                        if (channel.Max != null) newRow["Max"] = channel.Max;
                        else newRow["Max"] = DBNull.Value;
                        if (channel.Units != null) newRow["Units"] = channel.Units;
                        else newRow["Units"] = DBNull.Value;
                        if (channel.InLevel != null) newRow["In_Level"] = channel.InLevel;
                        else newRow["In_Level"] = DBNull.Value;
                        if (channel.Conversion != null) newRow["Conversion"] = channel.Conversion;
                        else newRow["Conversion"] = DBNull.Value;

                        switch(channel.ChannelType)
                        {
                            case "Adam4019+":
                                if (((NewModuleAdam4019Plus)module).DataFormat != null) newRow["DataFormat"] = ((NewModuleAdam4019Plus)module).DataFormat;
                                else newRow["DataFormat"] = DBNull.Value;
                                if (((NewChannelAdam4019Plus)channel).ChannelRange != null) newRow["ChannelRange"] = ((NewChannelAdam4019Plus) channel).ChannelRange;
                                else newRow["ChannelRange"] = DBNull.Value;
                                break;
                            
                            default:
                                newRow["DataFormat"] = DBNull.Value;
                                newRow["ChannelRange"] = DBNull.Value;
                                break;
                        }

                        _accDataSet.Tables["TagList"].Rows.Add(newRow);
                    }

                _accAdapter.Update(_accDataSet, "TagList");
                _accConnection.Close();
                return 0;
            }
            catch (OleDbException e)
            {
                MessageBox.Show(e.ErrorCode + "\n" + e.Message);
                return 3;
            }
        }
    #endregion
    }
}
