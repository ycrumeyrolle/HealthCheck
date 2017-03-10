using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.HealthCheck
{
    public class DefaultHealthCheckPolicyProvider : IHealthCheckPolicyProvider
    {
        private readonly Dictionary<string, HealthCheckPolicy> _policies;

        public DefaultHealthCheckPolicyProvider(HealthCheckPolicy defaultPolicy)
        {
            if (defaultPolicy == null)
            {
                throw new ArgumentNullException(nameof(defaultPolicy));
            }

            DefaultPolicy = defaultPolicy;
            _policies = CreatePolicies(defaultPolicy);
        }

        public HealthCheckPolicy DefaultPolicy { get; }

        public HealthCheckPolicy GetPolicy(string policyName)
        {
            if (string.IsNullOrEmpty(policyName))
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            if (string.Equals(policyName, Constants.DefaultPolicy, StringComparison.Ordinal))
            {
                return DefaultPolicy;
            }

            HealthCheckPolicy policy;
            if (_policies.TryGetValue(policyName, out policy))
            {
                return policy;
            }

            return null;
        }

        private Dictionary<string, HealthCheckPolicy> CreatePolicies(HealthCheckPolicy policy)
        {
            var policies = policy.WatchSettings
                .SelectMany(s => s.Value.Tags, (s, t) => new { Tag = t, Settings = s })
                .GroupBy(item => item.Tag)
                .ToDictionary(
                    group => group.Key,
                    group => group.Aggregate(new SettingsCollection(), (settings, item) =>
                    {
                        settings.Add(item.Settings);
                        return settings;
                    },
                    settings => new HealthCheckPolicy(settings)));
            return policies;
        }
    }
}