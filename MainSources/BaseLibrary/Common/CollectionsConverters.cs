using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseLibrary
{
    //Преобразование коолекций в строки и строк в коллекции
    public static class CollectionsConverters
    {
        //Сливает данный словарь с еще одним, если ключ повторяется, то берется из первого
        public static Dictionary<string, string> Merge(this Dictionary<string, string> dic1, Dictionary<string, string> dic2)
        {
            if (dic1 == null) return dic2;
            var res = dic1.ToDictionary(d => d.Key, d => d.Value);
            foreach (var d in dic2)
                if (!res.ContainsKey(d.Key)) 
                    res.Add(d.Key, d.Value);
            return res;
        } 

        //Переводит словарь в строку со свойствами 
        public static string ToPropertyString(this IDictionary<string, string> dic, string separator = ";")
        {
            if (dic == null) return "";
            return dic.Aggregate("", (current, s) => current + (s.Key + "=" + s.Value + separator));
        }
        public static string ToPropertyString(this DicS<string> dic, string separator = ";")
        {
            return dic.Dic.ToPropertyString(separator);
        }

        //Переводит строку со свойствами в словарь
        public static Dictionary<string, string> ToPropertyDictionary(this string str, string separator = ";")
        {
            try
            {
                var res = new Dictionary<string, string>();
                if (!str.IsEmpty())
                {
                    string[] st = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in st)
                    {
                        int p = s.IndexOf("=");
                        if (!res.ContainsKey(s.Substring(0, p)))
                            res.Add(s.Substring(0, p), s.Substring(p + 1));
                    }
                }
                return res;
            }
            catch { return new Dictionary<string, string>(); }
        }

        //Переводит строку со свойствами в словарь DicS
        public static DicS<string> ToPropertyDicS(this string str, string separator = ";")
        {
            var res = new DicS<string>();
            try
            {
                if (!str.IsEmpty())
                {
                    string[] st = str.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in st)
                    {
                        int p = s.IndexOf("=");
                        //res.Add(s.Substring(0, p), s.Substring(p + 1));
                        string key = "";
                        for (int i = 0; i < p; i++)
                            key += s[i];
                        string val = "";
                        for (int i = p + 1; i < s.Length; i++)
                            val += s[i];
                        res.Add(key, val);
                    }
                }
            }
            catch { }
            return res;
        }

        //Перводит список, множество и т.п. в строку
        public static string ToPropertyString(this IEnumerable<string> list, string separator = ";")
        {
            if (list == null) return "";
            return list.Aggregate("", (current, s) => current + s + separator);
        }

        //Перводит строку в список
        public static List<string> ToPropertyList(this string str, string separator = ";")
        {
            try
            {
                var res = new List<string>();
                if (!str.IsEmpty())
                {
                    string[] st = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                    res.AddRange(st);
                }
                return res;
            }
            catch { return new List<string>(); }
        }

        //Перводит строку в SetS
        public static SetS ToPropertyHashSet(this string str, string separator = ";")
        {
            try
            {
                var res = new SetS();
                if (!str.IsEmpty())
                {
                    string[] st = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in st)
                        res.Add(s);
                }
                return res;
            }
            catch { return new SetS(); }
        }

        //Перводит строку в SortedSet
        public static SortedSet<string> ToPropertySortedSet(this string str, string separator = ";")
        {
            try
            {
                var res = new SortedSet<string>();
                if (!str.IsEmpty())
                {
                    string[] st = str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var s in st)
                        res.Add(s);
                }
                return res;
            }
            catch { return new SortedSet<string>(); }
        }

        //Получает из словаря строковое значение
        public static string Get(this Dictionary<string, string> dic, string key, string defval = null)
        {
            if (dic == null || !dic.ContainsKey(key)) return defval;
            return dic[key];
        }
        public static string Get(this Dictionary<int, string> dic, int key, string defval = null)
        {
            if (dic == null || !dic.ContainsKey(key)) return defval;
            return dic[key];
        }
        public static int GetInt(this Dictionary<string, string> dic, string key, int defval = 0)
        {
            int res;
            if (dic == null || !dic.ContainsKey(key) || !int.TryParse(dic[key], out res)) return defval;
            return res;
        }
        public static int GetInt(this DicS<string> dic, string key, int defval = 0)
        {
            int res;
            if (dic == null || !dic.ContainsKey(key) || !int.TryParse(dic[key], out res)) return defval;
            return res;
        }
        public static bool GetBool(this Dictionary<string, string> dic, string key)
        {
            if (dic == null || !dic.ContainsKey(key)) return false;
            string s = dic[key].ToLower();
            return s == "true" || s == "да";
        }
        public static bool GetBool(this DicS<string> dic, string key)
        {
            if (dic == null || !dic.ContainsKey(key)) return false;
            string s = dic[key].ToLower();
            return s == "true" || s == "да";
        }
    }
}
