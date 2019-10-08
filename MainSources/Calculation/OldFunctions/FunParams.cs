using System;
using System.Collections.Generic;
using CommonTypes;

namespace Calculation
{
    //Флаги для настоек функций FunParams
    internal static class FunFlags
    {
        //Нет флагов
        public const int Empty = 0;
        //Добавлять расчет на начало периода
        public const int AddBegin = 1;
        //Добавлять расчет на конец периода
        public const int AddEnd = 2;
        //Хранить итоговое, предыдущее значение или массив Current
        public const int SaveResult = 4;
        //Возможны ли пустые списки значений
        public const int AllowEmptyList = 8;
        //Фильтр
        public const int Filter = 16;

        //Для вычисления максимумов
        public const int Max = 32;
        //Для вычисления среднего
        public const int Average = 64;
        //Для вычисления дисперсии 
        public const int Dispersion = 128;
        //Для вычисления ковариации 
        public const int Covariation = 256;
        //Для сумм целых чисел 
        public const int Int = 512;
        //Для сумм действительных чисел 
        public const int Real = 1024;
        //Для сумм строк 
        public const int String = 2048;

        //Нужно сортировать список по времени
        public const int Unsorted = 4096;
    }

    //---------------------------------------------------------------------------

    //Описание одного параметра в FunParams
    public class FunParamsParam
    {
        //Значение
        public MomentValue Value { get; set; }
        //True для номеров параметров, соответствующих времени
        public bool Current { get; set; }
        //Номер в List<MomentValue>
        public int Number { get; set; }
    }

    //---------------------------------------------------------------------------

    //Параметры для скалярных функций
    internal class FunParams
    {
        public FunParams(CalcParamRun calc, List<MomentValue>[] listpar, int flags)
        {
            Flags = flags;
            CalcParamRun = calc;
            ListPar = listpar;
            Par = new MomentValue[listpar.Length];
            Current = new bool[listpar.Length];
            Number = new int[listpar.Length];
            if ((flags & FunFlags.Filter) != 0)
            {
                Filter = new LinkedList<MomentValue>();
                FilterNd = new SortedList<int, int>();
                FilterErr = new SortedList<int, int>();
            }
            Time = calc.ThreadCalc.PeriodBegin;
        }

        //Флаги
        public int Flags { get; set; }
        //Ссылка на расчетный параметр
        public CalcParamRun CalcParamRun { get; set; }
        //Списки значений за весь период 
        public List<MomentValue>[] ListPar { get; set; }
        //Список текущих значений
        public MomentValue[] Par { get; set; }
        //True для номеров параметров, соответствующих времени
        public bool[] Current { get; set; }
        //Номера в List<MomentValue>
        public int[] Number { get; set; }
        //Текущее время
        public DateTime Time { get; set; }
        //Предыдущее значение
        public MomentValue Previous { get; set; }
        //Текущее итоговое значение 
        public MomentValue Result { get; set; }
        //Значения фильтра
        public LinkedList<MomentValue> Filter { get; set; }
        //Недостоверности фильтра
        public SortedList<int, int> FilterNd { get; set; }
        //Ошибки фильтра
        public SortedList<int, int> FilterErr { get; set; }
    }
}