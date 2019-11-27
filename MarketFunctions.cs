using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class MarketFunctions
    {
        ILogger _logger;
        public MarketFunctions(ILogger logger)
        {
            _logger = logger;
        }

        public double MACDIndicator(List<DailyMarketData> data, int periods)
        {
            if (periods != data.Count)
            {
                _logger.LogException(new ArgumentOutOfRangeException("data", $"data.Count is {data.Count} while demanded periods is {periods}"), string.Empty);
                return 0.0;
            }
            double macdValue = 1;

            return macdValue;
        }
        public double RSI(List<DailyMarketData> data, int periods)
        {
            if (periods != data.Count)
            {
                _logger.LogException(new ArgumentOutOfRangeException("data", $"data.Count is {data.Count} while demanded periods is {periods}"), string.Empty);
                return 0.0;
            }
            double gain, loss, difference;
            double avgGain = 0; double avgLoss = 0;
            for (int i = 1; i < periods; i++)
            {
                difference = data[i].Closing - data[i - 1].Closing;
                gain = Math.Max(0, difference);
                loss = Math.Max(0, -difference);
                avgGain += gain;
                avgLoss += loss;
            }

            return 100 - 100 / (1 - (avgGain / avgLoss));
        }
    }
}
