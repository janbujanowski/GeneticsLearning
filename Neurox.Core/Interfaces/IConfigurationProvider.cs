using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neurox.Core.Interfaces
{
    public interface IConfigurationProvider
    {
        string this[string key] { get; }
        string GetConfigurationString(string combinedWithDirectoryKey, string configKey);
    }
}