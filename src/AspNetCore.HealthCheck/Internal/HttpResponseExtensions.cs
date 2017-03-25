using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AspNetCore.HealthCheck
{
    public static class HttpResponseExtensions
    {
        public static void WriteRetryAfterHeader(this HttpResponse response, DateTimeOffset? retryAfter)
        {
            if (retryAfter.HasValue)
            {
                response.Headers[HeaderNames.RetryAfter] = retryAfter.Value.ToString("r");
            }
        }

        public static void WriteRetryAfterHeader(this HttpResponse response, int? retryAfter)
        {
            if (retryAfter.HasValue)
            {
                response.Headers[HeaderNames.RetryAfter] = retryAfter.Value.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
