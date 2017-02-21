namespace AspNetCore.HealthCheck.OracleDb
{
    public class OracleDbOptions : WatchOptions
    {
        public string ConnectionString { get; set; }
    }
}