using Neurox.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Neurox.Core
{
    public class Neuron
    {
		private List<double> _inputs;
		private List<double> _weights;
		private IOutputFunction _outputFunction;

		public List<double> Inputs
		{
			get { return _inputs; }
		}
		public List<double> Weights
		{
			get { return _weights; }
		}

		public double Output
		{
			get
			{
				return _outputFunction;
			}
		}
	}
}
