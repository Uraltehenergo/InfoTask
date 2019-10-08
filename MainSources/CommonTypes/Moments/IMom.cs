using System;
using BaseLibrary;

namespace CommonTypes
{
    //Интерфейс для Mom и MomEdit
    public interface IMom : IMomentsVal
    {
        //Время
        DateTime Time { get; }
        
        //Значения разных типов
        bool Boolean { get; }
        int Integer { get; }
        double Real { get; }
        DateTime Date { get; }
        string String { get; }

        //Сравнение значений и ошибок
        bool ValueEquals(Mom mom);
        bool ValueLess(Mom mom);
        bool ValueAndErrorEquals(Mom mom);
    } 
}