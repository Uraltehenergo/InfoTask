//Файл типов для Parser.fsy
namespace Ast
open System
open System.Collections.Generic

//Тип токена
type public TokenType =
    | Int = 0
    | Time = 1
    | Real = 2
    | String = 3
    | KeyWord = 4
    | Signal = 5
    | Symbol = 6
    | Ident = 7
    | Operation = 8    
    | DataType = 9
    | Comment = 10
    | System = 11

//----------------------------------------------------------------------

//Один токен
type public Token ( text:string, vtype:TokenType, position:int, line:int, column:int) =
    let _text  = text            
    let _type = vtype
    let _position = position
    let _line = line
    let _column = column
    let mutable _realText  = ""    
    let mutable _length = 0

    member public this.Text // Текст токена для польской записи
        with get() = _text
    member public this.RealText // Исходный текст токена
        with get() = _realText    
        and set value = _realText <- value
    member public this.Line  //Строка начала токена в выражении
        with get() = _line        
    member public this.Column  //Позиция начала токена в строке выражения
        with get() = _column        
    member public this.Position //Позиция начала токена в выражении
        with get() = _position
    member public this.Length //Длина токена
        with get() = _length
        and set value = _length <- value
    member public this.Type //Тип токена
        with get() = _type       
    member public this.StrType 
        with get() =
            match _type with
            | TokenType.Int -> "Int"
            | TokenType.Time -> "Time"
            | TokenType.Real -> "Real"            
            | TokenType.String -> "String"
            | TokenType.KeyWord -> "KeyWord"
            | TokenType.Signal -> "Signal"
            | TokenType.Symbol -> "Symbol"
            | TokenType.Ident -> "Ident"
            | TokenType.Operation -> "Operation"
            | TokenType.DataType -> "DataType"
            | TokenType.Comment -> "Comment"
            | TokenType.System -> "System"           
            | _ -> ""    

//------------------------------------------------------------------------

//Тип лексемы
type public LexemeType =
    | Int = 0 //Константа
    | Time = 1 //Константа
    | Real = 2 //Константа
    | String = 3 //Константа
    | Signal = 4 //Сигнал
    | Met = 5 //Метод
    | Fun = 6 //Функция или расчетный параметр или использование переменной
    | Op = 7 //Функция - ключевое слово языка
    | Prev = 8 //Первый параметр в цепочке функции Пред
    | PrevM = 9 //Не первый параметр в цепочке функции Пред
    | Var = 10 //Переменнная
    | Ident = 11 //Идентификатор
    | ClassP = 13 //Класс в списке параметров
    | DataTypeP = 14 // Тип данных в списке параметров
    
    
//------------------------------------------------------------------------

//Лексема
type public Lexeme (token : Token, text:string, vtype : LexemeType, paramsCount : int) =
    let _token = token
    let mutable _text = text
    let _type = vtype
    let mutable _paramsCount = paramsCount    
    //let mutable _goLinks = new List<Lexeme>();
    let mutable _number = 0;    

    member public this.Token //Токен соответствующий лексеме
        with get() = _token   
    member public this.Text //Текст лексемы
        with get() = _text     
        and set value = _text <- value      
    member public this.ParamsCount //Количество параметров (сама не считается)
        with get() = paramsCount    
        and set value = _paramsCount <- value
    member public this.Type //Тип лексемы
        with get() = _type
    member public this.StrType //Тип лексемы - строка
        with get() =
            match _type with
            | LexemeType.Int -> "Int"
            | LexemeType.Time -> "Time"
            | LexemeType.Real -> "Real"
            | LexemeType.String -> "String"
            | LexemeType.Signal -> "Signal"
            | LexemeType.Met -> "Met"
            | LexemeType.Fun -> "Fun"
            | LexemeType.Op -> "Op"
            | LexemeType.Prev -> "Prev"
            | LexemeType.PrevM -> "PrevM"
            | LexemeType.Var -> "Var"
            | LexemeType.Ident -> "Ident"         
            | _ -> ""
        member public this.Number //Номер в списке лексем
            with get() = _number;
            and set value = _number <- value


//------------------------------------------------------------------------

//Ошибка
type public Error (text : string, position:int, line : int, column : int) =
    let _text = text
    let _position = position
    let _line = line
    let _column = column
    
    member public this.Text //Сообщение
        with get() = _text 
    member public this.Position //Позиция в тексте в которой произошла ошибка
        with get() = _position 
    member public this.Line //Строка в которой произошла ошибка
        with get() = _line 
    member public this.Column //Позиция в строке в которой произошла ошибка
        with get() = _column 


//---------------------------------------------------------------------------------
//Типы элементов дерева разбора

type FullProg = FullProg of ProgVariant    

and ProgVariant =
    | GetProg of Prog
    | GetProgP of ProgP

//Разбор списка входов
and ProgP = 
    | ParamP of ParamP
    | GetListP of ParamP * ProgP

and ParamP =
    | ParamClassP of ClassP * Token
    | ParamArgP of ArgP
    | ParamArgDefaultP of ArgP * Const
    | ParamSignalP of ArgP * Token * SignalsListP

and SignalsListP =
    | SignalArgP of ArgP
    | SignalsListArgP of ArgP * SignalsListP

and ArgP =
    | ArgIdentP of Token
    | TypeIdentP of Token * Token

and ClassP =
    | ClassNameP of Token
    | GetClassP of Token * ClassP

//Разбор расчетного и управляющего выражения
and Prog =    
    | VoidProg of VoidProg
    | ValueProg of ValueProg

and ValueProg =  
    | Expr of Expr
    | GetValueProg of VoidProg * Expr    

and VoidProg =
    | VoidExpr of VoidExpr
    | GetVoidExpr of VoidProg * VoidExpr

and VoidExpr =
    | Void of Token
    | Assign of Token * Token * Expr    
    | AssignArr of Token * Expr * Token * Expr 
    | IfVoid of Token * IfVoidList    
    | While of Token * Expr * VoidProg
    | For of Token * Token * Expr * VoidProg

and Expr =    
    | ApplyOper1 of Expr * Token * Term1
    | Term1  of Term1
and Term1 =    
    | ApplyOper2 of Term1 * Token * Term2
    | ApplyNot of Token * Term1
    | Term2  of Term2
and Term2 =    
    | ApplyOper3 of Term2 * Token * Term3
    | Term3  of Term3
and Term3 =        
    | ApplyOper4 of Term3 * Token * Term4
    | ApplyMinus of Term3 * Token * Term4
    | Term4  of Term4
and Term4 =    
    | ApplyOper5 of Term4 * Token * Term5
    | Term5  of Term5
and Term5 =    
    | ApplyOper6 of Term5 * Token * Term
    | Term  of Term
    
and Term =
    | Const of Const
    | Signal of Token 
    | ParenExpr of Expr
    | ApplyMinusUn of Token * Term    
    | PrevExpr of Token * PrevChain * ParamList
    | IfValue of Token * IfValueList
    | Task of Token * Token
    | GetArr of FunExpr * Expr
    | GetMetExpr of FunExpr * MetExpr
    | FunExpr of FunExpr    

and Const=
    | Int of Token
    | Time of Token
    | Real of Token
    | Str of Token

and PrevChain =
    | GetPrevParam of Token
    | GetPrevChain of PrevChain * Token

and FunExpr =
    | FunName of FunName
    | GetFun of FunName * ParamList
    | GetSignal of Token * Expr * Expr
    | GetSignalStr of Token * Expr

and FunName= 
    | Func of Token
    | FunType of Token
    | Op of Token

and MetExpr=
    | MetName of MetName
    | GetMet of MetName * ParamList

and MetName=
    | Met of Token

and ParamList=
    | Param of Expr
    | GetParamList of Expr * ParamList
    
and IfVoidList=
    | GetIf of Expr * VoidProg
    | GetIfElse of Expr * VoidProg * VoidProg
    | GetIfList of Expr * VoidProg * IfVoidList 

and IfValueList=
    | GetIfValue of Expr * ValueProg
    | GetIfElseValue of Expr * ValueProg * ValueProg
    | GetIfListValue of Expr * ValueProg * IfValueList 
