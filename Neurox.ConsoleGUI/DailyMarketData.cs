﻿using System;

namespace Neurox.ConsoleGUI
{
    public class DailyMarketData
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }

        public double Opening { get; set; }
        public double Closing { get; set; }
        public double Max { get; set; }
        public double Min { get; set; }
        public int Volume { get; set; }
    }
}