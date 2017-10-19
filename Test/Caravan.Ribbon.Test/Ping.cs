using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    class TruePing : IPing
    {
        public bool IsAlive(Server server)
        {
            return true;
        }
    }

    class FalsePing : IPing
    {
        public bool IsAlive(Server server)
        {
            return false;
        }
    }

    class PredicatePing : IPing
    {
        private Func<Server, bool> Predicate;

        public PredicatePing(Func<Server, bool> predicate)
        {
            Predicate = predicate;
        }

        public bool IsAlive(Server server)
        {
            return Predicate(server);
        }
    }

    class Ping : IPing
    {
        public bool Success { get; set; }

        public Ping()
        { }

        public Ping(bool success)
        {
            Success = success;
        }

        public bool IsAlive(Server server)
        {
            return Success;
        }
    }
}
