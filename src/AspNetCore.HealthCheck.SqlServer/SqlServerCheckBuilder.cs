using System;

namespace AspNetCore.HealthCheck.SqlServer
{
    public class SqlServerCheckBuilder : SettingsHealthCheckBuilder<SqlServerSettings>
    {
        private string _connectionString;

        public SqlServerCheckBuilder(string name)
            : base(name)
        {
            Tags.Add("db");
        }

        public SqlServerCheckBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override SqlServerSettings Build()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("No connection string were defined.");
            }

            return new SqlServerSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}