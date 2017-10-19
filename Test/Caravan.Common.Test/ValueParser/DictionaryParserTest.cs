using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Com.Ctrip.Soa.Caravan.ValueParser
{
    [TestClass]
    public class DictionaryParserTest : TestBase
    {
        [TestMethod]
        public void DictionaryParser_Valid_Test()
        {
            var parser = new DictionaryParser<string, int>(StringParser.Instance, IntParser.Instance);
            var result = parser.Parse(" Tom:10, Jerry: 7");
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(10, result["Tom"]);
            Assert.AreEqual(7, result["Jerry"]);
        }

        [TestMethod]
        public void DictionaryParser_Invalid_Test()
        {
            var parser = new ListParser<int>(IntParser.Instance);
            List<int> result;
            Assert.IsFalse(parser.TryParse(" Tom:10x, Jerry: 7", out result));
        }
    }
}
