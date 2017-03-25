namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpCheckOptions : CheckOptions 
    {
        public int SmtpPort { get; set; }

        public bool UseSsl { get; set; }

        public string SmtpAddress { get; set; }
    }
}
