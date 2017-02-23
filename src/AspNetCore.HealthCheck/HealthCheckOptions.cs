using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public class HealthCheckOptions
    {
        public PathString Path { get; set; }

        public bool SendResults { get; set; }

        /// <summary>
        /// Gets or set the indication the health will be always checked, even when the server is in maintenance. 
        /// </summary>
        public bool CheckHealthEvenDisabled { get; set; }
    }
}