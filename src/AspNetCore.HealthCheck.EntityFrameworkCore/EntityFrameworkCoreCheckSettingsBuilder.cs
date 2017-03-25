using Microsoft.EntityFrameworkCore;

namespace AspNetCore.HealthCheck.EntityFrameworkCore
{
    public class EntityFrameworkCoreCheckSettingsBuilder<TDbContext> : HealthCheckSettingsBuilder<EntityFrameworkCoreCheckSettings<TDbContext>> where TDbContext : DbContext
    {
        public EntityFrameworkCoreCheckSettingsBuilder(string name)
            : base(name)
        {
            Tags.Add("db");
        }

        public override EntityFrameworkCoreCheckSettings<TDbContext> Build()
        {
            return new EntityFrameworkCoreCheckSettings<TDbContext>(Name, Critical, Frequency, Tags);
        }
    }
}