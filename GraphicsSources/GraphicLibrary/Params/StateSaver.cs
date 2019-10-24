using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using BaseLibrary;

namespace GraphicLibrary.Params
{
    public interface IStateSaver
    {
        bool SaveSate(Dictionary<string, object> state);
        Dictionary<string, object> ReadSate();
    }

    public class StateSaverAccess : IStateSaver, IDisposable
    {
        private const string TableName = "GraphicState";
        private const string MainTableName = "GraphicStateMain";
        private const string SubTableName = "GraphicStateSub";
        private const string FieldPropType = "PropType";
        private const string FieldPropName = "PropName";
        private const string FieldPropValue = "PropValue";
        private const string FieldPropId = "PropId";
        private const string FieldSubPropName = "SubPropName";
        private const string FieldSubPropValue = "SubPropValue";
        
        private readonly DaoDb _daoDb;

        public void Dispose()
        {
            if (_daoDb != null) _daoDb.Dispose();
        }

        public StateSaverAccess(string fileName)
        {
            //IEnumerable<string> tables = new[] { TableName };
            IEnumerable<string> tables = new[] { MainTableName, SubTableName };

            if (DaoDb.Check(fileName, tables))
            {
                _daoDb = new DaoDb(fileName);
            }
        }

        private bool SaveSateOld(Dictionary<string, string> state)
        {
            if (_daoDb != null)
            {
                _daoDb.Execute("DELETE * FROM " + TableName);

                const string stringSql = "SELECT " + FieldPropName + ", " + FieldPropValue + " FROM " + TableName + ";";
                using(var rs = new RecDao(_daoDb, stringSql))
                {
                    foreach(var key in state.Keys)
                    {
                        if (key != null)
                        {
                            rs.AddNew();
                            rs.Put(FieldPropName, key);
                            rs.Put(FieldPropValue, state[key]);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public bool SaveSate(Dictionary<string, object> state)
        {
            if (_daoDb != null)
            {
                _daoDb.Execute("DELETE * FROM " + SubTableName);
                _daoDb.Execute("DELETE * FROM " + MainTableName);

                string stringSql = "SELECT * FROM " + MainTableName + ";";
                using (var rsMain = new RecDao(_daoDb, stringSql))
                {
                    stringSql = "SELECT * FROM " + SubTableName + ";";
                    using (var rsSub = new RecDao(_daoDb, stringSql))
                    {
                        foreach(var propKey in state.Keys)
                        {
                            int id;
                            int num = 0;

                            foreach (var propState in (List<Dictionary<string, string>>)state[propKey])
                            {
                                rsMain.AddNew();
                                rsMain.Put(FieldPropType, propKey);
                                rsMain.Put(FieldPropName, ++num);

                                rsMain.Update();
                                rsMain.MoveLast();
                                id = rsMain.GetInt("Id");

                                foreach (var subProp in propState.Keys)
                                {
                                    rsSub.AddNew();
                                    rsSub.Put(FieldPropId, id);
                                    rsSub.Put(FieldSubPropName, subProp);
                                    rsSub.Put(FieldSubPropValue, propState[subProp]);
                                }
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public Dictionary<string, object> ReadSate()
        {
            var state = new Dictionary<string, object>();
            
            if (_daoDb != null)
            {
                const string stringSql = "SELECT * FROM " + MainTableName + " LEFT JOIN " + SubTableName + 
                                        " ON " + MainTableName + ".Id = " + SubTableName + "." + FieldPropId +
                                        " ORDER BY " + MainTableName + "." + FieldPropType + ", " + 
                                                       MainTableName + "." + FieldPropName + ", "  + 
                                                       SubTableName + "." + FieldSubPropName + ";";


                using (var rs = new RecDao(_daoDb, stringSql))
                {
                    List<Dictionary<string, string>> propState = null;
                    Dictionary<string, string> subPropState = null;
                    string curPropName = "0";
                    
                    while (!rs.EOF)
                    {
                        var propType = rs.GetString(FieldPropType);
                        if (!state.Keys.Contains(propType))
                        {
                            propState = new List<Dictionary<string, string>>();
                            state.Add(propType, propState);
                            curPropName = "0";
                        }

                        var propName = rs.GetString(FieldPropName);
                        if (propName != curPropName)
                        {
                            subPropState = new Dictionary<string, string>();
                            propState.Add(subPropState);
                            curPropName = propName;
                        }

                        subPropState.Add(rs.GetString(FieldSubPropName), rs.GetString(FieldSubPropValue));

                        rs.MoveNext();
                    }
                }
            }

            return state;
        }
    }
}
