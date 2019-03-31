using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EvoCore
{
    
    public class GeneticEnvironment
    {
        public uint COUNTVALUESTOMAP = UInt32.MaxValue;
        public double VALUESRANGE = 2 - (-2);
        public double DIVIDER
        {
            get
            {
                return COUNTVALUESTOMAP / VALUESRANGE;
            }
        } 
        public int POPULATIONSIZE = 20;
        public int POPULATIONCOUNTLIMIT = 1000;
        public double MUTATIONPROBABILITY = 0.15;
        public int NUMBEROFEVOLUTIONTRIALS = 100;
        public int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = 500;
        public SelectionMethods defaultSelectionMethod = SelectionMethods.Roulette;
        public int MUTATIONRETRIALS = 4;

        public void ParseParameters(string filePath)
        {
            try
            {
                string[] file = File.ReadAllLines(filePath);
                string[] values = file[0].Split(',');
                GeneticEnvironment.INSTANCE.POPULATIONSIZE = Int32.Parse(values[0]);//popsize
                GeneticEnvironment.INSTANCE.MUTATIONPROBABILITY = double.Parse(values[1]);//mutationprob
                GeneticEnvironment.INSTANCE.MUTATIONRETRIALS = Int32.Parse(values[2]);//mutationretry
                GeneticEnvironment.INSTANCE.defaultSelectionMethod = Enum.Parse<SelectionMethods>(values[3]);//enum
                GeneticEnvironment.INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = Int32.Parse(values[4]);//interationnonimprove
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't read the config file - running with default values. Exception message: {ex.Message}");
            }
        }
        private static GeneticEnvironment _config;
        public static GeneticEnvironment INSTANCE
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
        private GeneticEnvironment()
        {

        }
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