using System;

namespace Neurox.ConsoleGUI
{
    class Program
    {
        static void Main(string[] args)
        {
            GeneticEnvironment geneticParameters = GeneticEnvironment.INSTANCE;
            geneticParameters.ParseParameters(args);
            NeuroxEvolution evolution = new NeuroxEvolution(geneticParameters);
        }
    }
}
