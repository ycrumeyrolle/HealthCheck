using System.Collections.Generic;

namespace AspNetCore.HealthCheck.HttpEndpoint
{
    public class HttpEndpointWatchOptions : WatchOptions
    {
        public HttpRequestSettings Request { get; }
    }
}