using Newtonsoft.Json;
using srodowisko;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurox.ConsoleGUI
{
    public class GeneticDefaults
    {
        public SelectionMethods SelectionMethod = SelectionMethods.Roulette;
        public int POPULATIONSIZE = 20; // WTFFFFF TODO REWORK THIS PARAMS
        public double ModyfikatorWyniku = 1;
        private static GeneticDefaults _config;
        public static GeneticDefaults INSTANCE
        {
            get
            {
                if (_config == null)
                {
                    _config = new GeneticDefaults();
                }
                return _config;
            }
        }
        private GeneticDefaults()
        {
            
        }
        private static Random _CUBE;
        public static Random CUBE
        {
            get
            {
                if (_CUBE == null)
                {
                    _CUBE = new Random();
                }
                return _CUBE;
            }
        }
       
    }
}