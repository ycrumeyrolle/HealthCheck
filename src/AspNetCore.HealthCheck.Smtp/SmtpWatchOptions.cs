namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpWatchOptions : WatchOptions 
    {
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string SmtpAddress { get; set; }
    }
}
