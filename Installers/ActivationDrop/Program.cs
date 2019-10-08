using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace ActivationDrop
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey newKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\UAI");
            newKey.SetValue("cp", "чЧaШ[О_ТУPS^'jABCщьИ^L=rSКxэРL2* ?эче>$7(P4М}ux", RegistryValueKind.String);
            //newKey.SetValue("id", "Хъ/J[#4<M[l", RegistryValueKind.String);
            newKey.SetValue("id", "", RegistryValueKind.String);
            newKey.SetValue("un", "WqАъ+fwРб$56GОЯSdiwVgUfу$cu", RegistryValueKind.String);
            newKey.SetValue("ra", "XQЦ9{нZu''m", RegistryValueKind.String);
            newKey.SetValue("rc", "Ь$kSУJХ?Сtш", RegistryValueKind.String);
            newKey.SetValue("sr", "м&mФ6{&&zё%", RegistryValueKind.String);
            newKey.SetValue("rr", "цKР>»И5ЧK[Я", RegistryValueKind.String);
            newKey.SetValue("rm", "ь1x:|2«}/>В", RegistryValueKind.String);
            newKey.SetValue("rv", "ь1x:|2«}/>В", RegistryValueKind.String);
        }
    }
}
