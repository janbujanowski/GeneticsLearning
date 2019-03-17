using System;
using System.Collections.Generic;
using System.Linq;

namespace EvoCore
{
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

        public static Individual GetRandomParent(this Population population)
        {
            List<Individual> competitors = new List<Individual>();
            int TOURNAMENTSIZE = 3;
            for (int i = 0; i < TOURNAMENTSIZE; i++)
            {
                competitors.Add(population.genotypes[GeneticEnvironment.CUBE.Next(0, population.genotypes.Length)]);
            }
            return competitors.OrderByDescending(x => x.SurvivalScore).First();

            //var y = GeneticEnvironment.CUBE.Next(0, population.genotypes.Length);
            //if (population.genotypes[x].SurvivalScore >= population.genotypes[y].SurvivalScore)
            //{
            //    return population.genotypes[x];
            //}
            //else
            //{
            //    return population.genotypes[y];
            //}
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

        const int NUMBEROFEVOLUTIONTRIALS = 1;

        static Random CUBE = new Random();

        static uint Heaven1, Heaven2 = 0;
        static double HeavenScore1, HeavenScore2 = -2;

        static void Main(string[] args)
        {
            uint[,] evolutionStatistics = new uint[NUMBEROFEVOLUTIONTRIALS, 2];

            var heavens1 = new uint[NUMBEROFEVOLUTIONTRIALS];
            var heavens2 = new uint[NUMBEROFEVOLUTIONTRIALS];
            List<Individual> heavensOne = new List<Individual>();

            for (int i = 0; i < NUMBEROFEVOLUTIONTRIALS; i++)
            {
                Individual[] newRandomPopulation = GetNewRandomPopulation();
                Population pop = new Population()
                {
                    genotypes = newRandomPopulation
                };
                var populationCount = 0;
                while (populationCount < POPULATIONCOUNTLIMIT)
                {
                    pop = GetEvolvedPopulation(pop);
                    //Individual[] populationBestScores = GetTheBest(pop.genotypes);
                    Console.WriteLine($"Population {populationCount} : heaven1 : {pop.BestOne}");
                    if (heavensOne.Where(x => x.SurvivalScore > pop.BestOne.SurvivalScore).Count() == 0)
                    {
                        heavensOne.Add(pop.BestOne);
                    }
                    populationCount++;
                }
                //Console.WriteLine($"Trial {i} Heaven2 : {Fenotype(Heaven2)} with score {ScoreFunction(Fenotype(Heaven2))}");
            }

            Console.WriteLine($"Mean : {heavensOne.Select(x => x.Fenotype).Sum() / heavensOne.Count}, Median : {heavensOne[heavensOne.Count / 2]} ");
            Console.ReadKey();
        }
        //static uint Heaven1, Heaven2 = 0;
        //static double HeavenScore1, HeavenScore2 = -2;

        private static Individual[] GetTheBest(Individual[] genotypes)
        {
            double maxScore = -2;
            //double secondMaxScore = -2;
            int maxScoreIndex = 0;
            Individual best = new Individual() { genotype = 0 };
            //int maxScoreIndex2 = 0;
            for (int i = 0; i < genotypes.Length; i++)
            {
                if (maxScore < genotypes[i].SurvivalScore)
                {
                    //secondMaxScore = maxScore;
                    //maxScoreIndex2 = maxScoreIndex;
                    maxScore = genotypes[i].SurvivalScore;
                    best = genotypes[i];

                }
            }
            return new Individual[1] { best };
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
                //genotypes[i] = 
            }
            return population;
        }
        private static Individual[] GetNewNormalizedPopulation()
        {
            Individual[] genotypes = new Individual[POPULATIONSIZE];
            uint start = 0;
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                genotypes[i] = new Individual() { genotype = start };
                start += UInt32.MaxValue / POPULATIONSIZE - 1;
            }
            return genotypes;
        }
        private static Population GetEvolvedPopulation(Population population)
        {
            Individual mum, dad;
            mum = population.GetRandomParent();
            dad = population.GetRandomParent();
            var newGenotypes = new Individual[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                var child = GetRandomChild(mum.genotype, dad.genotype);
                if (CUBE.NextDouble() < MUTATIONPROBABILITY)
                {
                    child = Mutate(child.genotype);
                }
                newGenotypes[i] = child;
            }

            population.genotypes = newGenotypes;

            return population;
        }
        static Individual GetRandomChild(uint mum, uint dad)
        {
            var separationPos = GeneticEnvironment.CUBE.Next(1, 32);
            var mask = UInt32.MaxValue << separationPos;
            //var geneX = mum & (UInt32.MaxValue << separationPos);
            //var geneY = dad & (UInt32.MaxValue >> (32 - separationPos));
            return new Individual() { genotype = (mask & mum) | (dad & ~mask) };
        }
        static Individual Mutate(uint a)
        {
            uint randomPosition = (uint)Math.Pow(2, CUBE.Next(0, 32));
            return new Individual() { genotype = a ^ randomPosition };
        }
        static void PrintBinary(uint a)
        {
            Console.WriteLine(Convert.ToString(a, 2).PadLeft(32).Replace(" ", "0"));
        }
        static int GetNewParent(int populationsize, double[] genotypeScore)
        {
            var x = CUBE.Next(0, populationsize);
            var y = CUBE.Next(0, populationsize);
            var xScore = genotypeScore[x];
            var yScore = genotypeScore[y];
            if (xScore >= yScore)
            {
                return x;
            }
            else
            {
                return y;
            }
        }
        static double ScoreFunction(double x)
        {
            return x * Math.Sin(x) * Math.Sin(10 * x);
        }
        static double Fenotype(uint g)
        {
            return -2 + g / GeneticEnvironment.DIVIDER;
        }
    }
}

