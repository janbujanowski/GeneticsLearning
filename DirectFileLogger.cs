using System;
using System.Collections.Generic;
using System.Configuration;
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
            _pathToInfoFile = pathToInfoFile;
            _pathToExceptionFile = pathToExceptionFile;
        }
        public DirectFileLogger()
        {
            //get default logger
            _pathToInfoFile = IoCFactory.Resolve<IConfigurationProvider>().GetConfigurationString("workingDirectory", "pathToInfoFile");
            _pathToExceptionFile = IoCFactory.Resolve<IConfigurationProvider>().GetConfigurationString("workingDirectory", "pathToExceptionFile");
        }

        public void LogException(Exception ex, string message, params object[] args)
        {
            if (string.IsNullOrEmpty(_pathToExceptionFile))
            {
                throw new Exception("Exception log file is not specified");
            }
            WriteToFile(_pathToInfoFile, $"ERROR {ex.Message}");
            WriteToFile(_pathToExceptionFile, $"==========================================NEW ENTRY==========================================");
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
            var stream = File.AppendText(path);
            stream.WriteLine($"|{ DateTime.Now.ToString() }|: { message }");
            stream.Flush();
            stream.Dispose();
        }
    }
}