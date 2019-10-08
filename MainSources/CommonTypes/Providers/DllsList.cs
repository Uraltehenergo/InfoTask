using System;
using System.Collections.Generic;
using System.Reflection;
using BaseLibrary;

namespace CommonTypes
{
    //Общая часть класса General для разных приложений
    public class DllsList
    {
        //Создает список Dll провайдеров
        public DllsList(string infoTaskDir, string configFile)
        {
            SourcesCodes = new List<string>();
            ReceiversCodes = new List<string>();
            ArchivesCodes = new List<string>();
            using ( var rec = new ReaderAdo(configFile, "SELECT SysTabl.ParamName, SysTabl.ParamValue, SysSubTabl.SubParamName, SysSubTabl.SubParamValue " +
                        "FROM SysTabl INNER JOIN SysSubTabl ON SysTabl.ParamId=SysSubTabl.ParamId " +
                        "WHERE (SysTabl.ParamType='Provider') AND (SysSubTabl.SubParamName='ProviderFile')"))
                while (rec.Read())
                {
                    var ptype = rec.GetString("ParamValue").ToProviderType();
                    if (ptype != ProviderType.Communicator)
                    {
                        try
                        {
                            string file = infoTaskDir + rec.GetString("SubParamValue");
                            Assembly a = Assembly.LoadFile(file);
                            Type t = a.GetType("Provider." + ptype.ToEnglish());
                            Type[] tt = Type.EmptyTypes;
                            ConstructorInfo c = t.GetConstructor(tt);
                            var pr = (IProvider)c.Invoke(new object[] { });
                            PropertyInfo prop = t.GetProperty("Code");
                            var code = (string)prop.GetValue(pr, null);
                            switch (t.Name)
                            {
                                case "Source":
                                    SourcesCodes.Add(code);
                                    break;
                                case "Receiver":
                                    ReceiversCodes.Add(code);
                                    break;
                                case "Archive":
                                    ArchivesCodes.Add(code);
                                    break;
                            }
                            _providersDlls.Add(code, file);
                        }
                        catch (Exception ex)
                        {
                            ex.MessageError("", "Системная ошибка");
                        }
                    }
            }
        }

        //Список путей к Dll провайдеров: <Код, Путь>
        private readonly Dictionary<string, string> _providersDlls = new Dictionary<string, string>();

        //Списки кодов источников, приемников, архивов
        public List<string> SourcesCodes { get; private set; }
        public List<string> ReceiversCodes { get; private set; }
        public List<string> ArchivesCodes { get; private set; }

        //Возвращает ссылку на объект IProvider по заданному типу, имени и свойствам провайдера
        public IProvider RunProvider(ProviderType type, string code, string name)
        {
            Assembly a = Assembly.LoadFile(_providersDlls[code]);
            Type t = null;
            switch (type)
            {
                case ProviderType.Source:
                case ProviderType.Imitator:
                    t = a.GetType("Provider.Source");
                    break;
                case ProviderType.Receiver:
                    t = a.GetType("Provider.Receiver");
                    break;
                case ProviderType.Archive:
                    t = a.GetType("Provider.Archive");
                    break;
            }
            Type[] tt = Type.EmptyTypes;
            ConstructorInfo con = t.GetConstructor(tt);
            var pr = (IProvider)con.Invoke(new object[] { });
            pr.Name = name;
            return pr;            
        }
    }
}
