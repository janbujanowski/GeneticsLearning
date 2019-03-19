using System;
using System.Collections.Generic;
using System.Linq;

namespace EvoCore
{
    public enum SelectionMethods
    {
        Tournament,
        Roulette,
        RankedRoulette
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
                    break;
                case SelectionMethods.Roulette:
                    return population.GetRouletteParent();
                    break;
                case SelectionMethods.RankedRoulette:
                    return population.GetRankedRouletteParent();
                    break;
                default:
                    return population.GetParent(GeneticEnvironment.defaultSelectionMethod);
                    break;
            }
        }
    }
    public class Individual
    {
        public uint genotype;
        public double Fenotype
        {
            get
            {
                return -2 + genotype / GeneticEnvironment.DIVIDER;
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
            return $"Fenotype : {Fenotype} with score : {SurvivalScore}";
        }
    }
    public class Program
    {
        const int POPULATIONSIZE = 20;
        const int POPULATIONCOUNTLIMIT = 1000;
        const double MUTATIONPROBABILITY = 0.15;
        const int NUMBEROFEVOLUTIONTRIALS = 100;
        const int ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = 300;

        static void Main(string[] args)
        {
            for (int i = 0; i < NUMBEROFEVOLUTIONTRIALS; i++)
            {
                List<Individual> heavensOne = new List<Individual>();
                Individual[] newRandomPopulation = GetNewRandomPopulation();
                Population pop = new Population()
                {
                    genotypes = newRandomPopulation
                };
                heavensOne.Add(newRandomPopulation[0]);//for easier heavens adding criteria

                var populationCount = 0;
                bool stillImproving = true;
                int NoNimprovementCounter = 0;
                int maxIterationsWithoutImprovement = 300;
                while (NoNimprovementCounter <= maxIterationsWithoutImprovement)
                {
                    pop = GetNextGeneration(pop, SelectionMethods.RankedRoulette);
                    if (heavensOne[heavensOne.Count - 1].SurvivalScore < pop.BestOne.SurvivalScore)
                    {
                        heavensOne.Add(pop.BestOne);
                        NoNimprovementCounter = 0;
                    }
                    else
                    {
                        NoNimprovementCounter++;
                    }
                    populationCount++;
                    //Console.WriteLine($"Trial : {populationCount} Best:{heavensOne.OrderByDescending(x => x.SurvivalScore).First()}");
                }
                Console.WriteLine($"Trail {i} Best:{heavensOne.OrderByDescending(x => x.SurvivalScore).First()} after { populationCount - maxIterationsWithoutImprovement} population");
                #region PopulationCountCriteria
                while (populationCount < POPULATIONCOUNTLIMIT)
                {
                    pop = GetNextGeneration(pop, SelectionMethods.Tournament);
                    //Console.WriteLine($"Population {populationCount} : heaven1 : {pop.BestOne}");
                    if (heavensOne.Where(x => x.SurvivalScore >= pop.BestOne.SurvivalScore).Count() == 0)
                    {
                        heavensOne.Add(pop.BestOne);
                    }
                    populationCount++;
                }
                Console.WriteLine($"Trial : {i} Best:{heavensOne.OrderByDescending(x => x.SurvivalScore).First()}");
                #endregion
            }

            Console.ReadKey();
        }
        private static Individual[] GetNewRandomPopulation()
        {
            Individual[] population = new Individual[POPULATIONSIZE];

            uint[] genotypes = new uint[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                population[i] = new Individual()
                {
                    genotype = (uint)GeneticEnvironment.CUBE.Next(0, Int32.MaxValue)
                };
            }
            return population;
        }
        private static Population GetNextGeneration(Population population, SelectionMethods method)
        {
            Individual mum, dad;
            mum = population.GetParent(method);
            dad = population.GetParent(method);
            var newGenotypes = new Individual[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                var child = GetRandomChild(mum, dad);
                for (int j = 0; j < GeneticEnvironment.MUTATIONRETRIALS; j++)//without x >4 times mutation often gets stuck in local maximums
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
        static Individual GetRandomChild(Individual mum, Individual dad)
        {
            var separationPos = GeneticEnvironment.CUBE.Next(1, 32);
            var mask = UInt32.MaxValue << separationPos;
            return new Individual() { genotype = (mask & mum.genotype) | (~mask & dad.genotype) };
        }
        static Individual Mutate(uint a)
        {
            uint randomPosition = (uint)Math.Pow(2, GeneticEnvironment.CUBE.Next(0, 32));
            return new Individual() { genotype = a ^ randomPosition };
        }
        static void PrintBinary(uint a)
        {
            Console.WriteLine(Convert.ToString(a, 2).PadLeft(32).Replace(" ", "0"));
        }
    }
}

