using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [TestClass]
    public class DefaultServerSourceManagerTest : TestBase
    {
        private DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(List<LoadBalancerRoute>));

        [TestMethod]
        public void BackupRestoreTest()
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            try
            {
                var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), new DefaultServerSource(new Server[0]));
                var loadBalancerManager = LoadBalancerManager.GetManager(managerId, new LoadBalancerManagerConfig(configurationManager));
                var lbContext = new DefaultLoadBalancerContext(loadBalancerManager, loadBalancerId, loadBalancerConfig);

                var manager = new DefaultServerSourceManager(lbContext);
                List<ServerGroup> serverGroups = new List<ServerGroup>();
                for (int i = 0; i < 10; i++)
                {
                    var server = new Server("Server_" + i, i % 2 == 0);
                    serverGroups.Add(new ServerGroup(server.ServerId, i, new List<Server>() { server }));
                }
                var expected = new List<LoadBalancerRoute>() { new LoadBalancerRoute("default", serverGroups) };
                manager.Backup(expected);
                var actual = manager.Restore();
                CollectionAssert.AreEqual(expected, actual);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public void RemoteServerListFirstTest()
        {
            int serverCount = 10;
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            try
            {
                List<ServerGroup> serverGroups = new List<ServerGroup>();
                for (int i = 0; i < serverCount; i++)
                {
                    var server = new Server("Server_" + i, i % 2 == 0, new Dictionary<string, string>() { { "Index", i.ToString() } });
                    serverGroups.Add(new ServerGroup(server.ServerId, i, new List<Server>() { server }));
                }
                using (Stream stream = File.OpenWrite(fileName))
                {
                    _serializer.WriteObject(stream, new List<LoadBalancerRoute>() { new LoadBalancerRoute("default", serverGroups) });
                    stream.Flush();
                }

                List<Server> servers = new List<Server>();
                for (int i = serverCount; i < serverCount * 2; i++)
                {
                    var server = new Server("Server_" + i, new Dictionary<string, string>() { { "Index", i.ToString() } });
                    servers.Add(server);
                }

                var serverSource = new DefaultDynamicServerSource(servers.ToArray());
                var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
                var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
                var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
                var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

                LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
                LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });
                foreach (var server in defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers)
                {
                    int index = int.Parse(server.Metadata["Index"]);
                    Assert.IsTrue(index >= serverCount);
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public void ServerSourceRestoreEventTest()
        {
            int serverCount = 10;
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            try
            {
                List<ServerGroup> serverGroups = new List<ServerGroup>();
                for (int i = 0; i < serverCount; i++)
                {
                    var server = new Server("Server_" + i, i % 2 == 0, new Dictionary<string, string>() { { "Index", i.ToString() } });
                    serverGroups.Add(new ServerGroup(server.ServerId, i, new List<Server>() { server }));
                }
                using (Stream stream = File.OpenWrite(fileName))
                {
                    _serializer.WriteObject(stream, new List<LoadBalancerRoute>() { new LoadBalancerRoute("default", serverGroups) });
                    stream.Flush();
                }

                List<Server> servers = new List<Server>();

                EventHandler<ServerSourceRestoreEvent> handler = (o, e) => 
                {
                    foreach (var route in e.Routes)
                    {
                        foreach (var server in route.Servers)
                        {
                            int index = int.Parse(server.Metadata["Index"]);
                            server.Metadata["Index"] = (index + serverCount).ToString();
                        }
                    }
                };

                var serverSource = new DefaultDynamicServerSource(servers.ToArray());
                var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
                var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource) { ServerSourceRestoreEventHandler = handler };
                var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
                var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

                LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
                LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });
                foreach (var server in defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers)
                {
                    int index = int.Parse(server.Metadata["Index"]);
                    Assert.IsTrue(index >= serverCount);
                }
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}
