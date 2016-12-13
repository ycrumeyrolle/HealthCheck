using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.HttpEndpoint
{
    public class HttpEndpointWatcher : HealthWatcher<HttpEndpointWatchSettings>
    {
        public async override Task CheckHealthAsync(HealthContext context, HttpEndpointWatchSettings settings)
        {
            var requestSettings = settings.Request;
            Stopwatch dnsStopwatch = Stopwatch.StartNew();
            var entry = await Dns.GetHostEntryAsync(requestSettings.Uri.DnsSafeHost);
            dnsStopwatch.Stop();

            var properties = new Dictionary<string, object> { { "dns_resolve", dnsStopwatch.ElapsedMilliseconds } };
            using (HttpClient client = new HttpClient())
            {
                using (var request = new HttpRequestMessage(requestSettings.HttpMethod, requestSettings.Uri))
                {
                    request.Headers.Add("cache-control", "no-cache");
                    settings.BeforeSend?.Invoke(request);

                    using (var response = await client.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            context.Succeed(properties: properties);
                        }
                        else
                        {
                            context.Fail(response.ReasonPhrase, properties);
                        }
                    }
                }
            }
        }
    }
}