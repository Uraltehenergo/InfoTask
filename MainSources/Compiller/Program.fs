module Program

open System
open Microsoft.FSharp.Text.Lexing
open Ast
open Compile
open Lexer
open Parser


//let readAndProcess() =
//    let cdb = (new DBEngineClass()).OpenDatabase("ProjectTemplate.accdb"); 
//    let rs = cdb.OpenRecordset("SELECT * FROM CalcParams_Old ORDER BY CodeCalc"); 
//    rs.MoveFirst()
//    while rs.EOF = false do
//        let c = new Compiller(rs.Fields.["UserExpr1"].Value.ToString(), CompillerType.Expr)         
//        let res = c.Parse()                         
//        let mutable s = ""
//        if c.Error.Text = "" && c.Error.Position = 0 then
//            for x in c.Lexemes do
//                 s <- s + x.StrType + "." + x.Text + "(" + x.ParamsCount.ToString() + ":" + x.Token.Position.ToString() + "=" + x.Token.Line.ToString() + "+" + x.Token.Column.ToString()  + ");"
//        else
//            s <- c.Error.Text + " (" + c.Error.Position.ToString() + "=" + c.Error.Line.ToString() + "+" + c.Error.Column.ToString() +  ") "
//        let r = c.Lex()
//        let mutable sl=""
//        if c.Error.Text = "" && c.Error.Position = 0 then
//            for x in c.Tokens do
//                 sl <- sl + x.StrType + "." + x.Text + "-" + x.RealText + "(" + x.Position.ToString() + "=" + x.Line.ToString() + "+" + x.Column.ToString()  + ");"
//        rs.Edit()
//        rs.Fields.["Expr"].Value <- s
//        rs.Fields.["Lex"].Value <- sl
//        rs.Update()
//        rs.MoveNext()
//    rs.Close()
//    cdb.Close()
// readAndProcess()