using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class DirectFileLogger : ILogger
    {
        string _logPathException, _logPathInfo;
        public DirectFileLogger(string pathToInfoFile, string pathToExceptionFile)
        {
            _logPathException = pathToExceptionFile;
            _logPathInfo = pathToInfoFile;
        }
        public void LogException(Exception ex, string message)
        {
            if (string.IsNullOrEmpty(_logPathException))
            {
                throw new Exception("Exception log file is not specified");
            }
        }

        public void LogInfo(string message)
        {
            if (string.IsNullOrEmpty(_logPathInfo))
            {
                throw new Exception("Info log file is not specified");
            }
        }
    }
}
