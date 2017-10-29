namespace AspNetCore.HealthCheck.Http
{
    public class HttpCheckOptions : CheckOptions
    {
        public HttpRequestSettings Request { get; }
    }
}