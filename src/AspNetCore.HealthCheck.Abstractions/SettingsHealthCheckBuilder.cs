using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public abstract class SettingsHealthCheckBuilder<TSettings> where TSettings : IWatchSettings
    {
        private readonly string _name;
        private readonly HashSet<string> _tags = new HashSet<string>();

        private bool _critical;
        private int _frequency;

        public string Name => _name;

        public bool Critical => _critical;

        public ICollection<string> Tags => _tags;

        public int Frequency => _frequency;

        public SettingsHealthCheckBuilder(string name)
        {
            _name = name;
        }

        public SettingsHealthCheckBuilder<TSettings> IsCritical()
        {
            _critical = true;
            return this;
        }
        
        public SettingsHealthCheckBuilder<TSettings> HasTag(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _tags.Add(tag);
            }

            return this;
        }

        public SettingsHealthCheckBuilder<TSettings> HasFrequency(int frequency)
        {
            _frequency = frequency;
            return this;
        }

        public abstract TSettings Build();
    }
}