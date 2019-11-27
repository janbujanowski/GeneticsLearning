using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public interface ILogger
    {
        void LogException(Exception ex, string message);
        void LogInfo(string message);
    }
}
