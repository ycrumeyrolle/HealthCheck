using System;
using System.Net.Http;

namespace AspNetCore.HealthCheck.Http
{
    public class HttpRequestSettings
    {
        public Uri Uri { get; set; }

        public HttpMethod HttpMethod { get; set; }
    }
}