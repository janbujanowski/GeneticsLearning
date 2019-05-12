using AlgorytmEwolucyjny;
using srodowisko;
using System;
using System.IO;

namespace AlgorytmEwolucyjny
{
    public class Program
    {
        static readonly int POPULATIONSIZE = GeneticEnvironment.INSTANCE.POPULATIONSIZE;
        static readonly int POPULATIONCOUNTLIMIT = GeneticEnvironment.INSTANCE.POPULATIONCOUNTLIMIT;
        static readonly double MUTATIONPROBABILITY = GeneticEnvironment.INSTANCE.MUTATIONPROBABILITY;
        static readonly int NUMBEROFEVOLUTIONTRIALS = GeneticEnvironment.INSTANCE.NUMBEROFEVOLUTIONTRIALS;
        static readonly int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = GeneticEnvironment.INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT;

        static void Main(string[] args)
        {
            LogInfo("===================================SEPARATOR================================================");
            LogInfo($"New instance, passed parameters {string.Join(",", args)}");

            RunTests();
            GeneticEnvironment.INSTANCE.ParseParameters(args);
            GeneticEnvironment.INSTANCE.LoadCities("cities.tsp");


            for (int i = 0; i < NUMBEROFEVOLUTIONTRIALS; i++)
            {
                StatsInfo[] heavensOne = new StatsInfo[1];
                Individual[] newRandomPopulation = GetNewRandomPopulation();
                Population pop = new Population()
                {
                    genotypes = newRandomPopulation
                };

                heavensOne = EvolvePopulationCriteriaUntilDateStop(pop.GetCopy(), GeneticEnvironment.INSTANCE.StopDate, GeneticEnvironment.INSTANCE.SelectionMethod, GeneticEnvironment.INSTANCE.CrossoverMethod);

                LogInfo("==============Heavens csv===================");
                foreach (var line in heavensOne)
                {
                    LogInfo($"{line.Population},{line.Individual.SurvivalScore}");
                }
                LogInfo($"Ranked {i} Best:{ string.Join(",", heavensOne[heavensOne.Length - 1].Individual.genotype)} after { heavensOne[heavensOne.Length - 1].Population} population");
            }
            Console.ReadKey();
        }

        #region Metody testowe 
        private static void RunTests()
        {
            Console.WriteLine("PMX");
            TestPMX();
            Console.WriteLine("OX");
            TestOX();
            Console.WriteLine("CX");
            TestCX();
            LogInfo("Test losowości");
            for (int i = 0; i < 1000; i++)
            {
                LogInfo($"{GeneticEnvironment.CUBE.Next()},{GeneticEnvironment.CUBE.NextDouble()}");
            }
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
            LogInfo(string.Join(",", lol.genotype));
            LogInfo(string.Join(",", lol2.genotype));
        }
        #endregion
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
        //private static Dictionary<Individual, int> EvolvePopulationCriteriaMaxPopulationCount(Population pop, int pOPULATIONCOUNTLIMIT, SelectionMethods selectionMethod, CrossoverMethods crossover)
        //{
        //    Dictionary<Individual, int> heavenPopulationDict = new Dictionary<Individual, int>() { { pop.BestOne, 0 } };
        //    var populationCount = 0;
        //    while (populationCount < pOPULATIONCOUNTLIMIT)
        //    {
        //        pop = GetNextGeneration(pop, selectionMethod, crossover);
        //        if (heavenPopulationDict.ElementAt(heavenPopulationDict.Count - 1).Key.SurvivalScore < pop.BestOne.SurvivalScore)
        //        {
        //            heavenPopulationDict.Add(pop.BestOne, populationCount);
        //        }
        //        populationCount++;
        //    }
        //    return heavenPopulationDict;
        //}
        //private static Dictionary<Individual, int> EvolvePopulationCriteriaUntilLackOfImprovment(Population pop, int maxIterationsWithoutImprovement, SelectionMethods selectionMethod, CrossoverMethods crossover)
        //{
        //    Dictionary<Individual, int> heavenPopulationDict = new Dictionary<Individual, int>() { { pop.BestOne, 0 } };
        //    var populationCount = 0;
        //    int NoNimprovementCounter = 0;
        //    while (NoNimprovementCounter <= maxIterationsWithoutImprovement)
        //    {
        //        pop = GetNextGeneration(pop, selectionMethod, crossover);
        //        if (heavenPopulationDict.ElementAt(heavenPopulationDict.Count - 1).Key.SurvivalScore < pop.BestOne.SurvivalScore)
        //        {
        //            heavenPopulationDict.Add(pop.BestOne, populationCount);
        //            LogInfo($"Found new solution with score {pop.BestOne.SurvivalScore}");
        //            NoNimprovementCounter = 0;
        //        }
        //        else
        //        {
        //            NoNimprovementCounter++;
        //        }
        //        populationCount++;
        //    }
        //    return heavenPopulationDict;
        //}
        private static StatsInfo[] EvolvePopulationCriteriaUntilDateStop(Population pop, DateTime dateStop, SelectionMethods selectionMethod, CrossoverMethods crossover)
        {
            StatsInfo[] heavenPopulationDict;
            if (GeneticEnvironment.INSTANCE.BestGenotype != null)
            {
                heavenPopulationDict = new StatsInfo[1] { new StatsInfo() { Individual = new Individual() { genotype = GeneticEnvironment.INSTANCE.BestGenotype }, Population = 0 } };
            }
            else
            {
                heavenPopulationDict = new StatsInfo[1] { new StatsInfo() { Individual = pop.BestOne, Population = 0 } };
            }
            var populationCount = 0;
            int NoNimprovementCounter = 0;
            while (DateTime.Now <= dateStop)
            {
                pop = GetNextGeneration(pop, selectionMethod, crossover);
                if (heavenPopulationDict[heavenPopulationDict.Length - 1].Individual.SurvivalScore < pop.BestOne.SurvivalScore)
                {
                    heavenPopulationDict = heavenPopulationDict.Add(new StatsInfo() { Individual = pop.BestOne, Population = populationCount });
                    LogInfo($"Found new solution with score {pop.BestOne.SurvivalScore}");
                    NoNimprovementCounter = 0;
                }

                populationCount++;
            }
            LogInfo("Stopping evolution because of reaching dateStop :" + dateStop.ToString());
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
                var contains = Array.Find(dad.genotype, x => x == dad.genotype[i]);
                if (contains == -1)
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
                var contains = Array.Find(dad.genotype, x => x == dad.genotype[i]);
                if (contains == -1)
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
        static void LogInfo(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString()}]: {message}");
        }
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
        public int NUMBEROFEVOLUTIONTRIALS = 1;
        public int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = 5000;


        public int[] BestGenotype { get; private set; }
        public SelectionMethods SelectionMethod = SelectionMethods.Roulette;
        public CrossoverMethods CrossoverMethod = CrossoverMethods.OX;
        public int MUTATIONRETRIALS = 4;

        public Coords[] ParsedCitiesToVisit;
        public int[] GetRandomCitiesRoute
        {
            get
            {
                genotypeExample.Shuffle();
                return genotypeExample;
            }
        }
        private int[] genotypeExample;
        public void LoadCities(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int start = FindStartingIndex(lines);
                var end = FindEndIndex(lines);
                INSTANCE.ParsedCitiesToVisit = new Coords[end - start];
                for (int i = start; i < end; i++)
                {
                    string[] values = lines[i].Split(' ');
                    double lattitude = double.Parse(values[1]);
                    double longitude = double.Parse(values[2]);
                    ParsedCitiesToVisit[i - start] = new Coords()
                    {
                        X = lattitude,
                        Y = longitude
                    };
                }
                genotypeExample = new int[ParsedCitiesToVisit.Length];
                for (int i = 0; i < ParsedCitiesToVisit.Length; i++)
                {
                    genotypeExample[i] = i;
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

        public DateTime StopDate { get; private set; }

        public static double SurvivalFunction(Coords[] coordsToVisit)
        {
            double distance = 0;
            for (int i = 0; i < coordsToVisit.Length - 2; i++)
            {
                distance += DistanceBetweenCoords(coordsToVisit[i], coordsToVisit[i + 1]);
            }
            return distance;
        }
        private static double DistanceBetweenCoords(Coords startCoord, Coords endCoord)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(endCoord.Y - startCoord.Y), 2) + Math.Pow(Math.Abs(endCoord.X - startCoord.X), 2));
        }

        internal void ParseParameters(string[] args)
        {
            try
            {
                INSTANCE.StopDate = DateTime.Parse(args[0]);
                INSTANCE.POPULATIONSIZE = Int32.Parse(args[1]);//popsize
                INSTANCE.MUTATIONPROBABILITY = double.Parse(args[2]);//mutationprob
                INSTANCE.MUTATIONRETRIALS = Int32.Parse(args[3]);//mutationretry
                INSTANCE.SelectionMethod = (SelectionMethods)Enum.Parse(typeof(SelectionMethods), args[4]);//enum
                INSTANCE.CrossoverMethod = (CrossoverMethods)Enum.Parse(typeof(CrossoverMethods), args[5]);//OXCX
                INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = Int32.Parse(args[6]);//interationnonimprove
                if (args.Length > 7)
                {
                    var genotypeArray = args[7].Split(',');
                    INSTANCE.BestGenotype = new int[genotypeArray.Length];
                    for (int i = 0; i < genotypeArray.Length; i++)
                    {
                        INSTANCE.BestGenotype[i] = Int32.Parse(genotypeArray[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error parsing parameters :" + ex.Message);
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
                return genotypes.OrderBySurvivalScore()[genotypes.Length - 1];
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
    public class Individual
    {
        public Coords[] citiesToVisit = GeneticEnvironment.INSTANCE.ParsedCitiesToVisit;
        public int[] genotype;
        public Coords[] Fenotype
        {
            get
            {
                Coords[] result = new Coords[genotype.Length];
                for (int i = 0; i < genotype.Length; i++)
                {
                    result[i] = citiesToVisit[genotype[i]];
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
    public class StatsInfo
    {
        public Individual Individual { get; set; }
        public int Population { get; set; }
    }
    public struct Coords
    {
        public double X, Y;
    }
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
    public static class GeneticExtensions
    {
        public static Individual[] OrderBySurvivalScore(this Individual[] genotypes)
        {
            Array.Sort(genotypes, delegate (Individual x, Individual y) { return x.SurvivalScore.CompareTo(y.SurvivalScore); });
            return genotypes;
        }

        public static Individual GetTournamentParent(this Population population)
        {
            int TOURNAMENTSIZE = 2;
            Individual[] competitors = new Individual[TOURNAMENTSIZE];
            for (int i = 0; i < TOURNAMENTSIZE; i++)
            {
                competitors = competitors.Add(population.genotypes[GeneticEnvironment.CUBE.Next(0, population.genotypes.Length)]);
            }
            return competitors.OrderBySurvivalScore()[competitors.Length - 1];
        }

        public static Individual GetRouletteParent(this Population population)
        {
            var sumArray = population.genotypes.Sum();
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
            population.genotypes.OrderBySurvivalScore();

            var sumArray = ((population.genotypes.Length - 1) / 2) * population.genotypes.Length;
            Individual parent = null;
            var i = 0;
            var sum = 0;
            var rankedChosenValue = GeneticEnvironment.CUBE.Next(1, sumArray);
            while (parent == null)
            {
                if (rankedChosenValue <= sum)
                {
                    parent = population.genotypes[i];
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
                    return population.GetParent(GeneticEnvironment.INSTANCE.SelectionMethod);
            }
        }
    }
    public static class GenericExtensions
    {
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
        public static double Sum(this Individual[] arrayOfStuff)
        {
            double sum = 0;
            foreach (var item in arrayOfStuff)
            {
                sum += item.SurvivalScore;
            }
            return sum;
        }
        public static T[] Add<T>(this T[] array, T newItem)
        {
            var newLength = array.Length + 1;
            var newArray = new T[newLength];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = newItem;
            return newArray;
        }
    }
}

namespace srodowisko
{
    public class ProblemKlienta
    {
        public int Rozmiar(int numer_zbioru = 0)
        {
            return 2;
        }
        public double Ocena(int[] sciezka)
        {
            return -1;
        }

        private static double DistanceBetweenCoords(Coords startCoord, Coords endCoord)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(endCoord.Y - startCoord.Y), 2) + Math.Pow(Math.Abs(endCoord.X - startCoord.X), 2));
        }
    }
}
