using System;

namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpCheckSettingsBuilder : HealthCheckSettingsBuilder<SmtpCheckSettings>
    {
        private string _smtpAddress;
        private int _smtpPort;
        private bool _useSsl;

        public SmtpCheckSettingsBuilder(string name)
            : base(name)
        {
            _smtpPort = 25;
            Tags.Add("smtp");
        }

        public SmtpCheckSettingsBuilder WithAddress(string smtpAddress)
        {
            _smtpAddress = smtpAddress;
            return this;
        }

        public SmtpCheckSettingsBuilder OnPort(int smtpPort)
        {
            _smtpPort = smtpPort;
            return this;
        }

        public SmtpCheckSettingsBuilder WithSsl()
        {
            _useSsl = true;
            return this;
        }
        
        public override SmtpCheckSettings Build()
        {
            if (string.IsNullOrEmpty(_smtpAddress))
            {
                throw new InvalidOperationException("No SMTP address were defined.");
            }

            return new SmtpCheckSettings(Name, Critical, Frequency, Tags, _smtpAddress, _smtpPort, _useSsl);
        }
    }
}
