namespace AspNetCore.HealthCheck.Oracle
{
    public class OracleCheckOptions : CheckOptions
    {
        public string ConnectionString { get; set; }
    }
}