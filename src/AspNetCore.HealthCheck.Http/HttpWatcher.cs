using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Http
{
    public class HttpWatcher : HealthWatcher<HttpWatchSettings>
    {
        public async override Task CheckHealthAsync(HealthContext context, HttpWatchSettings settings)
        {
            var requestSettings = settings.Request;
            var entry = await Dns.GetHostEntryAsync(requestSettings.Uri.DnsSafeHost);

            var properties = new Dictionary<string, object> { { "dns_resolve", context.ElapsedMilliseconds } };
            
            using (HttpClient client = new HttpClient(settings.HttpHandler ?? new HttpClientHandler()))
            {
                using (var request = new HttpRequestMessage(requestSettings.HttpMethod, requestSettings.Uri))
                {
                    request.Headers.Add("cache-control", "no-cache");
                    settings.BeforeSend?.Invoke(request);

                    using (var response = await client.SendAsync(request, context.CancellationToken))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            context.Succeed(properties: properties);
                        }
                        else
                        {
                            context.Fail(response.ReasonPhrase, properties: properties);
                        }
                    }
                }
            }
        }
    }
}