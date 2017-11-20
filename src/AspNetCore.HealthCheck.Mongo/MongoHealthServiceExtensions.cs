using System;

namespace AspNetCore.HealthCheck.Mongo
{
    public static class MongoHealthServiceExtensions
    {
        public static HealthCheckBuilder AddMongoCheck(this HealthCheckBuilder builder, MongoOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var settings = new MongoSettings(options.Name, options.Critical, options.Frequency, options.Tags, options.ConnectionString);
            return builder.Add<MongoCheck>(settings);
        }

        public static HealthCheckBuilder AddMongoCheck(this HealthCheckBuilder builder, string name, Action<MongoSettingsBuilder> configure)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (configure == null)
                throw new ArgumentNullException(nameof(configure));

            var settingBuilder = new MongoSettingsBuilder(name);
            configure(settingBuilder);
            var settings = settingBuilder.Build();
            return builder.Add<MongoCheck>(settings);
        }
    }
}