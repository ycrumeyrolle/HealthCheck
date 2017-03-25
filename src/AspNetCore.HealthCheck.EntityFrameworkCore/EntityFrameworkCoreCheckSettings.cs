using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.HealthCheck.EntityFrameworkCore
{
    public class EntityFrameworkCoreCheckSettings<TDbContext> : HealthCheckSettings where TDbContext : DbContext
    {
        public EntityFrameworkCoreCheckSettings(string name, bool critical, int frequency, IEnumerable<string> tags)
            : base(name, critical, frequency, tags)
        {
        }
    }
}