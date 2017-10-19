using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    interface IServerSourceMonitor
    {
        void MonitorServers();

        event EventHandler ServerStatusChange;
    }
}
