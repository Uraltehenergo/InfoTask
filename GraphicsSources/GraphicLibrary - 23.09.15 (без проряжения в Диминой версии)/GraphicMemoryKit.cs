using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphicLibrary
{
    public class GraphicMemoryKit
    {
        public GraphicMemoryKit()
        {
            IsNull = true;
        }

        public GraphicMemoryKit(bool isStashViewMode, DateTime firstMoment, double updateTime, string updateTimeType,
            double viewPeriodSize, double viewPeriodNet, string viewPeriodType)
        {
            IsNull = false;
            IsStashViewMode = isStashViewMode;
            FirstMoment = firstMoment;
            UpdateTime = updateTime;
            UpdateTimeType = updateTimeType;
            ViewPeriodNet = viewPeriodNet;
            ViewPeriodSize = viewPeriodSize;
            ViewPeriodType = viewPeriodType;
        }

        public bool IsNull; //пуст ли
        public bool IsStashViewMode;// режим ли накопления
        public DateTime FirstMoment;
        public double UpdateTime;
        public string UpdateTimeType;
        public double ViewPeriodSize;
        public double ViewPeriodNet;
        public string ViewPeriodType;
    }
}
