namespace AspNetCore.HealthCheck
{
    public class RedisCheckOptions : CheckOptions 
    {
        public string Instance { get; set; }
    }
}
