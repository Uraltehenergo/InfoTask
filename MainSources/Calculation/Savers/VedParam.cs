using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Параметр ведомости
    internal class VedParam
    {
        public VedParam(CalcParamArchive param, VedSaver saver)
        {
            _vedSaver = saver;
            _param = param;
            _props.Add("Code", _param.ArchiveParam.FullCode);
            _props.Add("Name", _param.FirstParam.Name);
            _props.Add("DataType", _param.DataType.ToRussian());
            _props.Add("Units", _param.ArchiveParam.Units);
            _props.Add("Comment", _param.FirstParam.Comment);
            _props.Add("Min", _param.ArchiveParam.Min != null ? _param.ArchiveParam.Min.ToString() : null); //12.04.2019 не был обработан случай null
            _props.Add("Max", _param.ArchiveParam.Max != null ? _param.ArchiveParam.Max.ToString() : null); //12.04.2019 не был обработан случай null
            //ab 20.02.2019 
            //if (_param.FirstParam.CodeSignal != null)
            //{
            //var dic = _param.FirstParam.Project.SignalsSources[_param.FirstParam.CodeSignal].TagObject.ToPropertyDicS();
            //    _props.AddDic(dic);    
            //}
            _props.Add("CodeSignal", _param.FirstParam.CodeSignal);
            //\ab

        }

        //Ссылка на VedSaver
        private readonly VedSaver _vedSaver;
        //Ссылка на архивный параметр
        private readonly CalcParamArchive _param;
        //Id в таблице Params
        private int _id;
        //Словарь значений подпараметров
        private readonly Dictionary<string, SingleValue> _subParams = new Dictionary<string, SingleValue>();
        public Dictionary<string, SingleValue> SubParams { get { return _subParams; } }
        //Словарь характеристик параметра и сигнала
        private readonly DicS<string> _props = new DicS<string>();
        
        //Запись в таблицы Params и SubParams, columns - список колонок таблицы Params
        public void ToParamsTables(RecDao recParams, RecDao recSub, IEnumerable<VedColumn> columns)
        {
            recParams.AddNew();
            foreach (var col in columns)
            {
                if (col.SourceType == ColumnSourceType.ResultValue && SubParams.ContainsKey(col.Code))
                {
                    var m = SubParams[col.Code].LastMoment;
                    if (m.DataType != DataType.Value)
                        WriteValue(m, recParams, col.Code);
                }
                else if (col.SourceType == ColumnSourceType.ParamChar || col.SourceType == ColumnSourceType.SignalChar) 
                    WriteProp(col, recParams);
            }
            _id = recParams.GetInt("IdParam");
            recParams.Update();
            foreach (var sub in SubParams.Keys)
            {
                recSub.AddNew();
                recSub.Put("IdParam", _id);
                recSub.Put("CodeSubParam", sub);
            }
        }

        //Запись значений по параметру и подпараметрам в линейную ведомость, columns - список колонок таблицы LinVed
        public void ToLinVed(RecDao rec, DicS<VedColumn> columns)
        {
            if (_param.RunParam.CalcValue == null || _param.RunParam.CalcValue.SingleValue == null) return;
            var par = _param.RunParam.CalcValue.SingleValue.ToMomList();
            var subs = new List<VedMomList>();
            foreach (var col in columns.Values)
                if (SubParams.ContainsKey(col.Code))
                    subs.Add(new VedMomList(col.Code, SubParams[col.Code]));
            foreach (var mom in par)
            {
                rec.AddNew();
                rec.Put("IdParam", _id);
                rec.Put("TimeValue", mom.Time);
                rec.Put("TimeString", mom.Time.ToStringWithMs());
                if (mom.DataType.LessOrEquals(DataType.Real))
                {
                    if (columns.ContainsKey("ValueReal"))
                        rec.Put("ValueReal", mom.Real);
                }
                else if (columns.ContainsKey("ValueString"))
                    rec.Put("ValueString", mom.String);
                if (rec.ContainsField("Nd")) rec.Put("Nd", mom.Nd);
                if (rec.ContainsField("ErrorString")) rec.Put("ErrorString", mom.Error.Text);
                foreach (var sub in subs)
                    WriteValue(sub.ChangeMoment(mom.Time), rec, sub.Code);
                //foreach (var col in _vedSaver.ColumnsParams.Values)
                //    if (col.LinVedView != VedView.None)
                //        WriteProp(col, rec);
                rec.Update();
            }
        }

        //Записать в таблицу характеристику параметра или сигнала
        private void WriteProp(VedColumn col, RecDao rec)
        {
            if (col.Code == "Min" || col.Code == "Max")
            {
                string s = _props[col.Code];
                double? m = s == null ? (double?)null : s.ToDouble();
                rec.Put(col.Code, m);
            }
            else rec.Put(col.Code, _props[col.Code]);
        }

        //Запись мгновенного значения в ячейку
        private void WriteValue(Moment mom, RecDao rec, string field)
        {
            if (mom == null) return;
            switch (mom.DataType)
            {
                case DataType.Real:
                    rec.Put(field, mom.Real);
                    break;
                case DataType.Integer:
                case DataType.Boolean:
                    rec.Put(field, mom.Integer);
                    break;
                case DataType.String:
                    rec.Put(field, mom.String);
                    break;
                case DataType.Time:
                    rec.Put(field, mom.Date);
                    break;
            }
        }

        //------------------------------------------------------------------------------------------------------
        //Вспомогательный объект для записи мгновенных значений
        private class VedMomList
        {
            public VedMomList(string code, SingleValue v)
            {
                Code = code;
                _moments = v == null ? new List<Moment>() : v.ToMomList();
                _pos = -1;
            }

            //Код колонки
            public string Code { get; private set; }
            //Список значений
            private readonly List<Moment> _moments;
            //Текущая позиция в списке
            private int _pos;

            //Записать значение на текущее время
            public Moment ChangeMoment(DateTime time)
            {
                while (_pos < _moments.Count - 1 && _moments[_pos + 1].Time <= time)
                    _pos++;
                if (_moments.Count == 0) return null;
                if (_pos == -1) return _moments[0];
                return _moments[_pos];
            }
        }
    }
}