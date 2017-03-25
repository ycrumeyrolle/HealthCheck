using System;
using System.Collections.Generic;
using System.Net.Http;

namespace AspNetCore.HealthCheck.Http
{
    public class HttpCheckSettingsBuilder : HealthCheckSettingsBuilder<HttpCollectionCheckSettings>
    {
        private readonly IList<HttpRequestSettings> _requests = new List<HttpRequestSettings>();

        private Action<HttpRequestMessage> _onBeforeSend;

        public HttpCheckSettingsBuilder(string name)
            : base(name)
        {
            Tags.Add("http");
        }

        public HttpCheckSettingsBuilder WithUri(string uri, string httpMethod = "GET")
        {
            _requests.Add(new HttpRequestSettings { Uri = new Uri(uri), HttpMethod = new HttpMethod(httpMethod) });
            return this;
        }

        public HttpCheckSettingsBuilder OnBeforeSend(Action<HttpRequestMessage> beforeSendAction)
        {
            _onBeforeSend = beforeSendAction;
            return this;
        }

        public override HttpCollectionCheckSettings Build()
        {
            if (_requests.Count == 0)
            {
                throw new InvalidOperationException("No URI were defined.");
            }
            
            return new HttpCollectionCheckSettings(Name, Critical, Frequency, Tags, _requests, _onBeforeSend);
        }
    }
}