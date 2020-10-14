using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurox.ConsoleGUI
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        IConfigurationRoot config;
        public ConfigurationProvider()
        {
            //var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", true, true)
                //.AddJsonFile($"appsettings.{env}.json", true, true)
                //.AddEnvironmentVariables();
                ;
            
            config = builder.Build();
        }
        public string this[string key] { get => config[key]; }// ConfigurationManager.AppSettings[key];  }

        public string GetConfiguredFilePath(string configKeyDirectoryName, string configKeyFileName)
        {
            return Path.Combine(config[configKeyDirectoryName], config[configKeyFileName]);
        }
    }
}