using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

using SConsole = System.Console;

namespace Com.Ctrip.Soa.Caravan.Logging.Console
{
    /// <summary>
    /// A logger which log to default output stream when being called.
    /// </summary>
    public class ConsoleLog : LogBase
    {
        public ConsoleLog()
            : base()
        { }

        public ConsoleLog(string logName)
            : base(logName)
        { }

        protected override void Log(string level, string title, string message, Dictionary<string, string> tags = null)
        { 
            var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (string.IsNullOrWhiteSpace(title))
                title = "NA";
            SConsole.WriteLine("{0} [{1}] {2} [{3}] {4}", now, threadId, level, LogName, title);
            if (tags != null && tags.Count > 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var item in tags)
                {
                    builder.AppendFormat("{0}={1}, ", item.Key, item.Value);
                }
                builder.Remove(builder.Length - 2, 2);
                SConsole.WriteLine(builder.ToString());
            }
            if (!string.IsNullOrWhiteSpace(message))
                SConsole.WriteLine(message);
        }
    }
}