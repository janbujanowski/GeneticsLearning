using System;
using System.Collections.Generic;
using System.Text;

namespace EvoCore
{
    public static class GeneticEnvironment
    {
        public const uint COUNTVALUESTOMAP = UInt32.MaxValue;
        public const double VALUESRANGE = 2 - (-2);
        public const double DIVIDER = COUNTVALUESTOMAP / VALUESRANGE;

        private static Random _CUBE;
        public static Random CUBE
        {
            get
            {
                if (_CUBE == null)
                {
                    _CUBE = new Random();
                }
                return _CUBE;
            }
        }

        public static double SurvivalFunction(double x)
        {
            return x * Math.Sin(x) * Math.Sin(10 * x);
        }
    }
}
