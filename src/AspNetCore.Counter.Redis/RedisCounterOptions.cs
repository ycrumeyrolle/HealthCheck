namespace Microsoft.Extensions.DependencyInjection
{
    public class RedisCounterOptions
    {
        /// <summary>
        /// The configuration used to connect to Redis.
        /// </summary>
        public string Configuration { get; set; }
    }
}