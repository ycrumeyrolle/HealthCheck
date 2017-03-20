using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public abstract class HealthOptionsBase
    {
        public PathString Path { get; set; }

        public AuthorizationPolicy AuthorizationPolicy { get; set; }
    }
}