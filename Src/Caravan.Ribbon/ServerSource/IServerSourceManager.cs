using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon.ServerSource
{
    interface IServerSourceManager
    {
        void Backup(List<LoadBalancerRoute> routes);

        List<LoadBalancerRoute> Restore();

        event EventHandler<ServerSourceRestoreEvent> OnServerSourceRestore;
    }
}
