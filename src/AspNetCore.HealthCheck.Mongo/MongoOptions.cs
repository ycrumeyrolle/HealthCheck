namespace AspNetCore.HealthCheck.Mongo
{
    public class MongoOptions : CheckOptions
    {
        public string ConnectionString { get; set; }
    }
}