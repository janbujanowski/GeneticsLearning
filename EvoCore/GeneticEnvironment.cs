using System;
using System.Collections.Generic;
using System.Text;

namespace EvoCore
{
    
    public class GeneticEnvironment
    {
        private static GeneticEnvironment _config;

        private GeneticEnvironment()
        {

        }
        public GeneticEnvironment INSTANCE
        {
            get
            {
                if (_config == null)
                {
                    _config = new GeneticEnvironment();
                }
                return _config;
            }
        }
        public const uint COUNTVALUESTOMAP = UInt32.MaxValue;
        public const double VALUESRANGE = 2 - (-2);
        public const double DIVIDER = COUNTVALUESTOMAP / VALUESRANGE;

        public const int POPULATIONSIZE = 20;
        public const int POPULATIONCOUNTLIMIT = 1000;
        public const double MUTATIONPROBABILITY = 0.15;
        public const int NUMBEROFEVOLUTIONTRIALS = 10;
        public const int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = 500;

        public static SelectionMethods defaultSelectionMethod = SelectionMethods.Roulette;

        public const int MUTATIONRETRIALS = 4;

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