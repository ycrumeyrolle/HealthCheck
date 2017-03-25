using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public class InlineHealthCheckSettings : HealthCheckSettings
    {
        public InlineHealthCheckSettings(string name, bool critical, int frequency, IEnumerable<string> tags, Func<HealthCheckContext, Task> action)
            : base(name, critical, frequency, tags)
        {
            Action = action;
        }

        public Func<HealthCheckContext, Task> Action { get; set; }
    }
}