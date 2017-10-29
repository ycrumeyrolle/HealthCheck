using System;

namespace AspNetCore.HealthCheck.Oracle
{
    public class OracleCheckSettingsBuilder : HealthCheckSettingsBuilder<OracleCheckSettings>
    {
        private string _connectionString;

        public OracleCheckSettingsBuilder(string name) 
            : base(name)
        {
            Tags.Add("db");
        }

        public OracleCheckSettingsBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override OracleCheckSettings Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string were defined.");
            }

            return new OracleCheckSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}