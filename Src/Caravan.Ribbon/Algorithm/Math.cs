using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Algorithm
{
    public static class Math
    {
        public static int GCD(int a, int b)
        {
            int c;
            while (b != 0)
            {
                c = b;
                b = a % b;
                a = c;
            }
            return a;
        }
    }
}
