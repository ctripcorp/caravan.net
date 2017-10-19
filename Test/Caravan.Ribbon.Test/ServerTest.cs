using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Ribbon;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [TestClass]
    public class ServerTest
    {
        [TestMethod]
        public void ServerEquals_ServerId_CaseInsensitive_Test()
        {
            Server server1 = new Server("Abc", null);
            Server server2 = new Server("abc", null);
            Assert.AreEqual(server1, server2);
            Assert.AreEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_IsAliveNotEqual_Test()
        {
            Server server1 = new Server("Abc", true, null);
            Server server2 = new Server("abc", false, null);
            Assert.AreEqual(server1, server2);
            Assert.AreEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_ServerId_Equals_Test()
        {
            Server server1 = new Server("abc", null);
            Server server2 = new Server("abc", null);
            Assert.AreEqual(server1, server2);
            Assert.AreEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_Metadata_Equals_Test()
        {
            Server server1 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World" }, { "City", "Shanghai" } });
            Server server2 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World" }, { "City", "Shanghai" } });
            Assert.AreEqual(server1, server2);
            Assert.AreEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_Metadata_NullEqualEmpty_Test()
        {
            Server server1 = new Server("abc", null);
            Server server2 = new Server("abc", new Dictionary<string, string>());
            Assert.AreEqual(server1, server2);
            Assert.AreEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_Metadata_CountNotEquals_Test()
        {
            Server server1 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World" } });
            Server server2 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World" }, { "City", "Shanghai" } });
            Assert.AreNotEqual(server1, server2);
            Assert.AreNotEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_Metadata_KeyNotEquals_Test()
        {
            Server server1 = new Server("abc", new Dictionary<string, string>() { { "Hello ", "World" }, { "City", "Shanghai" } });
            Server server2 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World" }, { "City", "Shanghai" } });
            Assert.AreNotEqual(server1, server2);
            Assert.AreNotEqual(server2, server1);
        }

        [TestMethod]
        public void ServerEquals_Metadata_ValueNotEquals_Test()
        {
            Server server1 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World " }, { "City", "Shanghai" } });
            Server server2 = new Server("abc", new Dictionary<string, string>() { { "Hello", "World" }, { "City", "Shanghai" } });
            Assert.AreNotEqual(server1, server2);
            Assert.AreNotEqual(server2, server1);
        }
    }
}
