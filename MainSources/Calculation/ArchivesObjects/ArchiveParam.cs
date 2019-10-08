using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Один расчетный параметр из архива
    public class ArchiveParam
    {
        public ArchiveParam(string fullCode, DataType dtype, string units = null, CalcParamBase first = null, CalcParamBase last = null, 
                                        SuperProcess superProcess = SuperProcess.None, int? decPlaces = null, double? min = null, double? max = null)
        {
            FullCode = fullCode;
            FirstParam = first;
            LastParam = last;
            DataType = dtype;
            Units = units;
            SuperProcess = superProcess;
            DecPlaces = decPlaces;
            Min = min;
            Max = max;
        }

        public ArchiveParam(IRecordRead rec)
        {
            FullCode = rec.GetString("Code");
            var codep = rec.GetString("CodeParam");
            bool hasSubParams = FullCode.ToLower() != codep.ToLower();
            if (hasSubParams) CodeSubParam = FullCode.Substring(codep.Length + 1);
            DataType = rec.GetString("DataType").ToDataType();
            Units = rec.GetString("Units");
            SuperProcess = rec.GetString("SuperProcessType").ToSuperProcess();
            DecPlaces = rec.GetIntNull("DecPlaces");
            Min = rec.GetDoubleNull("Min");
            Max = rec.GetDoubleNull("Max");
            FirstParam = new CalcParamBase(rec.GetString("CodeParam"), rec.GetString("Name"), rec.GetString("Comment"), rec.GetString("Task"),
                                         rec.GetString("CalcParamType").ToCalcParamType(), rec.GetString("Tag"));
            if (hasSubParams) LastParam = new CalcParamBase(CodeSubParam, rec.GetString("SubName"), rec.GetString("SubComment"));
        }

        //Запись в Params архива, если провайдер позволяет
        public void ToRecordset(IRecordSet rec, int projectId, bool addnew = false)
        {
            ToRecordset(rec, addnew);
            rec.Put("ProjectId", projectId);
            rec.Put("Active", true);
            var t = DateTime.Now;
            rec.Put("TimeChange", t);
            if (rec.GetTimeNull("TimeAdd") == null)
                rec.Put("TimeAdd", t);
        }

        //Запись в CalcParams файла данных отчета
         public void ToRecordset(IRecordSet rec, string project, bool addnew = false)
         {
             ToRecordset(rec, addnew);
             rec.Put("Project", project);
         }

         private void ToRecordset(IRecordSet rec, bool addnew)
         {
             if (addnew) rec.AddNew();
             rec.Put("Code", FullCode);
             rec.Put("Units", Units);
             rec.Put("DataType", DataType.ToRussian());
             rec.Put("SuperProcessType", SuperProcess.ToRussian());
             rec.Put("Min", Min);
             rec.Put("Max", Max);
             rec.Put("DecPlaces", DecPlaces);
             if (FirstParam != null)
             {
                 rec.Put("CodeParam", FirstParam.Code);
                 rec.Put("Name", FirstParam.Name);
                 rec.Put("Task", FirstParam.Task);
                 rec.Put("Comment", FirstParam.Comment);
                 rec.Put("CalcParamType", FirstParam.CalcParamType.ToRussian());
                 rec.Put("Tag", FirstParam.Tag);
             }
             if (LastParam != null)
             {
                 rec.Put("CodeSubParam", LastParam.Code);
                 rec.Put("SubName", LastParam.Name);
                 rec.Put("SubComment", LastParam.Comment);
             }
         }

        //Полный код
        public string FullCode { get; private set; }
        //Полный код без кода первого параметра
        public string CodeSubParam { get; private set; }
        //Первый параметр в цепочке
        public CalcParamBase FirstParam { get; private set; }
        //Последний параметр в цепочке
        public CalcParamBase LastParam { get; private set; }
        
        //Тип данных
        public DataType DataType { get; private set; }
        //Тип накопления
        public SuperProcess SuperProcess { get; private set; }
        //Единицы измерения
        public string Units { get; private set; }
        //Минимум и максимум шкалы
        public double? Min { get; private set; }
        public double? Max { get; private set; }
        //Количество знаков после запятой
        public int? DecPlaces { get; private set; }

        //Id из Params архива
        public int Id { get; set; }
        //Мгновенные значения по интервалам, ключ - интервал, значение - список мгновенных значений
        private Dictionary<ArchiveInterval, SingleValue> _intervals;
        public Dictionary<ArchiveInterval, SingleValue> Intervals
        { get { return _intervals ?? (_intervals = new Dictionary<ArchiveInterval, SingleValue>()); } }
    }
}