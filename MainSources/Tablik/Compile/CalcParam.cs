using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseLibrary;
using CommonTypes;
using Compile;
using Ast;

namespace Tablik
{
    //Расчетный параметр
    internal class CalcParam
    {
        //Параметр включен в компиляцию
        public bool CalcOn { get; private set; }
        //Код параметра, как записано в таблице
        public string Code { get; private set; }
        //Полный код вместе с владельцами
        public string FullCode { get; private set; }
        //Имя
        public string Name { get; private set; }
        //Комментарий
        public string Comment { get; private set; }
        //Единицы измерения
        public string Units { get; private set; }
        //Задача
        public string Task { get; private set; }
        //Id в CalcParams
        public int CalcParamId { get; private set; }
        //Тип параметра
        public CalcParamType CalcParamType { get; private set; }
        // Тип накопления итоговых значений
        public SuperProcess SuperProcess { get; private set; }
        //Минимум и максимум шкалы
        public double? Min { get; private set; }
        public double? Max { get; private set; }
        //Количество знаков
        public int? DecPlaces { get; private set; }
        //Дополнительные сведения
        public string Tag { get; private set; }
        //Значение по умолчанию
        public string DefaultValue { get; private set; }
        //Сигнал, на основе которого сделан данный параметр
        public string CodeSignal { get; private set; }

        //Архивный параметр, если есть и однозначно определен
        public ParamArchive ParamArchive { get; set; }
        //Сигнал приемника
        public Signal ReceiverSignal { get; set; }
        //Ссылка на Tablik
        public TablikCompiler Tablik { get; private set; }

        //Владелец
        public CalcParam Owner { get; private set; }
        //Подпараметры, включенные в расчет и без грубых ошибок
        private readonly DicS<CalcParam> _methods = new DicS<CalcParam>();
        public DicS<CalcParam> Methods { get { return _methods; } }
        //Словарь всех подпараметров
        private readonly DicS<CalcParam> _methodsAll = new DicS<CalcParam>();
        public DicS<CalcParam> MethodsAll { get { return _methodsAll; }}
        //Словарь всех подпараметров, ключи - Id
        private readonly DicI<CalcParam> _methodsId = new DicI<CalcParam>();
        public DicI<CalcParam> MethodsId { get { return _methodsId; } }
        //Является подпараметром
        public bool IsSubParam { get; private set; }
        //Функции Пред, используемые в подпараметрах
        private readonly DicS<Prev> _prevs = new DicS<Prev>();
        public DicS<Prev> Prevs { get { return _prevs; } }
        //Параметры, тип которых должен иметь данный, чтобы не было ошибок
        private readonly Dictionary<CalcParam, Lexeme> _mustBeReturned = new Dictionary<CalcParam, Lexeme>();
        //Параметр, из которого был вызван данный 
        public CalcParam Caller { get; private set; }
        
        //Тип объекта
        public CalcType CalcType { get; set; }

        //Строка с входными параметрами
        public string InputsStr { get; private set; }
        //Список входных параметров
        private readonly List<Var> _inputs = new List<Var>();
        public List<Var> Inputs { get { return _inputs; } }
        //Список переменных вместе с входными параметрами
        private readonly Dictionary<string, Var> _vars = new Dictionary<string, Var>();
        public Dictionary<string, Var> Vars { get { return _vars; } }

        //Выражения
        public string UserExpr1 { get; private set; }
        public string UserExpr2 { get; private set; }
        //Разобранные выражения
        private readonly List<ExprLexeme> _exprs1 = new List<ExprLexeme>();
        public List<ExprLexeme> Exprs1 { get { return _exprs1; } }
        private readonly List<ExprLexeme> _exprs2 = new List<ExprLexeme>();
        public List<ExprLexeme> Exprs2 { get { return _exprs2; } }
        //Выражение, для записи в таблицу
        private readonly List<ExprLexeme> _exprs = new List<ExprLexeme>();
        public List<ExprLexeme> Exprs { get { return _exprs; }}

        //Стадия компиляции параметра
        public CompileStage Stage { get; set; }
        //Параметр, задающий сильносвязную компоненту
        public CalcParam StrongComponent { get; set; }
        //Список праметров, входящих в сильносвязную компоненту данного
        private readonly List<CalcParam> _strongParams = new List<CalcParam>();
        public List<CalcParam> StrongParams { get { return _strongParams; }}

        //Множество вызовов параметров без входа из параметров без входа для поиска циклических ссылок
        private readonly HashSet<CalcParam> _paramCalls = new HashSet<CalcParam>();
        public HashSet<CalcParam> ParamCalls { get { return _paramCalls; } }
        //Список параметров, использующих данный объект
        private DicS<CalcParam> _usingParams;
        public DicS<CalcParam> UsingParams { get { return _usingParams ?? (_usingParams = new DicS<CalcParam>()); } }

        //Какое выражение обрабатывается расчетное (расч.) или управляющее (упр.)
        private string _curExpr = "расч.";
        //Порядковый номер в расчете
        public int Number { get; set; }

        //Случилась ошибка, после которой невозможно продлжать компиляцию параметра
        private bool _isFatalError;

        //Сообщения об ошибках 
        private string _errMess="";
        public string ErrMess
        {
            get { return _errMess.Length < 255 ? _errMess : _errMess.Remove(254); }
            private set { _errMess = value; }
        }

        //Проверка кода параметра на правильность
        private bool CheckCode(string code)
        {
            if (code.IsWhiteSpace()) return false;
            string lcode = code.ToLower();
            bool e = false;
            const string leters = "abcdefghijklmnopqrstuvwxyzабвгдеёжзийклмнопрстуфхцчшщъыьэюя_";
            const string digits = "0123456789";
            foreach (char l in lcode)
            {
                int inl = leters.IndexOf(l), ind = digits.IndexOf(l);
                e |= inl != -1;
                if (inl == -1 && ind == -1) return false;
            }
            return e;
        }

        //rec - рекордсет с таблицей CalcParams, recg - стаблицей Grafic, Tablik - ссылка на Tablik
        public CalcParam(IRecordSet rec, TablikCompiler tablikCompiler, bool isSubParam = false)
        {
            Tablik = tablikCompiler;
            IsSubParam = isSubParam;
            Code = rec.GetString("Code");
            if (!CheckCode(Code))
                ErrMess += "Указан недопустимый код расчетного параметра; ";
            if (tablikCompiler.Funs.ContainsKey(Code))
                ErrMess += "Код расчетного параметра совпадает со встроенной функцией (" + Code + "); ";
            if (tablikCompiler.Grafics.ContainsKey(Code))
                ErrMess += "Код расчетного параметра совпадает с кодом графика (" + Code + "); ";
            Name = rec.GetString("Name");
            Comment = rec.GetString("Comment");
            Units = rec.GetString("Units");
            Tag = rec.GetString("Tag");
            Min = rec.GetDoubleNull("Min");
            Max = rec.GetDoubleNull("Max");
            DecPlaces = rec.GetIntNull("DecPlaces");

            CalcOn = rec.GetBool("CalcOn");
            CalcParamId = rec.GetInt("CalcParamId");
            if (!IsSubParam)
            {
                CalcOn &= rec.GetBool("TaskOn");
                CalcParamType = rec.GetString("CalcParamType").ToCalcParamType();
                DefaultValue = rec.GetString("DefaultValue", "");
                CodeSignal = rec.GetString("CodeSignal");
            }
            if (CalcParamType == CalcParamType.Error)
                ErrMess += "Указан недопустимый тип расчетного параметра (" + rec.GetString("CalcParamType", "") + ");";

            UserExpr1 = rec.GetString("UserExpr1", "");
            if (UserExpr1 == "")
            {
                ErrMess += "Не заполнено расчетное выражение; ";
                _isFatalError = true;
            }
            UserExpr2 = rec.GetString("UserExpr2", "");
            if (UserExpr2 == "")
            {
                ErrMess += "Не заполнено управляющее выражение; ";
                _isFatalError = true;
            }
            
            InputsStr = rec.GetString("Inputs");

            SuperProcess = rec.GetString("SuperProcessType").ToSuperProcess();
            if (SuperProcess == SuperProcess.Error)
                ErrMess += "Указан недопустимый тип накопления (" + rec.GetString("SuperProcessType") + ");";
            if (rec.GetString("InterpolationType").ToInterpolation() == InterpolationType.Error)
                ErrMess += "Указан недопустимый тип интерполяции (" + rec.GetString("SuperProcessType") + ");";

            switch (CalcParamType)
            {
                case CalcParamType.HandBool:
                    if (DefaultValue != "0" && DefaultValue != "1")
                        ErrMess += "Недопустимое значение по умолчанию для параметра типа ВводЛогич";
                    break;
                case CalcParamType.HandInt:
                    int i;
                    if (!int.TryParse(DefaultValue, out i))
                        ErrMess += "Недопустимое значение по умолчанию для параметра типа ВводЦелое";
                    break;
                case CalcParamType.HandTime:
                    DateTime t;
                    if (!DateTime.TryParse(DefaultValue, out t))
                        ErrMess += "Недопустимое значение по умолчанию для параметра типа ВводВремя";
                    break;
                case CalcParamType.HandReal:
                    if (double.IsNaN(DefaultValue.ToDouble()))
                        ErrMess += "Недопустимое значение по умолчанию для параметра типа ВводДейств";
                    break;
            }

            if (Code != null)
            {
                if (!IsSubParam)
                {
                    FullCode = Code;
                    tablikCompiler.CalcParamsId.Add(CalcParamId, this);
                    if (!tablikCompiler.CalcParamsAll.ContainsKey(Code))
                        tablikCompiler.CalcParamsAll.Add(Code, this);
                    else if (CalcOn)
                        tablikCompiler.CalcParamsAll[Code] = this;
                    if (CalcOn)
                    {
                        if (tablikCompiler.CalcParams.ContainsKey(Code))
                        {
                            ErrMess += "Повтор кода расчетного параметра (" + Code + "); ";
                            tablikCompiler.CalcParams[Code].ErrMess += "Повтор кода расчетного параметра (" + Code + "); ";
                        }
                        else tablikCompiler.CalcParams.Add(Code, this);
                    }
                    Task = rec.GetString("Task");
                }
                else
                {
                    var owid = rec.GetInt("OwnerId");
                    Owner = tablikCompiler.CalcParamsId[owid];
                    CalcOn &= Owner.CalcOn;
                    FullCode = Owner.FullCode + "." + Code;
                    Owner.MethodsId.Add(CalcParamId, this);
                    if (!Owner.MethodsAll.ContainsKey(Code))
                        Owner.MethodsAll.Add(Code, this);
                    else if (CalcOn)
                        Owner.MethodsAll[Code] = this;
                    if (CalcOn)
                    {
                        if (Owner.Methods.ContainsKey(Code))
                        {
                            ErrMess += "Повтор кода расчетного параметра (" + Code + "); ";
                            Owner.Methods[Code].ErrMess += "Повтор кода расчетного параметра (" + Code + "); ";
                        }
                        else Owner.Methods.Add(Code, this);
                    }
                    Task = Owner.Task;
                }
            }
           
            var rs = rec.GetString("ReceiverCode", "");
            if (rs != "")
            {
                if (!InputsStr.IsEmpty() || (Owner != null && !Owner.InputsStr.IsEmpty()))
                    ErrMess += "Нельзя указывать сигнал приемника для функций и подпараметров функций";
                else if (!Tablik.Signals.ContainsKey(rs))
                    ErrMess += "Не найден указанный сигнал приемника (" + rs + ")";
                else if (Tablik.Signals[rs].ReceiverName.IsEmpty())
                    ErrMess += "Cигнал " + rs + ", указанный как сигнал приемника не является сигналом приемника. Возможно в настройках коммуникатора не указано имя приемника.";
                else
                {
                    var sig = Tablik.Signals[rs];
                    sig.InUse = true;
                    if (CalcOn) sig.InUseReceiver = true;
                    ReceiverSignal = sig;
                } 
            }

            Stage = CompileStage.NotStarted;
            CalcType = new CalcType(ClassType.Undef);
            StrongComponent = this;
            if (CalcOn)
            {
                StrongParams.Add(this);
                if (ErrMess != "") rec.Put("ErrMess", _errMess, true);
            }
        }

        //Разбор выражений, входных параметров и базовых классов
        public void Parse()
        {
            _curExpr = "парам.";
            ParseInputs();
            _curExpr = "расч.";
            ParseExpr(Exprs1, UserExpr1);
            _curExpr = "упр.";
            ParseExpr(Exprs2, UserExpr2);
            foreach (var met in _methodsId.Values)
                met.Parse();
        }

        //Формирует строку, содержащуюю описание лексемы
        private string LexInf(Lexeme lex)
        {
            if (lex.Token == null) return " ";
            return " " + lex.Token.RealText + ", " + _curExpr + "стр." + lex.Token.Line + ", поз." + lex.Token.Column + "; ";
        }
        private string LexInf(Token tok)
        {
            if (tok == null) return " ";
            return " " + tok.RealText + ", " + _curExpr + "стр." + tok.Line + ", поз." + tok.Column + "; ";
        }

        //Разбор поля Inputs
        private void ParseInputs()
        {
            if (InputsStr.IsWhiteSpace()) return;
            var c = new Compiller(InputsStr, CompillerType.Params);
            c.Parse();
            if (c.Error.Text != "")
            {
                ErrMess += ParseError(c);
                return;
            }
            var stack = new Stack<Var>();
            for (int n = 0; n < c.Lexemes.Count; n++)
            {
                var lex = c.Lexemes[n];
                switch (lex.Type)
                {
                    case LexemeType.Ident:
                        stack.Push(new Var(VarType.Input, lex.Text));
                        break;
                    case LexemeType.DataTypeP:
                        stack.Peek().CalcType = new CalcType(lex.Text.ToDataType());
                        break;
                    case LexemeType.Int:
                    case LexemeType.Real:
                    case LexemeType.Time:
                    case LexemeType.String:
                        stack.Peek().DefaultValue = lex.Text;
                        break;
                    case LexemeType.Fun:
                        var v = stack.Pop();
                        for (int i = 0; i < lex.ParamsCount - 1; i++)
                        {
                            var vs = stack.Pop();
                            if (!v.CalcType.InputSignals.ContainsKey(vs.Code))
                                v.CalcType.InputSignals.Add(vs.Code, vs.CalcType.DataType);
                            else ErrMess += "Сигнал входного параметра встречается дважды. Вход: " + v.Code + ", сигнал: " + vs.Code;
                        }
                        stack.Push(v);
                        break;
                    case LexemeType.ClassP:
                        if (Tablik.CalcParams.ContainsKey(lex.Text))//Расчетный параметр
                        {
                            var cp = Tablik.CalcParams[lex.Text];
                            if (n + 1 < c.Lexemes.Count)
                            {
                                lex = c.Lexemes[n+1];
                                while (lex.Type == LexemeType.ClassP)
                                {
                                    if (!cp.Methods.ContainsKey(lex.Text))
                                        ErrMess += "Не найден расчетный параметр" + LexInf(lex);
                                    else
                                        cp = cp.Methods[lex.Text];
                                    lex = c.Lexemes[++n+1];
                                }    
                            }
                            //var ct = new CalcType(ClassType.Undef);
                            var ct = new CalcType(ClassType.Calc);
                            ct.ParentParams.Add(cp);
                            stack.Peek().CalcType = ct;
                        }
                        break;
                }
            }
            while (stack.Count > 0)
                AddInput(stack.Pop());
        }

        //Добавить вход
        private void AddInput(Var v)
        {
            if (!Vars.ContainsKey(v.Code))
            {
                Vars.Add(v.Code, v);
                Inputs.Insert(0, v);
            }
            else ErrMess += "Входной параметр встречается дважды " + v.Code;
        }

        //Разбирает ошибку возвращенную парсером
        private string ParseError(Compiller c)
        {
            if (c.TokensDic.ContainsKey(c.Error.Position))
            {
                Token tok = c.TokensDic[c.Error.Position];
                string typ = "";
                switch (tok.Type)
                {
                    case TokenType.Int:
                    case TokenType.Real:
                    case TokenType.String:
                    case TokenType.Time:
                        typ = "константы";
                        break;
                    case TokenType.DataType:
                        typ = "типа данных";
                        break;
                    case TokenType.Ident:
                        typ = "идентификатора";
                        break;
                    case TokenType.KeyWord:
                        typ = "ключевого слова";
                        break;
                    case TokenType.Operation:
                        typ = "операции";
                        break;
                    case TokenType.Signal:
                        typ = "сигнала";
                        break;
                    case TokenType.Symbol:
                        typ = "символа";
                        break;
                    case TokenType.System:
                        typ = "функции";
                        break;
                }
                return "Недопустимое использование " + typ + LexInf(tok);
            }
            
            if (c.Error.Position == c.Expr.Length)
                return "Недопустимый синтаксис в конце выражения, возможно незакрыта скобка или незавершен оператор"
                          + LexInf(new Token("", TokenType.Comment, c.Error.Position, c.Error.Line, c.Error.Column));

            string st = (c.Error.Position + 10 < c.Expr.Length) ? c.Expr.Substring(c.Error.Position, 10) : c.Expr.Substring(c.Error.Position);
            var tk = new Token(st, TokenType.Comment, c.Error.Position, c.Error.Line, c.Error.Column) {RealText = st};
            return "Недопустимое выражение" + LexInf(tk);
        }

        //Разбор полей UserExpr1 (num=1) и UserExpr2 (num=2) 
        private void ParseExpr(List<ExprLexeme> exprs, string userExpr)
        {
            var c = new Compiller(userExpr, CompillerType.Expr);
            c.Parse();
            if (c.Error.Text != "")
            {
                ErrMess += ParseError(c);
                _isFatalError = true;
                return;
            }
            if (!Vars.ContainsKey("calc"))
                Vars.Add("calc", new Var(VarType.Var, "calc"));
            foreach (var lex in c.Lexemes)
            {
                switch (lex.Type)
                {
                    case LexemeType.Int: 
                        if (lex.Text == "0" || lex.Text == "1")
                            exprs.Add(new ExprLexeme(lex, new CalcType(DataType.Boolean, lex.Text), ExprType.Const));
                        else
                        {
                            int resi;
                            exprs.Add(new ExprLexeme(lex, new CalcType(DataType.Integer, lex.Text), ExprType.Const));
                            if (!int.TryParse(lex.Text, out resi))
                                ErrMess += "Недопустимое целое число" + LexInf(lex);
                        }
                        break;

                    case LexemeType.Real:
                        double resr;
                        exprs.Add(new ExprLexeme(lex, new CalcType(DataType.Real, lex.Text), ExprType.Const));
                        if(!double.TryParse(lex.Text,NumberStyles.Any, new NumberFormatInfo{NumberDecimalSeparator = "."}, out resr))
                            ErrMess += "Недопустимое число" + LexInf(lex);
                        break;

                    case LexemeType.Time:
                        DateTime resd;
                        exprs.Add(new ExprLexeme(lex, new CalcType(DataType.Time, lex.Text), ExprType.Const));
                        if (!DateTime.TryParse(lex.Text, out resd))
                            ErrMess += "Недопустимое дата" + LexInf(lex);
                        break;

                    case LexemeType.String:
                        exprs.Add(new ExprLexeme(lex, new CalcType(DataType.String, lex.Text), ExprType.Const));
                        if (lex.Text.Length > 255)
                            ErrMess += "Строковая констранта не должна иметь длину более 255 символов" + LexInf(lex);
                        break;

                    case LexemeType.Signal: //Сигналы
                        if (Tablik.Signals.ContainsKey(lex.Text))
                        {
                            Signal signal = Tablik.Signals[lex.Text];
                            signal.InUse = true;
                            if (CalcOn) signal.InUseSource = true;
                            signal.UsingParams.Add(Code, this);
                            lex.Text = signal.FullCode.ToLower();
                            exprs.Add(new ExprLexeme(lex, new CalcType(signal), ExprType.Signal));
                        }
                        else
                        {
                            exprs.Add(new ExprLexeme(lex, ExprType.Error));
                            ErrMess += "Не найден сигнал" + LexInf(lex);
                        }
                        break;

                    //Переменные
                    case LexemeType.Var:
                        Var v;
                        if (!Vars.ContainsKey(lex.Text))
                        {
                            v = new Var(VarType.Var, lex.Text);
                            Vars.Add(lex.Text, v);
                        }
                        else v = Vars[lex.Text];
                        exprs.Add(new ExprLexeme(lex, new CalcType(v), ExprType.Var));
                        break;

                    case LexemeType.Fun: //Функции
                        exprs.Add(new ExprLexeme(lex, Tablik.Funs[lex.Text]));
                        break;

                    case LexemeType.Op://Ключевые слова
                        exprs.Add(new ExprLexeme(lex, Tablik.Funs[lex.Text], ExprType.Op));
                        break;

                    case LexemeType.Met: //Методы
                        exprs.Add(new ExprLexeme(lex, ExprType.Met));
                        break;

                    case LexemeType.PrevM:
                        exprs.Add(new ExprLexeme(lex, ExprType.PrevM));
                        break;

                    case LexemeType.Ident: //Идентификатор
                    case LexemeType.Prev:
                        ExprLexeme el = null;
                        if (Tablik.Funs.ContainsKey(lex.Text)) //Встроенная функция
                            el = new ExprLexeme(lex, Tablik.Funs[lex.Text]);
                        else if (Tablik.Grafics.ContainsKey(lex.Text)) //График
                        {
                            var gr = Tablik.Grafics[lex.Text];
                            el = new ExprLexeme(lex, new CalcType(DataType.Real), ExprType.Grafic) {ParamString = ""};
                            for (int i = 1; i < gr.Dimension; i++)
                                el.ParamString += ",%r";
                            gr.UsingParams.Add(Code, this);
                        }
                        //Переменные и подпараметры
                        bool isFound = el != null;
                        CalcParam calc = this;
                        while (calc != null && el == null)
                        {
                            if (calc.Vars.ContainsKey(lex.Text))
                            {
                                el = new ExprLexeme(lex, calc.Vars[lex.Text]);
                                isFound = true;
                            }
                            else 
                            {
                                if (calc.MethodsAll.ContainsKey(lex.Text))
                                    el = new ExprLexeme(lex, calc.MethodsAll[lex.Text], lex.Type == LexemeType.Prev);
                                isFound |= calc.Methods.ContainsKey(lex.Text);
                            }
                            calc = calc.Owner;
                        }
                        if (!isFound && Tablik.CalcParamsAll.ContainsKey(lex.Text)) //Расчетный параметр
                            el = new ExprLexeme(lex, Tablik.CalcParamsAll[lex.Text], lex.Type == LexemeType.Prev);
                        isFound |= Tablik.CalcParams.ContainsKey(lex.Text);
                        if (el == null)
                            exprs.Add(new ExprLexeme(lex, ExprType.Error));
                        else exprs.Add(el);
                        //exprs.Add(el ?? new ExprLexeme(lex, ExprType.Error));
                        if (!isFound)
                        {
                            if (el == null) ErrMess += "Неизвестный параметр, функция или переменная" + LexInf(lex);
                            else ErrMess = "Использование выключенного из расчета параметра" + LexInf(lex);
                        }
                        break;
                }
            }
        }

        //Компиляция параметра, caller - параметр из которого вызван данный
        public void Compile(CalcParam caller)
        {
            if (_isFatalError) return;
            _mustBeReturned.Clear();
            Stage = CompileStage.Started;
            Caller = caller;
            _curExpr = "расч.";
            Compile(Exprs1);
            _curExpr = "упр.";
            Compile(Exprs2);
            if (this == StrongComponent && Stage == CompileStage.Changed && ErrMess == "") 
                Compile(Caller);
            Stage = CompileStage.Finished;
            foreach (var d in MethodsId.Values)
                if (d.Stage == CompileStage.NotStarted)
                    d.Compile((CalcParam)null);
            if (Number == 0 && CalcOn)
                Number = ++Tablik.CalcParamNumber;
        }

        //Компиляция параметра, exprs - список лексем
        private void Compile(List<ExprLexeme> exprs)
        {
            var stack = new Stack<CalcType>();//Стек
            var par = new List<CalcType>();//Список параметров для вызова текущей функции
            foreach (ExprLexeme exp in exprs)
            {
                par.Clear();
                if (stack.Count < exp.Lexeme.ParamsCount)
                {
                    ErrMess += "Ошибка" + LexInf(exp.Lexeme);
                    break;
                }
                for (int i = 0; i < exp.Lexeme.ParamsCount; i++)
                    par.Insert(0, stack.Pop());
                switch (exp.ExprType)
                {
                    case ExprType.Const:
                    case ExprType.Signal:
                    case ExprType.Var:
                        stack.Push(exp.CalcType);
                        break;
                    case ExprType.VarUse:
                        if (exp.CalcType.ClassType != ClassType.Undef || exp.CalcType.ParentParam != null)
                            stack.Push(exp.CalcType);
                        else ErrMess += "Использование неприсвоенной переменной" + LexInf(exp.Lexeme);
                        break;

                    case ExprType.Grafic:
                        var cg = new CalcType(DataType.Real);
                        bool eg = true;
                        foreach (var p in par)
                            eg &= p.LessOrEquals(cg);
                        if (Tablik.Grafics[exp.Lexeme.Text].Dimension == par.Count + 1 && eg)
                            stack.Push(exp.CalcType);
                        else ErrMess += "Недопустимые параметры графика" + LexInf(exp.Lexeme);
                        break;

                    case ExprType.Fun:
                        stack.Push(CheckFun(exp, par));
                        break;

                    case ExprType.Calc:
                    case ExprType.Prev:
                        stack.Push(CheckCalc(exp, par));
                        break;

                    case ExprType.Met:
                    case ExprType.PrevM:
                        if (par[0].ParentParam == null)
                        {
                            exp.ExprType = ExprType.Error;
                            ErrMess += "Недопустимое получение подпараметра" + LexInf(exp.Lexeme);
                            stack.Push(new CalcType(ClassType.Error));
                            break;
                        }
                        var mpar = new List<CalcType>();
                        for (int i = 1; i < par.Count; i++) mpar.Add(par[i]);
                        CalcParam cp = null;
                        bool isOn = false;
                        foreach (var pp in par[0].ParentParams)
                        {
                            if (cp == null && !isOn && pp.MethodsAll.ContainsKey(exp.Lexeme.Text))
                                cp = pp.MethodsAll[exp.Lexeme.Text];
                            isOn |= pp.Methods.ContainsKey(exp.Lexeme.Text);
                        }
                        if (cp == null)
                        {
                            exp.ExprType = ExprType.Error;
                            ErrMess += "Не найден подпараметр" + LexInf(exp.Lexeme);
                            stack.Push(new CalcType(ClassType.Error));
                        }
                        else
                        {
                            exp.CalcParam = cp;
                            stack.Push(CheckCalc(exp, mpar));
                            if (!isOn) ErrMess += "Использование отключенного подпараметра" + LexInf(exp.Lexeme);
                        }
                        break;

                    case ExprType.Error:
                        stack.Push(new CalcType(ClassType.Error));
                        break;

                    case ExprType.Op:
                        switch (exp.Lexeme.Text)
                        {
                            case "void":
                                exp.CalcTypeConst = new CalcType(ClassType.Void);
                                break;

                            case "hand":
                                if (par.Count > 0)
                                {
                                    ErrMess += "Функция не может иметь параметров" + LexInf(exp.Lexeme);
                                    stack.Push(new CalcType(ClassType.Error));
                                }
                                else
                                {
                                    DataType dt = CalcParamType.HandDataType();
                                    if (dt == DataType.Error)
                                        ErrMess += "Функция " + LexInf(exp.Lexeme) + " может использоваться только в параметрах ручного ввода";
                                    else if (Tablik.HandInputSource.IsEmpty())
                                        ErrMess += "Для использования функции " + LexInf(exp.Lexeme) + " в списке провайдеров должен присутствовать источник ручного ввода";
                                    else
                                    {
                                        exp.ExprType = ExprType.HandSignal;
                                        exp.Lexeme.Text = Code + ".руч";
                                        if (!Tablik.HandSignals.ContainsKey(exp.Lexeme.Text))
                                        {
                                            var sig = new Signal(Code, dt, Tablik.HandInputSource);
                                            Tablik.HandSignals.Add(sig.FullCode, sig);
                                        }    
                                    }
                                    exp.CalcTypeConst = new CalcType(dt);
                                    stack.Push(exp.CalcTypeConst);
                                }
                                break;

                            case "owner":
                                if (Owner == null)
                                {
                                    ErrMess += "Данный расчетный параметр не имеет владельца, нельзя использовать ключевое слово" + LexInf(exp.Lexeme);
                                    exp.CalcTypeConst = new CalcType(ClassType.Error);
                                }
                                else
                                {
                                    exp.CalcTypeConst = Owner.CalcType;
                                    stack.Push(Owner.CalcType);
                                }
                                break;

                            case "signalbool":
                            case "signalint":
                            case "signalreal":
                            case "signalstring":
                                if (par.Count != 1 || !par[0].LessOrEquals(new CalcType(DataType.String)))
                                {
                                    ErrMess += "Недопустимый параметр функции " + LexInf(exp.Lexeme);
                                    exp.CalcTypeConst = new CalcType(ClassType.Error);
                                }
                                else
                                {
                                    exp.CalcTypeConst = new CalcType(exp.Lexeme.Text.Substring(6).ToDataType());
                                    stack.Push(exp.CalcTypeConst);
                                }
                                exp.ParamString = ",%v";
                                break;

                            case "takecode":
                            case "takename":
                            case "takeunits":
                            case "taketask":
                            case "takecomment":
                            case "takecodesignal":
                            case "takenamesignal":
                                if (par.Count > 1 || (par.Count == 0 && (exp.Lexeme.Text == "takecodesignal" || exp.Lexeme.Text == "takenamesignal")))
                                {
                                    ErrMess += "Недопустимые параметры функции " + LexInf(exp.Lexeme);
                                    exp.CalcTypeConst = new CalcType(ClassType.Error);
                                }
                                exp.ParamString = par.Count == 0 ? "" : ",%v";
                                exp.CalcTypeConst = new CalcType(DataType.String);
                                stack.Push(exp.CalcTypeConst);
                                break;

                            case "=":
                                CheckAssign(exp, par);
                                break;

                            case "assigna"://присваивается assigni или assigns
                                CheckAssign(exp, par);
                                break;

                            case "assignr":
                                var cv = par[0];
                                par[0] = par[1];
                                par[1] = cv;
                                CheckAssign(exp, par);
                                break;

                            case "assignv":
                                par.Add(new CalcType(ClassType.Void));
                                CheckAssign(exp, par);
                                break;

                            case "signal":
                                CalcType ct = CheckGetSignal(exp, par);
                                exp.CalcTypeConst = ct;
                                stack.Push(ct);
                                break;

                            case "[]":
                                ct = par[0].GetIndex(par[1]);
                                if (ct.ClassType == ClassType.Max)
                                {
                                    ErrMess += "Недопустимое получение элемента массива" + LexInf(exp.Lexeme);
                                    ct = new CalcType(ClassType.Error);
                                }
                                exp.CalcTypeConst = ct;
                                stack.Push(ct);
                                exp.ParamString = ",var," + par[1].IndexType.ToLetter();
                                break;

                            case "prev":
                            case "prevabs":
                            case "prevhour":
                            case "prevday":
                            case "prevperiod":
                            case "prevhourperiod":
                            case "prevdayperiod":
                            case "prevmom":
                                stack.Push(CheckPrev(exp, par));
                                break;

                            default://Операторы if, for и while всех видов
                                ct = CheckFun(exp, par);
                                if (ct.ClassType != ClassType.Void)
                                    stack.Push(ct);
                                break;
                        }
                        break;
                }
            }
            if (_curExpr == "расч." && exprs[exprs.Count - 1].Lexeme.Text != "assignr" && exprs[exprs.Count - 1].Lexeme.Text != "assignv")
            {
                exprs.Add(new ExprLexeme(new Lexeme(null, "calc", LexemeType.Var, 0), new CalcType(Vars["calc"]), ExprType.Var));
                exprs.Add(stack.Count == 0 ? new ExprLexeme(new Lexeme(null, "assignv", LexemeType.Op, 1), Tablik.Funs["assignv"], ExprType.Op) 
                                                          : new ExprLexeme(new Lexeme(null, "assignr", LexemeType.Op, 2), Tablik.Funs["assignr"], ExprType.Op));
                par.Clear();
                par.Add(new CalcType(Vars["calc"]));
                par.Add(stack.Count == 0 ? new CalcType(ClassType.Void) : stack.Pop());
                CheckAssign(exprs[exprs.Count - 1], par);   
            }
            if (_curExpr == "упр.") CompileFinish(stack);
        }
       
        public void CompileFinish(Stack<CalcType> stack)
        {
            //Получение типа данных
            CalcType = stack.Count > 0 ? stack.Pop().Clone() : new CalcType(ClassType.Void);
            if (_mustBeReturned.Count >= 2 || (_mustBeReturned.Count == 1 && (CalcType.ParentParam == null || _mustBeReturned.Keys.First().Code.ToLower() != CalcType.ParentParam.Code.ToLower())))
                ErrMess += "Использование ненакапливаемого параметра-функции, содержащего подпараметры, на которые ссылаются функции получения предыдущих значений " 
                                        + _mustBeReturned.Aggregate("", (current, p) => current + (LexInf(p.Value)));
            if (ErrMess == "" && StrongComponent.Stage != CompileStage.Changed)
            {
                if (ErrMess == "" && (CalcType.ClassType.LessOrEqual(ClassType.Error) || CalcType.ClassType == ClassType.Max))
                    ErrMess = "Не удается определить тип данных параметра; ";
                if (!SuperProcess.IsNone() && ErrMess == "")
                {
                    if (CalcType.ClassType != ClassType.Single)
                        ErrMess += "Тип накопления не может быть указан для параметров типа объект или пустой (void); ";
                    else if (!CalcType.DataType.LessOrEquals(DataType.Real) && (SuperProcess.ToProcess() == SuperProcess.AverageP || SuperProcess.ToProcess() == SuperProcess.AvNonZeroP))
                        ErrMess += "Недопустимый тип накопления для параметра типа строка или время; ";
                    else if (CalcType.DataType == DataType.Time && SuperProcess.ToProcess() == SuperProcess.SummP)
                        ErrMess += "Недопустимый тип накопления для параметра типа время; ";
                }
                if (ReceiverSignal != null && !CalcType.LessOrEquals(new CalcType(ReceiverSignal.DataType)))
                    ErrMess += "Недопустимый тип данных параметра для указанного сигнала приемника; ";
            }
            
            //Склейка выражений в одно и добавление значений по умолчанию
            Exprs.Clear();
            AddToOneExpr(Exprs1);
            AddToOneExpr(Exprs2);
            AddLinkAdresses();
        }

        private void AddToOneExpr(IEnumerable<ExprLexeme> exprs)
        {
            foreach (var expr in exprs)
            {
                if (expr.Defaults != null)
                    foreach (var d in expr.Defaults)
                        Exprs.Add(d);
                Exprs.Add(expr);
            }
        }

        //Добавляет ссылки по If
        private void AddLinkAdresses()
        {
            var stack = new Stack<int>();
            var stackw = new Stack<int>();
            int i = 0;
            while (i < Exprs.Count)
            {
                if (Exprs[i].ExprType == ExprType.Op)
                {
                    var t = Exprs[i].Lexeme.Text;
                    if (t == "begin") stackw.Push(i);
                    if ((t+"#").Substring(0, 2) == "if" || t == "while" || t == "whilep" || t == "for")
                        stack.Push(i);
                    if (t == "else" || t == "elsev")
                    {
                        Exprs[stack.Pop()].LinkAddress = i;
                        stack.Push(i);
                    }
                    if (t == "ret")
                    {
                        if (Exprs[stack.Peek()].Lexeme.Text == "for")
                        {
                            Exprs[stack.Peek()].LinkAddress = i+1;
                            Exprs[i].LinkAddress = stack.Pop();
                        }
                        else Exprs[i].LinkAddress = stackw.Pop() + 1;
                    }
                    if (t == "end") Exprs[stack.Pop()].LinkAddress = i;
                }
                i++;
            }
        }

        //Обработка присвоения 
        private void CheckAssign(ExprLexeme exp, List<CalcType> par)
        {
            exp.CalcTypeConst = new CalcType(ClassType.Void);
            exp.ParamString = ",var";
            if (exp.Lexeme.Text != "assignv")
                for (int i = 1; i < par.Count; i++)
                    exp.ParamString += "," + WriteType(par[i]);
            var v = par[0].Var;
            var r = par.Last();
            bool isarr = exp.Lexeme.Text == "assigna";
            if (isarr)
            {
                r = r.AddIndex(par[1]);
                exp.Lexeme.Text = par[1].DataType.LessOrEquals(DataType.Integer) ? "assigni" : "assigns";
            }
            if (!r.LessOrEquals(v.CalcType) || r.ParentParam != null)
            {
                v.CalcType = v.CalcType.Add(r, true);
                if (v.CalcType.ClassType == ClassType.Max)
                    ErrMess += (isarr ? "Недопустимое получение элемента массива для присвоения" 
                                                : "Присвоение переменной значения недопустимого типа") + LexInf(exp.Lexeme);
            }
        }

        //Обработка функции Сигнал
        private CalcType CheckGetSignal(ExprLexeme exp, List<CalcType> par)
        {
            if (par[0].Signal == null && par[0].InputSignals.Count == 0)
            {
                ErrMess += "Применение функции получения сигнала к выражению, не являющемуся сигналом" + LexInf(exp.Lexeme);
                return new CalcType(ClassType.Error);
            }
            if (par[1].Text == null || (par[0].Signal != null && !par[0].Signal.ObjectSignal.Signals.ContainsKey(par[1].Text)) || (par[0].Signal == null &&!par[0].InputSignals.ContainsKey(par[1].Text)))
            {
                ErrMess += "Не найден или не задан сигнал " + par[1].Text + "," + LexInf(exp.Lexeme);
                return  new CalcType(ClassType.Error);
            }

            exp.ParamString = ",%v,%s";
            if (par[0].Signal != null)
            {
                var sig = par[0].Signal.ObjectSignal.Signals[par[1].Text];
                sig.InUse = true;
                if (CalcOn) sig.InUseSource = true;
                return new CalcType(sig);
            }
            return new CalcType(par[0].InputSignals[par[1].Text]) {InputSignals = par[0].InputSignals};
    }

        //Обрабока лексемы - функции
        private CalcType CheckFun(ExprLexeme exp, List<CalcType> par)
        {
            var funClass = exp.FunOverload.Owner;
            exp.Defaults = new List<ExprLexeme>();
            foreach (var ov in funClass.Overloads)
            {
                //Проверка, подходит ли перегрузка
                int i, k = 0;
                for (i = 0; i < par.Count; i++)
                {
                    if (i < ov.Inputs.Count)
                    {
                        if (!par[i].LessOrEquals(ov.Inputs[i].CalcType)) break;
                    }
                    else
                    {
                        if (ov.InputsMore.Count > 0)
                        {
                            if (!par[i].LessOrEquals(ov.InputsMore[k].CalcType)) break;
                            k = (k + 1) % ov.InputsMore.Count;
                        }
                        else break;
                    }
                }
                if (i == par.Count)
                {
                    //Если данная перегрузка подходит
                    if (i == ov.Inputs.Count || (i < ov.Inputs.Count && ov.Inputs[i].Default != null) || (i > ov.Inputs.Count && ov.InputsMore.Count > 0 && k == 0))
                    {
                        if (exp.FunOverload != ov)
                        {
                            StrongComponent.Stage = CompileStage.Changed;
                            exp.FunOverload = ov;
                        }
                        exp.ParamString = "";
                        exp.CalcTypeConst = new CalcType(ClassType.Error);
                        for (int j = 0; j < ov.Inputs.Count; j++)
                        {
                            var inp = ov.Inputs[j];
                            exp.ParamString += "," + WriteType(inp.CalcType);
                            if (j < par.Count)
                            {
                                if (ov.IsCombined && inp.FormResult)
                                    exp.CalcTypeConst = exp.CalcTypeConst.Add(par[j]);
                            }
                            else
                            {
                                exp.Defaults.Add(new ExprLexeme(new Lexeme(null, inp.Default, LexemeType.String, 0), inp.CalcType, ExprType.Const));
                                if (ov.IsCombined && inp.FormResult)
                                    exp.CalcTypeConst = exp.CalcTypeConst.Add(inp.CalcType);
                            } 
                        }
                        k = 0;
                        for (int j = ov.Inputs.Count; j < par.Count; j++)
                        {
                            var inpm = ov.InputsMore[k];
                            exp.ParamString += ",$" + WriteType(inpm.CalcType);
                            if (ov.IsCombined && inpm.FormResult)
                                exp.CalcTypeConst = exp.CalcTypeConst.Add(par[j]);
                            k = (k + 1) % ov.InputsMore.Count;
                        }
                        if (!ov.IsCombined) exp.CalcTypeConst = ov.Result.Clone();
                        CheckArr(exp, par, exp.CalcTypeConst);
                        return exp.CalcTypeConst;
                    }
                }
            }
            ErrMess += "Недопустимые типы параметров функции" + LexInf(exp.Lexeme);
            return new CalcType(ClassType.Error);
        }

        //Цепочка кода параметра ниже ближайшей функции-родителя или от начала, если такой нет
        private string _prevChain;
        public string PrevChain 
        { 
            get 
            {
                if (_prevChain != null) return _prevChain;
                string s = Code;
                CalcParam calc = Owner;
                while (calc != null && calc.Inputs.Count == 0)
                {
                    s = calc.Code + "." + s;
                    calc = calc.Owner;
                }
                _prevChain = s;
                return s;
            }
        }

        //Обработка функций Пред
        private CalcType CheckPrev(ExprLexeme exp, List<CalcType> par)
        {
            string s = exp.Lexeme.Text;
            var cmin = new CalcType(ClassType.Error);
            exp.CalcTypeConst = cmin;
            var c = par[0].ParentParam;
            exp.ParamString = ",calc";
            string serr = "Недопустимые типы параметров функции" + LexInf(exp.Lexeme);
            if ((par.Count != 2 && (s != "prevabs" || par.Count != 3)) || c == null || !c.CalcType.LessOrEquals(new CalcType(DataType.String)) || !par[1].ClassType.LessOrEqual(ClassType.Single))
            {
                ErrMess += serr;
                return cmin;
            }
            var cct = c.CalcType.Clone();
            if (s == "prevabs" || s == "prev" || s == "prevday" || s == "prevhour")
            {
                cct = cct.Add(par[1]);
                exp.ParamString += "," + WriteType(par[1]);
            }
            bool b = false;
            switch (s)
            {
                case "prevabs":
                    if (par.Count == 3)
                    {
                        if (par[2].LessOrEquals(new CalcType(DataType.Boolean)))
                            exp.ParamString += ",%b";
                        else b = true;
                    }
                    break;
                case "prevmom":
                case "prevperiod":
                case "prevhourperiod":
                case "prevdayperiod":
                    if (par[1].LessOrEquals(new CalcType(DataType.Real)))
                        exp.ParamString += ",%r";
                    else b = true;
                    break;
            }
            if (b)
            {
                ErrMess += serr;
                return cmin;
            }
            
            var sp = c.SuperProcess;
            if (sp.IsNone() || (sp != SuperProcess.Moment && s == "prevmom") || (!sp.IsAbsolute() && s == "prevabs") || (!sp.IsPeriodic() && (s != "prevmom" && s != "prevabs")))
            {
                ErrMess += "Недопустимый тип накопления у параметра " + c.Code + " для использования функции" + LexInf(exp.Lexeme);
                return cmin;
            }
            if (s == "prevhour" || s == "prevhourperiod" || s == "prevday" || s == "prevdayperiod" || s == "prevabs")
                cct = new CalcType(c.CalcType.DataType.AplySuperProcess(c.SuperProcess)).Add(par[1]);
            cct.ParentParams.Clear();

            string sc = c.Code;
            c = c.Owner;
            while (c != null && c.Inputs.Count == 0)
            {
                sc = c.Code + "." + sc;
                c = c.Owner;
            }

            if (CalcOn)
            {
                var p = new Prev(sc);
                p.PrevAbs = s == "prevabs";
                p.ManyMoments = s == "prevmom";
                p.LastBase = s == "prev";
                p.ManyBase = s == "prevperiod";
                p.LastHour = s == "prevhour";
                p.ManyHour = s == "prevhourperiod";
                p.LastDay = s == "prevday";
                p.ManyDay = s == "prevdayperiod";
                var dic = c == null ? Tablik.Prevs : c.Prevs;
                if (!dic.ContainsKey(sc))
                {
                    dic.Add(sc, p);
                    Stage = CompileStage.Changed;
                }
                else dic[sc] = p.Clone(sc, dic[sc]);   
            }
            exp.CalcTypeConst = cct;
            return cct;
        }

        //Обрабока лексемы - расчетного параметра
        private CalcType CheckCalc(ExprLexeme exp, List<CalcType> par)
        {
            var calc = exp.CalcParam ;
            calc.UsingParams.Add(Code, this);
            //Добавка в ParamsCalls
            if ((exp.ExprType == ExprType.Calc || exp.ExprType == ExprType.Met) && Inputs.Count == 0 && calc.Inputs.Count == 0 && !ParamCalls.Contains(calc))
                ParamCalls.Add(calc);   
            
            //Проверка типов входов
            bool b = false;
            for (int i = 0; i < par.Count; i++)
            {
                b |= i >= calc.Inputs.Count || !par[i].LessOrEquals(calc.Inputs[i].CalcType, true);
                if (!b && par[i].Signal != null) //Добавление сигналов для входа с указаными сигналами
                    foreach (var cs in calc.Inputs[i].CalcType.InputSignals.Keys)
                    {
                        var sig = par[i].Signal.ObjectSignal.Signals[cs];
                        sig.InUse = true;
                        if (CalcOn) sig.InUseSource = true;
                    }
            }
            if (b || (calc.Inputs.Count > par.Count && calc.Inputs[par.Count].DefaultValue == null))
            {
                ErrMess += "Недопустимые типы входов расчетного параметра" + LexInf(exp.Lexeme);
                return new CalcType(ClassType.Error);
            }
            if ((exp.ExprType == ExprType.Prev || exp.ExprType == ExprType.PrevM) && par.Count > 0)
            { 
                ErrMess += "Параметр функции получения предыдущих значений не может являться функцией" + LexInf(exp.Lexeme);
                return new CalcType(ClassType.Error);
            }

            //Добавление значений по умолчанию
            exp.ParamString = "";
            exp.Defaults = new List<ExprLexeme>();
            for (int i = 0; i < calc.Inputs.Count; i++)
            {
                var inp = calc.Inputs[i];
                exp.ParamString += "," + WriteType(inp.CalcType);
                if (i >= par.Count)
                    exp.Defaults.Add(new ExprLexeme(new Lexeme(null, inp.DefaultValue, LexemeType.String, 0), inp.CalcType, ExprType.Const));
            }
            
            var sc = calc.StrongComponent;
            //Компиляция вызываемого параметра
            if (calc.Stage == CompileStage.NotStarted || (calc.ErrMess == "" && calc.Stage == CompileStage.Finished && sc == StrongComponent && calc.Caller == this))
                calc.Compile(this);
            //Склейка компонент связности
            if ((sc.Stage == CompileStage.Started || sc.Stage == CompileStage.Changed) && sc != StrongComponent)
            {
                var cc = this;
                while (cc != sc)
                {
                    if (cc.StrongComponent != sc)
                        foreach (var p in cc.StrongParams)
                        {
                            p.StrongComponent = sc;
                            calc.StrongParams.Add(p);
                        }
                    cc = cc.Caller;
                }
                sc.Stage = CompileStage.Changed;
            }

            //Перенос параметров Пред
            if (calc.Inputs.Count > 0 && calc.Prevs.Count > 0)
            {
                var c = this;
                _mustBeReturned.Add(calc, exp.Lexeme);
                while (c != null && c.Inputs.Count > 0) c = c.Caller;
                if (c != null)
                {
                    string pr = "";
                    while (c != null && c.Inputs.Count == 0)
                    {
                        pr = c.Code + (pr == "" ? "" : ".") + pr;
                        c = c.Owner;
                    }
                    foreach (var p in calc.Prevs.Values)
                    {
                        string pcode = pr + "." + p.Code;
                        var po = p.Clone(pr + "." + p.Code);
                        var dic = c == null ? Tablik.Prevs : c.Prevs;
                        if (!dic.ContainsKey(po.Code))
                            dic.Add(pcode, p.Clone(pr + "." + p.Code));
                        else dic[pcode] = p.Clone(pcode, dic[po.Code]);
                    }
                }   
            }
            
            var ctres = calc.CalcType.Clone();
            if (ctres.ParentParam != null && ctres.ParentParam.Inputs.Count == 0)
                ctres.ParentParams.Clear();
            ctres.ParentParams.Add(calc);
            return ctres;
        }

        //Определение размерности массива результата функции
        private void CheckArr(ExprLexeme exp, List<CalcType> par, CalcType ct)
        {
            var f = exp.FunOverload.Owner;
            if (f.Type != "array")//Не массивы
            {
                if (par.Count == 0) return;
                bool isint = false, isstring = false;
                foreach (CalcType t in par)
                {
                    isint |= t.IndexType == DataType.Integer;
                    isstring |= t.IndexType == DataType.String;
                }
                if (isint && isstring)
                    ErrMess += "Параметры функции имеют различные типы индексов массивов" + LexInf(exp.Lexeme);
                else if (isint) ct.IndexType = DataType.Integer;
                if (isstring) ct.IndexType = DataType.String;
            }
            else//Массивы
            {
                bool e = true;
                switch (f.Code)
                {
                    case "size":
                        break;
                    case "array":
                    case "arraybynumbers":
                    case "arraybystrings":
                        foreach (CalcType t in par)
                            if (t.IndexType != DataType.Value)
                                ErrMess += "Не допускается формирование массива из массивов" + LexInf(exp.Lexeme);
                        ct.IndexType = f.Code == "arraybystrings" ? DataType.String : DataType.Integer;
                        break;
                    case "deleteelement":
                    case "containsindex":
                        e = par[1].IndexType == DataType.Value && par[0].IndexType != DataType.Value && par[1].DataType.LessOrEquals(par[0].IndexType);
                        ct.IndexType = f.Code == "deleteelement" ? par[0].IndexType : DataType.Value;
                        break;
                    case "strsplit":
                        ct.IndexType = DataType.Integer;
                        break;
                    case "for":
                        if (par[1].IndexType == DataType.Value|| (!par[0].Var.CalcType.ClassType.LessOrEqual(ClassType.Error) && !par[0].Var.CalcType.DataType.LessOrEquals(par[1].IndexType)))
                            ErrMess += "Недопустимые параметры цикла Для(For)" + LexInf(exp.Lexeme);
                        par[0].Var.CalcType = new CalcType(par[1].IndexType);
                        break;
                }
                if (!e) ErrMess += "Недопустимые параметры функции" + LexInf(exp.Lexeme);
            }
        }

        //Проверка на циклические ссылки
        public void CheckCycleLinks(CalcParam caller)
        {
            if (Inputs.Count == 0)
            {
                Stage = CompileStage.Started;
                Caller = caller;
                foreach (var p in ParamCalls)
                {
                    if (p.Stage == CompileStage.Started)
                    {
                        ErrMess += "Циклическая ссылка на параметр " + p.FullCode + "; ";
                        var c = this;
                        while (c != p)
                        {
                            c.Caller.ErrMess += "Циклическая ссылка на параметр " + c.FullCode + "; ";
                            c = c.Caller;
                        }
                    }
                    else if (p.Stage == CompileStage.NotStarted)
                        p.CheckCycleLinks(this);
                }
                Stage = CompileStage.Finished;
            }
        }

        //Переводит CalcType в строку
        private static string WriteType(CalcType type, bool asLetter = true)
        {
            if (type == null) return "error";
            switch (type.ClassType)
            {
                case ClassType.Void:
                    return "void";
                case ClassType.Var:
                    return "var";
                case ClassType.Single:
                    if (asLetter) return "%" + type.DataType.ToLetter();
                    return type.DataType.ToRussian();
            }
            return "error";
        }

        //Добавляет элемент для записи в UsedUnits
        private void AddUsed(IDictionary<string, string> used, string rcode, string type, string pars = null)
        {
            if (rcode.Contains("|") || rcode.Contains("=")) return;
            if (!used.ContainsKey(rcode))
            {
                if (pars == null) used.Add(rcode, type);
                else used.Add(rcode, type + "(" + pars + ")");
            }
        }

        //Запись скомпилированных выражений в таблицу
        public void SaveCompile(RecDao rec)
        {
            if (CalcOn)
            {
                rec.Put("ErrMess", ErrMess, true);
                if (!rec.GetString("ErrMess").IsEmpty() || !ErrMess.IsEmpty())
                    Tablik.ErrorsCount++;
            }
            if (ReceiverSignal != null) rec.Put("ReceiverCode", ReceiverSignal.FullCode);
            
            rec.Put("CalcNumber", Number);
            switch (CalcType.ClassType)
            {
                case ClassType.Single:
                    rec.Put("ResultType", CalcType.DataType.ToRussian());
                    break;
                case ClassType.Void:
                    rec.Put("ResultType", "void");
                    break;
                case ClassType.Error:
                case ClassType.Undef:
                    rec.Put("ResultType", "error");
                    break;
            }
            if (!IsSubParam) 
                rec.Put("SubParamsCount", MethodsAll.Count + (Methods.Count == MethodsAll.Count ? "" : (", включено " + Methods.Count)) );

            var used = new SortedDictionary<string, string>();
            string firstSignal = null;
            bool isOldSignal = false;
            string s = Vars.Count + ";" + Inputs.Count + ";";
            foreach (var v in Inputs)
                s += v.Code + "(" + WriteType(v.CalcType) + ");";
            foreach (var v in Vars.Values)
                if (v.VarType != VarType.Input)
                    s += v.Code + "(" + WriteType(v.CalcType) + ");";
            s += Exprs.Count + ";";
            foreach (var lex in Exprs)
            {
                var ct = lex.CalcType;
                var ltext = lex.Lexeme.Text;
                switch (lex.ExprType)
                {
                    case ExprType.Const:
                        if (ct.DataType == DataType.String)
                            s += "const!'" + ltext + "'(" + WriteType(ct);
                        else
                            s += "const!" + ltext + "(" + WriteType(ct);
                        break;
                    case ExprType.Signal:
                        s += "signal!{" + ltext + "}(" + WriteType(ct);
                        if (Tablik.Signals.ContainsKey(ltext))
                        {
                            var sig = Tablik.Signals[ltext];
                            AddUsed(used, "{" + sig.FullCode + "}", sig.Default ? "Объект" : "Сигнал");
                            firstSignal = firstSignal ?? sig.FullCode;
                            isOldSignal |= sig.FullCode == CodeSignal;
                        }
                        break;
                    case ExprType.HandSignal:
                        s += "handsignal!{" + ltext + "}(" + WriteType(ct);
                        break;
                    case ExprType.Var:
                        s += "var!" + ltext + "(" + WriteType(ct.Var.CalcType);
                        if (ltext != "calc") AddUsed(used, ltext, "Переменная", WriteType(ct.Var.CalcType, false));
                        break;
                    case ExprType.VarUse:
                        s += "varuse!" + ltext + "(" + WriteType(lex.Var.CalcType);
                        string sv = lex.Var.VarType == VarType.Input ? "Вход" : "Переменная";
                        if (!Vars.ContainsKey(ltext) && (Owner != null && Owner.Vars.ContainsKey(ltext)))
                            sv += " владельца";
                        if (ltext != "calc") AddUsed(used, ltext, sv, WriteType(lex.Var.CalcType, false));
                        break;
                    case ExprType.Calc:
                    case ExprType.Prev:
                    case ExprType.PrevM:
                        if (lex.CalcParam == null)
                            s += "calc!" + ltext + "(" + WriteType(ct);
                        else
                        {
                            if (lex.ExprType == ExprType.Calc)
                                s += "calc!" + lex.CalcParam.Code + "(" + WriteType(ct) + lex.ParamString;
                            else s += "prev!" + lex.CalcParam.Code + "(" + WriteType(ct) + (lex.ExprType == ExprType.Prev ? "" : ",calc") + lex.ParamString;
                            bool b = lex.CalcParam.Owner == null;
                            sv = b ? "Параметр" : "Подпараметр";
                            if (lex.CalcParam.Inputs.Count > 0) sv += "-функция";
                            if (b) AddUsed(used, lex.CalcParam.Code, sv);
                            else
                            {
                                AddUsed(used, lex.Lexeme.Token.RealText, sv, lex.CalcParam.FullCode);
                                AddUsed(used, lex.CalcParam.FullCode, sv);
                            }
                        }
                        break;
                    case ExprType.Met:
                        if (lex.CalcParam == null)
                            s += "met!" + ltext + "(" + WriteType(ct);
                        else
                        {
                            s += "met!" + lex.CalcParam.Code + "(" + WriteType(ct) + lex.ParamString;
                            var b = lex.CalcParam.Owner.Inputs.Count == 0;
                            sv = "Подпараметр" + (lex.CalcParam.Inputs.Count > 0 ? "-функция" : "");
                            if (b) AddUsed(used, lex.CalcParam.FullCode, sv);
                            else
                            {
                                AddUsed(used, "." + lex.Lexeme.Token.RealText, sv, lex.CalcParam.FullCode);
                                AddUsed(used, lex.CalcParam.FullCode, sv);
                            }    
                        }
                        break;
                    case ExprType.Grafic:
                        s += "grafic!" + ltext + "(" + WriteType(ct) + lex.ParamString;
                        AddUsed(used, ltext, "График");
                        break;
                    case ExprType.Fun:
                        if (lex.FunOverload == null)
                            s += "fun!" + ltext + "(" + WriteType(ct);
                        else 
                            s += "fun!" + lex.FunOverload.Owner.Code + "(" + WriteType(ct) + lex.ParamString;
                        break;
                    case ExprType.Op:
                        if (ltext == "=") lex.Lexeme.Text = "assign";
                        if (ltext == "[]") lex.Lexeme.Text = "getelement";
                        s += "op!" + lex.Lexeme.Text + (lex.LinkAddress == -1 ? "" : ("_" + lex.LinkAddress)) + "(" + WriteType(ct) + lex.ParamString;
                        break;
                }
                s += ");";
            }
            rec.Put("Expr", s);
            rec.Put("UsedUnits", used.ToPropertyString("|") + TablikCompiler.UsingParamsString(UsingParams));
            if (!IsSubParam && !isOldSignal) 
                rec.Put("CodeSignal", firstSignal);
            rec.Update();
        }
    }
}
