namespace Compile

open System
open System.Collections.Generic
open System.Linq
open Microsoft.FSharp.Text.Lexing
open Ast
open Lexer
open Parser

//Тип компилируемого выражения
type public CompillerType =
    | Expr = 0 //Расчетное и управляющее выражения
    | Params = 1 //Список параметров

// Компилятор
type public Compiller (s : string, ctype:CompillerType) =
    let _expr = s //Разбираемая строка
    let _ctype = ctype //Тип компилируемого выражения
    let mutable _tokens = new List<Token>() //Список токенов
    let mutable _tokensDic = new Dictionary<int, Token>() //Словарь токенов, ключ - положение в строке   
    let mutable _lexemes = new List<Lexeme>() //Список лексем
    let mutable _error  = new Error("", 0, 0, 0) //Ошибка, по умолчанию текст - пустая строка
    let mutable _lexbuf = LexBuffer<char>.FromString(_expr) // Буфер лексического разбора
    let _nullToken = new Token("_nulltoken_", TokenType.System, 0, 0, 0) //Вместо null

    //FullProg
    let rec evalFullProg fullProg =
        match fullProg with
        | FullProg prog -> evalProgVariant prog            

    and evalProgVariant progv =
        match progv with
        | GetProgP prog -> evalProgP prog
        | GetProg prog -> evalProg prog

    //Список входных параметров для расчетного параметра
    and evalProgP prog = 
        match prog with
        | ParamP par -> evalParamP par
        | GetListP (par, prog) -> 
            (evalParamP par) @ (evalProgP prog) 
    
    //Один входной параметр
    and evalParamP cpar =
        match cpar with
        | ParamClassP (cl, par)->
            let lex = new Lexeme(_tokensDic.[par.Position], par.Text, LexemeType.Ident, 0)
            [lex] @ (evalClassP cl)
        | ParamArgP par -> evalArgP par
        | ParamArgDefaultP(par, def) -> (evalArgP par) @ (evalConst def)
        | ParamSignalP (par, fsig, slist) -> 
            let (res, x) = evalSignalsListP slist
            let lex = new Lexeme(_tokensDic.[fsig.Position], fsig.Text, LexemeType.Fun, x + 1)
            res @ (evalArgP par) @ [lex]

     //Список допустимых сигналов входного параметра
    and evalSignalsListP ctype =
        match ctype with
        | SignalArgP par -> (evalArgP par, 1)
        | SignalsListArgP (par, slist) -> 
            let (res, x) = (evalSignalsListP slist)
            ((evalArgP par) @ res, x + 1)            

     //Тип данных + имя входного параметра
    and evalArgP ctype =
        match ctype with
        | ArgIdentP par -> 
            let lex = new Lexeme(_tokensDic.[par.Position], par.Text, LexemeType.Ident, 0)
            let lext = new Lexeme(_nullToken, "real", LexemeType.DataTypeP, 0)
            [lex] @ [lext]
        | TypeIdentP (dt, par) ->
            let lex = new Lexeme(_tokensDic.[par.Position], par.Text, LexemeType.Ident, 0)
            let lexd = new Lexeme(_tokensDic.[dt.Position], dt.Text, LexemeType.DataTypeP, 0)
            [lex] @ [lexd]                         
        
    //Входной параметр - класс
    and evalClassP cla =
        match cla with
        | ClassNameP name -> [new Lexeme(_tokensDic.[name.Position], name.Text, LexemeType.ClassP , 0)]            
        | GetClassP (name, cl) ->
            let lex = new Lexeme(_tokensDic.[name.Position], name.Text, LexemeType.ClassP, 0)
            [lex] @ (evalClassP cl) 

    //Prog - программа
    and evalProg prog =
        match prog with        
        | VoidProg vprog -> evalVoidProg vprog
        | ValueProg vprog -> evalValueProg vprog

    //ValueProg - программа, возвращающая значение
    and evalValueProg vprog =      
        match vprog with
        | Expr expr -> evalExpr expr        
        | GetValueProg (vprog, expr) ->                            
            (evalVoidProg vprog) @ (evalExpr expr)

    //VoidProg - программа, не возвращающая значение
    and evalVoidProg vprog =
        match vprog with
        | VoidExpr vexpr -> evalVoidExpr vexpr
        | GetVoidExpr ( vprog, vexpr) -> 
            (evalVoidProg vprog) @ (evalVoidExpr vexpr)

    //VoidExpr - выражение, не возвращающее значение
    and evalVoidExpr vexpr =
        match vexpr with
        | Void v -> [new Lexeme(_tokensDic.[v.Position], v.Text, LexemeType.Op, 0)]      
        | Assign (var, assign, expr) -> 
            let vlex = new Lexeme(_tokensDic.[var.Position], var.Text, LexemeType.Var, 0)
            let lex = new Lexeme(_tokensDic.[assign.Position], assign.Text, LexemeType.Op, 2)                      
            [vlex] @ (evalExpr expr) @ [lex]     
        | AssignArr (var, ind, assign, expr) -> 
            let vlex = new Lexeme(_tokensDic.[var.Position], var.Text, LexemeType.Var, 0)
            let lex = new Lexeme(_tokensDic.[assign.Position], "assigna", LexemeType.Op, 3)                      
            [vlex] @ (evalExpr ind) @ (evalExpr expr) @ [lex]     
        | IfVoid (op, list) -> evalIfVoidList (op, list)                
        | While (op, cond, vprog) -> 
            let rcond = evalExpr cond
            let rprog = evalVoidProg vprog
            let lexbegin = new Lexeme(_nullToken, "begin", LexemeType.Op, 0)
            let lexwhile = new Lexeme(_tokensDic.[op.Position], op.Text, LexemeType.Op, 1)
            let lexret = new Lexeme(_nullToken, "ret", LexemeType.Op, 0)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 0)
            [lexbegin] @ rcond @ [lexwhile] @ rprog @ [lexret] @ [lexend]                  
        | For (op, var, expr, vprog) -> 
            let rvar = new Lexeme(_tokensDic.[var.Position], var.Text, LexemeType.Var, 0)            
            let rexpr = evalExpr expr
            let rprog = evalVoidProg vprog
            let lexfor = new Lexeme(_tokensDic.[op.Position], op.Text, LexemeType.Op, 2)
            let lexret = new Lexeme(_nullToken, "ret", LexemeType.Op, 0)            
            [rvar] @ rexpr @ [lexfor] @ rprog @ [lexret]    

    //Expr - выражение с операциями
    and evalExpr expr =
        match expr with
        | ApplyOper1 (expr, oper, term) ->  
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalExpr expr) @ (evalTerm1 term) @ [lex]             
        | Term1 term -> evalTerm1 term
    and evalTerm1 expr =
        match expr with
        | ApplyOper2 (expr, oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalTerm1 expr) @ (evalTerm2 term) @ [lex]
        | ApplyNot (oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 1)                      
            (evalTerm1 term) @ [lex]
        | Term2 term -> evalTerm2 term
    and evalTerm2 expr =
        match expr with
        | ApplyOper3 (expr, oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalTerm2 expr) @ (evalTerm3 term) @ [lex] 
        | Term3 term -> evalTerm3 term
    and evalTerm3 expr =
        match expr with
        | ApplyOper4 (expr, oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalTerm3 expr) @ (evalTerm4 term) @ [lex]  
        | ApplyMinus (expr, oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalTerm3 expr) @ (evalTerm4 term) @ [lex]
        | Term4 term -> evalTerm4 term
    and evalTerm4 expr =
        match expr with
        | ApplyOper5 (expr, oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalTerm4 expr) @ (evalTerm5 term) @ [lex]
        | Term5 term -> evalTerm5 term
    and evalTerm5 expr =
        match expr with
        | ApplyOper6 (expr, oper, term) -> 
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 2)                      
            (evalTerm5 expr) @ (evalTerm term) @ [lex]  
        | Term term -> evalTerm term

    //Term - выражение без операций
    and evalTerm term =
        match term with
        | Const c -> evalConst c
        | Signal x -> [new Lexeme(_tokensDic.[x.Position], x.Text, LexemeType.Signal, 0)]            
        | ParenExpr expr -> evalExpr expr
        | ApplyMinusUn (oper, x) -> 
            let ex = evalTerm x
            let lex = new Lexeme(_tokensDic.[oper.Position], oper.Text, LexemeType.Fun, 1)          
            ex @ [lex]                 
        | PrevExpr (f, chain, list) ->
            let (res, x) = (evalParamList list) 
            (evalPrevChain chain) @ res @ [new Lexeme(_tokensDic.[f.Position], f.Text, LexemeType.Op, x+1)]          
        | IfValue (op, list) -> evalIfValueList (op, list)
        | Task (func, task) -> 
            let lexf = new Lexeme(_tokensDic.[func.Position], func.Text, LexemeType.Op, 1)
            let lexs = new Lexeme(_tokensDic.[task.Position], task.Text, LexemeType.String, 0)
            [lexs] @ [lexf]        
        | GetArr (term, expr) ->        
            let lex = new Lexeme(_nullToken, "[]", LexemeType.Op, 2)               
            (evalFunExpr term) @ (evalExpr expr) @ [lex]
        | GetMetExpr (term, expr) ->
            (evalFunExpr term) @ (evalMetExpr expr)
        | FunExpr expr -> evalFunExpr expr

    //Const - константы
    and evalConst c =
        match c with
        | Int x -> [new Lexeme(_tokensDic.[x.Position], x.Text, LexemeType.Int, 0)]            
        | Time x -> [new Lexeme(_tokensDic.[x.Position], x.Text, LexemeType.Time, 0)]            
        | Real x -> [new Lexeme(_tokensDic.[x.Position], x.Text, LexemeType.Real, 0)]            
        | Str x -> [new Lexeme(_tokensDic.[x.Position], _expr.Substring(x.Position+1, x.Text.Length), LexemeType.String, 0)]            
                
    //PrevChain - первый параметр функций Пред (архивный параметр)
    and evalPrevChain chain = 
        match chain with
        | GetPrevParam par -> [new Lexeme(_tokensDic.[par.Position], par.Text, LexemeType.Prev, 0)]            
        | GetPrevChain (chain, par) ->
            let res = evalPrevChain chain
            res @ [new Lexeme(_tokensDic.[par.Position], par.Text, LexemeType.PrevM, 1)] 

    //FunExpr - функция с параметрами
    and evalFunExpr expr =
        match expr with
        | FunName name -> (evalFunName name 0)
        | GetFun (name, list) -> 
            let (res, x) = (evalParamList list)
            res @ (evalFunName name x)
        | GetSignal (keyword, expr, code) -> 
            let gs = new Lexeme(_tokensDic.[keyword.Position], keyword.Text, LexemeType.Op, 2)            
            (evalExpr expr) @ (evalExpr code) @ [gs]

    //FunName - имя функции (pcount - количество параметров)
    and evalFunName name pcount =
        match name with
        | Func f -> [new Lexeme(_tokensDic.[f.Position], f.Text, LexemeType.Ident, pcount)]            
        | FunType f -> [new Lexeme(_tokensDic.[f.Position], f.Text, LexemeType.Fun, pcount)]            
        | Op f -> [new Lexeme(_tokensDic.[f.Position], f.Text, LexemeType.Op, pcount)]              

     //MetExpr - метод с параметрами
     and evalMetExpr expr =
        match expr with
        | MetName name -> (evalMetName name 1)
        | GetMet (name, list) -> 
            let (res, x) = (evalParamList list)
            res @ (evalMetName name (x+1))

    //MetName - имя метода (pcount - количество параметров)
    and evalMetName name pcount =
        match name with
        | Met f -> [new Lexeme(_tokensDic.[f.Position], f.Text, LexemeType.Met, pcount)]                                  
        
    //ParamList - список параметров функции
    and evalParamList plist =
        match plist with
        | Param expr -> (evalExpr expr, 1)
        | GetParamList (expr, list) -> 
            let (res, x) = (evalParamList list)
            ((evalExpr expr) @ res, x + 1)      

    //VoidList - параметры If, не возвращающей значение
    and evalIfVoidList (op, clist) = 
        match clist with
        | GetIf (cond, vprog) ->
            let rcond = evalExpr cond
            let rprog = evalVoidProg vprog
            let lexif = new Lexeme(_tokensDic.[op.Position], op.Text, LexemeType.Op, 1)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 0)
            rcond @ [lexif] @ rprog @ [lexend]       
        | GetIfElse (cond, vprog, eprog) ->
            let rcond = evalExpr cond
            let rprog = evalVoidProg vprog
            let reprog = evalVoidProg eprog
            let lexif = new Lexeme(_tokensDic.[op.Position], op.Text, LexemeType.Op, 1)            
            let lexelse = new Lexeme(_nullToken, "else", LexemeType.Op, 0)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 0)
            rcond @ [lexif] @ rprog @ [lexelse] @ reprog @ [lexend]       
        | GetIfList (cond, vprog, list) ->
            let rcond = evalExpr cond
            let rprog = evalVoidProg vprog
            let rlist = evalIfVoidList (op, list)
            let lexif = new Lexeme(_tokensDic.[op.Position], op.Text, LexemeType.Op, 1)            
            let lexelse = new Lexeme(_nullToken, "else", LexemeType.Op, 0)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 0)
            rcond @ [lexif] @ rprog @ [lexelse] @ rlist @ [lexend]           

    //ValueList - параметры If, возвращающей значение
    and evalIfValueList (op, clist) = 
        match clist with
        | GetIfValue (cond, vprog) ->
            let rcond = evalExpr cond
            let rprog = evalValueProg vprog
            let lexif = new Lexeme(_tokensDic.[op.Position], op.Text + "v", LexemeType.Op, 1)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 2)
            rcond @ [lexif] @ rprog @ [lexend]       
        | GetIfElseValue (cond, vprog, eprog) ->
            let rcond = evalExpr cond
            let rprog = evalValueProg vprog
            let reprog = evalValueProg eprog
            let lexif = new Lexeme(_tokensDic.[op.Position], op.Text + "v", LexemeType.Op, 1)            
            let lexelse = new Lexeme(_nullToken, "elsev", LexemeType.Op, 0)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 4)
            rcond @ [lexif] @ rprog @ [lexelse] @ reprog @ [lexend]       
        | GetIfListValue (cond, vprog, list) ->
            let rcond = evalExpr cond
            let rprog = evalValueProg vprog
            let rlist = evalIfValueList (op, list)
            let lexif = new Lexeme(_tokensDic.[op.Position], op.Text + "v", LexemeType.Op, 1)            
            let lexelse = new Lexeme(_nullToken, "elsev", LexemeType.Op, 0)            
            let lexend = new Lexeme(_nullToken, "end", LexemeType.Op, 4)
            rcond @ [lexif] @ rprog @ [lexelse] @ rlist @ [lexend]             

    //Вывод результатов компиляции в маасив лексем
    let rec getList list =
        match list with
        | head :: tail -> 
            _lexemes.Add(head)
            getList tail
        | [] -> ()
            
    member this.Expr // Текст токена для польской записи
        with get() = _expr
    member this.Tokens//Список токенов
        with get() = _tokens
    member this.TokensDic//Словарь токенов
        with get() = _tokensDic
    member this.Lexemes//Список лексем
        with get() = _lexemes
    member this.Error//Ошибка
        with get() = _error

    // Провести лексический анализ, создать список и словарь токенов
    member public this.Lex() = 
        _tokens.Clear()
        _tokensDic.Clear()
        match _ctype with
        | CompillerType.Expr -> 
            _lexbuf <- LexBuffer<char>.FromString("$1\n" + _expr.ToLower())                  
        | CompillerType.Params -> 
            _lexbuf <- LexBuffer<char>.FromString("$2\n" + _expr.ToLower())           
        | _ -> ()       
        while (_lexbuf.IsPastEndOfStream = false) && (_error.Text = "") do  
            try
                let gs = Lexer.tokenize _lexbuf 
                match gs with
                | EOF -> ()
                | BEGINEXPR t | BEGINPARAM t -> ()
                | DATATYPE t | WHILE t | IF t | FOR t
                | LFIGURE t | RFIGURE t | DOT t | COMMA t | COLON t | LPAREN t | RPAREN t | OP t | VOID t | PREV t | TASK t | GETSIGNAL t
                | ASSIGN t | NOT t | MINUS t | OPER6 t | OPER5 t | OPER4 t | OPER3 t | OPER2 t | OPER1 t 
                | INT t | TIME t | IDENT t | REAL t-> 
                    t.Length <- _lexbuf.LexemeLength
                    t.RealText <- _expr.Substring(t.Position, t.Length)
                    _tokens.Add(t)                
                    _tokensDic.Add(t.Position, t)             
                | STRING t  ->
                    t.Length <- t.Text.Length + 2
                    t.RealText <- _expr.Substring(t.Position, t.Length)
                    _tokens.Add(t)                
                    _tokensDic.Add(t.Position, t)
                | SIGNAL t ->
                    t.Length <- t.Text.Length + 2
                    t.RealText <- _expr.Substring(t.Position, t.Length)
                    _tokens.Add(t)                
                    _tokensDic.Add(t.Position, t)             
                with
                | _ as e -> 
                    if _tokens.Count = 0 then
                        _error <- new Error("Недопустимое выражение", 0 , 1, 1)
                    else
                        let tok = _tokens.[_tokens.Count-1]
                        _error <- new Error("Недопустимое выражение", tok.Position + tok.Text.Length , tok.Line, tok.Column + tok.Text.Length)
    
    // Провести синтаксический анализ, создать список лексем
    member public this.Parse() = 
        this.Lex()
        if _error.Text = "" then
            match _ctype with
            | CompillerType.Expr -> 
                _lexbuf <- LexBuffer<char>.FromString("$1\n" + _expr.ToLower())                  
            | CompillerType.Params -> 
                _lexbuf <- LexBuffer<char>.FromString("$2\n" + _expr.ToLower())    
            | _ -> ()                   
            try                    
                let fp = Parser.start Lexer.tokenize _lexbuf                                                
                _lexemes.Clear()   
                getList (evalFullProg fp)                                             
            with ex ->
                _error <- new Error(_lexbuf.Lexeme.ToString(), _lexbuf.StartPos.AbsoluteOffset-3,  _lexbuf.StartPos.Line, _lexbuf.StartPos.Column+1)                                   
        ()
