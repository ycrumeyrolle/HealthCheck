using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCore.HealthCheck.Smtp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.HealthCheck.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<FakeDbContext>(
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
                        .SetDefaultTimeout(1000)
                        .AddEntityFrameworkCoreCheck<FakeDbContext>("myDatabase", dbContext =>
                        {
                            dbContext
                                .IsCritical()
                                .HasTag("authentication");
                        })
                        .AddEntityFrameworkCoreCheck<FakeDbContext>("myDatabase2", dbContext =>
                        {
                            dbContext
                                .IsCritical()
                                .HasTag("authentication");
                        })
                        .AddX509CertificateCheck("OAuth2 token certificate", certificate =>
                        {
                            certificate
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

                        .AddX509CertificateCheck("Payment validation certificate", certificate =>
                        {
                            certificate
                                .WithThumbprint("d9 b2 18 8e 26 3f 54 dd 56 e4 eb 74 8f f5 42 c9 60 f1 ab bc")
                                .WarnIfExpiresIn(1051200)
                                .HasTag("payment", "certificates");
                        })
                        .AddCounterCheck("Payment failures", counter =>
                        {
                            counter
                                .WithWarningThreshold(5)
                                .WithErrorThreshold(10)
                                .HasTag("payment");
                        })

                        .AddVirtualMemorySizeCheck("Virtual memory check", memory =>
                        {
                            memory
                                .WithErrorThreshold(1024)
                                .IsCritical()
                                .HasTag("system");
                        })
                        .AddAvailableDiskSpaceCheck("Available free space on drive c", drive =>
                        {
                            drive
                                .WithDrive("c")
                                .WithWarningThreshold(2048)
                                .WithErrorThreshold(1024)
                                .IsCritical();
                        })
                        .AddCheck("custom check", ctx =>
                        {
                            ctx.Succeed();
                            return TaskCache.CompletedTask;
                        });
                });

            services.AddLogging();
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddDebug(LogLevel.Trace)
                .AddConsole(LogLevel.Trace);

            app.Map("/loopback", a => a.Run(ctx => ctx.Response.WriteAsync("OK")));

            app.UseMiddleware<FakeAuthenticationMiddleware>();
            app.UseHealthCheck(new HealthCheckOptions
            {
                Path = "/healthcheck",
                AuthorizationPolicy = new AuthorizationPolicyBuilder().RequireUserName("bob").Build()
            });

            app.UseCanary("/canary");
        }
    }

    public class FakeDbContext : DbContext
    {
        public FakeDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Entity> Entity { get; set; }
    }

    public class Entity
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    public class FakeAuthenticationMiddleware : AuthenticationMiddleware<FakeAuthenticationOptions>
    {
        public FakeAuthenticationMiddleware(RequestDelegate next, IOptions<FakeAuthenticationOptions> options, ILoggerFactory loggerFactory, UrlEncoder encoder)
            : base(next, options, loggerFactory, encoder)
        {
        }

        protected override AuthenticationHandler<FakeAuthenticationOptions> CreateHandler()
        {
            return new FakeAuthenticationHandler();
        }
    }

    public class FakeAuthenticationHandler : AuthenticationHandler<FakeAuthenticationOptions>
    {
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "bob") })), new AuthenticationProperties(), "apikey")));
        }
    }

    public class FakeAuthenticationOptions : AuthenticationOptions
    {
        public FakeAuthenticationOptions()
        {
            AutomaticAuthenticate = true;
        }
    }
}
