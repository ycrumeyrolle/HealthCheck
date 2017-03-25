namespace AspNetCore.HealthCheck.SqlServer
{
    public class SqlServerCheckOptions : CheckOptions
    {
        public string ConnectionString { get; set; }
    }
}
