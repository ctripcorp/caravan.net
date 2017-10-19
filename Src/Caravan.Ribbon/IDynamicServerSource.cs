using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    public interface IDynamicServerSource : IServerSource
    {
        event EventHandler OnChange;
    }
}
