using srodowisko;
using System;

namespace AlgorytmEwolucyjny
{
    public class Program
    {

        static void Main(string[] args)
        {
            LogInfo("===================================SEPARATOR================================================");
            LogInfo($"New instance, passed parameters {string.Join(",", args)}");

            RunTests();
            GeneticEnvironment.INSTANCE.ParseParameters(args);

            for (int i = 0; i < GeneticEnvironment.INSTANCE.NUMBEROFEVOLUTIONTRIALS; i++)
            {
                StatsInfo[] heavensOne = new StatsInfo[1];
                Individual[] newRandomPopulation = GetNewRandomPopulation();
                Population pop = new Population()
                {
                    genotypes = newRandomPopulation
                };

                heavensOne = EvolvePopulationCriteriaUntilDateStop(pop.GetCopy(), GeneticEnvironment.INSTANCE.StopDate, GeneticEnvironment.INSTANCE.SelectionMethod, GeneticEnvironment.INSTANCE.CrossoverMethod);

                LogInfo("===================================Heavens csv================================================");
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
            TestPMX();
            TestOX();
            TestCX();
            LogInfo("Test losowości");
            for (int i = 0; i < 1000; i++)
            {
                LogInfo($"{GeneticEnvironment.CUBE.Next()},{GeneticEnvironment.CUBE.NextDouble()}");
            }
        }
        private static void TestPMX()
        {
            LogInfo("Test PMX");
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
            LogInfo(string.Join(",", lol.genotype));
            LogInfo(string.Join(",", lol2.genotype));
        }
        private static void TestOX()
        {
            LogInfo("Test OX");
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
            LogInfo(string.Join(",", lol.genotype));
            LogInfo(string.Join(",", lol2.genotype));
        }
        private static void TestCX()
        {
            LogInfo("Test CX");
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
            LogInfo($"Początkowy najlepszy osobnik {heavenPopulationDict[heavenPopulationDict.Length - 1].Individual.ToString()}");
            while (DateTime.Now <= dateStop)
            {
                pop = GetNextGeneration(pop, selectionMethod, crossover, heavenPopulationDict[heavenPopulationDict.Length - 1].Individual);
                if (heavenPopulationDict[heavenPopulationDict.Length - 1].Individual.SurvivalScore * GeneticEnvironment.INSTANCE.ModyfikatorWyniku < GeneticEnvironment.INSTANCE.ModyfikatorWyniku * pop.BestOne.SurvivalScore)
                {
                    heavenPopulationDict = heavenPopulationDict.Add(new StatsInfo() { Individual = pop.BestOne, Population = populationCount });
                    LogInfo($"Nowy osobnik {pop.BestOne.ToString()}");
                }

                populationCount++;
            }
            LogInfo("Koniec ewolucji z powodu limitu czasu : " + dateStop.ToString());
            return heavenPopulationDict;
        }
        private static Individual[] GetNewRandomPopulation()
        {
            Individual[] population = new Individual[GeneticEnvironment.INSTANCE.POPULATIONSIZE];

            for (int i = 0; i < GeneticEnvironment.INSTANCE.POPULATIONSIZE; i++)
            {
                population[i] = new Individual()
                {
                    genotype = GeneticEnvironment.INSTANCE.GetRandomCitiesRoute
                };
            }
            return population;
        }
        private static Population GetNextGeneration(Population population, SelectionMethods method, CrossoverMethods crossover, Individual currentHeaven)
        {
            Individual mum, dad;
            mum = population.GetParent(method);
            dad = population.GetParent(method);
            var newGenotypes = new Individual[GeneticEnvironment.INSTANCE.POPULATIONSIZE];
            for (int i = 0; i < GeneticEnvironment.INSTANCE.POPULATIONSIZE; i++)
            {
                var child = GetRandomChild(mum, dad, crossover);
                for (int j = 0; j < GeneticEnvironment.INSTANCE.MUTATIONRETRIALS; j++)//without x >4 times mutation often gets stuck in local maximums
                {
                    if (GeneticEnvironment.CUBE.NextDouble() < GeneticEnvironment.INSTANCE.MUTATIONPROBABILITY)
                    {
                        child = Mutate(child.genotype);
                    }
                }
                newGenotypes[i] = child;
            }
            newGenotypes = newGenotypes.Add(currentHeaven);
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
        public double ModyfikatorWyniku = 1;

        public int[] BestGenotype { get; private set; }
        public SelectionMethods SelectionMethod = SelectionMethods.Roulette;
        public CrossoverMethods CrossoverMethod = CrossoverMethods.OX;
        public int MUTATIONRETRIALS = 4;
        ProblemKlienta ProblemKlienta;
        public Coords[] ParsedCitiesToVisit;
        public int[] GetRandomCitiesRoute
        {
            get
            {
                int[] genotype = new int[ProblemKlienta.Rozmiar()];
                for (int i = 0; i < ProblemKlienta.Rozmiar(); i++)
                {
                    genotype[i] = i;
                }
                genotype.Shuffle();
                return genotype;
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
            this.ProblemKlienta = new ProblemKlienta();
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

        public static double SurvivalFunction(int[] sciezka)
        {
            return INSTANCE.ProblemKlienta.Ocena(sciezka);
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
                INSTANCE.ModyfikatorWyniku = double.Parse(args[7]);//1 lub -1 zaleznie od rodzaju problemu maksymalizacji/minimalizacji
                if (args.Length > 8)
                {
                    var genotypeArray = args[8].Split(',');
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
                if (GeneticEnvironment.INSTANCE.ModyfikatorWyniku < 0)
                {
                    return genotypes.OrderBySurvivalScore()[0];
                }
                else
                {
                    return genotypes.OrderBySurvivalScore()[genotypes.Length - 1];
                }
            }
        }
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
        public bool CzyPoprawny
        {
            get
            {
                bool czy = true;
                int[] testArray = new int[1] { -1 };
                for (int i = 0; i < genotype.Length; i++)
                {
                    if (!Array.Exists(testArray, x=> x == genotype[i]))
                    {
                        testArray = testArray.Add(genotype[i]);
                    }
                    else
                    {
                        czy = false;
                        break;
                    }
                }
                return czy;
            }
        }
        public int[] genotype;
        public int[] Fenotype
        {
            get
            {
                return genotype;
            }
        }
        public double SurvivalScore
        {
            get
            {
                return GeneticEnvironment.SurvivalFunction(Fenotype);
            }
        }
        public override string ToString()
        {
            return $"Dlugosc trasy :{SurvivalScore} genotyp {genotype.ToScreen()} Czy poprawny ? : {CzyPoprawny}";
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
            var sorted = population.genotypes.OrderBySurvivalScore();
            var theLastIndividual = sorted[sorted.Length - 1];
            double maxSumRange = 0;
            for (int j = 0; j < population.genotypes.Length; j++)
            {
                maxSumRange += theLastIndividual.SurvivalScore / population.genotypes[j].SurvivalScore;
            }
            Individual parent = null;
            var i = 0;
            var sum = theLastIndividual.SurvivalScore / population.genotypes[i].SurvivalScore;
            var randomScoreValue = GeneticEnvironment.CUBE.NextDouble() * maxSumRange;
            while (parent == null)
            {
                if (randomScoreValue <= sum)
                {
                    parent = population.genotypes[i];
                    break;
                }
                i++;
                sum += theLastIndividual.SurvivalScore / population.genotypes[i].SurvivalScore;
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
        public static string ToScreen(this int[] array)
        {
            return string.Join(",", array);
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