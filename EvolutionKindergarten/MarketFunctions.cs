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

            return 100 - 100 / (1 + (avgGain / avgLoss));
        }

        internal double MACDIndicator(List<DailyMarketData> dailyMarketDatas, double MACDEMA1, double MACDEMA2, double MACDEMASignal)
        {
            var dailyMarketDatasSorted = dailyMarketDatas.OrderByDescending(data => data.Date);
            
            var macdDaily = new DailyMarketData[(int)MACDEMASignal];
            for (int i = 0; i < (int)MACDEMASignal; i++)
            {
                var ema1 = EMA(dailyMarketDatas.Skip(i).Take((int)MACDEMA1).ToList());
                var ema2 = EMA(dailyMarketDatas.Skip(i).Take((int)MACDEMA2).ToList());
                macdDaily[i] = new DailyMarketData()
                {
                    Date = dailyMarketDatas[i].Date,
                    Closing = ema2 - ema1
                };
            }
            var emaMacdSignal = EMA(macdDaily.ToList());
            if (MACDEMASignal == 0)
            {
                return 0;
            }
            return emaMacdSignal - macdDaily[0].Closing;
        }

        internal double[] BollingerBands(List<DailyMarketData> dailyMarketDatas, int bollingerPeriods, double bollingerDeviation)
        {
            var bollingerBands = new double[3];

            var ema = EMA(dailyMarketDatas);
            var sma = SMA(dailyMarketDatas);
            List<double> rootMeanSquareNumerator = dailyMarketDatas.Select(data => data.Closing - sma).ToList();
            var standardDeviation = RootMeanSquare(rootMeanSquareNumerator);


            bollingerBands[0] = ema - standardDeviation * bollingerDeviation;
            bollingerBands[1] = ema;
            bollingerBands[2] = ema + standardDeviation * bollingerDeviation;
            return bollingerBands;
        }

        internal double EMA(List<DailyMarketData> dailyMarketDatas)
        {
            if (dailyMarketDatas == null || dailyMarketDatas.Count == 0)
            {
                return 0.0;
            }
            dailyMarketDatas = dailyMarketDatas.OrderBy(d => d.Date).ToList();
            double ema = 0;
            double emaDenominator = 0;
            for (int i = 1; i <= dailyMarketDatas.Count; i++)
            {
                emaDenominator += i;
                ema += dailyMarketDatas[i - 1].Closing;
            }
            return ema / emaDenominator;
        }
        internal double SMA(List<DailyMarketData> dailyMarketDatas)
        {
            if (dailyMarketDatas == null || dailyMarketDatas.Count == 0)
            {
                return 0.0;
            }
            return dailyMarketDatas.OrderBy(d => d.Date).Sum(x => x.Closing) / dailyMarketDatas.Count;
        }
        internal double RootMeanSquare(List<double> values)
        {
            if (values == null || values.Count == 0)
            {
                return 0;
            }
            return Math.Sqrt(values.Sum(val => Math.Pow(val, 2)) / values.Count);
        }
    }
}
