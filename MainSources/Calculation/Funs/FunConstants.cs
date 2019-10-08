using System;
using CommonTypes;

namespace Calculation
{
    //Константы
    public class FunConstants : SingleFunction
    {
        public ISingleVal TrueFun(ISingleVal[] par)
        {
            return new MomBoolean(true);
        }

        public ISingleVal FalseFun(ISingleVal[] par)
        {
            return new MomBoolean(false);
        }

        public ISingleVal Pi(ISingleVal[] par)
        {
            return new MomReal(Math.PI);
        }
    }
}