namespace Microsoft.AspNetCore.Builder
{
    public class HealthCheckOptions : HealthOptionsBase
    {
        public bool SendResults { get; set; } = true;
    }
}