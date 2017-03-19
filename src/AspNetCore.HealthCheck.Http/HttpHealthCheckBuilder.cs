using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.HealthCheck.Http
{
    public class HttpHealthCheckBuilder : SettingsHealthCheckBuilder<HttpCollectionWatchSettings>
    {
        private readonly IList<HttpRequestSettings> _requests = new List<HttpRequestSettings>();

        private Action<HttpRequestMessage> _onBeforeSend;

        public HttpHealthCheckBuilder(string name)
            : base(name)
        {
            Tags.Add("http");
        }

        public HttpHealthCheckBuilder WithUri(string uri, string httpMethod = "GET")
        {
            _requests.Add(new HttpRequestSettings { Uri = new Uri(uri), HttpMethod = new HttpMethod(httpMethod) });
            return this;
        }

        public HttpHealthCheckBuilder OnBeforeSend(Action<HttpRequestMessage> beforeSendAction)
        {
            _onBeforeSend = beforeSendAction;
            return this;
        }

        public override HttpCollectionWatchSettings Build()
        {
            if (_requests.Count == 0)
            {
                throw new InvalidOperationException("No URI were defined.");
            }
            
            return new HttpCollectionWatchSettings(Name, Critical, Frequency, Tags, _requests, _onBeforeSend);
        }
    }
}