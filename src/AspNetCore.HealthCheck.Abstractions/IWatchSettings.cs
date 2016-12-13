using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public interface IWatchSettings
    {
        string Name { get; }

        int Frequency { get; }

        bool Critical { get; }

        IList<string> Tags { get; }
    }
}