using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using System.Threading;
using System.Threading.Tasks;
using Com.Ctrip.Soa.Caravan.Ribbon.ServerSource;
using System.Reflection;
using System.Configuration;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    [TestClass]
    public class DefaultServerSourceMonitorTest : TestBase
    {
        [TestMethod]
        public void PingFailureServerToSuccess_MultiServer_Test()
        {
            PingFailureServerToSuccessTest(MethodInfo.GetCurrentMethod().Name, 10);
        }

        [TestMethod]
        public void PingFailureServerToSuccess_SingleServer_Test()
        {
            PingFailureServerToSuccessTest(MethodInfo.GetCurrentMethod().Name, 1);
        }

        private void PingFailureServerToSuccessTest(string loadBalancerId, int serverCount)
        {
            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var server = new Server("Server_" + i, null);
                servers.Add(server);
            }

            var routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            var routeConfigs = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new TruePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;
            foreach (var serverContext in defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(routeConfigs).Servers)
                serverContext.IsAlive = false;
            defaultLoadBalancer.ServerSourceFilter.Refresh();

            Thread.Sleep(3000);
            foreach (var serverContext in defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(routeConfigs).Servers)
                Assert.IsTrue(serverContext.IsAlive);
        }

        [TestMethod]
        public void HighFailurePercentageTest()
        {
            HighFailurePercentageAndPingFailure(MethodInfo.GetCurrentMethod().Name, null);
        }

        [TestMethod]
        public void HighFailurePercentageAndPingFailureTest()
        {
            HighFailurePercentageAndPingFailure(MethodInfo.GetCurrentMethod().Name, new FalsePing());
        }

        private void HighFailurePercentageAndPingFailure(string loadBalancerId, IPing ping = null)
        {
            int serverCount = 10;
            int repeatTimes = 10;
            int repeatInterval = 100;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, metadata);
                servers.Add(server);
            }

            var routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            var routeConfigs = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(ping, serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var defaultLoadBalancer = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    for (int i = 0; i < repeatTimes; i++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(routeConfigs);
                        int index = int.Parse(requestContext.Server.Metadata["Index"]);
                        if (index % 2 == 0)
                            requestContext.MarkServerAvailable();
                        else
                            requestContext.MarkServerUnavailable();
                    }
                    Thread.Sleep(repeatInterval);
                }
            });

            Thread.Sleep(serverCount * repeatTimes * repeatInterval);

            foreach (var serverContext in defaultLoadBalancer.ServerSourceFilter.GetLoadBalancerRoute(routeConfigs).Servers)
            {
                int index = int.Parse(serverContext.Metadata["Index"]);
                if (index % 2 == 0)
                    Assert.IsTrue(serverContext.IsAlive, serverContext.ToString());
                else
                    Assert.IsFalse(serverContext.IsAlive, serverContext.ToString());
            }
        }

        [TestMethod]
        public void RetainMinAvailableServerCount_MultiServer_Test()
        {
            RetainLastServerTest(MethodInfo.GetCurrentMethod().Name, 10);
        }

        [TestMethod]
        public void RetainMinAvailableServerCount_MinAvailableServerCount_Test()
        {
            int minAvailableServerCount = int.Parse(ConfigurationManager.AppSettings["soa." + MethodInfo.GetCurrentMethod().Name + ".ribbon.min-available-server-count"]);
            RetainLastServerTest(MethodInfo.GetCurrentMethod().Name, minAvailableServerCount);
        }

        private void RetainLastServerTest(string loadBalancerId, int serverCount)
        {
            int repeatTimes = 10;
            int repeatInterval = 100;

            IConfiguration configuration = ObjectFactory.CreateAppSettingConfiguration();
            var configurationSource = ObjectFactory.CreateDefaultConfigurationSource(0, "App.config", configuration);
            var configurationManager = ObjectFactory.CreateDefaultConfigurationManager(configurationSource);

            List<Server> servers = new List<Server>();
            for (int i = 0; i < serverCount; i++)
            {
                var metadata = new Dictionary<string, string>();
                metadata["Index"] = i.ToString();
                var server = new Server("Server_" + i, metadata);
                servers.Add(server);
            }

            var routeConfig = new LoadBalancerRouteConfig("default", 1, false);
            var requestConfig = new LoadBalancerRequestConfig(new List<LoadBalancerRouteConfig>() { routeConfig });

            var serverSource = new DefaultDynamicServerSource(servers);
            var factory = LoadBalancerManager.GetManager("soa", new LoadBalancerManagerConfig(configurationManager));
            var loadBalancerConfig = new LoadBalancerConfig(new FalsePing(), serverSource);
            var loadBalancer = factory.GetLoadBalancer(loadBalancerId, loadBalancerConfig);
            var loadBalancerContext = ((DefaultLoadBalancer)loadBalancer).LoadBalancerContext;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    for (int i = 0; i < repeatTimes; i++)
                    {
                        var requestContext = loadBalancer.GetRequestContext(requestConfig);
                        requestContext.MarkServerUnavailable();
                    }
                    Thread.Sleep(repeatInterval);
                }
            });

            Thread.Sleep(serverCount * repeatTimes * repeatInterval);

            Console.WriteLine("MinAvailableServerCount: " + loadBalancerContext.MinAvailableServerCount);
            Assert.AreEqual(serverCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).Servers.Length);
            Assert.AreEqual(loadBalancerContext.MinAvailableServerCount, loadBalancerContext.ServerSourceFilter.GetLoadBalancerRoute(requestConfig).AvailableServers.Length);
        }
    }
}
