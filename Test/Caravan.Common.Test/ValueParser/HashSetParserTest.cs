using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    [TestClass]
    public class HashSetParserTest : TestBase
    {
        [TestMethod]
        public void HashSetParser_Valid_Test()
        {
            var parser = new HashSetParser<int>(IntParser.Instance);
            var result = parser.Parse(" 1,2,3 , 3,4 ,,5, ,5");
            Assert.AreEqual(5, result.Count);
        }

        [TestMethod]
        public void HashSetParser_Invalid_Test()
        {
            var parser = new HashSetParser<int>(IntParser.Instance);
            HashSet<int> result;
            Assert.IsFalse(parser.TryParse(" 1,2,3 , c3,4 ,5, 5", out result));
        }
    }
}
