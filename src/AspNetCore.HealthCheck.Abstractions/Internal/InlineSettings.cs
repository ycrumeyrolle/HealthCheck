using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.HealthCheck;

namespace AspNetCore.HealthCheck
{
    public class InlineSettings : WatchSettings
    {
        public InlineSettings(string name, bool critical, int frequency, IEnumerable<string> tags, Func<HealthContext, Task> action)
            : base(name, critical, frequency, tags)
        {
            Action = action;
        }

        public Func<HealthContext, Task> Action { get; set; }
    }
}