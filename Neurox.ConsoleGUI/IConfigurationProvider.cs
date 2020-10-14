using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurox.ConsoleGUI
{
    public interface IConfigurationProvider
    {
        string this[string key] { get; }
        string GetConfiguredFilePath(string combinedWithDirectoryKey, string configKey);
    }
}