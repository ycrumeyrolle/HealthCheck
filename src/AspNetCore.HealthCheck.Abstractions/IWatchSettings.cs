using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public interface IWatchSettings
    {
        string Name { get; set; }

        int Frequency { get; set; }

        bool Critical { get; set; }

        ICollection<string> Tags { get; set; }

        int Timeout { get; set; }
    }
}