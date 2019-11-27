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

        public double MACDIndicator(List<DailyMarketData> data,int periods)
        {
            if (periods != data.Count)
            {
                _logger.LogException(new ArgumentOutOfRangeException("data", $"data.Count is {data.Count} while demanded periods is {periods}"),string.Empty);
                return 0.0;
            }
            double macdValue = 1;

            return macdValue;
        }
    }
}
