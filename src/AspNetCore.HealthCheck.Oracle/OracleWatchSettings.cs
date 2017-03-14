using System.Collections.Generic;

namespace AspNetCore.HealthCheck.Oracle
{
    public class OracleWatchSettings : WatchSettings
    {
        public OracleWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string connectionString) 
            : base(name, critical, frequency, tags)
        {
            ConnectionString = connectionString;
        }
        
        public string ConnectionString { get; set; }
    }
}