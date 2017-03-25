using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public abstract class HealthCheckSettingsBuilder<TSettings> where TSettings : IHealthCheckSettings
    {
        private readonly string _name;
        private readonly HashSet<string> _tags = new HashSet<string>();

        private bool _critical;
        private int _frequency;

        public string Name => _name;

        public bool Critical => _critical;

        public ICollection<string> Tags => _tags;

        public int Frequency => _frequency;

        public HealthCheckSettingsBuilder(string name)
        {
            _name = name;
        }

        public HealthCheckSettingsBuilder<TSettings> IsCritical()
        {
            _critical = true;
            return this;
        }
        
        public HealthCheckSettingsBuilder<TSettings> HasTag(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _tags.Add(tag);
            }

            return this;
        }

        public HealthCheckSettingsBuilder<TSettings> HasFrequency(int frequency)
        {
            _frequency = frequency;
            return this;
        }

        public abstract TSettings Build();
    }
}