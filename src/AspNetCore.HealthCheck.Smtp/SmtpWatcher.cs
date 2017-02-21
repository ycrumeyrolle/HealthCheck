using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpWatcher : HealthWatcher<SmtpWatchSettings>
    {
        private const string SmtpRequestedActionOkResponseCode = "250";

        public override async Task CheckHealthAsync(HealthContext context, SmtpWatchSettings settings)
        {
            using (var tcpClient = new TcpClient())
            {
                await tcpClient.ConnectAsync(settings.SmtpAddress, settings.SmtpPort);
                using (NetworkStream networkStream = tcpClient.GetStream())
                {
                    StreamWriter writer = null;
                    StreamReader reader = null;


                    if (settings.UseSsl)
                    {
#if NETSTANDARD1_3
                        // Ssl Stream is not implemented in netstandard1.3
                        // it will come in 2.0
                        throw new NotSupportedException();
#else
                        var sslStream = new SslStream(networkStream);
                        await sslStream.AuthenticateAsClientAsync(settings.SmtpAddress);
                        writer = new StreamWriter(sslStream);
                        reader = new StreamReader(sslStream);
#endif
                    }
                    else
                    {
                        writer = new StreamWriter(networkStream);
                        reader = new StreamReader(networkStream);
                    }

                    using (reader)
                    {
                        using (writer)
                        {
                            await reader.ReadLineAsync();
                            await writer.WriteLineAsync(settings.EhloCommand);
                            await writer.FlushAsync();
                            string smtpResponse = await reader.ReadLineAsync();

                            if (smtpResponse.StartsWith(SmtpRequestedActionOkResponseCode))
                            {
                                context.Succeed();
                            }
                            else
                            {
                                // TODO : log error to let people understand why it is failing
                                context.Fail();
                            }
                        }
                    }
                }
            }
        }
    }
}
