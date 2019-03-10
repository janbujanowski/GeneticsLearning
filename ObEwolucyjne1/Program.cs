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
        const double MUTATIONPROBABILITY = 0.20;
        static Random cube = new Random();
        static uint Heaven1,Heaven2 = 0;
        static double HeavenScore1, HeavenScore2 = -2;

        static void Main(string[] args)
        {

            uint[] genotypes = new uint[POPULATIONSIZE];
            double[] genotypeScore = new double[POPULATIONSIZE];
            int population = 0;

            for (int i = 0; i < POPULATIONSIZE; i++)
            {
                genotypes[i] = (uint)cube.Next(0, Int32.MaxValue);
            }

            while (population <= POPULATIONCOUNTLIMIT)
            {
                double maxScore = -2;
                double secondMaxScore = -2;
                int maxScoreIndex = 0;
                int secondScoreIndex = 0;
                for (int i = 0; i < genotypes.Length; i++)
                {
                    genotypeScore[i] = ScoreFuntion(Fenotype(genotypes[i]));
                    if (HeavenScore1 < genotypeScore[i])
                    {
                        HeavenScore2 = HeavenScore1;
                        Heaven2 = Heaven1;
                        HeavenScore1 = genotypeScore[i];
                        maxScore = genotypeScore[i];
                        Heaven1 = genotypes[i];
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
            Console.WriteLine($"Heaven1 : {Fenotype(Heaven1)} with score {HeavenScore1}");
            Console.WriteLine($"Heaven1 : {Fenotype(Heaven2)} with score {HeavenScore2}");
            Console.ReadKey();
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
        static double ScoreFuntion(double x)
        {
            return x * Math.Sin(x) * Math.Sin(10 * x);
        }
        static double Fenotype(uint g)
        {
            return -2 + g /DIVIDER;
        }
    }
}
