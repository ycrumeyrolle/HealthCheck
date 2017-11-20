﻿using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCore.HealthCheck.Mongo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddAuthentication(options => options.DefaultAuthenticateScheme = "apiKey")
                .AddScheme<AuthenticationSchemeOptions, FakeAuthenticationHandler>("apiKey", "test",
                    options => { });

            services.AddEntityFrameworkInMemoryDatabase()
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
                        })
                        .AddMongoCheck("mongo check", mongo =>
                        {
                            mongo
                                .WithConnectionString("mongodb://localhost:27017/test")
                                .IsCritical()
                                .HasTag("mongo", "database");
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

            app.UseAuthentication();
            app.UseHealthCheck(new HealthCheckOptions
            {
                Path = "/health",
                AuthorizationPolicy = new AuthorizationPolicyBuilder().RequireUserName("bob").Build()
            });

            app.UseCanary(new CanaryOptions
            {
                Path = "/canary",
                PolicyName = "critical"
            });
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

    public class FakeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string FakeAuthenticationScheme = "apiKey";

        public FakeAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, Microsoft.AspNetCore.Authentication.ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "bob") });
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationProperties = new AuthenticationProperties();
            AuthenticationTicket authenticationTicket = new AuthenticationTicket(claimsPrincipal, authenticationProperties, FakeAuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }
}
