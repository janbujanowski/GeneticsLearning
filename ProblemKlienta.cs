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
    public class ProblemKlienta
    {
        List<DailyMarketData> marketData;
        string _pathToDataFile = ConfigurationManager.AppSettings["pathToMarketDataFile"];
        ILogger _logger;
        MarketFunctions _marketFunctions;
        public ProblemKlienta(ILogger loggerInstance)
        {
            _logger = loggerInstance;
            _marketFunctions = new MarketFunctions(_logger);
            LoadMarketData(_pathToDataFile);
        }
        public void LoadMarketData(string filePath)
        {
            string[] lines = new string[1];
            int start = 0; int end = 0;
            try
            {
                lines = File.ReadAllLines(filePath);
                start = FindStartingIndex(lines);
                end = FindEndIndex(lines);
                marketData = new List<DailyMarketData>();
                for (int i = start; i < end; i++)
                {
                    string[] values = lines[i].Split(',');

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
        public int Rozmiar(int numer_zbioru = 0)
        {
            return 35;
        }
        public double Ocena(int[] oneLayeredNetwork)
        {
            double startBalance = 10000.0;
            double currentBallance = startBalance;
            for (int i = oneLayeredNetwork.GetPeriods(); i < marketData.Count; i++)
            {
                var rsiRangeIndex = i - oneLayeredNetwork.GetPeriods();
                List<DailyMarketData> historicalData = marketData.Skip(rsiRangeIndex).Take(oneLayeredNetwork.GetPeriods()).ToList();
                currentBallance = BuyOrSellAndGetCurrentBallance(historicalData, currentBallance, oneLayeredNetwork);
            }
            return currentBallance - startBalance;
        }

        private double BuyOrSellAndGetCurrentBallance(List<DailyMarketData> historicalData, double currentBallance, int[] oneLayeredNetwork)
        {
            double networkOutcomeIndicator = 0;
            var rsiValue = _marketFunctions.RSI(historicalData, oneLayeredNetwork.GetPeriods());
            networkOutcomeIndicator = rsiValue * oneLayeredNetwork.GetRSIModifier();
            if (networkOutcomeIndicator > oneLayeredNetwork.GetBuyLimit())
            {
                currentBallance -= historicalData.OrderBy(x => x.Date).Last().Closing * oneLayeredNetwork.GetVolume();
            }
            else if (networkOutcomeIndicator < oneLayeredNetwork.GetStopLimit())
            {
                currentBallance += historicalData.OrderBy(x => x.Date).Last().Closing * oneLayeredNetwork.GetVolume();
            }

            return currentBallance;
        }
    }
}
