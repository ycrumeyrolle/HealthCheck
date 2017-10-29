using AspNetCore.HealthCheck;

namespace Microsoft.AspNetCore.Builder
{
    public class CanaryOptions : HealthOptionsBase
    {
        public string PolicyName { get; set; } = Constants.DefaultPolicy;

        public bool EnableHealthCheck { get; set; } = true;
    }
}