using AspNetCore.HealthCheck;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public class CanaryOptions
    {
        public PathString Path { get; set; }

        public string PolicyName { get; set; } = Constants.DefaultPolicy;

        public bool EnableHealthCheck { get; set; } = true;
    }
}