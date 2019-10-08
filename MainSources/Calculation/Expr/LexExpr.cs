using System;
using System.Collections.Generic;
using BaseLibrary;

namespace Calculation
{
    //Одна лексема для расчета
    public class LexExpr
    {
        public LexExpr(string s)
        {
            var ss = s.Split(new[] { '!', '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
            Type = ss[0];
            Code = ss[1];
            DataType = ss[2].ToDataType();
            for (int i = 3; i < ss.Length; i++)
                Pars.Add(ss[i]);
        }

        //Каждая лексема в выражении записана в виде Type!Name(DataType,Par,Par,.....)
        public string Type { get; private set; }
        public string Code { get; private set; }
        public DataType DataType { get; private set; }
        private readonly List<string> _pars = new List<string>();
        public List<string> Pars { get { return _pars; } }
    }
}