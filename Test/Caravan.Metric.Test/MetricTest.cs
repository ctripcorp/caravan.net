using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Com.Ctrip.Soa.Caravan.Metric;
using Com.Ctrip.Soa.Caravan.Metric.Null;
using System.Threading;
using System.Threading.Tasks;

namespace Caravan.Metric
{
    [TestClass]
    public class MetricTest
    {
        [Ignore]
        [TestMethod]
        public void CLogMetricTest()
        {
            string auditMetricName = "Caravan.Metric.Test.CLogMetricTest";
            IAuditMetricManager manager = NullAuditMetricManager.Instance;
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            metadata["metric_name_audit"] = auditMetricName.ToLower();
            IAuditMetric auditMetric = manager.GetMetric(auditMetricName.ToLower(), new MetricConfig(metadata));
            bool isCompleted = false;
            var task = Task.Factory.StartNew(() =>
            {
                Random random = new Random();
                while (!isCompleted)
                {
                    auditMetric.AddValue(random.Next(100, 200));
                    Thread.Sleep(10);
                }
            });
            Thread.Sleep(1000 * 60 * 60);
            isCompleted = true;
            task.Wait();
        }
    }
}
