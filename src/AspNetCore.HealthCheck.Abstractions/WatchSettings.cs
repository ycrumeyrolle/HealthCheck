using System;
using System.Collections.Generic;
using System.Linq;

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
            Tags = tags.ToList();
        }

        public string Name { get; set; }

        public int Frequency { get; set; }

        public bool Critical { get; set; }

        public IList<string> Tags { get; set; }

        public int Timeout { get; set; }
    }
}