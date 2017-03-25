using System;

namespace AspNetCore.HealthCheck.SqlServer
{
    public class SqlServerCheckSettingsBuilder : HealthCheckSettingsBuilder<SqlServerCheckSettings>
    {
        private string _connectionString;

        public SqlServerCheckSettingsBuilder(string name)
            : base(name)
        {
            Tags.Add("db");
        }

        public SqlServerCheckSettingsBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override SqlServerCheckSettings Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string were defined.");
            }

            return new SqlServerCheckSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}