namespace AspNetCore.HealthCheck.SqlServerDb
{
    public class SqlServerDbOptions : WatchOptions
    {
        public string ConnectionString { get; set; }
    }
}
