using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ActivatorUTE
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey newKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\UAI");
            newKey.SetValue("cp", "чЧaШ[О_ТУPS^'jABCщьИ^L=rSКxэРL2* ?эче>$7(P4М}ux", RegistryValueKind.String);
            newKey.SetValue("id", "5pГю0:PYl«РАНдь$7Яр|Мgёш2!9JcЖЧХз", RegistryValueKind.String);
            newKey.SetValue("un", "WqАъ+fwРб$56GОЯSdiwVgUfу$cu", RegistryValueKind.String);
            newKey.SetValue("ra", "l'\\jырf;ё", RegistryValueKind.String);
            newKey.SetValue("rc", "яVЙ9К3ЗГ)", RegistryValueKind.String);
            newKey.SetValue("sr", "{CxHЩЩOьb", RegistryValueKind.String);
            newKey.SetValue("rr", "&уHy*x,uщ", RegistryValueKind.String);
            newKey.SetValue("rm", "sДщи[Uй,r", RegistryValueKind.String);
            newKey.SetValue("rv", "sДщи[Uй,r", RegistryValueKind.String);
        }
    }
}
