using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class CheckOptions
    {
        public string Name { get; set; }

        public int Frequency { get; set; }

        public bool Critical { get; set; }

        public IList<string> Tags { get; set; }
    }
}