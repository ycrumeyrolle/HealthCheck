using Microsoft.EntityFrameworkCore;

namespace AspNetCore.HealthCheck.EntityFrameworkCore
{
    public class EntityFrameworkCoreHealthCheckBuilder<TDbContext> : SettingsHealthCheckBuilder<EntityFrameworkCoreWatchSettings<TDbContext>> where TDbContext : DbContext
    {
        public EntityFrameworkCoreHealthCheckBuilder(string name)
            : base(name)
        {
            Tags.Add("ef");
            Tags.Add("db");
        }

        public override EntityFrameworkCoreWatchSettings<TDbContext> Build()
        {
            return new EntityFrameworkCoreWatchSettings<TDbContext>(Name, Critical, Frequency, Tags);
        }
    }
}