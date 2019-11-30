using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public static class IntArrayExtensions
    {
        public static int GetPeriods(this int[] neuralArray)
        {
            return neuralArray[10];
        }
        public static double GetRSIModifier(this int[] neuralArray)
        {
            if (neuralArray[12] == 0)
            {
                return 0;
            }
            return neuralArray[11]/ neuralArray[12];
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
        public static bool IsValidNeuralArray(this int[] neuralArray)
        {
            return neuralArray.Length > 0 && neuralArray.Length <= 30;
        }
    }
}
