using System;
namespace ObEwolucyjne1
{
    public static class Environment
    {
        public static Random CUBE = new Random();
        public const uint COUNTVALUESTOMAP = UInt32.MaxValue;
        public const double DIVIDER = COUNTVALUESTOMAP / VALUESRANGE;
        public const double VALUESRANGE = 2 - (-2);

        public static double SurvivalFunction(double x)
        {
            return x * Math.Sin(x) * Math.Sin(10 * x);
        }
    }
}