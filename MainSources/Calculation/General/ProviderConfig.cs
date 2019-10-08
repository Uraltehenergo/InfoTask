using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Описание одного провайдера из Config
    public class ProviderConfig
    {
        public ProviderConfig(ProviderType type, string code)
        {
            Type = type;
            Code = code;
        }

        //Тип провайдера
        public ProviderType Type { get; private set; }
        //Код провайдера
        public string Code { get; private set; }
        //Пути к файлу и папке dll провайдера
        private string _file;
        public string File
        {
            get { return _file; } 
            set
            {
                _file = value;
                Dir = new FileInfo(_file).DirectoryName;
            }
        }
        public string Dir { get; private set; }
        //Коды связанных провайдеров
        private readonly List<string> _jointProviders = new List<string>();
        public List<string> JointProviders { get { return _jointProviders; }}

        //Запуск экземпляра провайдера через MEF, позднее связывание с dll
        [ImportMany(typeof(IProvider))]
        private Lazy<IProvider, IDictionary<string, object>>[] ImportProvs { get; set; }

        public IProvider RunProvider(string code, string name, string inf, Logger logger)
        {
            var prc = General.ProviderConfigs[code];
            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(prc.Dir));
            var container = new CompositionContainer(catalog);
            container.ComposeParts(this);
            try
            {
                var pr = (from codeVault in ImportProvs
                          where (string)codeVault.Metadata["Code"] == code
                          select codeVault).Single().Value;
                pr.Logger = logger;
                pr.Name = name;
                pr.Inf = inf;
                return pr;
            }
            catch { return null;}
        }
    }
}