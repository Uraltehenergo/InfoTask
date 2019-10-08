using System;
using System.Windows.Forms;
using BaseLibrary;

namespace VersionSynch
{
    public class Version
    {
        private readonly int _subVersion1;
        private readonly int _subVersion2;
        private readonly int _subVersion3;

        public Version(string versionString, string versionDate = "")
        {
            try
            {
                string ver = versionString;
                int dx = ver.IndexOf(".");

                if (dx == -1)
                {
                    _subVersion1 = 0;
                    _subVersion2 = 0;
                    _subVersion3 = 0;
                    return;
                }

                _subVersion1 = int.Parse(ver.Substring(0, dx));
                ver = ver.Substring(dx + 1, ver.Length - dx - 1);
                dx = ver.IndexOf(".");
                _subVersion2 = int.Parse(ver.Substring(0, dx));
                ver = ver.Substring(dx + 1, ver.Length - dx - 1);
                _subVersion3 = int.Parse(ver);

                Date = versionDate;
            }
            catch (Exception ex)
            {
                ex.MessageError();
            }
        }

        public bool Equals(Version other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other._subVersion1 == _subVersion1 && other._subVersion2 == _subVersion2 && other._subVersion3 == _subVersion3;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Version)) return false;
            return Equals((Version)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = _subVersion1;
                result = (result * 397) ^ _subVersion2;
                result = (result * 397) ^ _subVersion3;
                return result;
            }
        }

        public static bool operator ==(Version a, Version b)
        {
            if (a._subVersion1.Equals(b._subVersion1) && a._subVersion2.Equals(b._subVersion2) && a._subVersion3.Equals(b._subVersion3))
                return true;
            return false;
        }

        public static bool operator !=(Version a, Version b)
        {
            return !(a == b);
        }

        public static bool operator >(Version a, Version b)
        {
            if (a._subVersion1 > b._subVersion1) return true;
            if (a._subVersion1 < b._subVersion1) return false;
            if (a._subVersion2 > b._subVersion2) return true;
            if (a._subVersion2 < b._subVersion2) return false;
            if (a._subVersion3 > b._subVersion3) return true;
            if (a._subVersion3 < b._subVersion3) return false;
            return false;
        }

        public static bool operator <(Version a, Version b)
        {
            if (a._subVersion1 < b._subVersion1) return true;
            if (a._subVersion1 > b._subVersion1) return false;
            if (a._subVersion2 < b._subVersion2) return true;
            if (a._subVersion2 > b._subVersion2) return false;
            if (a._subVersion3 < b._subVersion3) return true;
            if (a._subVersion3 > b._subVersion3) return false;
            return false;
        }

        public static bool operator >=(Version a, Version b)
        {
            return !(a < b);
        }

        public static bool operator <=(Version a, Version b)
        {
            return !(a > b);
        }

        public override string ToString()
        {
            return _subVersion1 + "." + _subVersion2 + "." + _subVersion3;
        }

        public string Date;
    }

    //----------------------------------------------------------------------------------------------------------------------

    internal class UpdateDelegate
    {
        //uAction - действие выполняемое для обновления, uVersion - версия, uDate - дата версии, isBack - файл работает на предыдущей версии ПО
        public UpdateDelegate(Action uAction, string uVersion, string uDate, bool isBack = true)
        {
            Date = uDate;
            StrVersion = uVersion;
            Version = new Version(StrVersion, uDate);
            Action = uAction;
            IsBack = isBack;
        }

        public Action Action;
        public string StrVersion;
        public Version Version;
        public string Date;
        public bool IsBack;
    }

    //---------------------------------------------------------------------------------------------------------------------

    public enum DbMode
    {
        Project, 
        AnalyzerAppData, 
        ConstructorAppData, 
        Archive, 
        Imit, 
        Clone,
        ControllerData, 
        ReporterData, 
        ArchiveSQL
    }
}
