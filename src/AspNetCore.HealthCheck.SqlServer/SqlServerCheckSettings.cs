using System.Collections.Generic;

namespace AspNetCore.HealthCheck.SqlServer
{
    public class SqlServerCheckSettings : HealthCheckSettings
    {
        public SqlServerCheckSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string connectionString)
            : base(name, critical, frequency, tags)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}