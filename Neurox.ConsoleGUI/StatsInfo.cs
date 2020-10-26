using System;
using System.Collections.Generic;
using System.Text;

namespace Neurox.ConsoleGUI
{
    public class StatsInfo
    {
        public double[,] HiddenLayerWeights { get; set; }
        public double[,] HiddenToOutputLayerWeights { get; set; }
        public Dictionary<string, double> InputLayerModifiers { get; set; }
        public Individual Individual { get; set; }
        public int Population { get; set; }
        public DateTime Date { get; set; }
    }
}
