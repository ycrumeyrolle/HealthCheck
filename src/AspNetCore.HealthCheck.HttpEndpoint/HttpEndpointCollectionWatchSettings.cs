using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.HealthCheck.HttpEndpoint
{
    public class HttpEndpointCollectionWatchSettings : WatchSettings
    {
        public HttpEndpointCollectionWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, IList<HttpRequestSettings> requests, Action<HttpRequestMessage> beforeSend = null)
            : base(name, critical, frequency, tags)
        {
            Requests = requests;
            BeforeSend = beforeSend;
        }

        public Action<HttpRequestMessage> BeforeSend { get; }

        public IList<HttpRequestSettings> Requests { get; }
    }
}