using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Com.Ctrip.Soa.Caravan.Logging;

namespace Com.Ctrip.Soa.Caravan.Utility
{
    /// <summary>
    /// Host 类
    /// </summary>
    public static class HostUtility
    {
        private static ILog _logger = LogManager.GetLogger(typeof(HostUtility));

        /// <summary>
        /// 获取当前机器的 IPv4 地址
        /// </summary>
        public static string IPv4 { get; private set; }
        
        /// <summary>
        /// 获取当前机器的名称
        /// </summary>
        public static string Name { get; private set; }

        static HostUtility() 
        {
            Name = GetHostName();
            IPv4 = GetIPAddressFromNetworkInterface() ?? GetIPAddressFromDns();
        }

        private static bool NetworkInterfaceHasKeyword(NetworkInterface networkInterface, string keyword)
        {
            return ContainsIgnoreCase(networkInterface.Name, keyword) || ContainsIgnoreCase(networkInterface.Description, keyword);
        }

        private static bool ContainsIgnoreCase(string source, string target)
        {
            return source.IndexOf(target, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private static string GetIPAddressFromNetworkInterface()
        {
            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                var weightToAddressMap = new Dictionary<int, IPAddress>();

                foreach (var ni in networkInterfaces)
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                        continue;
                    if (ni.OperationalStatus != OperationalStatus.Up)
                        continue;

                    int weight = 256;
                    if (NetworkInterfaceHasKeyword(ni, "Virtual"))
                        weight -= 128;

                    if (NetworkInterfaceHasKeyword(ni, "本地连接") || NetworkInterfaceHasKeyword(ni, "Local Area Connection"))
                        weight += 64;

                    if (NetworkInterfaceHasKeyword(ni, "Loopback"))
                        weight -= 32;

                    if (!weightToAddressMap.ContainsKey(weight))
                    {
                        var ipProperties = ni.GetIPProperties();
                        foreach (var unicastAddress in ipProperties.UnicastAddresses)
                        {
                            if (unicastAddress.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                weightToAddressMap.Add(weight, unicastAddress.Address);
                                break;
                            }
                        }
                    }
                }

                if (weightToAddressMap.Count == 0)
                    return null;
                var address = weightToAddressMap.OrderByDescending(item => item.Key).FirstOrDefault();
                return address.Value.ToString();
            }
            catch (Exception ex)
            {
                _logger.Warn("GetIPAddressFromNetworkInterface Failed.", ex);
                return null;
            }
        }

        private static string GetIPAddressFromDns()
        {
            try
            {
                return Dns.GetHostAddresses(Name)
                    .Where(c => c.AddressFamily == AddressFamily.InterNetwork)
                    .Select(c => c.ToString()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Warn("GetIPAddressFromDns Failed.", ex);
                return null;
            }
        }

        private static string GetHostName()
        {
            try
            {
                var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                return properties.HostName;
            }
            catch (Exception ex)
            {
                _logger.Warn("GetHostName Failed.", ex);
                return null; 
            }
        }
    }
}
