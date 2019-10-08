using System;
using Microsoft.Office.Interop.Access.Dao;

namespace BaseLibrary
{
    //Типы данных
    public enum DataType
    {
        Value, //Время, недостоверность и ошибка без начения
        Boolean, //Логическое значение
        Integer, //Целое значение
        Time, //Значение тип Время
        Real, //Действительное значение
        String, //Строковое значение
        Variant, //Значение неопределенного типа
        Segments, //Набор сегментов
        Error //Задан неверный тип
    }

    //---------------------------------------------------------------------

    public static class DataTypeConv
    {
        //Преобразование в DataType
        public static DataType ToDataType(this string s)
        {
            if (s == null) return DataType.Error;
            switch (s.ToLower())
            {
                case "%v":
                case "value":
                case "величина":
                    return DataType.Value;
                case "%b":
                case "bool":
                case "логич":
                    return DataType.Boolean;
                case "%i":
                case "int":
                case "целое":
                    return DataType.Integer;
                case "%d":
                case "время":
                case "time":
                    return DataType.Time;
                case "%r":
                case "real":
                case "действ":
                    return DataType.Real;
                case "%s":
                case "string":
                case "строка":
                    return DataType.String;
                case "%u":
                case "variant":
                case "вариант":
                    return DataType.Variant;
                case "%g":
                case "segments":
                case "сегменты":
                    return DataType.Segments;
                case "%e":
                case "error":
                case "ошибка":
                    return DataType.Error;
            }
            return DataType.Error;
        }

        //Получение буквы типа 
        public static char ToLetter(this DataType d)
        {
            switch (d)
            {
                case DataType.Value:
                    return 'v';
                case DataType.Boolean:
                    return 'b';
                case DataType.Integer:
                    return 'i';
                case DataType.Time:
                    return 'd';
                case DataType.Real:
                    return 'r';
                case DataType.String:
                    return 's';
                case DataType.Variant:
                    return 'u';
                case DataType.Segments:
                    return 'g';
            }
            return 'e';
        }

        //Получение русского имени типа 
        public static string ToRussian(this DataType d)
        {
            switch (d)
            {
                case DataType.Value:
                    return "величина";
                case DataType.Boolean:
                    return "логич";
                case DataType.Integer:
                    return "целое";
                case DataType.Time:
                    return "время";
                case DataType.Real:
                    return "действ";
                case DataType.String:
                    return "строка";
                case DataType.Variant:
                    return "вариант";
                case DataType.Segments:
                    return "сегменты";
            }
            return "ошибка";
        }

        //Получение имени типа 
        public static string ToEnglish(this DataType d)
        {
            switch (d)
            {
                case DataType.Value:
                    return "value";
                case DataType.Boolean:
                    return "bool";
                case DataType.Integer:
                    return "int";
                case DataType.Time:
                    return "time";
                case DataType.Real:
                    return "real";
                case DataType.String:
                    return "string";
                case DataType.Variant:
                    return "variant";
                case DataType.Segments:
                    return "segments";
            }
            return "error";
        }

        //Получение типа данных из типа данных таблицы Access
        public static DataType ToDataType(this short d)
        {
            switch ((DataTypeEnum)d)
            {
                case DataTypeEnum.dbText:
                case DataTypeEnum.dbMemo:
                    return DataType.String;
                case DataTypeEnum.dbInteger:
                case DataTypeEnum.dbLong:
                case DataTypeEnum.dbBigInt:
                case DataTypeEnum.dbByte:
                    return DataType.Integer;
                case DataTypeEnum.dbDouble:
                case DataTypeEnum.dbSingle:
                case DataTypeEnum.dbDecimal:
                case DataTypeEnum.dbFloat:
                    return DataType.Real;
                case DataTypeEnum.dbBoolean:
                    return DataType.Boolean;
                case DataTypeEnum.dbDate:
                case DataTypeEnum.dbTime:
                case DataTypeEnum.dbTimeStamp:
                    return DataType.Time;
            }
            return DataType.Error;
        }

        //Получение типа данных таблицы Access из типа данных 
        public static DataTypeEnum ToColumnType(this DataType d)
        {
            switch (d)
            {
                case DataType.String:
                    return DataTypeEnum.dbText;
                case DataType.Real:
                    return DataTypeEnum.dbDouble;
                case DataType.Boolean:
                    return DataTypeEnum.dbBoolean;
                case DataType.Integer:
                    return DataTypeEnum.dbLong;
                case DataType.Time:
                    return DataTypeEnum.dbDate;
            }
            return DataTypeEnum.dbText;
        }

        //Сравнение двух типов данных, True - если первый приводим ко второму
        public static bool LessOrEquals(this DataType t1, DataType t2)
        {
            if (t1 == t2) return true;
            if (t1 == DataType.Value && t2 != DataType.Segments) return true;
            switch (t2)
            {
                case DataType.Integer:
                    if (t1 == DataType.Boolean) return true;
                    break;
                case DataType.Real:
                    if (t1 == DataType.Boolean || t1 == DataType.Integer) return true;
                    break;
                case DataType.String:
                    if (t1 == DataType.Boolean || t1 == DataType.Integer || t1 == DataType.Time || t1 == DataType.Real) return true;
                    break;
                case DataType.Variant:
                    if (t1 != DataType.Error && t1 != DataType.Segments) return true;
                    break;
            }
            return false;
        }

        //Возвращает общий минимум для двух типов
        public static DataType Inf(this DataType t1, DataType t2)
        {
            if (t2.LessOrEquals(t1)) return t2;
            if (t1.LessOrEquals(t2)) return t1;
            return DataType.Value;
        }

        //Возвращает общий максимум для двух типов
        public static DataType Add(this DataType t1, DataType t2)
        {
            if (t2.LessOrEquals(t1)) return t1;
            if (t1.LessOrEquals(t2)) return t2;
            if (t1 == DataType.Segments || t2 == DataType.Segments)
                return DataType.Error;
            return DataType.String;
        }

        //Проверяет, является ли заданная строка константой указанного типа данных (без кавычек и диезов)
        public static bool IsOfType(this string s, DataType type)
        {
            switch (type)
            {
                case DataType.Boolean:
                    return s == "0" || s == "1";
                case DataType.Integer:
                    int resi;
                    return int.TryParse(s, out resi);
                case DataType.Real:
                    return !double.IsNaN(s.ToDouble());
                case DataType.Time:
                    DateTime rest;
                    return DateTime.TryParse(s, out rest);
                case DataType.String:
                    return s.Length <= 255;
            }
            return true;
        }

        
    }
}
