using System;
using System.Collections.Generic;
using System.Text;

namespace Neurox.Core.Interfaces
{
    public interface IOutputFunction
    {
        double Output(List<double> inputs, List<double> weights);
    }
}
