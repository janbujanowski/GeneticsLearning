using System;

namespace ObEwolucyjne1
{
    class Program
    {
        const double VALUESRANGE = 2 - (-2);
        const uint COUNTVALUESTOMAP = UInt32.MaxValue;
        const double DIVIDER = COUNTVALUESTOMAP / VALUESRANGE;
        const int POPULATIONSIZE = 20;
        const int POPULATIONCOUNTLIMIT = 1000;
        const double MUTATIONPROBABILITY = 0.10;
        static Random cube = new Random();
        static uint Heaven1, Heaven2 = 0;
        static double HeavenScore1, HeavenScore2 = -2;
        const int NUMBEROFEVOLUTIONTRIALS = 2;

        static void Main(string[] args)
        {
            uint[,] evolutionStatistics = new uint[NUMBEROFEVOLUTIONTRIALS, 2];

            uint[] populationHeaven = GetNewFittedPopulation();
            Console.WriteLine($"Heaven1 : {Fenotype(Heaven1)} with score {ScoreFunction(Fenotype(Heaven1))}");
            Console.WriteLine($"Heaven2 : {Fenotype(Heaven2)} with score {ScoreFunction(Fenotype(Heaven2))}");
            Console.ReadKey();
        }

        private static uint[] GetNewRandomPopulation()
        {
            uint[] genotypes = new uint[POPULATIONSIZE];
            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                genotypes[i] = (uint)cube.Next(0, Int32.MaxValue);
            }
            return genotypes;
        }
        private static uint[] GetNewFittedPopulation()
        {
            uint[] newRandomPopulation = GetNewRandomPopulation();
            uint[] evolvedPopulation = EvolvePopulation(newRandomPopulation);
            //uint[] twoBestIndividuals = GetTwoBestOfPopulation(evolvedPopulation);
            return evolvedPopulation;
        }

        //private static uint[] GetTwoBestOfPopulation(uint[] evolvedPopulation)
        //{
        //    throw new NotImplementedException();
        //}

        private static uint[] EvolvePopulation(uint[] genotypes)
        {
            int population = 0;
            double[] genotypeScore = new double[POPULATIONSIZE];

            while (population <= POPULATIONCOUNTLIMIT)
            {
                double maxScore = HeavenScore1;
                double secondMaxScore = HeavenScore2;
                int maxScoreIndex = 0;
                for (int i = 0; i < genotypes.Length; i++)
                {
                    genotypeScore[i] = ScoreFunction(Fenotype(genotypes[i]));
                    if (maxScore < genotypeScore[i])
                    {
                        maxScore = genotypeScore[i];
                        maxScoreIndex = i;
                        if (HeavenScore1 < maxScore)
                        {

                            HeavenScore2 = HeavenScore1;
                            Heaven2 = Heaven1;
                            HeavenScore1 = genotypeScore[i];
                            Heaven1 = genotypes[i];
                        }
                    }
                }
                Console.WriteLine($"Population {population}, Max score {maxScore}, fenotype : {Fenotype(genotypes[maxScoreIndex])}");
                uint mum, dad;
                mum = genotypes[GetNewParent(POPULATIONSIZE, genotypeScore)];
                dad = genotypes[GetNewParent(POPULATIONSIZE, genotypeScore)];
                var newGenotypes = new uint[POPULATIONSIZE];
                for (int i = 0; i < POPULATIONSIZE; i++)
                {
                    var child = GetRandomChild(mum, dad);
                    if (cube.NextDouble() < MUTATIONPROBABILITY)
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
            var separationPos = cube.Next(1, 32);
            var geneX = mum & (UInt32.MaxValue << separationPos);
            //PrintBinary(geneX);
            var geneY = dad & (UInt32.MaxValue >> (32 - separationPos));
            //PrintBinary(geneY);
            //PrintBinary(geneX ^ geneY);
            //Console.WriteLine("------");
            return geneX ^ geneY;
        }
        static uint Mutate(uint a)
        {
            uint randomPosition = (uint)Math.Pow(2, cube.Next(0, 32));
            return a ^ randomPosition;
        }
        static void PrintBinary(uint a)
        {
            Console.WriteLine(Convert.ToString(a, 2).PadLeft(32).Replace(" ", "0"));
        }
        static int GetNewParent(int populationsize, double[] genotypeScore)
        {
            var x = cube.Next(0, populationsize);
            var y = cube.Next(0, populationsize);
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
