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
            int TOURNAMENTSIZE = 2;
            for (int i = 0; i < TOURNAMENTSIZE; i++)
            {
                competitors.Add(population.genotypes[GeneticEnvironment.CUBE.Next(0, population.genotypes.Length)]);
            }
            return competitors.OrderByDescending(x => x.SurvivalScore).First();
        }

    }
    public class Individual
    {
        public uint genotype;
        public double Fenotype
        {
            get
            {
                return -2 + genotype /GeneticEnvironment.DIVIDER;
            }
        }
        public double SurvivalScore
        {
            get
            {
                return GeneticEnvironment.SurvivalFunction(Fenotype);
            }
        }

        public bool LethallyMutated
        {
            get
            {
                { return Fenotype > 2.0 || Fenotype < -2.0; }
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
                var populationCount = 0;
                while (populationCount < POPULATIONCOUNTLIMIT)
                {
                    pop = GetEvolvedPopulation(pop);
                    //Console.WriteLine($"Population {populationCount} : heaven1 : {pop.BestOne}");
                    if (heavensOne.Where(x => x.SurvivalScore >= pop.BestOne.SurvivalScore).Count() == 0)
                    {
                        heavensOne.Add(pop.BestOne);
                    }
                    populationCount++;
                }
                Console.WriteLine($"Trial : {i} Best:{heavensOne.OrderByDescending(x => x.SurvivalScore).First()}");
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
        private static Population GetEvolvedPopulation(Population population)
        {
            Individual mum, dad;
            mum = population.GetRandomParent();
            dad = population.GetRandomParent();
            var newGenotypes = new Individual[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                //do
                //{
                    var child = GetRandomChild(mum, dad);
                    for (int j = 0; j < 4; j++)//without x >4 times mutation gets stuck in local maximum
                    {
                        if (GeneticEnvironment.CUBE.NextDouble() < MUTATIONPROBABILITY)
                        {
                            child = Mutate(child.genotype);
                        }
                    }
                    newGenotypes[i] = child;
                //} while (newGenotypes[i].LethallyMutated);
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

