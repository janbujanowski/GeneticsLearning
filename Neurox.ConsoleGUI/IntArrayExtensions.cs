using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurox.ConsoleGUI
{
    public static class IntArrayExtensions
    {
        public static int hasSharesIndex = 26;

        public static int periodsIndex_MACDEMA1 = 30;
        public static int periodsIndex_MACDEMA2 = 31;
        public static int periodsIndex_MACDEMASignal = 32;
        public static int periodsIndex_RSI = 33;
        public static int periodsIndex_BollingerPeriods = 34;
        public static int periodsIndex_BollingerDeviationEnumerator = 21;
        public static int periodsIndex_BollingerDeviationDenominator = 22;
        public static List<int> periodsIndexes = new List<int>();

        public static int neuralNetworkGenotypeLength = 500;
        public static int neuronsInputLayerCount = 9;
        public static int neuronsHiddenLayerCount = 3;
        public static int neuronsOutputLayerCount = 1;

        public static void SetDefaultModifiers(this int[] neuralArray)
        {
            neuralArray[periodsIndex_MACDEMA1] = 26;
            neuralArray[periodsIndex_MACDEMA2] = 12;
            neuralArray[periodsIndex_MACDEMASignal] = 9;
            neuralArray[periodsIndex_RSI] = 14;
            neuralArray[periodsIndex_BollingerPeriods] = 20;
            neuralArray[periodsIndex_BollingerDeviationEnumerator] = 2;
            neuralArray[periodsIndex_BollingerDeviationDenominator] = 1;
           
            periodsIndexes = new List<int>() { periodsIndex_MACDEMA1 ,
                                               periodsIndex_MACDEMA2 ,
                                               periodsIndex_MACDEMASignal ,
                                               periodsIndex_RSI,
                                               periodsIndex_BollingerPeriods };

            neuralArray[hasSharesIndex] = 0;
        }


        public static int GetLongestPeriods(this int[] neuralArray)
        {
            var max = -1;
            foreach (var periodIndex in periodsIndexes)
            {
                if (neuralArray[periodIndex] > max)
                {
                    max = neuralArray[periodIndex];
                }
            }
            if (neuralArray[periodsIndex_MACDEMA1] + neuralArray[periodsIndex_MACDEMASignal] > max)
            {
                max = neuralArray[periodsIndex_MACDEMA1] + neuralArray[periodsIndex_MACDEMASignal];
            }
            if (neuralArray[periodsIndex_MACDEMA2] + neuralArray[periodsIndex_MACDEMASignal] > max)
            {
                max = neuralArray[periodsIndex_MACDEMA2] + neuralArray[periodsIndex_MACDEMASignal];
            }
            return max;
        }
        public static double[,] GetHiddenLayerWeights(this int[] neuralArray)
        {
            double[,] hiddenLayer = new double[neuronsInputLayerCount, neuronsHiddenLayerCount];

            for (int i = 0; i < neuronsInputLayerCount; i++)
            {
                for (int j = 0; j < neuronsHiddenLayerCount; j++)
                {
                    var numerator = int.Parse($"1{i}{j}");
                    var denominator = int.Parse($"2{i}{j}");
                    hiddenLayer[i, j] = neuralArray[denominator] == 0 ? 0 : (double)neuralArray[numerator] / (double)neuralArray[denominator];
                }
            }
            return hiddenLayer;
        }
        public static double[,] GetHiddenToOutputLayerWeights(this int[] neuralArray)
        {
            double[,] hiddenLayer = new double[neuronsHiddenLayerCount, neuronsOutputLayerCount];

            for (int i = 0; i < neuronsHiddenLayerCount; i++)
            {
                for (int j = 0; j < neuronsOutputLayerCount; j++)
                {
                    var numerator = int.Parse($"3{i}{j}");
                    var denominator = int.Parse($"4{i}{j}");
                    hiddenLayer[i, j] = neuralArray[denominator] == 0 ? 0.0 : (double)neuralArray[numerator] / (double)neuralArray[denominator];
                }
            }
            return hiddenLayer;
        }
        public static Dictionary<string, double> GetInputLayerModifiers(this int[] neuralArray)
        {
            var modifiers = new Dictionary<string, double>();

            modifiers.Add("Volume", neuralArray[12] == 0 ? 0 : (double)neuralArray[11] / (double)neuralArray[12]);
            modifiers.Add("Opening", neuralArray[14] == 0 ? 0 : (double)neuralArray[13] / (double)neuralArray[14]);
            modifiers.Add("Min", neuralArray[16] == 0 ? 0 : (double)neuralArray[15] / (double)neuralArray[16]);
            modifiers.Add("Max", neuralArray[18] == 0 ? 0 : (double)neuralArray[17] / (double)neuralArray[18]);
            modifiers.Add("Closing", neuralArray[20] == 0 ? 0 : (double)neuralArray[19] / (double)neuralArray[20]);

            modifiers.Add("MACDEMA1", neuralArray[periodsIndex_MACDEMA1] <= 0 ? 0 : neuralArray[periodsIndex_MACDEMA1]);
            modifiers.Add("MACDEMA2", neuralArray[periodsIndex_MACDEMA2] <= 0 ? 0 : neuralArray[periodsIndex_MACDEMA2]);
            modifiers.Add("MACDEMASignal", neuralArray[periodsIndex_MACDEMASignal] <= 0 ? 0 : neuralArray[periodsIndex_MACDEMASignal]);
            modifiers.Add("RSI", neuralArray[periodsIndex_RSI] <= 0 ? 0 : neuralArray[periodsIndex_RSI]);
            modifiers.Add("BollingerPeriods", neuralArray[periodsIndex_BollingerPeriods] <= 0 ? 0 : neuralArray[periodsIndex_BollingerPeriods]);
            modifiers.Add("BollingerDeviation", neuralArray[periodsIndex_BollingerDeviationDenominator] == 0 ? 0 : (double)neuralArray[periodsIndex_BollingerDeviationEnumerator] / (double)neuralArray[periodsIndex_BollingerDeviationDenominator]);

            return modifiers;
        }
        public static int GetBuyLimit(this int[] neuralArray)
        {
            return neuralArray[29];
        }
        public static int GetStopLimit(this int[] neuralArray)
        {
            return neuralArray[28];
        }
        public static int GetVolume(this int[] neuralArray)
        {
            return neuralArray[27];
        }
        public static bool GetHasShares(this int[] neuralArray)
        {
            return neuralArray[hasSharesIndex] == 1;
        }
        public static void BuyShares(this int[] neuralArray)
        {
            neuralArray[hasSharesIndex] = 1;
        }
        public static void SellShares(this int[] neuralArray)
        {
            neuralArray[hasSharesIndex] = 0;
        }
        public static bool IsValidNeuralArray(this int[] neuralArray)
        {
            return neuralArray.Length > 0 && neuralArray.Length <= neuralNetworkGenotypeLength;
        }
    }
}
