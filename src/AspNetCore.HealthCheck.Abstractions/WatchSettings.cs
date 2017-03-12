using System;
using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class WatchSettings : IWatchSettings
    {
        public WatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Critical = critical;
            Frequency = frequency;
            Tags = tags == null ? new HashSet<string>() : new HashSet<string>(tags);
        }

        public string Name { get; set; }

        public int Frequency { get; set; }

        public bool Critical { get; set; }

        public ICollection<string> Tags { get; set; }

        public int Timeout { get; set; }
    }
}