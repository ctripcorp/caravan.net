using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using System.Threading;
using System.Threading.Tasks;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using System.Reflection;
using Com.Ctrip.Soa.Caravan.Threading.Atomic;
using System.Collections.Concurrent;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [TestClass]
    public class LoadBalancerTest : TestBase
    {
        [TestMethod]
        public void RoundRobinRuleTest()
        {
            int serverCount = 10;
            int repeatTimes = 5;
            int threadCount = 1;
            int availableCount = 0;
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            var urlCountMap = new ConcurrentDictionary<Server, AtomicInteger>();
            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, metadata);
                servers.Add(server);
                urlCountMap[server] = 0;
                if (server.IsAlive)
                    availableCount++;
            }

            var routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            var routeConfigs = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var ping = new TruePing();
            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);

            Action action = () =>
            {
                for (int i = 0; i < availableCount * repeatTimes; i++)
                {
                    urlCountMap[loadBalancer.GetRequestContext(routeConfigs).Server].IncrementAndGet();
                }
            };

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(action));
            }
            Task.WaitAll(tasks.ToArray());


            foreach (var item in urlCountMap)
            {
                int index = int.Parse(item.Key.Metadata["Index"]);
                if (ping.IsAlive(item.Key))
                    Assert.AreEqual(repeatTimes * threadCount, (int)item.Value);
                else
                    Assert.AreEqual(0, (int)item.Value);
            }
        }

        [TestMethod]
        public void RoundRobinRuleWithUnavailableServerTest()
        {
            int serverCount = 10;
            int repeatTimes = 5;
            int threadCount = 1;
            int availableCount = 0;
            string loadBalancerId = MethodInfo.GetCurrentMethod().Name;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            var urlCountMap = new ConcurrentDictionary<Server, AtomicInteger>();
            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, i % 2 == 0, metadata);
                servers.Add(server);
                urlCountMap[server] = 0;
                if (server.IsAlive)
                    availableCount++;
            }

            var routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            var routeConfigs = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var ping = new PredicatePing(server => int.Parse(server.Metadata["Index"]) % 2 == 0);
            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);

            Action action = () =>
            {
                for (int i = 0; i < availableCount * repeatTimes; i++)
                {
                    urlCountMap[loadBalancer.GetRequestContext(routeConfigs).Server].IncrementAndGet();
                }
            };

            List<Task> tasks = new List<Task>();
            for (int i = 0; i < threadCount; i++)
            {
                tasks.Add(Task.Factory.StartNew(action));
            }
            Task.WaitAll(tasks.ToArray());


            foreach (var item in urlCountMap)
            {
                int index = int.Parse(item.Key.Metadata["Index"]);
                if (ping.IsAlive(item.Key))
                    Assert.AreEqual(repeatTimes * threadCount, (int)item.Value);
                else
                    Assert.AreEqual(0, (int)item.Value);
            }
        }
    }
}
