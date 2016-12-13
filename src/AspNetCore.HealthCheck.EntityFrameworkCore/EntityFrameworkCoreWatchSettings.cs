using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.HealthCheck.EntityFrameworkCore
{
    public class EntityFrameworkCoreWatchSettings<TDbContext> : WatchSettings where TDbContext : DbContext
    {
        public EntityFrameworkCoreWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags)
            : base(name, critical, frequency, tags)
        {
        }
    }
}