using EvoCore;
using srodowisko;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace EvoCore
{
    public enum SelectionMethods
    {
        Tournament,
        Roulette,
        RankedRoulette
    }
    public enum CrossoverMethods
    {
        PMX,
        OX,
        CX
    }
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
    public static class GenericHelpers
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = GeneticEnvironment.CUBE.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public static void Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = GeneticEnvironment.CUBE.Next(n + 1);
                T value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }
    }
    public struct Population
    {
        public Individual[] genotypes;
        public Individual BestOne
        {
            get
            {
                return genotypes.OrderByDescending(x => x.SurvivalScore).First();
            }
        }
        public Individual BestSecond;
        public Population GetCopy()
        {
            Individual[] copiedGenotypes = new Individual[GeneticEnvironment.INSTANCE.POPULATIONSIZE];
            for (int i = 0; i < copiedGenotypes.Length; i++)
            {
                copiedGenotypes[i] = new Individual() { genotype = genotypes[i].genotype };
            }
            return new Population { genotypes = this.genotypes };
        }
    }
    public static class GeneticHelpers
    {
        public static uint Sum(this uint[] arrayOfStuff)
        {
            uint sum = 0;
            foreach (var item in arrayOfStuff)
            {
                sum += item;
            }
            return sum;
        }

        public static Individual GetTournamentParent(this Population population)
        {
            List<Individual> competitors = new List<Individual>();
            int TOURNAMENTSIZE = 2;
            for (int i = 0; i < TOURNAMENTSIZE; i++)
            {
                competitors.Add(population.genotypes[GeneticEnvironment.CUBE.Next(0, population.genotypes.Length)]);
            }
            return competitors.OrderByDescending(x => x.SurvivalScore).First();
        }

        public static Individual GetRouletteParent(this Population population)
        {
            var sumArray = population.genotypes.Sum(x => 2 + x.SurvivalScore);
            Individual parent = null;
            var i = 0;
            var sum = population.genotypes[i].SurvivalScore;
            var rankedChosenValue = GeneticEnvironment.CUBE.Next(1, (int)sumArray);
            while (parent == null)
            {
                if (rankedChosenValue <= sum)
                {
                    parent = population.genotypes[i];
                }
                i++;
                sum += i;
            }
            if (parent == null)
            {
                parent = population.genotypes[population.genotypes.Length - 1];
            }
            return parent;
        }

        public static Individual GetRankedRouletteParent(this Population population)
        {
            var sorted = population.genotypes.OrderBy(x => x.SurvivalScore).ToArray();
            var sumArray = ((population.genotypes.Length - 1) / 2) * population.genotypes.Length;
            Individual parent = null;
            var i = 0;
            var sum = 0;
            var rankedChosenValue = GeneticEnvironment.CUBE.Next(1, sumArray);
            while (parent == null)
            {
                if (rankedChosenValue <= sum)
                {
                    parent = sorted[i];
                }
                i++;
                sum += i;
            }
            return parent;
        }

        public static Individual GetParent(this Population population, SelectionMethods method)
        {
            switch (method)
            {
                case SelectionMethods.Tournament:
                    return population.GetTournamentParent();
                case SelectionMethods.Roulette:
                    return population.GetRouletteParent();
                case SelectionMethods.RankedRoulette:
                    return population.GetRankedRouletteParent();
                default:
                    return population.GetParent(GeneticEnvironment.INSTANCE.defaultSelectionMethod);
            }
        }
    }
    public class Individual
    {
        public List<Coords> citiesToVisit = GeneticEnvironment.INSTANCE.ParsedCitiesToVisit;
        public int[] genotype;
        public List<Coords> Fenotype
        {
            get
            {
                List<Coords> result = new List<Coords>();
                for (int i = 0; i < genotype.Length; i++)
                {
                    result.Add(citiesToVisit[genotype[i]]);
                }
                return result;
            }
        }
        public double SurvivalScore
        {
            get
            {
                return -GeneticEnvironment.SurvivalFunction(Fenotype);
            }
        }
        public override string ToString()
        {
            return $"Fenotype : [not writeable] with score : {SurvivalScore}";
        }
    }

    public class Program
    {
        static readonly int POPULATIONSIZE = GeneticEnvironment.INSTANCE.POPULATIONSIZE;
        static readonly int POPULATIONCOUNTLIMIT = GeneticEnvironment.INSTANCE.POPULATIONCOUNTLIMIT;
        static readonly double MUTATIONPROBABILITY = GeneticEnvironment.INSTANCE.MUTATIONPROBABILITY;
        static readonly int NUMBEROFEVOLUTIONTRIALS = GeneticEnvironment.INSTANCE.NUMBEROFEVOLUTIONTRIALS;
        static readonly int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = GeneticEnvironment.INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT;

        static void Main(string[] args)
        {
            Console.WriteLine("===================================SEPARATOR================================================");
            Console.WriteLine($"New instance, passed parameters {string.Join(',', args)}");
            Console.WriteLine(DateTime.Now.ToString());
            Console.WriteLine("PMX");
            TestPMX();
            Console.WriteLine("OX");
            TestOX();
            Console.WriteLine("CX");
            TestCX();
            GeneticEnvironment.INSTANCE.ParseParameters("geneticConfig.csv");
            GeneticEnvironment.INSTANCE.LoadCities("world.tsp");
            Dictionary<int, int> iterationMaxPopulationDictRanked = new Dictionary<int, int>();
            Dictionary<int, int> iterationMaxPopulationDictRoulette = new Dictionary<int, int>();
            Dictionary<int, int> iterationMaxPopulationDictTournament = new Dictionary<int, int>();

            //Test zlozonosci obliczeniowej
            Dictionary<int, long> timeTest = new Dictionary<int, long>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Dlugosc genotypu,Czas operacji z A do B i B do A");
            for (int i = 2; i < 1000; i++)
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();
                var mum = new Individual()
                {
                    genotype = GetRandomGenotype(i)
                };
                var dad = new Individual()
                {
                    genotype = GetRandomGenotype(i)
                };
                var cut1 = GeneticEnvironment.CUBE.Next(0, mum.genotype.Length / 2);
                var cut2 = GeneticEnvironment.CUBE.Next(cut1 + 1, mum.genotype.Length);
                var lol = GetChildPMX(mum, dad, cut1, cut2);
                var lol2 = GetChildPMX(dad, mum, cut1, cut2);
                sb.AppendLine($"{i}, {timer.ElapsedTicks}");

            }

            File.WriteAllText("KrzyzowaniePMXCzasPracy.csv", sb.ToString());
            for (int i = 0; i < NUMBEROFEVOLUTIONTRIALS; i++)
            {
                Dictionary<Individual, int> heavensOne = new Dictionary<Individual, int>();
                Individual[] newRandomPopulation = GetNewRandomPopulation();
                Population pop = new Population()
                {
                    genotypes = newRandomPopulation
                };

                heavensOne = EvolvePopulationCriteriaUntilLackOfImprovment(pop.GetCopy(), ITERATIONSWITHOUTBETTERSCOREMAXCOUNT, SelectionMethods.RankedRoulette, CrossoverMethods.OX);
                SaveHeavensToFile($"C:\\REPOS\\Ewolucyjne\\RankedRoulette{i}.csv", heavensOne);
                iterationMaxPopulationDictRanked.Add(i, heavensOne.Last().Value);
                Console.WriteLine($"Ranked {i} Best:{heavensOne.Last().Key} after { heavensOne.Last().Value} population");

                Individual bestie = new Individual() { genotype = GeneticEnvironment.INSTANCE.GetRandomCitiesRoute };
                for (int j = 0; j < 50000; j++)
                {
                    Individual trial = new Individual() { genotype = GeneticEnvironment.INSTANCE.GetRandomCitiesRoute };
                    if (trial.SurvivalScore > bestie.SurvivalScore)
                    {
                        bestie = trial;
                    }

                }
                Console.WriteLine($"simply random 5000 times score : {bestie.SurvivalScore}");

                //heavensOne = EvolvePopulationCriteriaUntilLackOfImprovment(pop.GetCopy(), ITERATIONSWITHOUTBETTERSCOREMAXCOUNT, SelectionMethods.Roulette);
                //SaveHeavensToFile($"C:\\REPOS\\Ewolucyjne\\Roulette{i}.csv", heavensOne);
                //iterationMaxPopulationDictRoulette.Add(i, heavensOne.Last().Value);
                //Console.WriteLine($"Roulette {i} Best:{heavensOne.Last().Key} after { heavensOne.Last().Value} population");

                //heavensOne = EvolvePopulationCriteriaUntilLackOfImprovment(pop.GetCopy(), ITERATIONSWITHOUTBETTERSCOREMAXCOUNT, SelectionMethods.Tournament);
                //SaveHeavensToFile($"C:\\REPOS\\Ewolucyjne\\Tournament{i}.csv", heavensOne);
                //iterationMaxPopulationDictTournament.Add(i, heavensOne.Last().Value);
                //Console.WriteLine($"Tournament {i} Best:{heavensOne.Last().Key} after { heavensOne.Last().Value} population");

                //heavensOne = EvolvePopulationCriteriaMaxPopulationCount(pop, POPULATIONCOUNTLIMIT, SelectionMethods.RankedRoulette);
                //Console.WriteLine($"Trial : {i} Best: {heavensOne.Last().Key}");
            }

            //Console.WriteLine($"The worst iteration Ranked : {iterationMaxPopulationDictRanked.OrderByDescending(x => x.Value).First().Key} ");
            //Console.WriteLine($"The worst iteration Roulette : {iterationMaxPopulationDictRoulette.OrderByDescending(x => x.Value).First().Key} ");
            //Console.WriteLine($"The worst iteration Tournamet : {iterationMaxPopulationDictTournament.OrderByDescending(x => x.Value).First().Key} ");
            Console.ReadKey();
        }
        private static int[] GetRandomGenotype(int length)
        {
            int[] genotype = new int[length];
            for (int i = 0; i < length; i++)
            {
                genotype[i] = i + 1;
            }
            genotype.Shuffle();
            return genotype;
        }

        private static void TestPMX()
        {
            var mum = new Individual()
            {

                genotype = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };
            var dad = new Individual()
            {
                genotype = new int[9] { 4, 5, 2, 1, 8, 7, 6, 9, 3 }
            };

            var lol = GetChildPMX(mum, dad, 3, 7);
            var lol2 = GetChildPMX(dad, mum, 3, 7);
            foreach (var city in lol.genotype)
            {
                Console.Write(city + ",");
            }
            Console.WriteLine();
            foreach (var city in lol2.genotype)
            {
                Console.Write(city + ",");
            }
            Console.WriteLine();
        }
        private static void TestOX()
        {
            var mum = new Individual()
            {
                genotype = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };
            var dad = new Individual()
            {
                genotype = new int[9] { 4, 5, 2, 1, 8, 7, 6, 9, 3 }
            };

            var lol = GetChildOX(mum, dad, 3, 7);
            var lol2 = GetChildOX(dad, mum, 3, 7);
            foreach (var city in lol.genotype)
            {
                Console.Write(city + ",");
            }
            Console.WriteLine();
            foreach (var city in lol2.genotype)
            {
                Console.Write(city + ",");
            }
            Console.WriteLine();
        }
        private static void TestCX()
        {
            var mum = new Individual()
            {

                genotype = new int[9] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            };
            var dad = new Individual()
            {
                genotype = new int[9] { 4, 1, 2, 8, 7, 6, 9, 3, 5 }
            };

            var lol = GetChildCX(mum, dad, 3, 7);
            var lol2 = GetChildCX(dad, mum, 3, 7);
            foreach (var city in lol.genotype)
            {
                Console.Write(city + ",");
            }
            Console.WriteLine();
            foreach (var city in lol2.genotype)
            {
                Console.Write(city + ",");
            }
            Console.WriteLine();
        }

        private static void SaveHeavensToFile(string v, Dictionary<Individual, int> heavensOne)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var line in heavensOne)
            {
                sb.AppendLine($"{line.Value},{line.Key.SurvivalScore}");
            }
            File.WriteAllText(v, sb.ToString());
        }

        private static Dictionary<Individual, int> EvolvePopulationCriteriaMaxPopulationCount(Population pop, int pOPULATIONCOUNTLIMIT, SelectionMethods selectionMethod, CrossoverMethods crossover)
        {
            Dictionary<Individual, int> heavenPopulationDict = new Dictionary<Individual, int>() { { pop.BestOne, 0 } };
            var populationCount = 0;
            while (populationCount < pOPULATIONCOUNTLIMIT)
            {
                pop = GetNextGeneration(pop, selectionMethod, crossover);
                if (heavenPopulationDict.ElementAt(heavenPopulationDict.Count - 1).Key.SurvivalScore < pop.BestOne.SurvivalScore)
                {
                    heavenPopulationDict.Add(pop.BestOne, populationCount);
                }
                populationCount++;
            }
            return heavenPopulationDict;
        }

        private static Dictionary<Individual, int> EvolvePopulationCriteriaUntilLackOfImprovment(Population pop, int maxIterationsWithoutImprovement, SelectionMethods selectionMethod, CrossoverMethods crossover)
        {
            Dictionary<Individual, int> heavenPopulationDict = new Dictionary<Individual, int>() { { pop.BestOne, 0 } };
            var populationCount = 0;
            int NoNimprovementCounter = 0;
            while (NoNimprovementCounter <= maxIterationsWithoutImprovement)
            {
                pop = GetNextGeneration(pop, selectionMethod, crossover);
                if (heavenPopulationDict.ElementAt(heavenPopulationDict.Count - 1).Key.SurvivalScore < pop.BestOne.SurvivalScore)
                {
                    heavenPopulationDict.Add(pop.BestOne, populationCount);
                    NoNimprovementCounter = 0;
                }
                else
                {
                    NoNimprovementCounter++;
                }
                populationCount++;
            }
            return heavenPopulationDict;
        }

        private static Individual[] GetNewRandomPopulation()
        {
            Individual[] population = new Individual[POPULATIONSIZE];

            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                population[i] = new Individual()
                {
                    genotype = GeneticEnvironment.INSTANCE.GetRandomCitiesRoute
                };
            }
            return population;
        }
        private static Population GetNextGeneration(Population population, SelectionMethods method, CrossoverMethods crossover)
        {
            Individual mum, dad;
            mum = population.GetParent(method);
            dad = population.GetParent(method);
            var newGenotypes = new Individual[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                var child = GetRandomChild(mum, dad, crossover);
                for (int j = 0; j < GeneticEnvironment.INSTANCE.MUTATIONRETRIALS; j++)//without x >4 times mutation often gets stuck in local maximums
                {
                    if (GeneticEnvironment.CUBE.NextDouble() < MUTATIONPROBABILITY)
                    {
                        child = Mutate(child.genotype);
                    }
                }
                newGenotypes[i] = child;
            }

            population.genotypes = newGenotypes;

            return population;
        }
        static Individual GetRandomChild(Individual mum, Individual dad, CrossoverMethods crossover)
        {
            Individual child = new Individual();
            switch (crossover)
            {
                case CrossoverMethods.PMX:
                    var cut1 = GeneticEnvironment.CUBE.Next(0, mum.genotype.Length / 2);
                    var cut2 = GeneticEnvironment.CUBE.Next(cut1 + 1, mum.genotype.Length);
                    child = GetChildPMX(mum, dad, cut1, cut2);
                    break;
                case CrossoverMethods.OX:
                    cut1 = GeneticEnvironment.CUBE.Next(0, mum.genotype.Length / 2);
                    cut2 = GeneticEnvironment.CUBE.Next(cut1 + 1, mum.genotype.Length);
                    child = GetChildOX(mum, dad, cut1, cut2);
                    break;
                case CrossoverMethods.CX:
                    cut1 = GeneticEnvironment.CUBE.Next(0, mum.genotype.Length / 2);
                    cut2 = GeneticEnvironment.CUBE.Next(cut1 + 1, mum.genotype.Length);
                    child = GetChildCX(mum, dad, cut1, cut2);
                    break;
                default:
                    var separationPos = GeneticEnvironment.CUBE.Next(1, mum.genotype.Length);
                    child.genotype = new int[mum.genotype.Length];
                    for (int i = 0; i < separationPos; i++)
                    {
                        child.genotype[i] = mum.genotype[i];
                    }
                    for (int i = separationPos; i < mum.genotype.Length; i++)
                    {
                        child.genotype[i] = dad.genotype[i];
                    }
                    break;
            }

            return new Individual() { genotype = child.genotype };
        }

        private static Individual GetChildCX(Individual mum, Individual dad, int cut1, int cut2)
        {
            int[] newGenotype = new int[mum.genotype.Length];
            for (int i = 0; i < newGenotype.Length; i++)
            {
                newGenotype[i] = -1;
            }
            var mumIterator = 0;
            var dadIterator = 0;
            var newIterator = 0;
            var cycle = false;
            do
            {
                newGenotype[newIterator] = dad.genotype[dadIterator];
                dadIterator = Array.IndexOf(dad.genotype, mum.genotype[dadIterator]);
                if (newGenotype[dadIterator] != -1)
                {
                    cycle = true;
                }
                newIterator = dadIterator;
            } while (!cycle);

            for (int i = 0; i < newGenotype.Length; i++)
            {
                if (newGenotype[i] == -1)
                {
                    newGenotype[i] = mum.genotype[i];
                }
            }
            return new Individual() { genotype = newGenotype };
        }
        private static Individual GetChildOX(Individual mum, Individual dad, int cut1, int cut2)
        {
            int[] newGenotype = new int[mum.genotype.Length];
            for (int i = 0; i < newGenotype.Length; i++)
            {
                newGenotype[i] = -1;
            }
            for (int i = cut1; i < cut2; i++)
            {
                newGenotype[i] = mum.genotype[i];
            }
            var newIterator = cut2;
            var dadIterator = Array.IndexOf(dad.genotype, mum.genotype[cut2 - 1]);
            do
            {
                var valueToCheck = dad.genotype[dadIterator % dad.genotype.Length];
                if (Array.IndexOf(newGenotype, valueToCheck) == -1)
                {
                    newGenotype[newIterator % newGenotype.Length] = valueToCheck;
                    newIterator++;
                }
                dadIterator++;

            } while (Array.IndexOf(newGenotype, -1) != -1);

            return new Individual() { genotype = newGenotype };
        }
        private static Individual GetChildPMX(Individual mum, Individual dad, int cut1, int cut2)
        {
            int[] newGenotype = new int[mum.genotype.Length];
            for (int i = cut1; i < cut2; i++)
            {
                newGenotype[i] = mum.genotype[i];
            }
            for (int i = 0; i < cut1; i++)
            {
                if (!newGenotype.Contains(dad.genotype[i]))
                {
                    newGenotype[i] = dad.genotype[i];
                }
                else
                {
                    var newIndexInDad = Array.IndexOf(mum.genotype, dad.genotype[i]);
                    newGenotype[i] = dad.genotype[newIndexInDad];
                }
            }
            for (int i = cut2; i < mum.genotype.Length; i++)
            {
                if (!newGenotype.Contains(dad.genotype[i]))
                {
                    newGenotype[i] = dad.genotype[i];
                }
                else
                {
                    var newIndexInDad = Array.IndexOf(mum.genotype, dad.genotype[i]);
                    newGenotype[i] = dad.genotype[newIndexInDad];
                }
            }
            return new Individual() { genotype = newGenotype };
        }

        static Individual Mutate(int[] a)
        {
            int randomPosition = GeneticEnvironment.CUBE.Next(1, a.Length - 1);
            int randomPosition2 = GeneticEnvironment.CUBE.Next(1, a.Length - 1);
            int temp = a[randomPosition];
            a[randomPosition] = a[randomPosition2];
            a[randomPosition2] = temp;

            return new Individual() { genotype = a };
        }
        static void PrintBinary(uint a)
        {
            Console.WriteLine(Convert.ToString(a, 2).PadLeft(32).Replace(" ", "0"));
        }
    }
}
namespace srodowisko
{
    public class ProblemKlienta
    {
        public List<Coords> citiesToVisit = GeneticEnvironment.INSTANCE.ParsedCitiesToVisit;

        public List<Coords> Fenotype(int[] genotype)
        {
            List<Coords> result = new List<Coords>();
            for (int i = 0; i < genotype.Length; i++)
            {
                result.Add(citiesToVisit[genotype[i]]);
            }
            return result;
        }
        public int Rozmiar(int numer_zbioru = 0)
        {
            return 2;
        }
        public double Ocena(int[] sciezka)
        {
            return SurvivalFunction(Fenotype(sciezka));
        }

        private static double SurvivalFunction(List<Coords> coordsToVisit)
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
}

