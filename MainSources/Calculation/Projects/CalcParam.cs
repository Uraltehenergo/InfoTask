using System;
using System.Collections.Generic;
using BaseLibrary;
using CommonTypes;

namespace Calculation
{
    //Расчетный параметр
    public class CalcParam : CalcParamBase
    {
        //Конструктор для чтения информации по параметру из рекордсета
        //rec - рекордсет с таблицей CalcParams, project - проект
        public CalcParam(Project project, IRecordRead rec, bool isSubParam)
        {
            Project = project;
            _logger = Project.ThreadCalc;
            try
            {
                Code = rec.GetString("Code");
                Id = rec.GetInt("CalcParamId");
                Name = rec.GetString("Name");
                Units = rec.GetString("Units");
                Comment = rec.GetString("Comment");
                Interpolation = rec.GetString("InterpolationType").ToInterpolation();
                if (Interpolation == InterpolationType.None) Interpolation = project.Interpolation;
                ExprString = rec.GetString("Expr");
                ResultType = rec.GetString("ResultType");
                Min = rec.GetDoubleNull("Min");
                Max = rec.GetDoubleNull("Max");
                DecPlaces = rec.GetIntNull("DecPlaces");
                Tag = rec.GetString("Tag");
                
                if (!isSubParam)
                {
                    CalcParamType = rec.GetString("CalcParamType").ToCalcParamType();
                    if (CalcParamType != CalcParamType.Class)
                        HandInputValue = new Moment(CalcParamType.HandDataType(), rec.GetString("DefaultValue"));   
                    Task = rec.GetString("Task");
                    CodeSignal = rec.GetString("CodeSignal");
                }

                //Архив
                SuperProcess = rec.GetString("SuperProcessType").ToSuperProcess();

                //Приемник
                var rcode = rec.GetString("ReceiverCode");
                if (Project.SignalsReceivers.ContainsKey(rcode))
                    ReceiverSignal = Project.SignalsReceivers[rcode].ReceiverSignal;
                
                if (!rec.GetString("ErrMess").IsEmpty())
                    _logger.AddError("Список расчетных параметров содержит ошибки компиляции", null, "Параметр=" + Code);
            }
            catch (Exception ex)
            {
                AddLog(ex);
            }
        }

        //Добавляет в лог ошибку
        private void AddLog(Exception ex = null)
        {
            _logger.AddError("Список расчетных параметров загружен с ошибками, необходима повторная компиляция расчета", ex, "Параметр=" + Code);
        }

        //Project содержащий данный параметр
        public Project Project { get; private set; }
        //Logger для ошибок (ThreadCalc)
        private readonly Logger _logger;
        //Полный код
        public string FullCode { get; set; }
        
        //CalcParamId
        public int Id { get; private set; }
        //Тип интерполяции
        public InterpolationType Interpolation { get; private set; }

        //Единицы измерения
        public string Units { get; private set; }
        //Тип накопления
        public SuperProcess SuperProcess { get; private set; }
        //Минимум и максимум шкалы
        public double? Min { get; private set; }
        public double? Max { get; private set; }
        //Количество знаков после запятой
        public int? DecPlaces { get; private set; }
        //Полный код сигнала, по которому сформирован данный параметр
        public string CodeSignal { get; private set; }

        //Выражение
        public string ExprString { get; private set; }
        //Возвращаемый тип данных 
        public string ResultType { get; private set; }

        //Владелец
        public CalcParam Owner { get; set; }
        //Методы
        private readonly DicS<CalcParam> _methods = new DicS<CalcParam>();
        public DicS<CalcParam> Methods { get { return _methods; } }
        //Является параметром без входов или подпараметром без входов параметра без входов
        public bool IsNotObject { get; private set;}

        //Формула, как массив лексем, нумерация с 0
        public Expr[] Exprs { get; private set; }
        //Словарь внутренних переменных и входных параметров
        private readonly DicS<DataType> _vars = new DicS<DataType>();
        public DicS<DataType> Vars { get { return _vars; } }
        //Список входных параметров
        private readonly List<string> _inputs = new List<string>();
        public List<string> Inputs { get { return _inputs; } }

        //Список строковых констант
        private readonly List<string> _signals = new List<string>();
        public List<string> Signals{ get { return _signals; } }
        private readonly List<string> _strings = new List<string>();
        public List<string> Strings { get { return _strings; } }

        //Для параметров без входных параметров
        public CalcParamRun RunParam { get; set; }
        //Значение ручного ввода
        public Moment HandInputValue { get; set; }
        //Item приемника
        public ProviderSignal ReceiverSignal { get; private set; }


        //Разбор выражений, входных параметров и т.д.
        public void Parse()
        {
            if (ResultType == null)
            {
                AddLog();
                return;
            }
            try
            {
                ParseExpression();
                IsNotObject = Inputs.Count == 0 && (Owner == null || Owner.Inputs.Count == 0);
                foreach (CalcParam met in Methods.Values)
                    met.Parse();   
            }
            catch (Exception ex)
            {
                AddLog(ex);
            }
        }
        
        //Разбор формулы, заполнение Exprs для параметра calc из DataReader rec
        private void ParseExpression()
        {
            if (ExprString == null)
            {
                AddLog();
                return;
            }
            try
            {
                //Вычленение сигналов и строк
                string rexp = FindSignalsAndStrings();

                char[] c = { ';' };
                string[] sexp = rexp.Split(c, StringSplitOptions.RemoveEmptyEntries);

                //Переменные и входные параметры
                int count = Int32.Parse(sexp[0]);
                int icount = Int32.Parse(sexp[1]);
                int num = 2;
                for (int k = 0; k < count; k++)
                {
                    char[] cc = { '(', ')'};
                    string[] ss = sexp[num].Split(cc, StringSplitOptions.RemoveEmptyEntries);
                    Vars.Add(ss[0], ss[1].ToDataType());
                    if (k < icount) Inputs.Add(ss[0]);
                    num ++;
                }

                //Программа
                count = Int32.Parse(sexp[num]);
                Exprs = new Expr[count];
                num++;
                for (int k = 0; k < count; k++)
                    Exprs[k] = Expr.New(sexp[k+num], this);
            }
            catch (Exception ex)
            {
                AddLog(ex);
            }
        }

        //Вычленение из выражения сигналов и строк 
        private string FindSignalsAndStrings()
        {
            string rexp = "";
            int i = 0;
            string state = "a";
            while (i < ExprString.Length)
            {
                switch (state)
                {
                    case "a":
                        if (ExprString[i] == '\'')
                        {
                            Strings.Add("");
                            state = "s";
                        }
                        else if (ExprString[i] == '{')
                        {
                            Signals.Add("");
                            state = "o";
                        }
                        else rexp += ExprString[i];
                        break;

                    case "s":
                        if (ExprString[i] == '\'')
                        {
                            if (ExprString[i + 1] == '\'')
                            {
                                Strings[Strings.Count - 1] += "'";
                                i++;
                            }
                            else
                            {
                                state = "a";
                                rexp += (Strings.Count - 1);
                            }
                        }
                        else Strings[Strings.Count - 1] += ExprString[i];
                        break;

                    case "o":
                        if (ExprString[i] == '}')
                        {
                            state = "a";
                            rexp += (Signals.Count - 1);
                        }
                        else Signals[Signals.Count - 1] += ExprString[i];
                        break;
                }
                i++;
            }
            return rexp;
        }

        //Произвести расчет
        public void Calculate()
        {
            if (RunParam == null)
                RunParam = new CalcParamRun(this, null, Owner == null ? Project.RootParam : Owner.RunParam, null);
            if (ReceiverSignal != null) ReceiverSignal.Value = RunParam.CalcValue.SingleValue;
            RunParam.JoinArchiveParam(FullCode);
        }
    }
}