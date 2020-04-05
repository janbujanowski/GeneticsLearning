using AlgorytmEwolucyjny;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srodowisko
{
    public class StockMarketEvaluator
    {
        List<DailyMarketData> marketData;
        string _pathToDataFile;
        ILogger _logger;
        MarketFunctions _marketFunctions;
        public StockMarketEvaluator(ILogger loggerInstance, string pathToDataFile = null)
        {
            _logger = loggerInstance;
            _marketFunctions = new MarketFunctions(_logger);
            _pathToDataFile = pathToDataFile;
            if (string.IsNullOrEmpty(_pathToDataFile))
            {
                _pathToDataFile = IoCFactory.Resolve<IConfigurationProvider>().GetConfigurationString("workingDirectory", "pathToMarketDataFile");
            }
            LoadMarketData(_pathToDataFile);
        }
        public void LoadMarketData(string filePath)
        {
            string[] lines = new string[1];
            int start = 0; int end = 0;
            var currentrow = 0;
            try
            {
                lines = File.ReadAllLines(filePath);
                start = FindStartingIndex(lines);
                end = FindEndIndex(lines);
                marketData = new List<DailyMarketData>();

                for (int i = start; i < end; i++)
                {
                    string[] values = lines[i].Split(',');
                    currentrow = i;
                    DateTime date = DateTime.Parse(values[0]);
                    double opening = double.Parse(values[1]);
                    double max = double.Parse(values[2]);
                    double min = double.Parse(values[3]);
                    double closing = double.Parse(values[4]);
                    int volume = int.Parse(values[5]);
                    marketData.Add(new DailyMarketData()
                    {
                        Date = date,
                        Opening = opening,
                        Max = max,
                        Min = min,
                        Closing = closing,
                        Volume = volume
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "During loading data", filePath, lines.Count(), start, end);
            }
        }
        private int FindEndIndex(string[] lines)
        {
            var end = lines.Length - 1;
            DateTime tryParse;
            while (!DateTime.TryParse(lines[end].Split(',')[0], out tryParse))
            {
                end--;
            }
            return end;
        }
        private int FindStartingIndex(string[] lines)
        {
            var i = 0;
            DateTime tryParse;
            while (!DateTime.TryParse(lines[i].Split(',')[0], out tryParse))
            {
                i++;
            }
            return i;
        }
        public double Ocena(int[] oneLayeredNetwork)
        {
            var lineofCode = 89;
            try
            {
                double startBalance = 10000.0;
                double currentBallance = startBalance;
                var modifiers = oneLayeredNetwork.GetInputLayerModifiers();
                var neuralayer = oneLayeredNetwork.GetHiddenLayerWeights();
                var neuralayerOutput = oneLayeredNetwork.GetHiddenToOutputLayerWeights();
                var longestPeriods = oneLayeredNetwork.GetLongestPeriods();
                if (longestPeriods > marketData.Count)
                {
                    return -100000;
                }
                for (int i = longestPeriods; i < marketData.Count; i++)
                {
                    double[] inputLayerScores = new double[IntArrayExtensions.neuronsInputLayerCount];
                    var rsiPeriods = (int)modifiers["RSI"];
                    var bollingerPeriods = (int)modifiers["BollingerPeriods"];
                    lineofCode = 107;
                    inputLayerScores[0] = marketData[i].Min;
                    inputLayerScores[1] = marketData[i].Max;
                    inputLayerScores[2] = marketData[i].Closing;
                    inputLayerScores[3] = marketData[i].Volume * modifiers["Volume"];
                    lineofCode = 112;
                    inputLayerScores[4] = _marketFunctions.MACDIndicator(marketData.GetRange(i - longestPeriods, longestPeriods), modifiers["MACDEMA1"], modifiers["MACDEMA2"], modifiers["MACDEMASignal"]);
                    lineofCode = 114;
                    inputLayerScores[5] = _marketFunctions.RSI(marketData.GetRange(i - rsiPeriods, rsiPeriods), (int)modifiers["RSI"]);
                    lineofCode = 116;
                    var bollingerBands = _marketFunctions.BollingerBands(marketData.GetRange(i - bollingerPeriods, bollingerPeriods), bollingerPeriods, modifiers["BollingerDeviation"]);
                    inputLayerScores[6] = bollingerBands[0];
                    inputLayerScores[7] = bollingerBands[1];
                    inputLayerScores[8] = bollingerBands[2];

                    var hiddenLayerScores = new double[IntArrayExtensions.neuronsHiddenLayerCount];
                    for (int k = 0; k < IntArrayExtensions.neuronsHiddenLayerCount; k++)
                    {
                        hiddenLayerScores[k] = 0;
                        for (int j = 0; j < IntArrayExtensions.neuronsInputLayerCount; j++)
                        {
                            hiddenLayerScores[k] += neuralayer[j, k] * inputLayerScores[j];
                        }
                    }
                    var outputLayerScores = new double[IntArrayExtensions.neuronsOutputLayerCount];
                    for (int k = 0; k < IntArrayExtensions.neuronsOutputLayerCount; k++)
                    {
                        outputLayerScores[k] = 0;
                        for (int j = 0; j < IntArrayExtensions.neuronsHiddenLayerCount; j++)
                        {
                            outputLayerScores[k] += neuralayerOutput[j, k] * hiddenLayerScores[j];
                        }
                    }

                    if (outputLayerScores[0] > oneLayeredNetwork.GetBuyLimit() && !oneLayeredNetwork.GetHasShares())
                    {
                        oneLayeredNetwork.BuyShares();
                        currentBallance -= marketData[i].Closing * oneLayeredNetwork.GetVolume();
                    }
                    else if (outputLayerScores[0] < oneLayeredNetwork.GetStopLimit() && oneLayeredNetwork.GetHasShares())
                    {
                        oneLayeredNetwork.SellShares();
                        currentBallance += marketData[i].Closing * oneLayeredNetwork.GetVolume();
                    }
                }
                return currentBallance - startBalance;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, $"During network indicator line: {lineofCode} ");
            }
            return -100000;
        }
    }
}