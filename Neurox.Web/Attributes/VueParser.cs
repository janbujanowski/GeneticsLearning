using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neurox.Web.Attributes
{
    public class VueParser : IVueParser
    {
        public Dictionary<string, object> ParseData<TModel>(TModel model)
        {
            var props = model.GetType().GetProperties();
            var result = new Dictionary<string, object>();

            foreach (var prop in props)
            {
                var attr = prop.GetCustomAttributes(typeof(VueData), true)?.FirstOrDefault()
                    as VueData;

                if (attr == null)
                {
                    continue;
                }

                result.Add(attr.Name, prop.GetValue(model));
            }

            return result;
        }
    }
}
