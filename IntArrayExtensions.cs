using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public static class IntArrayExtensions
    {
        static int hasSharesIndex = 26;
        static int periodsIndex = 10;
        public static int GetPeriods(this int[] neuralArray)
        {
            if (neuralArray[periodsIndex] < 0)
            {
                neuralArray[periodsIndex] = GeneticEnvironment.CUBE.Next(1, 255);
            }
            return neuralArray[periodsIndex];
        }
        public static Dictionary<string,double> GetModifiers(this int[] neuralArray)
        {
            var modifiers = new Dictionary<string, double>();
            modifiers.Add("RSI", neuralArray[12] == 0 ? 0 : (double)neuralArray[11] / (double)neuralArray[12]);
            modifiers.Add("Opening", neuralArray[14] == 0 ? 0 : (double)neuralArray[13] / (double)neuralArray[14]);
            modifiers.Add("Min", neuralArray[16] == 0 ? 0 : (double)neuralArray[15] / (double)neuralArray[16]);
            modifiers.Add("Max", neuralArray[18] == 0 ? 0 : (double)neuralArray[17] / (double)neuralArray[18]);
            modifiers.Add("Closing", neuralArray[20] == 0 ? 0 : (double)neuralArray[19] / (double)neuralArray[20]);
            modifiers.Add("Periods", neuralArray.GetPeriods());
            modifiers.Add("BuyLimit", neuralArray.GetBuyLimit());
            modifiers.Add("StopLimit", neuralArray.GetStopLimit());
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
            return neuralArray.Length > 0 && neuralArray.Length <= 30;
        }
    }
}
