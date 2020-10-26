using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neurox.ConsoleGUI
{
    public class NeuroxEvolution
    {
        public StatsInfo BestOutcome { get; set; }
        GeneticEnvironment _geneticParameters;
        public NeuroxEvolution(GeneticEnvironment geneticParameters)
        {
            _geneticParameters = geneticParameters;
            ILogger loggerInstance = new DirectFileLogger();
            MarketFunctions market = new MarketFunctions(loggerInstance);
            //RunTests();
            _geneticParameters.StartDate = DateTime.Now;
            
            StatsInfo[] heavensOne;
            Individual[] newRandomPopulation = GetNewRandomPopulation(_geneticParameters.NrProblemu);
            LogInfo($"Nr Problemu {_geneticParameters.NrProblemu} Rozmiar tablicy : {newRandomPopulation[0].genotype.Length} ");
            Population pop = new Population()
            {
                genotypes = newRandomPopulation
            };
            try
            {
                heavensOne = EvolvePopulationCriteriaUntilDateStop(loggerInstance, pop.GetCopy(), _geneticParameters.StopDate, _geneticParameters.SelectionMethod, _geneticParameters.CrossoverMethod);
                var bestIndividualStats = heavensOne.Last();
                LogInfo($"Najlepszy osobnik z populacji { bestIndividualStats.Population } { bestIndividualStats.Individual.ToString() }");
                LogInfo($"Fenotyp : { string.Join(",", bestIndividualStats.Individual.Fenotype) }");
                LogInfo($"Genotyp : { string.Join(",", bestIndividualStats.Individual.genotype) }");
                string csvFileContent = string.Empty;
                var lolek = heavensOne.SelectMany(x => string.Join(",", x.Population.ToString(), x.Individual.SurvivalScore, x.Individual.TestedSurvivalScore, x.Date) + Environment.NewLine);
                csvFileContent += string.Join("", lolek);
                BestOutcome = bestIndividualStats;
                BestOutcome.InputLayerModifiers = bestIndividualStats.Individual.genotype.GetInputLayerModifiers();
                BestOutcome.HiddenLayerWeights = bestIndividualStats.Individual.genotype.GetHiddenLayerWeights();
                BestOutcome.HiddenToOutputLayerWeights = bestIndividualStats.Individual.genotype.GetHiddenToOutputLayerWeights();
                //File.WriteAllText(IoCFactory.Resolve<IConfigurationProvider>().GetConfiguredFilePath("workingDirectory", "pathToOutputCsvFile"), csvFileContent);
                string jsonIndividual = JsonConvert.SerializeObject(bestIndividualStats.Individual, Formatting.Indented);
                //File.WriteAllText(IoCFactory.Resolve<IConfigurationProvider>().GetConfiguredFilePath("workingDirectory", "StartingIndividual"), jsonIndividual);
            }
            catch (Exception ex)
            {
                loggerInstance.LogException(ex, "Critical evolution error");
            }
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
        private StatsInfo[] EvolvePopulationCriteriaUntilDateStop(ILogger loggerInstance, Population pop, DateTime dateStop, SelectionMethods selectionMethod, CrossoverMethods crossover)
        {
            StatsInfo[] heavenPopulationDict;
            if (_geneticParameters.BestGenotype != null)
            {
                heavenPopulationDict = new StatsInfo[1] { new StatsInfo() { Individual = new Individual() { genotype = _geneticParameters.BestGenotype }, Population = 0, Date = DateTime.Now } };
            }
            else
            {
                heavenPopulationDict = new StatsInfo[1] { new StatsInfo() { Individual = pop.BestOne, Population = 0, Date = DateTime.Now } };
            }
            pop.genotypes.Add(new Individual() { genotype = _geneticParameters.BestGenotype });
            var populationCount = 0;
            LogInfo($"Początkowy najlepszy osobnik {heavenPopulationDict[heavenPopulationDict.Length - 1].Individual.ToString()}");
            while (DateTime.Now <= dateStop)
            {
                pop = GetNextGeneration(pop, selectionMethod, crossover, heavenPopulationDict[heavenPopulationDict.Length - 1].Individual);
                if (heavenPopulationDict[heavenPopulationDict.Length - 1].Individual.SurvivalScore * _geneticParameters.ModyfikatorWyniku < _geneticParameters.ModyfikatorWyniku * pop.BestOne.SurvivalScore)
                {
                    heavenPopulationDict = heavenPopulationDict.Add(new StatsInfo() { Individual = pop.BestOne, Population = populationCount, Date = DateTime.Now });
                    loggerInstance.LogInfo($"Populacja { populationCount } Nowy osobnik { pop.BestOne.ToString() }");
                    string csvFileContent = string.Empty;

                    var lolek = string.Join(",", populationCount, pop.BestOne.SurvivalScore, pop.BestOne.TestedSurvivalScore, DateTime.Now) + Environment.NewLine;
                    csvFileContent += string.Join("", lolek);
                    File.AppendAllText(IoCFactory.Resolve<IConfigurationProvider>().GetConfiguredFilePath("workingDirectory", "pathToOutputCsvFile"), csvFileContent);
                    string jsonIndividual = JsonConvert.SerializeObject(pop.BestOne, Formatting.Indented);
                    File.WriteAllText(IoCFactory.Resolve<IConfigurationProvider>().GetConfiguredFilePath("workingDirectory", "StartingIndividual"), jsonIndividual);
                }
                populationCount++;
                //if (populationCount % 20 == 0)
                //{
                Console.WriteLine($"|{ DateTime.Now.ToString() }|: Populacja : {populationCount} ");
                //}
            }
            LogInfo("Koniec ewolucji z powodu limitu czasu : " + dateStop.ToString());
            LogInfo($"Czas pracy w minutach : {(_geneticParameters.StopDate - _geneticParameters.StartDate).TotalMinutes}");
            return heavenPopulationDict;
        }
        private Individual[] GetNewRandomPopulation(int nrProblemu)
        {
            Individual[] population = new Individual[_geneticParameters.POPULATIONSIZE];

            for (int i = 0; i < _geneticParameters.POPULATIONSIZE; i++)
            {
                population[i] = new Individual()
                {
                    genotype = _geneticParameters.GetRandomParametersArray(nrProblemu)
                };
            }
            return population;
        }
        private Population GetNextGeneration(Population population, SelectionMethods method, CrossoverMethods crossover, Individual currentHeaven)
        {
            Individual mum, dad;
            mum = population.GetParent(method);
            dad = population.GetParent(method);
            var newGenotypes = new Individual[_geneticParameters.POPULATIONSIZE];
            for (int i = 0; i < _geneticParameters.POPULATIONSIZE; i++)
            {
                var child = GetRandomChild(mum, dad, crossover);
                for (int j = 0; j < _geneticParameters.MUTATIONRETRIALS; j++)//without x >4 times mutation often gets stuck in local maximums
                {
                    if (GeneticEnvironment.CUBE.NextDouble() < _geneticParameters.MUTATIONPROBABILITY)
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
            a[randomPosition] = GeneticEnvironment.CUBE.Next(-1000, 1000);
            return new Individual() { genotype = a };
        }
        static void LogInfo(string message)
        {
            var logger = IoCFactory.Resolve<ILogger>();
            logger.LogInfo(message);
        }
    }

    public struct Population
    {
        public Individual[] genotypes;
        public Individual BestOne
        {
            get
            {
                if (GeneticDefaults.INSTANCE.ModyfikatorWyniku < 0)
                {
                    return genotypes.OrderBy(p => p.SurvivalScore).ToArray()[0];
                }
                else
                {
                    return genotypes.OrderBy(p => p.SurvivalScore).ToArray()[genotypes.Length - 1];
                }
            }
        }
        public Population GetCopy()
        {
            Individual[] copiedGenotypes = new Individual[genotypes.Length];
            for (int i = 0; i < copiedGenotypes.Length; i++)
            {
                copiedGenotypes[i] = new Individual() { genotype = genotypes[i].genotype };
            }
            return new Population { genotypes = this.genotypes };
        }
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
        CX,
        Basic
    }
    public static class GeneticExtensions
    {

        public static Individual GetTournamentParent(this Population population)
        {
            int TOURNAMENTSIZE = 2;
            Individual[] competitors = new Individual[TOURNAMENTSIZE];
            for (int i = 0; i < TOURNAMENTSIZE; i++)
            {
                competitors[i] = population.genotypes[GeneticEnvironment.CUBE.Next(0, population.genotypes.Length)];
            }
            return competitors.OrderBy(p => p.SurvivalScore).ToArray()[competitors.Length - 1];
        }

        public static Individual GetRouletteParent(this Population population)
        {
            var sorted = population.genotypes.OrderBy(p => p.SurvivalScore).ToArray();
            var theLastIndividual = sorted[sorted.Length - 1];
            double? maxSumRange = 0;
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
            var sorted = population.genotypes.OrderBy(p => p.SurvivalScore).ToArray();

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
                    return population.GetParent(GeneticDefaults.INSTANCE.SelectionMethod);
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