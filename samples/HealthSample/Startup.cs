using System.Threading.Tasks;
using AspNetCore.HealthCheck.Smtp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace AspNetCore.HealthCheck.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<MyDbContext>(
                    (sp, builder) =>
                        builder.UseInternalServiceProvider(sp)
                        .UseInMemoryDatabase("test"));

            services
                .AddServerFileSwitch(options =>
                {
                    options.FileProvider = new PhysicalFileProvider(@"c:\temp");
                    options.FilePath = @"test.txt";
                })
                .AddHealth(builder =>
                {
                    builder
                        .AddEntityFrameworkCoreCheck<MyDbContext>("myDatabase", dbContext =>
                        {
                            dbContext
                                .IsCritical()
                                .HasTag("authentication");
                        })
                        .AddX509CertificateCheck("OAuth2 token certificate", settings =>
                        {
                            settings
                                .WithThumbprint("D9B2188E2635F4DD56E4EB748FF542C960F1ABBC")
                                .HasTag("authentication", "certificates");
                        })

                        .AddHttpEndpointCheck("Payment service", endpoint =>
                        {
                            endpoint
                                .WithUri("http://localhost:35463/loopback")
                                .WithUri("http://localhost:35463/loopback")
                                .HasTag("payment")
                                .HasFrequency(60)
                                .IsCritical();
                        })

                        .AddX509CertificateCheck("Payment validation certificate", settings =>
                        {
                            settings
                                .WithThumbprint("40D34AC15D2561A4ED8ED2CFBFB5F24C7FA7467D")
                                .HasTag("payment", "certificates");
                        })
                        .AddCounterCheck("Payment failures", counter =>
                        {
                            counter
                                .WithThreshold(10)
                                .HasTag("payment");
                        })

                        //.AddVirtualMemorySizeCheck("Virtual memory check", 1024, critical: true, tag: "system")
                        //.AddAvailableFreeSpaceCheck("Available free space on drive c", "c", 1024, tag: "system")
                        .AddCheck("custom check", ctx =>
                        {
                            ctx.Succeed();
                            return TaskCache.CompletedTask;
                        })
                        .AddSmtpCheck("MySmtp", smtp => smtp.WithAddress("smtp.gmail.com").OnPort(465));
                });

            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddDebug()
                .AddConsole();

            app.Map("/loopback", a => a.Run(ctx => ctx.Response.WriteAsync("OK")));

            app.UseHealthCheck(new HealthCheckOptions
            {
                Path = "/diagnostic",
                SendResults = true
            });

            app.UseHealthCheck(new HealthCheckOptions
            {
                Path = "/ping",
                SendResults = false
            });
        }
    }

    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entity> Entity { get; set; }
    }

    public class Entity
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }
}
