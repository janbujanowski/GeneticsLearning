using Newtonsoft.Json;
using srodowisko;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class GeneticEnvironment
    {
        public uint COUNTVALUESTOMAP = UInt32.MaxValue;
        public double VALUESRANGE = 2 - (-2);
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
        StockMarketEvaluator problemToResolve;
        ILogger _logger;

        public int[] GetRandomParametersArray(int nrProblemu)
        {
            int[] genotype = new int[problemToResolve.ProblemSize(nrProblemu)];
            for (int i = 0; i < problemToResolve.ProblemSize(nrProblemu); i++)
            {
                genotype[i] = CUBE.Next(-1000, 1000);
            }
            return genotype;
        }
        private static GeneticEnvironment _config;
        public static GeneticEnvironment INSTANCE
        {
            get
            {
                if (_config == null)
                {
                    _config = new GeneticEnvironment(IoCFactory.Resolve<ILogger>());
                }
                return _config;
            }
        }
        private GeneticEnvironment(ILogger loggerInstance)
        {
            problemToResolve = new StockMarketEvaluator(new DirectFileLogger());
            _logger = loggerInstance;
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
        public int NrProblemu { get; internal set; }
        public DateTime StartDate { get; internal set; }

        public static double SurvivalFunction(int[] sciezka)
        {
            return INSTANCE.problemToResolve.Ocena(sciezka);
        }

        internal void ParseParameters(string[] args)
        {
            try
            {
                Console.Write($"Parsowanie daty stopu : {args[0]}");
                INSTANCE.StopDate = DateTime.Parse(ConfigurationManager.AppSettings["StopDate"]); //DateTime.Parse(args[0]);//Data stopu
                Console.WriteLine($" Sparsowano : {INSTANCE.StopDate.ToString()}");
                Console.WriteLine($"DATA STARTU |{INSTANCE.StopDate.ToString()}|");
                Console.WriteLine($"DATA ZAKONCZENA |{INSTANCE.StopDate.ToString()}|");


                Console.Write($"Parsowanie rozmiaru populacji (int) : {args[1]}");
                INSTANCE.POPULATIONSIZE = Int32.Parse(args[1]);//Rozmiar Populacji
                Console.WriteLine($" Sparsowano : {INSTANCE.POPULATIONSIZE.ToString()}");

                Console.Write($"Parsowanie prawdopodobieństwo mutacji (double) : {args[2]}");
                INSTANCE.MUTATIONPROBABILITY = double.Parse(ConfigurationManager.AppSettings["MUTATIONPROBABILITY"]);//Prawdopodobienstwo mutacji
                Console.WriteLine($" Sparsowano : {INSTANCE.MUTATIONPROBABILITY.ToString()}");

                Console.Write($"Parsowanie ilosc prob mutacji (int) : {args[3]}");
                INSTANCE.MUTATIONRETRIALS = Int32.Parse(ConfigurationManager.AppSettings["MUTATIONRETRIALS"]);//Ilosc prob mutacji
                Console.WriteLine($" Sparsowano : {INSTANCE.MUTATIONRETRIALS.ToString()}");

                Console.Write($"Parsowanie selekcji (enum) : {args[4]}");
                INSTANCE.SelectionMethod = (SelectionMethods)Enum.Parse(typeof(SelectionMethods), ConfigurationManager.AppSettings["SelectionMethod"]);//Rodzaj selekcji
                Console.WriteLine($" Sparsowano : {INSTANCE.SelectionMethod.ToString()}");

                Console.Write($"Parsowanie krzyzowania (enum) : {args[5]}");
                INSTANCE.CrossoverMethod = (CrossoverMethods)Enum.Parse(typeof(CrossoverMethods), ConfigurationManager.AppSettings["CrossoverMethod"]);//Rodzaj krzyzowania
                Console.WriteLine($" Sparsowano : {INSTANCE.CrossoverMethod.ToString()}");

                Console.Write($"Parsowanie iteracji (int) : {args[6]}");
                INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT = Int32.Parse(args[6]);//Ilosc iteracji bez poprawy
                Console.WriteLine($" Sparsowano : {INSTANCE.ITERATIONSWITHOUTBETTERSCOREMAXCOUNT.ToString()}");

                Console.Write($"Parsowanie modyfikatora (double) : {args[7]}");
                INSTANCE.ModyfikatorWyniku = double.Parse(ConfigurationManager.AppSettings["ScoreModifier"]);//1 lub -1 zaleznie od rodzaju problemu maksymalizacji/minimalizacji
                Console.WriteLine($" Sparsowano : {INSTANCE.ModyfikatorWyniku.ToString()}");

                Console.Write($"Parsowanie numeru zbioru (int) : {args[8]}");
                INSTANCE.NrProblemu = Int32.Parse(args[8]);//numer zbioru
                Console.WriteLine($" Sparsowano : {INSTANCE.NrProblemu.ToString()}");

                Console.Write($"Parsowanie czasu do stopu (double) : {args[9]}");
                var dataStopuOdMinut = DateTime.Now.AddMinutes(double.Parse(ConfigurationManager.AppSettings["MinutesLimit"]));
                if (dataStopuOdMinut < StopDate)
                {
                    INSTANCE.StopDate = dataStopuOdMinut;
                    Console.WriteLine($" Sparsowano : {INSTANCE.StopDate.ToString()}");
                }
                var jsonContent = File.ReadAllText(ConfigurationManager.AppSettings["StartingIndividual"]);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    //Console.Write($"Parsowanie osobnika : {args[10]}");
                    //var genotypeArray = args[10].Split(',');
                    INSTANCE.BestGenotype = JsonConvert.DeserializeObject<Individual>(jsonContent).genotype;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "During parsing parameters");
            }
        }
    }
}
