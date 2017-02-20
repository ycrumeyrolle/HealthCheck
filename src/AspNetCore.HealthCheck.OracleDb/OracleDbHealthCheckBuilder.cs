using System;

namespace AspNetCore.HealthCheck.OracleDb
{
    public class OracleDbHealthCheckBuilder : SettingsHealthCheckBuilder<OracleDbWatchSettings>
    {
        private string _connectionString;

        public OracleDbHealthCheckBuilder(string name) 
            : base(name)
        {
        }

        public OracleDbHealthCheckBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override OracleDbWatchSettings Build()
        {
            return new OracleDbWatchSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}