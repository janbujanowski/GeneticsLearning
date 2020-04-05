using System;
using System.Collections.Generic;
using System.Text;

namespace Neurox.Core.Networks
{
    public class NeuralNetwork
    {
        private List<List<Neuron>> _layers;
        public List<List<Neuron>> Layers
        {
            get
            {
                return _layers;
                { }
            }
        }
    }
}
