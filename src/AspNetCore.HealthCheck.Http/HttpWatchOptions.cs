using System.Collections.Generic;

namespace AspNetCore.HealthCheck.Http
{
    public class HttpWatchOptions : WatchOptions
    {
        public HttpRequestSettings Request { get; }
    }
}