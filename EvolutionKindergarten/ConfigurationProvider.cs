using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorytmEwolucyjny
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public string this[string key] { get => ConfigurationManager.AppSettings[key];  }

        public string GetConfigurationString(string combinedWithDirectoryKey, string configKey)
        {
            return Path.Combine(ConfigurationManager.AppSettings[combinedWithDirectoryKey], ConfigurationManager.AppSettings[configKey]);
        }
    }
}