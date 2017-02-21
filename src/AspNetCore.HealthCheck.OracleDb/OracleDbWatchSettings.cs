using System.Collections.Generic;

namespace AspNetCore.HealthCheck.OracleDb
{
    public class OracleDbWatchSettings : WatchSettings
    {
        public OracleDbWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string connectionString) 
            : base(name, critical, frequency, tags)
        {
            ConnectionString = connectionString;
        }
        
        public string ConnectionString { get; set; }
    }
}