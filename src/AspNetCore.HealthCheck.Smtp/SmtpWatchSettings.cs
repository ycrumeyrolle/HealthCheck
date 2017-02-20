using System.Collections.Generic;
using System.Security;

namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpWatchSettings : WatchSettings
    {
        public SmtpWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string smtpAddress, 
            int smtpPort, bool useSsl = false)
            : base(name, critical, frequency, tags)
        {
            SmtpAddress = smtpAddress;
            SmtpPort = smtpPort;
            UseSsl = useSsl;
        }

        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
    }
}