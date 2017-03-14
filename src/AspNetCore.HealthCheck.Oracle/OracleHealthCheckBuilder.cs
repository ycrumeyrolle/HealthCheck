using System;

namespace AspNetCore.HealthCheck.Oracle
{
    public class OracleHealthCheckBuilder : SettingsHealthCheckBuilder<OracleWatchSettings>
    {
        private string _connectionString;

        public OracleHealthCheckBuilder(string name) 
            : base(name)
        {
            Tags.Add("db");
        }

        public OracleHealthCheckBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override OracleWatchSettings Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string were defined.");
            }

            return new OracleWatchSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}