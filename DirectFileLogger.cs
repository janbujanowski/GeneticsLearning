using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class DirectFileLogger : ILogger
    {
        string _pathToExceptionFile, _pathToInfoFile;
        public DirectFileLogger(string pathToInfoFile, string pathToExceptionFile)
        {
            _pathToExceptionFile = pathToExceptionFile;
            _pathToInfoFile = pathToInfoFile;
        }

        public void LogException(Exception ex, string message, params object[] args)
        {
            if (string.IsNullOrEmpty(_pathToExceptionFile))
            {
                throw new Exception("Exception log file is not specified");
            }
            WriteToFile(_pathToExceptionFile, $"Error message : [{ message }]");
            WriteToFile(_pathToExceptionFile, $"Exception message : [{ ex.Message }]");
            WriteToFile(_pathToExceptionFile, $"Stacktrace : [{ ex.StackTrace }]");
            if (args != null)
            {
                foreach (var item in args)
                {
                    WriteToFile(_pathToExceptionFile, $"Param [name][value] : [{ nameof(item) }][{ item.ToString() }]");
                }
            }
        }

        public void LogInfo(string message)
        {
            if (string.IsNullOrEmpty(_pathToInfoFile))
            {
                throw new Exception("Info log file is not specified");
            }
            WriteToFile(_pathToInfoFile, $"Info : [{ message }]");
        }

        private void WriteToFile(string path, string message)
        {
            var stream = File.AppendText(_pathToInfoFile);
            stream.Write($"|{ DateTime.Now.ToString() }|: { message }");
            stream.Flush();
            stream.Dispose();
        }
    }
}