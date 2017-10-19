using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Com.Ctrip.Soa.Caravan.Logging.Null
{
    /// <summary>
    /// A null logger which do nothing when being called.
    /// </summary>
    public class NullLog : LogBase
    {
        public NullLog()
            : base()
        { }

        public NullLog(string logName)
            : base(logName)
        { }

        protected override void Log(string level, string title, string message, Dictionary<string, string> tags = null)
        {
            //do nothing
        }
    }
}