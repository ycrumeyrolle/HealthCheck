namespace AspNetCore.HealthCheck.SqlServer
{
    public class SqlServerOptions : WatchOptions
    {
        public string ConnectionString { get; set; }
    }
}
