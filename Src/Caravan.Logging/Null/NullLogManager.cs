using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Com.Ctrip.Soa.Caravan.Logging.Null
{
    public class NullLogManager : ILogManager
    {
        private ConcurrentDictionary<string, NullLog> _cache = new ConcurrentDictionary<string, NullLog>();
        
        public static NullLogManager Instance { get; private set; }

        static NullLogManager()
        {
            Instance = new NullLogManager();
        }

        private NullLogManager()
        { 
        }

        public string Name
        {
            get { return "NullLogManager"; }
        }

        public ILog GetLogger(Type type)
        {
            var name = type == null ? "NoName" : type.FullName;
            return _cache.GetOrAdd(name, logName => new NullLog(logName));
        }

        public ILog GetLogger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "NoName";
            return _cache.GetOrAdd(name, logName => new NullLog(logName));
        }
    }
}
