using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Com.Ctrip.Soa.Caravan
{
    [TestClass]
    public class TestBase
    {
        static TestBase()
        {
            LogManager.CurrentManager = ConsoleLogManager.Instance;
        }
    }
}
