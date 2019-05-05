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
        public int NUMBEROFEVOLUTIONTRIALS = 0;
        public int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = 5000;
        public SelectionMethods defaultSelectionMethod = SelectionMethods.Roulette;
        public int MUTATIONRETRIALS = 4;

        public List<Coords> ParsedCitiesToVisit;
        public int[] GetRandomCitiesRoute
        {
            get
            {
                genotypeExample.Shuffle();
                return genotypeExample.ToArray();
            }
        }

        private List<int> genotypeExample;
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
        public void LoadCities(string filePath)
        {
            this.ParsedCitiesToVisit = new List<Coords>();
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int start = FindStartingIndex(lines);
                var end = FindEndIndex(lines);
                for (int i = start; i < end; i++)
                {
                    string[] values = lines[i].Split(' ');
                    double lattitude = double.Parse(values[1]);
                    double longitude = double.Parse(values[2]);
                    ParsedCitiesToVisit.Add(new Coords()
                    {
                        X = lattitude,
                        Y = longitude
                    });
                }
                genotypeExample = new List<int>();
                for (int i = 0; i < ParsedCitiesToVisit.Count; i++)
                {
                    genotypeExample.Add(i);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't read the cities file - Cannot continue. Exception message: {ex.Message}");
            }
        }

        private int FindEndIndex(string[] lines)
        {
            var end = lines.Length - 1;
            while (!lines[end].Contains("EOF"))
            {
                end--;
            }
            return end;
        }

        private int FindStartingIndex(string[] lines)
        {
            var i = 0;
            while (!char.IsDigit(lines[i][0]))
            {
                i++;
            }
            return i;
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
        public static double SurvivalFunction(List<Coords> coordsToVisit)
        {
            double distance = 0;
            for (int i = 0; i < coordsToVisit.Count - 2; i++)
            {
                distance += DistanceBetweenCoords(coordsToVisit[i], coordsToVisit[i + 1]);
            }
            return distance;
        }

        private static double DistanceBetweenCoords(Coords startCoord, Coords endCoord)
        {
            double distancePerOneDegree = 111;//mean distance in km
            double degreesDistance = Math.Sqrt(Math.Pow(Math.Abs(endCoord.Y - startCoord.Y), 2) + Math.Pow(Math.Abs(endCoord.X - startCoord.X), 2));
            return degreesDistance * distancePerOneDegree;
        }
    }

    public struct Coords
    {
        public double X, Y;
    }
}