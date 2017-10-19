using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Com.Ctrip.Soa.Caravan.Logging.Console
{
    public class ConsoleLogManager : ILogManager
    {
        private ConcurrentDictionary<string, ConsoleLog> _cache = new ConcurrentDictionary<string, ConsoleLog>();
        
        public static ConsoleLogManager Instance { get; private set; }

        static ConsoleLogManager()
        {
            Instance = new ConsoleLogManager();
        }

        private ConsoleLogManager()
        { 
        }

        public string Name
        {
            get { return "ConsoleLogManager"; }
        }

        public ILog GetLogger(Type type)
        {
            var name = type == null ? "NoName" : type.FullName;
            return _cache.GetOrAdd(name, logName => new ConsoleLog(logName));
        }

        public ILog GetLogger(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = "NoName";
            return _cache.GetOrAdd(name, logName => new ConsoleLog(logName));
        }
    }
}
