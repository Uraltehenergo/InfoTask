using CommonTypes;

namespace ReporterCommon
{
    //Расчетный узел
    internal interface ICalcNode : INode
    {
        //Значение
        Mean GetMean(IReportShape rshape, int value);//Значение привязанного параметра
        //Входные параметры
        ICalcNode[] Params { get; }
    }
}