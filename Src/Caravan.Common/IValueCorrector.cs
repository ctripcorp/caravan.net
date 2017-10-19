using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan
{
    public interface IValueCorrector<T>
    {
        T Correct(T value);
    }
}
