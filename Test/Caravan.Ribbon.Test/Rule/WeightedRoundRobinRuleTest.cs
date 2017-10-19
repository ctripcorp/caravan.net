using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Collections.Generic;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Threading.Atomic;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Rule
{
    [TestClass]
    public class WeightedRoundRobinRuleTest
    {
        LoadBalancerRequestConfig requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { new LoadBalancerRouteConfig("default", 0, false) });

        [TestMethod]
        public void ShuffleServersTest()
        {
            List<Server> servers = new List<Server>();
            for (int i = 0; i < 10; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, metadata);
                servers.Add(server);
            }
            servers.Shuffle();
            servers.ForEach(item => Console.WriteLine(item + " "));
        }

        [TestMethod]
        public void WeightedRoundRobinTest()
        {
            int serverCount = 10;
            int repeatTimes = 100;
            int repeatInterval = 0;
            int threadCount = 20;
            int serverSumCount = 0;
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            Dictionary<int, AtomicInteger> indexCountMapping = new Dictionary<int, AtomicInteger>();
            List<ServerGroup> serverGroups = new List<ServerGroup>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, metadata);
                serverGroups.Add(new ServerGroup(server.ServerId, i, new List<Server>() { server }));
                indexCountMapping[i] = 0;
                serverSumCount += i;
            }

            var ping = new TruePing();
            var serverSource = new DefaultDynamicServerSource(serverGroups);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Action action = () =>
            {
                for (int i = 0; i < repeatTimes; i++)
                {
                    for (int j = 0; j < serverSumCount; j++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(requestConfig);
                        int index = int.Parse(requestContext.Server.Metadata["Index"]);
                        indexCountMapping[index].IncrementAndGet();
                        if (repeatInterval > 0)
                            Thread.Sleep(repeatInterval);
                    }
                }
            };

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(action));
            }
            Task.WaitAll(tasks.ToArray());

            foreach (var item in indexCountMapping)
            {
                Assert.AreEqual(item.Key * repeatTimes * threadCount, item.Value.Value, item.Key.ToString());
            }
        }

        [TestMethod]
        public void WeightedRoundRobinWithUnavailableServerTest()
        {
            int serverCount = 10;
            int repeatTimes = 100;
            int repeatInterval = 0;
            int threadCount = 20;
            int serverSumCount = 0;
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            Dictionary<int, AtomicInteger> indexCountMapping = new Dictionary<int, AtomicInteger>();
            List<ServerGroup> serverGroups = new List<ServerGroup>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, i % 2 == 0, metadata);
                serverGroups.Add(new ServerGroup(server.ServerId, i, new List<Server>() { server }));
                indexCountMapping[i] = 0;
                if (server.IsAlive)
                    serverSumCount += i;
            }

            var ping = new PredicatePing(server => int.Parse(server.Metadata["Index"]) % 2 == 0);
            var serverSource = new DefaultDynamicServerSource(serverGroups);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Action action = () =>
            {
                for (int i = 0; i < repeatTimes; i++)
                {
                    for (int j = 0; j < serverSumCount; j++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(requestConfig);
                        int index = int.Parse(requestContext.Server.Metadata["Index"]);
                        indexCountMapping[index].IncrementAndGet();
                        if (repeatInterval > 0)
                            Thread.Sleep(repeatInterval);
                    }
                }
            };

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(action));
            }
            Task.WaitAll(tasks.ToArray());

            foreach (var item in indexCountMapping)
            {
                if (item.Key % 2 == 0)
                    Assert.AreEqual(item.Key * repeatTimes * threadCount, (int)item.Value, item.Key.ToString());
                else
                    Assert.AreEqual(0, (int)item.Value, item.Key.ToString());
            }
        }

        [TestMethod]
        public void WeightedRoundRobinWithSpecificWeightTest()
        {
            int serverCount = 10;
            int repeatTimes = 100;
            int repeatInterval = 0;
            int threadCount = 20;
            int serverSumCount = 0;
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            Dictionary<int, AtomicInteger> indexCountMapping = new Dictionary<int, AtomicInteger>();
            List<ServerGroup> serverGroups = new List<ServerGroup>();
            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, metadata);
                servers.Add(server);
                serverGroups.Add(new ServerGroup(server.ServerId, i, new List<Server>() { server }));
                indexCountMapping[i] = 0;
                serverSumCount += i;
            }
            LoadBalancerRoute route0 = new LoadBalancerRoute("default", serverGroups);

            ServerGroup route1Group0 = new ServerGroup("group0", 2, new List<Server>() { servers[0] });
            ServerGroup route1Group1 = new ServerGroup("group1", 1, new List<Server>() { servers[1] });
            LoadBalancerRoute route1 = new LoadBalancerRoute("route1", new List<ServerGroup>() { route1Group0, route1Group1 });

            ServerGroup route2Group0 = new ServerGroup("group0", 2, new List<Server>() { servers[0] });
            ServerGroup route2Group1 = new ServerGroup("group1", 3, new List<Server>() { servers[1] });
            LoadBalancerRoute route2 = new LoadBalancerRoute("route2", new List<ServerGroup>() { route2Group0, route2Group1 });

            LoadBalancerRequestConfig route1Config = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { new LoadBalancerRouteConfig("route1", 1, false) });
            LoadBalancerRequestConfig route2Config = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { new LoadBalancerRouteConfig("route2", 1, false) });

            int server0Count = route1Group0.Weight + route2Group0.Weight;
            int server1Count = route1Group1.Weight + route2Group1.Weight;
            int route1RequestCount = route1Group0.Weight + route1Group1.Weight;
            int route2RequestCount = route2Group0.Weight + route2Group1.Weight;
       
            var ping = new TruePing();
            var serverSource = new DefaultDynamicServerSource(new List<LoadBalancerRoute>() { route0, route1, route2 });
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Action action = () =>
            {
                for (int i = 0; i < repeatTimes; i++)
                {
                    for (int j = 0; j < serverSumCount; j++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(requestConfig);
                        int index = int.Parse(requestContext.Server.Metadata["Index"]);
                        indexCountMapping[index].IncrementAndGet();
                        if (repeatInterval > 0)
                            Thread.Sleep(repeatInterval);
                    }

                    for (int j = 0; j < route1RequestCount; j++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(route1Config);
                        int index = int.Parse(requestContext.Server.Metadata["Index"]);
                        indexCountMapping[index].IncrementAndGet();
                        if (repeatInterval > 0)
                            Thread.Sleep(repeatInterval);
                    }

                    for (int j = 0; j < route2RequestCount; j++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(route2Config);
                        int index = int.Parse(requestContext.Server.Metadata["Index"]);
                        indexCountMapping[index].IncrementAndGet();
                        if (repeatInterval > 0)
                            Thread.Sleep(repeatInterval);
                    }
                }
            };

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(action));
            }
            Task.WaitAll(tasks.ToArray());

            foreach (var item in indexCountMapping)
            {
                if (item.Key == 0)
                    Assert.AreEqual((item.Key + server0Count) * repeatTimes * threadCount, item.Value.Value, item.Key.ToString());
                else if (item.Key == 1)
                    Assert.AreEqual((item.Key + server1Count) * repeatTimes * threadCount, item.Value.Value, item.Key.ToString());
                else
                    Assert.AreEqual(item.Key * repeatTimes * threadCount, item.Value.Value, item.Key.ToString());
            }
        }
    }
}
