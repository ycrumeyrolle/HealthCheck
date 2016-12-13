using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public static class InlineHealthCheckBuilderExtensions
    {
        public static HealthCheckBuilder Add(this HealthCheckBuilder builder, string name, Func<HealthContext, Task> action, string tag, bool critical = false, int frequency = 0)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.AddCheck(name, action, TagsHelper.FromTag(tag), critical, frequency);
        }

        public static HealthCheckBuilder AddCheck(this HealthCheckBuilder builder, string name, Func<HealthContext, Task> action, IEnumerable<string> tags = null, bool critical = false, int frequency = 0)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.Add<InlineHealthWatcher>(new InlineSettings(name, critical, frequency, TagsHelper.FromTag(tags), action));
        }

    }
}