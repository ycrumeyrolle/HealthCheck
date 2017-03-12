using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.HealthCheck.HttpEndpoint
{
    public class HttpEndpointWatchSettings : WatchSettings
    {
        public HttpEndpointWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, HttpRequestSettings request, Action<HttpRequestMessage> beforeSend = null)
            : base(name, critical, frequency, tags)
        {
            Request = request;
            BeforeSend = beforeSend;
        }

        public Action<HttpRequestMessage> BeforeSend { get; }

        public HttpRequestSettings Request { get; }

        public HttpMessageHandler HttpHandler { get; set; }
    }
}