using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Сохранение результатов в Result
    public class ResultSaver : IDisposable
    {
        public ResultSaver(string projectProvider, ThreadCalc thread)
        {
            _projectProvider = projectProvider;
            _db = new DaoDb(thread.ResultFile);
            _rec = new RecDao(_db, "DebugParams");
            _recv = new RecDao(_db, "DebugParamsValues");
            _thread = thread;
        }

        //Очистка ресурсов
        public void Dispose()
        {
            try {_rec.Dispose();} catch { }
            try { _recv.Dispose();} catch { }
            try { _db.Dispose(); } catch { }
        }

        //Код проекта или имя источника
        private readonly string _projectProvider;
        //Рекордсеты DebugParams и DebugParamsValues
        private readonly RecDao _rec, _recv;
        //База данных Result
        private readonly DaoDb _db;
        //Поток
        private readonly ThreadCalc _thread;

        //Значение для записи в Value или Variables
        private static string SvToStr(SingleValue sv)
        {
            if (sv == null || sv.Type == SingleType.Void) return "";
            if (sv.Type == SingleType.Segments)
            {
                if (sv.Error != null) return "сегменты(Ошибка)";
                return "сегменты";
            }
            var mv = sv.LastMoment;
            string s = "";
            if (mv.DataType != DataType.Value)
                s = mv.DataType == DataType.Time ? mv.Date.ToStringWithMs() : mv.String;
            if (mv.Error == null)
            {
                if (mv.Nd == 0) return s;
                return s + "(Nd=" + mv.Nd + ")";
            }
            if (mv.Nd == 0) return s + "(Ошибка)";
            return s + "(Ошибка,Nd=" + mv.Nd + ")";
        }

        //Запись мгновенного значения в DebugValues
        private static void MomToRec(Moment mv, RecDao recv, int id)
        {
            if (mv == null) return;
            recv.AddNew();
            recv.Put("DebugParamId", id);
            if (mv.DataType == DataType.Time)
                recv.Put("StrValue", mv.Date.ToStringWithMs());
            else
            {
                if (mv.DataType == DataType.String)
                    recv.Put("StrValue", mv.String);
                else recv.Put("Value", mv.Real);
            }
            recv.Put("Time", mv.Time);
            recv.Put("Nd", mv.Nd);
            if (mv.Error != null) recv.Put("ErrMess", mv.Error.ToString(), true);
            recv.Update();
        }

        //Запись SingleValue в DebugParams и DebugValues, возвращает записываемую ошибку, adderr - для записи сообщения об ошибке в подпараметрах
        public void ValuesToRec(string realCode, string debugType, int paramId, Dictionary<string, string> variables, SingleValue sv, string adderr = "")
        {
            if (sv == null) return;
            _rec.AddNew();
            _rec.Put("ProjectProvider", _projectProvider);
            _rec.Put("Code", realCode);
            _rec.Put("ResultType", sv.DataType.ToRussian());
            _rec.Put("DebugParamType", debugType);
            _rec.Put("ParamId", paramId);
            if (variables != null) _rec.Put("Variables", variables.ToPropertyString());
            var err = sv.LastError;
            if (err.IsEmpty()) err = adderr;
            else err += "; " + adderr;
            _rec.Put("ErrMess", err, true);
            _rec.Put("Value", SvToStr(sv));
            _rec.Put("MomentsCount", sv.Type == SingleType.Moment ? 1 : (sv.Moments == null ? 0 : sv.Moments.Count));
            int id = _rec.GetInt("DebugParamId");
            _rec.Update();
            if (_thread.IsSaveValues)
            {
                if (sv.Type == SingleType.Moment)
                    MomToRec(sv.Moment, _recv, id);
                if (sv.Type == SingleType.List && sv.Moments != null)
                    foreach (var mv in sv.Moments)
                        MomToRec(mv, _recv, id);
            }
        }

        private void AddDic(Dictionary<string, string> dic, Dictionary<string, string> add, string code)
        {
            foreach (var s in add)
            {
                var scode = code == null ? s.Key : (s.Key == "" ? code : code + "." + s.Key);
                if (!dic.ContainsKey(scode)) dic.Add(scode, s.Value);
            }
        }

        //Сохранение одного параметра или одной переменной, возвращает словарь ключи - коды переменных, значения - величичны
        //code - полный код для для записи в DebugParams
        //debugType - Параметр, Подпараметр, Объект, Переменная, Сигнал, paramId - id параметра или подпараметра, для остальных 0
        //c - расчетный параметр, v - переменная, всегда одно из двух, level - уровень вложенности, 
        //saveToRec - сохранять в рекордсет и переменные, если false, то сам параметр не сохраняется но могут сохраняться его подпараметры и переменные
        public Dictionary<string, string> SaveValues(string code, string debugType, int paramId, CalcParamRun c, VarRun v, int level, bool saveToRec = true)
        {
            if (level > 3) return null;
            var dic = new Dictionary<string, string>();
            CalcValue cv;
            string adderr = "";
            if (v != null) cv = v.CalcValue;
            else
            {
                if (c == null || c.CalcValue == null) return null;
                if (_thread.IsSaveProperties)
                    foreach (var met in c.Methods.Values)
                    {
                        bool e = met.CalcParam.IsNotObject;
                        AddDic(dic, SaveValues(code + "." + met.CalcParam.Code, e ? "Подпараметр" : "Порождаемый", e ? met.CalcParam.Id : 0, met, null, level + 1), met.CalcParam.Code);
                        if (met.CalcValue.Error != null && adderr.IsEmpty())
                            adderr = "Ошибки в " + (e ? "" : "порожденных ") + "подпараметрах";
                    }
                if (_thread.IsSaveVariables)
                    foreach (var vr in c.Vars.Dic)
                        if (vr.Key != "calc" || (c.CalcValue.Type == CalcValueType.Single && c.CalcValue.SingleValue != vr.Value.CalcValue.SingleValue))
                            AddDic(dic, SaveValues(code + "." + vr.Key, "Переменная", 0, null, vr.Value, level + 1), vr.Key);
                cv = c.CalcValue;
            }

            if (cv != null)
            {
                switch (cv.Type)
                {
                    case CalcValueType.Void:
                    case CalcValueType.Single:
                        if ((saveToRec && cv.SingleValue != null) || !adderr.IsEmpty())
                        {
                            ValuesToRec(code, debugType, paramId, dic, cv.SingleValue ?? new SingleValue(), adderr);
                            dic.Add("", SvToStr(cv.SingleValue));   
                        }
                        break;
                    case CalcValueType.IntArray:
                        foreach (var a in cv.IntArray)
                            AddDic(dic, SaveValues(code + "[" + a.Key + "]", "Переменная", 0, null, a.Value, level + 1), null);
                        break;
                    case CalcValueType.StringArray:
                        foreach (var a in cv.StringArray)
                            AddDic(dic, SaveValues(code + "[" + a.Key + "]", "Переменная", 0, null, a.Value, level + 1), null);
                        break;
                }
                if (cv.ParentParam != null && cv.ParentParam.Inputs.Count != 0)
                {
                    if (c == cv.ParentParam)
                        ValuesToRec(code, debugType, paramId, dic, new SingleValue());
                    else
                        AddDic(dic, SaveValues(code + ":" + cv.ParentParam.CalcParam.Code, debugType, paramId, cv.ParentParam, null, level, false), null);
                }
            }
            return dic;
        }
    }
}