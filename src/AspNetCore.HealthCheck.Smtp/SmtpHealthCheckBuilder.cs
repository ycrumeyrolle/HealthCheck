using System.Security;

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
        }

        public SmtpHealthCheckBuilder WithAddress(string smtpAddress)
        {
            this._smtpAddress = smtpAddress;
            return this;
        }

        public SmtpHealthCheckBuilder OnPort(int smtpPort)
        {
            this._smtpPort = smtpPort;
            return this;
        }

        public SmtpHealthCheckBuilder WithSsl()
        {
            this._useSsl = true;
            return this;
        }
        
        public override SmtpWatchSettings Build()
        {
            return new SmtpWatchSettings(Name, Critical, Frequency, Tags, _smtpAddress, _smtpPort, _useSsl);
        }
    }
}