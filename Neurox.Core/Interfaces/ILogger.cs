using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurox.Core.Interfaces
{
    public interface ILogger
    {
        void LogException(Exception ex, string message,params object[] args);
        void LogInfo(string message);
    }
}
