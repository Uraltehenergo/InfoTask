using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Tablik
{
    //Один параметр для архива
    internal class ParamArchive
    {
        //owner - владелец, caller - вызывающий
        public ParamArchive(CalcParam param, ParamArchive owner = null, ParamArchive caller = null, bool alreadyConcidered = false)
        {
            LastParam = param;
            if (owner == null && caller == null)
            {
                FirstParam = param;
                FullCode = param.FullCode;
            }
            else
            {
                if (caller != null)
                {
                    FirstParam = caller.FirstParam;
                    FullCode = caller.FullCode;
                }   
                else
                {
                    FirstParam = owner.FirstParam;
                    FullCode = owner.FullCode + "." + param.Code;
                }
            }
            bool ac = false;
            var ct = param.CalcType;
            if (!alreadyConcidered && ct.ClassType == ClassType.Single && !param.SuperProcess.IsNone())
            {
                ac = true;
                var tab = param.Tablik;
                if (!tab.ArchiveParams.ContainsKey(FullCode))
                {
                    tab.ArchiveParams.Add(FullCode, this);
                    tab.ArchiveParamsCount++;
                    if (tab.ArchiveParams.Count == 1) tab.UsedProviders.Add("Archive");
                    if (param.SuperProcess == SuperProcess.Moment) tab.IsMoments = true;
                    if (param.SuperProcess.IsPeriodic()) tab.IsPeriodic = true;
                    if (param.SuperProcess.IsAbsolute()) tab.IsAbsolute = true;    
                }
            }
            //Параметры с владельцем значения
            var cp = ct.ParentParam;
            if (cp != null && cp.Inputs.Count > 0 && cp.CalcOn && cp.ErrMess == "" && cp != param)
                new ParamArchive(cp, null, this, ac);
            //Методы
            foreach (var met in param.Methods.Values)
                if (met.CalcOn && met.ErrMess == "" && met.Inputs.Count == 0) 
                    new ParamArchive(met, this);
        }

        //Первый параметр в цепочке
        public CalcParam FirstParam { get; private set; }
        //Последний параметр в цепочке
        public CalcParam LastParam { get; private set; }
        //Полный код для записи в архив, с большой буквы, cуммируется по всей цепочке
        public string FullCode { get; private set; }

        //Является архивным парамтреов расчета при построении отчета
        public bool IsCalcReport { get; private set; }
        //Используется как сигнал при расчете при построении отчета
        public bool UseInCalcReport { get; private set; }

        //Записывает параметр в таблицу
        public void ToRecordset(RecDao rec, bool addnew)
        {
            try
            {
                if (addnew) rec.AddNew();
                rec.Put("CalcParamId", FirstParam.CalcParamId);
                bool b = FirstParam != LastParam;
                if (b) rec.Put("CalcSubParamId", LastParam.CalcParamId);
                var par = LastParam ?? FirstParam;
                rec.Put("SuperProcessType", par.SuperProcess.ToRussian());
                var dt = par.CalcType.DataType.AplySuperProcess(par.SuperProcess);
                rec.Put("DataType", dt.ToRussian());
                if (dt == DataType.Integer || dt == DataType.Real)
                {
                    rec.Put("Units", par.Units.IsEmpty() ? FirstParam.Units : par.Units);
                    rec.Put("Min", par.Min ?? FirstParam.Min);
                    rec.Put("Max", par.Max ?? FirstParam.Max);
                }
                if (dt == DataType.Real)
                    rec.Put("DecPlaces", par.DecPlaces ?? FirstParam.DecPlaces);
                if (FirstParam.Tablik.Prevs.ContainsKey(FullCode))
                    FirstParam.Tablik.Prevs[FullCode].Id = rec.GetInt("Id");
                rec.Put("FullCode", FullCode);
                rec.Update();    
            }
            catch {}
            //catch//Повтор архивных параметров
            //{
            //    using ( var recc = new RecDao(FirstParam.Tablik.ProjectFile, "SELECT CalcParams.ErrMess FROM CalcParams WHERE CalcParamId=" + FirstParam.CalcParamId))
            //    {
            //        if (recc.GetString("ErrMess").IsEmpty()) FirstParam.Tablik.ErrorsCount++;
            //        recc.Put("ErrMess", "По данному параметру формируется два архивных параметра с одинаковым кодом (" + FullCode + ")");    
            //    }
            //}
        }
    }
}
