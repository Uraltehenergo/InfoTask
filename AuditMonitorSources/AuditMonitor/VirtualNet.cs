using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AuditMonitor
{
    class ClassVirtualNet
    {
        private readonly Dictionary<string, ClassVirtualAdamModule4019Plus> _virtualNet = new Dictionary<string, ClassVirtualAdamModule4019Plus>();
        
        public ClassVirtualNet(string modules)
        {
            if (modules != null)
            {
                var mods = modules.Split(new[] {"/"}, StringSplitOptions.None);
                for (int i = 0; i <= mods.GetUpperBound(0); i++)
                {
                    switch (mods[i])
                    {
                        case "4019+":
                            //--string address = ClassAbstractAdamModule.ByteToHex((byte) i);
                            string address = NewModuleAbstract.ByteToHex((byte)i);
                            _virtualNet.Add(address, new ClassVirtualAdamModule4019Plus(address));
                            break;
                    }
                }
            }
        }

        public string SendCommand(string command)
        {
            if (command.Length >= 3)
            {
                Thread.Sleep(10);
                var module = GetModule(command.Substring(1, 2));                
                if (module != null) return module.SendCommand(command);
            }
            return "";
        }
        
        private ClassVirtualAdamModule4019Plus GetModule(string moduleAddress)
        {
            if (_virtualNet.ContainsKey(moduleAddress))
            {
                return _virtualNet[moduleAddress];
            }
            return null;
        }

        public IEnumerator GetEnumerator()
        {
            return _virtualNet.Keys.GetEnumerator();
        }
    }
}
