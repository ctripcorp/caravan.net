using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan
{
    public interface IValueParser<T>
    {
        T Parse(string input);

        bool TryParse(string input, out T result);
    }
}
