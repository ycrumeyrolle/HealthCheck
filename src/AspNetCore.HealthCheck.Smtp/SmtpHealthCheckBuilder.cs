using System;

namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpHealthCheckBuilder : SettingsHealthCheckBuilder<SmtpWatchSettings>
    {
        private string _smtpAddress;
        private int _smtpPort;
        private bool _useSsl;

        public SmtpHealthCheckBuilder(string name)
            : base(name)
        {
            _smtpPort = 25;
            Tags.Add("smtp");
        }

        public SmtpHealthCheckBuilder WithAddress(string smtpAddress)
        {
            _smtpAddress = smtpAddress;
            return this;
        }

        public SmtpHealthCheckBuilder OnPort(int smtpPort)
        {
            _smtpPort = smtpPort;
            return this;
        }

        public SmtpHealthCheckBuilder WithSsl()
        {
            _useSsl = true;
            return this;
        }
        
        public override SmtpWatchSettings Build()
        {
            if (string.IsNullOrEmpty(_smtpAddress))
            {
                throw new InvalidOperationException("No SMTP address were defined.");
            }

            return new SmtpWatchSettings(Name, Critical, Frequency, Tags, _smtpAddress, _smtpPort, _useSsl);
        }
    }
}
