using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Collections.Concurrent.CircularBuffer.TimeBucket.Buffer;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.ValueParser;

namespace Com.Ctrip.Soa.Caravan.Ribbon
{
    class ServerStats
    {
        private static ILog _logger = LogManager.GetLogger(typeof(ServerStats));

        private const string BufferTimeWindowConfigKey = "buffer-time-window";
        private const string BucketTimeWindowConfigKey = "bucket-time-window";

        private const int BufferTimeWindowMinValue = 2000;
        private const int BufferTimeWindowMaxValue = 20000;
        private const int BucketTimeWindowMinValue = 200;
        private const int BucketTimeWindowMaxValue = 2000;

        private const int BufferTimeWindowDefaultValue = 2000;
        private const int BucketTimeWindowDefaultValue = 200;

        private static readonly TimeBufferConfig DefaultConfig = new TimeBufferConfig(BufferTimeWindowDefaultValue, BucketTimeWindowDefaultValue);
        private static readonly TimeBufferConfigParser ConfigParser = new TimeBufferConfigParser();
        private static readonly TimeBufferConfigCorrector ConfigCorrector = new TimeBufferConfigCorrector();

        private volatile CounterBuffer<bool> _counterBuffer;
        private volatile TimeBufferConfig _timeBufferConfig;
        
        private ILoadBalancerContext _loadBalancerContext;
        private IProperty<TimeBufferConfig> _globalCounterBufferConfigProperty;
        private IProperty<TimeBufferConfig> _counterBufferConfigProperty;
        
        public ServerStats(ILoadBalancerContext loadBalancerContext)
        {
            _loadBalancerContext = loadBalancerContext;

            string prefix = loadBalancerContext.LoadBalancerKey;
            _counterBufferConfigProperty = loadBalancerContext.ConfigurationManager.GetProperty(prefix + "." + ConfigurationKeys.CounterBuffer, null, ConfigParser, ConfigCorrector);

            string globalPrefix = loadBalancerContext.ManagerId;
            _globalCounterBufferConfigProperty = loadBalancerContext.ConfigurationManager.GetProperty(globalPrefix + "." + ConfigurationKeys.CounterBuffer, DefaultConfig, ConfigParser, ConfigCorrector);

            _counterBufferConfigProperty.OnChange += OnChange;
            _globalCounterBufferConfigProperty.OnChange += OnChange;

            Reset();
        }

        public void AddAvailableCount()
        {
            _counterBuffer.IncreaseCount(true);
        }

        public void AddUnavailableCount()
        {
            _counterBuffer.IncreaseCount(false);
        }

        public int GetAvailableCount()
        {
            return _counterBuffer.GetCount(true);
        }

        public int GetUnavailableCount()
        {
            return _counterBuffer.GetCount(false);
        }

        private void Reset()
        {
            if (_timeBufferConfig == null)
            {
                _timeBufferConfig = GetCounterBufferConfig();
                _counterBuffer = new CounterBuffer<bool>(_timeBufferConfig.BufferTimeWindow, _timeBufferConfig.BufferTimeWindow / _timeBufferConfig.BucketTimeWindow);
                return;
            }

            TimeBufferConfig newConfig = GetCounterBufferConfig();
            if (_timeBufferConfig.Equals(newConfig))
                return;

            var message = string.Format("Count buffer reset due to config change, new config: {0}", newConfig);
            _logger.Info(message, _loadBalancerContext.AdditionalInfo);
            _timeBufferConfig = newConfig;
            _counterBuffer = new CounterBuffer<bool>(_timeBufferConfig.BufferTimeWindow, _timeBufferConfig.BufferTimeWindow / _timeBufferConfig.BucketTimeWindow);
        }

        private TimeBufferConfig GetCounterBufferConfig()
        {
            return _counterBufferConfigProperty.Value ?? _globalCounterBufferConfigProperty.Value;
        }

        private void OnChange(object sender, PropertyChangedEventArgs<TimeBufferConfig> e)
        {
            Reset();
        }

        class TimeBufferConfig : IEquatable<TimeBufferConfig>
        {
            public int BufferTimeWindow { get; set; }

            public int BucketTimeWindow { get; set; }

            public TimeBufferConfig(int bufferTimeWindow, int bucketTimeWindow)
            {
                this.BufferTimeWindow = bufferTimeWindow;
                this.BucketTimeWindow = bucketTimeWindow;
            }

            public bool Equals(TimeBufferConfig other)
            {
                return other != null && other.BufferTimeWindow == BufferTimeWindow && other.BucketTimeWindow == BucketTimeWindow;
            }

            public override string ToString()
            {
                return string.Format(@"{{""BufferTimeWindow"":{0}, ""BucketTimeWindow"":{1}}}", BufferTimeWindow, BucketTimeWindow);
            }
        }

        class TimeBufferConfigParser : IValueParser<TimeBufferConfig>
        {
            public DictionaryParser<string, int> _dictionaryParser = new DictionaryParser<string, int>(StringParser.Instance, IntParser.Instance);

            public TimeBufferConfig Parse(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return null;

                Dictionary<string, int> result;
                if (!_dictionaryParser.TryParse(input, out result))
                    return null;

                int bufferTimeWindow, bucketTimeWindow;
                if (result.Count != 2)
                    return null;
                if(!result.TryGetValue(BufferTimeWindowConfigKey, out bufferTimeWindow))
                    return null;
                if (!result.TryGetValue(BucketTimeWindowConfigKey, out bucketTimeWindow))
                    return null;

                return new TimeBufferConfig(bufferTimeWindow, bucketTimeWindow);
            }

            public bool TryParse(string input, out TimeBufferConfig result)
            {
                result = Parse(input);
                return result != null;
            }
        }

        class TimeBufferConfigCorrector : IValueCorrector<TimeBufferConfig>
        {
            public TimeBufferConfig Correct(TimeBufferConfig value)
            {
                if (value == null)
                    return null;

                if (value.BucketTimeWindow == 0 || value.BufferTimeWindow % value.BucketTimeWindow != 0)
                    return null;

                if (value.BufferTimeWindow < BufferTimeWindowMinValue || value.BufferTimeWindow > BufferTimeWindowMaxValue)
                    return null;

                if (value.BucketTimeWindow < BucketTimeWindowMinValue || value.BucketTimeWindow > BucketTimeWindowMaxValue)
                    return null;

                return value;
            }
        }
    }
}
