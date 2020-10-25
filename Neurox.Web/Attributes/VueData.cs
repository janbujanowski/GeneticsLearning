using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neurox.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VueData : Attribute
    {
        public VueData(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
