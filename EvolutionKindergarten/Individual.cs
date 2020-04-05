using srodowisko;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class Individual
    {
        public Individual()
        {
            _survivalScore = null;
        }

        public int[] genotype;
        public Dictionary<string, double> Fenotype
        {
            get
            {
                return genotype.GetInputLayerModifiers();
            }
        }

        private double? _survivalScore;
        public double? SurvivalScore
        {
            get
            {
                if (_survivalScore == null && Fenotype != null)
                {
                    _survivalScore = GeneticEnvironment.SurvivalFunction(genotype);
                }

                return _survivalScore;
            }
        }
        private double? _testedSurvivalScore;
        public double? TestedSurvivalScore
        {
            get
            {
                if (_testedSurvivalScore == null && Fenotype != null)
                {
                    _testedSurvivalScore = new StockMarketEvaluator(IoCFactory.Resolve<ILogger>(), IoCFactory.Resolve<IConfigurationProvider>().GetConfigurationString("workingDirectory", "pathToTestMarketDataFile")).Ocena(genotype);
                }
                return _testedSurvivalScore;
            }
        }
        public override string ToString()
        {
            return $"Zarobil :{ SurvivalScore } oraz na testowych danych : { TestedSurvivalScore }";
        }
    }
}
