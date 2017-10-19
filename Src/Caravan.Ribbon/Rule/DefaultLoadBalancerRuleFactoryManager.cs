using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Com.Ctrip.Soa.Caravan.Logging;
using Com.Ctrip.Soa.Caravan.Ribbon.LoadBalancer;
using System.Collections.Concurrent;
using Com.Ctrip.Soa.Caravan.Configuration;
using Com.Ctrip.Soa.Caravan.Utility;

namespace Com.Ctrip.Soa.Caravan.Ribbon.Rule
{
    class DefaultLoadBalancerRuleFactoryManager : ILoadBalancerRuleFactoryManager
    {
        private static ILog _logger = LogManager.GetLogger(typeof(DefaultLoadBalancerRuleFactoryManager));

        public const string RoundRobinRuleName = "round-robin-rule";

        public const string RoundRobinRuleDescription = "简单轮巡算法";

        public const string WeightedRoundRobinRuleName = "weighted-round-robin-rule";

        public const string WeightedRoundRobinRuleDescription = "带权重的轮巡算法";

        private ILoadBalancerContext _loadBalancerContext;
        private ConcurrentDictionary<string, Func<ILoadBalancerRule>> _loadBalanceRuleFactoryMap;
        private IProperty _ruleProperty;
        private String _ruleId;
        private Func<ILoadBalancerRule> _ruleFactory;
        private ConcurrentDictionary<string, ILoadBalancerRule> _loadBalancerRules;
        
        public DefaultLoadBalancerRuleFactoryManager(ILoadBalancerContext loadBalancerContext) 
        {
            _loadBalancerContext = loadBalancerContext;
            _loadBalanceRuleFactoryMap = new ConcurrentDictionary<string, Func<ILoadBalancerRule>>();
            _loadBalanceRuleFactoryMap[RoundRobinRuleName] = () => new RoundRobinRule();
            _loadBalanceRuleFactoryMap[WeightedRoundRobinRuleName] = () => new WeightedRoundRobinRule();

            _ruleId = WeightedRoundRobinRuleName;
            _ruleFactory = _loadBalanceRuleFactoryMap[WeightedRoundRobinRuleName];
            _loadBalancerRules = new ConcurrentDictionary<string, ILoadBalancerRule>();
        
            InitializeRuleProperty();
        }
    
        private void InitializeRuleProperty() 
        {
            string rulePropertyKey = string.Format("{0}.{1}", _loadBalancerContext.LoadBalancerKey, ConfigurationKeys.LoadBalancerRuleName);
            _ruleProperty = _loadBalancerContext.ConfigurationManager.GetProperty(rulePropertyKey, WeightedRoundRobinRuleName);
            _ruleProperty.OnChange += (o, e) => SetLoadBalanceByRuleId(e.NewValue);

            SetLoadBalanceByRuleId(_ruleProperty.Value);
        }

        private void SetLoadBalanceByRuleId(string newRuleId) 
        {
            if (string.IsNullOrWhiteSpace(newRuleId)) 
            {
                _logger.Warn(string.Format("Invalid rule id {0}", newRuleId), _loadBalancerContext.AdditionalInfo);
                return;
            }

            if (_ruleFactory != null && newRuleId == _ruleId) 
            {
                _logger.Info(string.Format("Not modified: {0}", newRuleId), _loadBalancerContext.AdditionalInfo);
                return;
            }

            Func<ILoadBalancerRule> newRuleFactory;
            if (!_loadBalanceRuleFactoryMap.TryGetValue(newRuleId, out newRuleFactory))
            {
                _logger.Warn(string.Format("Invalid rule id {0}", newRuleId), _loadBalancerContext.AdditionalInfo);
                return;
            }

            string message = string.Format("LoadBalancerRule changed from {0} to {1}", _ruleId, newRuleId);
            _logger.Info(message, _loadBalancerContext.AdditionalInfo);
            _ruleId = newRuleId;
            _ruleFactory = newRuleFactory;
            _loadBalancerRules = new ConcurrentDictionary<string, ILoadBalancerRule>();
        }

        public ILoadBalancerRule GetLoadBalancerRule(string ruleId)
        {
            return _loadBalancerRules.GetOrAdd(ruleId, ruleIdKey => GetLoadBalancerRuleFactory()());
        }

        public Func<ILoadBalancerRule> GetLoadBalancerRuleFactory()
        {
            return _ruleFactory;
        }

        public void RegisterLoadBalancerRuleFactory(Func<ILoadBalancerRule> ruleFactory)
        {
            ParameterChecker.NotNull(ruleFactory, "ruleFactory");

            ILoadBalancerRule rule = ruleFactory();
            ParameterChecker.NotNullOrWhiteSpace(rule.Id, "ruleId");

            if (_loadBalanceRuleFactoryMap.ContainsKey(rule.Id))
                return;
            _loadBalanceRuleFactoryMap[rule.Id] = ruleFactory;
            SetLoadBalanceByRuleId(rule.Id);
        }
    }
}
