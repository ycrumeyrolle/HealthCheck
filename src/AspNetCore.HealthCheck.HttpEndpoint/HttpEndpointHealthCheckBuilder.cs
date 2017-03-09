using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.HealthCheck.HttpEndpoint
{
    public class HttpEndpointHealthCheckBuilder : SettingsHealthCheckBuilder<HttpEndpointCollectionWatchSettings>
    {
        private readonly IList<HttpRequestSettings> _requests = new List<HttpRequestSettings>();

        private Action<HttpRequestMessage> _onBeforeSend;

        public HttpEndpointHealthCheckBuilder(string name)
            : base(name)
        {
            Tags.Add("http");
        }

        public HttpEndpointHealthCheckBuilder WithUri(string uri, string httpMethod = "GET")
        {
            _requests.Add(new HttpRequestSettings { Uri = new Uri(uri), HttpMethod = new HttpMethod(httpMethod) });
            return this;
        }

        public HttpEndpointHealthCheckBuilder OnBeforeSend(Action<HttpRequestMessage> beforeSendAction)
        {
            _onBeforeSend = beforeSendAction;
            return this;
        }

        public override HttpEndpointCollectionWatchSettings Build()
        {
            if (_requests.Count == 0)
            {
                throw new InvalidOperationException("No URI were defined.");
            }
            
            return new HttpEndpointCollectionWatchSettings(Name, Critical, Frequency, Tags, _requests, _onBeforeSend);
        }
    }
}