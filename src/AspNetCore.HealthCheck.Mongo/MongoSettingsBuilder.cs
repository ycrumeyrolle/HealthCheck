namespace AspNetCore.HealthCheck.Mongo
{
    public class MongoSettingsBuilder : HealthCheckSettingsBuilder<MongoSettings>
    {
        public string ConnectionString { get; private set; }

        public MongoSettingsBuilder(string name)
            : base(name)
        {
        }

        public MongoSettingsBuilder WithConnectionString(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        public override MongoSettings Build()
        {
            return new MongoSettings(Name, Critical, Frequency, Tags, ConnectionString);
        }
    }
}