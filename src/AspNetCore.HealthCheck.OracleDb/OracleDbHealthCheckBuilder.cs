using System;

namespace AspNetCore.HealthCheck.OracleDb
{
    public class OracleDbHealthCheckBuilder : SettingsHealthCheckBuilder<OracleDbWatchSettings>
    {
        private string _connectionString;

        public OracleDbHealthCheckBuilder(string name) 
            : base(name)
        {
            Tags.Add("db");
        }

        public OracleDbHealthCheckBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override OracleDbWatchSettings Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string were defined.");
            }

            return new OracleDbWatchSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}