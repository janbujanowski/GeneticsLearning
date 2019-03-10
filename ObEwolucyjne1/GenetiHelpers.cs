using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObEwolucyjne1
{
    public static class GenetiHelpers
    {
        public static uint Sum(this uint[] arrayOfStuff)
        {
            uint sum = 0;
            foreach (var item in arrayOfStuff)
            {
                sum += item;
            }
            return sum;
        }
    }
}
