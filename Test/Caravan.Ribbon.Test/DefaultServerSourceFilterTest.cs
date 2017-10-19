using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Reflection;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Ribbon.Util;
using System.Configuration;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [TestClass]
    public class DefaultServerSourceFilterTest : TestBase
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultServerSourceFilterTest));
        private DataContractJsonSerializer _serializer = new DataContractJsonSerializer(typeof(List<ServerGroup>));

        [TestMethod]
        public void UpdateServerList_NotUpdateEmptyList_Test()
        {
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);

            servers.Clear();
            serverSource.LoadBalancerRoutes = new List<LoadBalancerRoute>() { new LoadBalancerRoute("default") }; // clear and set empty list
            Assert.AreNotEqual(0, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
        }

        [TestMethod]
        public void UpdateServerListTest()
        {
            int serverCount = 10;
            bool onChangeEventRaised = false;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers.ToArray());
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);

            servers.Clear();
            for (int i = 0; i < serverCount * 2; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }
            serverSource.OnChange += (o, e) => onChangeEventRaised = true;
            serverSource.LoadBalancerRoutes = new DefaultDynamicServerSource(servers).LoadBalancerRoutes;
            Assert.AreEqual(serverCount * 2, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.IsTrue(onChangeEventRaised);
        }

        [TestMethod]
        public void UpdateServerList_AlwaysChange_Test()
        {
            int serverCount = 10;
            bool onChangeEventRaised = false;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            
            List<Server> newServers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                newServers.Add(servers[serverCount - i - 1]);
            }
            loadBalancerContext.ServerSourceFilter.OnChange += (o, e) => onChangeEventRaised = true;

            var serverGroups = new List<ServerGroup>() { new ServerGroup("default", 1, servers) };
            var newLoadBalancerRoutes = new List<LoadBalancerRoute>() { new LoadBalancerRoute("default", serverGroups) };
            serverSource.LoadBalancerRoutes = newLoadBalancerRoutes;
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);

            var targetRoute = loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig);
            var originRoute = loadBalancerContext.ServerSourceFilter.LoadBalancerRoutes[0];
            Assert.AreSame(originRoute, targetRoute);

            Assert.IsTrue(onChangeEventRaised);
        }

        [TestMethod]
        public void UpdateServerList_RetainStatus_Test()
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            int serverCount = 10;
            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                servers.Add(new Server("Server_" + i, i % 2 == 0, new Dictionary<string, string>() { { "Index", i.ToString() } }));
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var ping = new PredicatePing(server =>
            {
                int index = int.Parse(server.Metadata["Index"]);
                return index >= serverCount || index % 2 == 0;
            });
            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager(managerId, new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            List<Server> newServers = new List<Server>();
            for (int i = 0; i < serverCount * 2; i++)
            {
                var metadata = new Dictionary<string, string>() 
                {
                    { "Index", i.ToString() },
                    { "Hello", "World" }
                };
                newServers.Add(new Server("Server_" + i, metadata));
            }
            serverSource.LoadBalancerRoutes = new DefaultDynamicServerSource(newServers).LoadBalancerRoutes;
            Assert.AreEqual(serverCount * 2, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            foreach (var server in loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers)
            {
                int index = int.Parse(server.Metadata["Index"]);
                Assert.AreEqual(2, server.Metadata.Count);
                if (index < serverCount)
                {
                    Assert.AreEqual(index % 2 == 0, server.IsAlive, index.ToString());
                }
                else
                {
                    Assert.IsTrue(server.IsAlive);
                }
            }
        }

        [TestMethod]
        public void UpdateServerList_UnavailableServer_Test()
        {
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, new Dictionary<string, string>() { { "Index", i.ToString() } });
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(null, serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);

            for (int i = serverCount; i < serverCount * 2; i++)
            {
                var server = new Server("Server_" + i, i % 2 == 0, new Dictionary<string, string>() { { "Index", i.ToString() } });
                servers.Add(server);
            }
            serverSource.LoadBalancerRoutes = new DefaultDynamicServerSource(servers).LoadBalancerRoutes;
            Assert.AreEqual(serverCount * 2, defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(serverCount + (serverCount / 2), defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);
            foreach (var server in defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers)
            {
                int index = int.Parse(server.Metadata["Index"]);
                if (index < serverCount)
                    Assert.IsTrue(server.IsAlive);
                else
                    Assert.AreEqual(index % 2 == 0, server.IsAlive, server.ToString());
            }
        }

        [TestMethod]
        public void UpdateServerList_FirstTimeNotPing_Test()
        {
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            int pingCount = 0;
            var ping = new PredicatePing(server => { ++pingCount; return true; });
            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(0, pingCount);
        }

        [TestMethod]
        public void UpdateServerList_NotPingMinAvailableServerCountServer_Test()
        {
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new FalsePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);

            servers = new List<Server>();
            int minAvailableServerCount = int.Parse(ConfigurationManager.AppSettings["soa." + MethodInfo.GetCurrentMethod().Name + ".ribbon.min-available-server-count"]);
            for (int i = 0; i < minAvailableServerCount; i++)
            {
                servers.Add(new Server("newServer_" + i));
            }
            Console.WriteLine("MinAvailableServerCount: " + minAvailableServerCount);
            serverSource.LoadBalancerRoutes = new DefaultDynamicServerSource(servers).LoadBalancerRoutes;
            Assert.AreEqual(minAvailableServerCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(minAvailableServerCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);
        }

        [TestMethod]
        public void UpdateServerList_NotPingExistedServer_Test()
        {
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new FalsePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);

            servers = new List<Server>(servers);
            for (int i = serverCount; i < serverCount * 2; i++)
            {
                servers.Add(new Server("newServer_" + i));
            }
            serverSource.LoadBalancerRoutes = new DefaultDynamicServerSource(servers).LoadBalancerRoutes;
            Assert.AreEqual(serverCount * 2, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);
        }

        [TestMethod]
        public void UpdateServerList_PullOutServer_Test()
        {
            int repeat = 2;
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);

            Dictionary<string, int> serverCountMap = new Dictionary<string, int>();
            for (int i = 0; i < servers.Count * repeat; i++)
            {
                var requestContext = loadBalancer.GetRequestContext(requestConfig);
                if (!serverCountMap.ContainsKey(requestContext.Server.ServerId))
                    serverCountMap[requestContext.Server.ServerId] = 1;
                else
                    serverCountMap[requestContext.Server.ServerId] += 1;
            }
            foreach (var count in serverCountMap.Values)
            {
                Assert.AreEqual(2, count);
            }

            servers.RemoveAt(0);
            serverSource.LoadBalancerRoutes = new DefaultDynamicServerSource(servers).LoadBalancerRoutes;
            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);

            serverCountMap.Clear();
            for (int i = 0; i < servers.Count * repeat; i++)
            {
                var requestContext = loadBalancer.GetRequestContext(requestConfig);
                if (!serverCountMap.ContainsKey(requestContext.Server.ServerId))
                    serverCountMap[requestContext.Server.ServerId] = 1;
                else
                    serverCountMap[requestContext.Server.ServerId] += 1;
            }
            foreach (var count in serverCountMap.Values)
            {
                Assert.AreEqual(2, count);
            }
        }
        
        [TestMethod]
        public void UpdateServerList_AddNewRoute_Test()
        {
            int repeat = 2;
            int serverCount = 10;
            string routeId = "default";

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i);
                servers.Add(server);
            }

            List<LoadBalancerRoute> routes = new List<LoadBalancerRoute>();
            routes.Add(new LoadBalancerRoute(routeId, new List<ServerGroup>(){new ServerGroup("default", 1, servers, null)}));

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig(routeId, 0, true);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>(){routeConfig});

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);

            Dictionary<string, int> serverCountMap = new Dictionary<string, int>();
            for (int i = 0; i < servers.Count * repeat; i++)
            {
                var requestContext = loadBalancer.GetRequestContext(requestConfig);
                if (!serverCountMap.ContainsKey(requestContext.Server.ServerId))
                    serverCountMap[requestContext.Server.ServerId] = 1;
                else
                    serverCountMap[requestContext.Server.ServerId] += 1;
            }
            foreach (var count in serverCountMap.Values)
            {
                Assert.AreEqual(2, count);
            }


            List<LoadBalancerRoute> newRoutes = new List<LoadBalancerRoute>();
            List<Server> newServers = new List<Server>(){servers[0]};
            newRoutes.Add(new LoadBalancerRoute(routeId, new List<ServerGroup>() { new ServerGroup("default", 1, servers, null) }));
            newRoutes.Add(new LoadBalancerRoute("newRoute", new List<ServerGroup>() { new ServerGroup("default", 1, newServers, null) }));
            Console.WriteLine(string.Join(Environment.NewLine, newRoutes));
            serverSource.LoadBalancerRoutes = newRoutes;

            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(servers.Count, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);

            serverCountMap.Clear();
            for (int i = 0; i < servers.Count * repeat; i++)
            {
                var requestContext = loadBalancer.GetRequestContext(requestConfig);
                if (!serverCountMap.ContainsKey(requestContext.Server.ServerId))
                    serverCountMap[requestContext.Server.ServerId] = 1;
                else
                    serverCountMap[requestContext.Server.ServerId] += 1;
            }
            foreach (var count in serverCountMap.Values)
            {
                Assert.AreEqual(2, count);
            }
        }

        [TestMethod]
        public void FilterInvalidEntitiesTest1()
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), new DefaultServerSource(new Server[0]));
            var loadBalancerManager = LoadBalancerManager.GetManager(managerId, new LoadBalancerManagerConfig(configurationManager));
            var lbContext = new DefaultLoadBalancerContext(loadBalancerManager, loadBalancerId, loadBalancerConfig);

            try
            {
                string data = "[{\"RouteId\":\"default-route-rule\",\"ServerGroups\":[{\"GroupId\":\"default-group-key\",\"Metadata\":[],\"Servers\":[{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.44.138/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.44.138/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.44.138\"},{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.42.106/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.42.106/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.42.106\"}],\"Weight\":5}]}]";
                File.WriteAllText(fileName, data);

                var result = lbContext.ServerSourceManager.Restore();
                result = result.FilterInvalidEntities(_logger, new Dictionary<string, string>());
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(1, result[0].ServerGroups.Length);
                Assert.AreEqual(2, result[0].ServerGroups[0].Servers.Length);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public void FilterInvalidEntitiesTest2()
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), new DefaultServerSource(new Server[0]));
            var loadBalancerManager = LoadBalancerManager.GetManager(managerId, new LoadBalancerManagerConfig(configurationManager));
            var lbContext = new DefaultLoadBalancerContext(loadBalancerManager, loadBalancerId, loadBalancerConfig);

            try
            {
                string data = "[{\"RouteId\":\"\",\"ServerGroups\":[{\"GroupId\":\"default-group-key\",\"Metadata\":[],\"Servers\":[{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.44.138/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.44.138/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.44.138\"},{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.42.106/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.42.106/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.42.106\"}],\"Weight\":5}]}]";
                File.WriteAllText(fileName, data);

                var result = lbContext.ServerSourceManager.Restore();
                result = result.FilterInvalidEntities(_logger, new Dictionary<string, string>());
                Assert.AreEqual(0, result.Count);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public void FilterInvalidEntitiesTest3()
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), new DefaultServerSource(new Server[0]));
            var loadBalancerManager = LoadBalancerManager.GetManager(managerId, new LoadBalancerManagerConfig(configurationManager));
            var lbContext = new DefaultLoadBalancerContext(loadBalancerManager, loadBalancerId, loadBalancerConfig);

            try
            {
                string data = "[{\"RouteId\":\"default-route-rule\",\"ServerGroups\":[{\"GroupId\":\"\",\"Metadata\":[],\"Servers\":[{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.44.138/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.44.138/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.44.138\"},{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.42.106/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.42.106/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.42.106\"}],\"Weight\":5}]}]";
                File.WriteAllText(fileName, data);

                var result = lbContext.ServerSourceManager.Restore();
                result = result.FilterInvalidEntities(_logger, new Dictionary<string, string>());
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(0, result[0].ServerGroups.Length);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public void FilterInvalidEntitiesTest4()
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            string managerId = "soa";
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;
            string fileName = configuration.GetPropertyValue("soa.ribbon.local-cache.data-folder") + "\\" + managerId + "." + loadBalancerId;
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), new DefaultServerSource(new Server[0]));
            var loadBalancerManager = LoadBalancerManager.GetManager(managerId, new LoadBalancerManagerConfig(configurationManager));
            var lbContext = new DefaultLoadBalancerContext(loadBalancerManager, loadBalancerId, loadBalancerConfig);

            try
            {
                string data = "[{\"RouteId\":\"default-route-rule\",\"ServerGroups\":[{\"GroupId\":\"default-group-key\",\"Metadata\":[],\"Servers\":[{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.44.138/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.44.138/test-service/checkhealth.json\"}],\"ServerId\":\"\"},{\"IsAlive\":true,\"Metadata\":[{\"Key\":\"subenv\",\"Value\":\"uat\"},{\"Key\":\"url\",\"Value\":\"http://10.2.42.106/test-service/\"},{\"Key\":\"healthCheckUrl\",\"Value\":\"http://10.2.42.106/test-service/checkhealth.json\"}],\"ServerId\":\"10.2.42.106\"}],\"Weight\":5}]}]";
                File.WriteAllText(fileName, data);

                var result = lbContext.ServerSourceManager.Restore();
                result = result.FilterInvalidEntities(_logger, new Dictionary<string, string>());
                Assert.AreEqual(1, result.Count);
                Assert.AreEqual(1, result[0].ServerGroups.Length);
                Assert.AreEqual(1, result[0].ServerGroups[0].Servers.Length);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [TestMethod]
        public void CanReplaceRouteTest()
        {
            int repeat = 2;
            int serverCount = 10;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }

            LoadBalancerRouteConfig routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });
            var groups = new List<ServerGroup>() { new ServerGroup("default", 1, servers) };
            var routes = new List<LoadBalancerRoute>() { new LoadBalancerRoute("default", groups) };

            var newRoute = new LoadBalancerRoute("newRoutes", new List<ServerGroup>() { new ServerGroup("newGroup", 1, servers.Take(1)) });

            EventHandler<SeekRouteEventArgs> seekRouteEventHandler = (o, e) => e.Route = newRoute;

            var serverSource = new DefaultDynamicServerSource(routes);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource) { SeekRouteEventHandler = seekRouteEventHandler };
            var loadBalancer = factory.GetLoadBalancer(MethodInfo.GetCurrentMethod().Name, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.LoadBalancerRoutes[0].Servers.Length);
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.LoadBalancerRoutes[0].AvailableServers.Length);

            Dictionary<Server, int> serverCountMap = new Dictionary<Server, int>();
            for (int i = 0; i < serverCount * repeat; i++)
            {
                ILoadBalancerRequestContext requestContext = loadBalancer.GetRequestContext(requestConfig);
                if (serverCountMap.ContainsKey(requestContext.Server))
                    serverCountMap[requestContext.Server] += 1;
                else
                    serverCountMap[requestContext.Server] = 1;
            }

            Assert.AreEqual(1, serverCountMap.Count);
            foreach (int count in serverCountMap.Values)
            {
                Assert.AreEqual(serverCount * repeat, count);
            }
        }
    }
}
