using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Smtp
{
    public class SmtpCheck : HealthCheck<SmtpCheckSettings>
    {
        private const string SmtpRequestedActionOkResponseCode = "250";

#if NETSTANDARD2_0        
        public override async Task CheckHealthAsync(HealthCheckContext context, SmtpCheckSettings settings)
        {
            using (var tcpClient = new TcpClient())
            {
                await ForceTimeout(tcpClient.ConnectAsync(settings.SmtpAddress, settings.SmtpPort), settings.Timeout);
                await CheckSmtpServer(context, settings, tcpClient);
            }
        }
        
        private static Task<Stream> WrapStream(SmtpCheckSettings settings, NetworkStream networkStream)
        {
            if (!settings.UseSsl)
            {
                return Task.FromResult<Stream>(networkStream);
            }

            return Task.FromResult<Stream>(new SslStream(networkStream));
        }

        public static async Task ForceTimeout(Task task, int millisecondsTimeout)
        {
            if (task != await Task.WhenAny(task, Task.Delay(millisecondsTimeout)))
            {
                throw new TimeoutException();
            }

            await task;
        }
#else
        public override async Task CheckHealthAsync(HealthCheckContext context, SmtpCheckSettings settings)
        {
            using (var tcpClient = new TcpClient())
            {
                IAsyncResult asyncResult = tcpClient.BeginConnect(settings.SmtpAddress, settings.SmtpPort, null, null);
                WaitHandle handle = asyncResult.AsyncWaitHandle;
                try
                {
                    if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(settings.Timeout), false))
                    {
                        tcpClient.Close();
                        throw new TimeoutException();
                    }

                    tcpClient.EndConnect(asyncResult);
                    await CheckSmtpServer(context, settings, tcpClient);
                }
                finally
                {
                    handle.Close();
                }
            }
        }

        private static async Task<Stream> WrapStream(SmtpCheckSettings settings, NetworkStream networkStream)
        {
            if (!settings.UseSsl)
            {
                return networkStream;
            }

            var sslStream = new SslStream(networkStream);
            await sslStream.AuthenticateAsClientAsync(settings.SmtpAddress);
            return sslStream;
        }
#endif

        private static async Task CheckSmtpServer(HealthCheckContext context, SmtpCheckSettings settings, TcpClient tcpClient)
        {
            using (NetworkStream networkStream = tcpClient.GetStream())
            {
                using (Stream stream = await WrapStream(settings, networkStream))
                using (var reader = new StreamReader(stream))
                {
                    using (var writer = new StreamWriter(stream))
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
                            context.Fail();
                        }
                    }
                }
            }
        }
    }
}
