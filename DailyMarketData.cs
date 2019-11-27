using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class DailyMarketData
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public double Opening { get; set; }
        public double Closing { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public int Capacity { get; set; }
    }
}
