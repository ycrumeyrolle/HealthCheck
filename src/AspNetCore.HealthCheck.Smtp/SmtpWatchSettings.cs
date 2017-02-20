using System.Collections.Generic;

namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpWatchSettings : WatchSettings
    {
        private const string EhloSmtpCommandText = "EHLO";
        public SmtpWatchSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string smtpAddress, 
            int smtpPort, bool useSsl = false)
            : base(name, critical, frequency, tags)
        {
            SmtpAddress = smtpAddress;
            SmtpPort = smtpPort;
            UseSsl = useSsl;
            EhloCommand = $"{EhloSmtpCommandText} {smtpAddress}";
        }

        public string SmtpAddress { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string EhloCommand { get; private set; }
    }
}