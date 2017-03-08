namespace AspNetCore.HealthCheck.SqlServerDb
{
    public class SqlServerDbCheckBuilder : SettingsHealthCheckBuilder<SqlServerDbSettings>
    {
        private string _connectionString;

        public SqlServerDbCheckBuilder(string name)
            : base(name)
        {
            Tags.Add("sql_server");
            Tags.Add("db");
        }

        public SqlServerDbCheckBuilder WithConnectionString(string connectionString)
        {
            _connectionString = connectionString;
            return this;
        }

        public override SqlServerDbSettings Build()
        {
            return new SqlServerDbSettings(Name, Critical, Frequency, Tags, _connectionString);
        }
    }
}