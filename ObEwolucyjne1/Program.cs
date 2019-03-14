﻿using System;

namespace ObEwolucyjne1
{
    public class Program
    {
        const int POPULATIONSIZE = 20;
        const int POPULATIONCOUNTLIMIT = 2000;
        const double MUTATIONPROBABILITY = 0.10;
        const double VALUESRANGE = 2 - (-2);
        const int NUMBEROFEVOLUTIONTRIALS = 100;

        const uint COUNTVALUESTOMAP = UInt32.MaxValue;
        const double DIVIDER = COUNTVALUESTOMAP / VALUESRANGE;
        static Random CUBE = new Random();

        static uint Heaven1, Heaven2 = 0;
        static double HeavenScore1, HeavenScore2 = -2;

        static void Main(string[] args)
        {
            uint[,] evolutionStatistics = new uint[NUMBEROFEVOLUTIONTRIALS, 2];

            var heavens1 = new uint[NUMBEROFEVOLUTIONTRIALS];
            var heavens2 = new uint[NUMBEROFEVOLUTIONTRIALS];

            for (int i = 0; i < NUMBEROFEVOLUTIONTRIALS; i++)
            {
                uint[] newRandomPopulation = GetNewRandomPopulation();
                uint[] evolvedPopulation = EvolvePopulation(newRandomPopulation);
                uint[] populationBestScores = GetTwoBestGenotypes(evolvedPopulation);

                heavens1[i] = Heaven1;
                heavens2[i] = Heaven2;
                Console.WriteLine($"Trial {i} Heaven1 : {Fenotype(Heaven1)} with score {ScoreFunction(Fenotype(Heaven1))}");
                //Console.WriteLine($"Trial {i} Heaven2 : {Fenotype(Heaven2)} with score {ScoreFunction(Fenotype(Heaven2))}");
            }

            Console.WriteLine($"Mean : {ScoreFunction(Fenotype(heavens1.Sum() / POPULATIONSIZE))}, Median : {Fenotype(heavens1[NUMBEROFEVOLUTIONTRIALS / 2])} and score : {ScoreFunction(Fenotype(heavens1[NUMBEROFEVOLUTIONTRIALS / 2]))}");
            Console.ReadKey();
        }


        private static uint[] GetTwoBestGenotypes(uint[] genotypes)
        {
            double[] genotypeScore = new double[POPULATIONSIZE];
            double maxScore = HeavenScore1;
            double secondMaxScore = HeavenScore2;
            int maxScoreIndex = 0;
            int maxScoreIndex2 = 0;
            for (int i = 0; i < genotypes.Length; i++)
            {
                genotypeScore[i] = ScoreFunction(Fenotype(genotypes[i]));
                if (maxScore < genotypeScore[i])
                {
                    secondMaxScore = maxScore;
                    maxScoreIndex2 = maxScoreIndex;
                    maxScore = genotypeScore[i];
                    maxScoreIndex = i;
                }
            }
            return new uint[2] { genotypes[maxScoreIndex], genotypes[maxScoreIndex2] };
        }
        private static double[] ScorePopulation(uint[] genotypes)
        {
            var scores = new double[genotypes.Length];
            for (int i = 0; i < genotypes.Length; i++)
            {
                scores[i] = ScoreFunction(genotypes[i]);
            }
            return scores;
        }
        private static uint[] GetNewRandomPopulation()
        {
            uint[] genotypes = new uint[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                genotypes[i] = (uint)CUBE.Next(0, Int32.MaxValue);
            }
            return genotypes;
        }
        private static uint[] GetNewNormalizedPopulation()
        {
            uint[] genotypes = new uint[POPULATIONSIZE];
            uint start = 0;
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                genotypes[i] = start;
                start += UInt32.MaxValue / POPULATIONSIZE - 1;
            }
            return genotypes;
        }

        private static uint[] EvolvePopulation(uint[] genotypes)
        {
            int population = 0;
            var genotypeScore = ScorePopulation(genotypes);
            var heavens = GetTwoBestGenotypes(genotypes);
            Heaven1 = heavens[0];
            Heaven2 = heavens[1];
            while (population <= POPULATIONCOUNTLIMIT)
            {
                uint mum, dad;
                mum = genotypes[GetNewParent(POPULATIONSIZE, genotypeScore)];
                dad = genotypes[GetNewParent(POPULATIONSIZE, genotypeScore)];
                var newGenotypes = new uint[POPULATIONSIZE];
                for (int i = 0; i < POPULATIONSIZE; i++)
                {
                    var child = GetRandomChild(mum, dad);
                    if (CUBE.NextDouble() < MUTATIONPROBABILITY)
                    {
                        child = Mutate(child);
                    }
                    newGenotypes[i] = child;
                }
                population++;
                genotypes = newGenotypes;
            }
            return genotypes;
        }
        static uint GetRandomChild(uint mum, uint dad)
        {
            var separationPos = CUBE.Next(1, 32);
            var geneX = mum & (UInt32.MaxValue << separationPos);
            var geneY = dad & (UInt32.MaxValue >> (32 - separationPos));
            return geneY | geneX;
        }
        static uint Mutate(uint a)
        {
            uint randomPosition = (uint)Math.Pow(2, CUBE.Next(0, 32));
            return a ^ randomPosition;
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
            return -2 + g / DIVIDER;
        }
    }
}
