# HealthCheck

[![Build status](https://ci.appveyor.com/api/projects/status/pv8jbl2tomrbfn30?svg=true)](https://ci.appveyor.com/project/ycrumeyrolle/healthcheck)

Contains healthcheck and canary middleware for ASP.NET Core.

## Canary endpoint
The Canary endpoint provides a binary health status of the aplication. Dead or alive.
This endpoint is designed for HTTP load balancer, responding 200 HTTP status or 503 HTTP status.
Basic usage is :
```C#
  public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
  {
      // Other middleware ... 
      app.UseCanary(new CanaryOptions
      {
          Path = "/canary"
      });
  }
```
Health checks are [defined in the `ConfigureServices` method](#adding-health-checks).

## Health check endpoint
The Health check endpoint provides a more detailed health status of the application. 
This endpoint is designed for montoring systems.
Basic usage is :
```C#
  public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
  {
      // Other middleware ... 
      app.UseHealthCheck(new HealthCheckOptions
      {
          Path = "/health"
      });
  }
```
Health checks are [defined in the `ConfigureServices` method](#adding-health-checks).

Example of output: 
```JSON
[
  {
    "name": "database",
    "status": "ok",
    "elapsed": 45,
    "issued": 1489956222,
    "critical": true
  },
  {
    "name": "certificate",
    "status": "warning",
    "elapsed": 14,
    "message": "Certificate is about to expire.",
    "issued": 1489956222,
    "critical": false
  },
  {
    "name": "http service 1",
    "status": "ko",
    "elapsed": 107,
    "message": "An error occured. See logs for details.",
    "issued": 1489956222,
    "critical": true,
    "dns_resolve": 18
  }
]
```

## Adding health checks
Health checks are used both by canary & health check endpoints. 
Health checks are defined in the application `Statup` as in the following sample:
```C#
  public void ConfigureServices(IServiceCollection services)
  {
    // Others services
    // ...
    services
      .AddHealth(builder =>
      {
        builder
          .SetDefaultTimeout(1000)
          .AddX509CertificateCheck("certificate", certificate =>
          {
            certificate
              .WithThumbprint("ABCDEF23456789")
              .HasTag("authentication");
          })
          // More health checks
          .AddHttpEndpointCheck("http service", endpoint =>
          {
            endpoint
              .WithUri("http://server.domain.com/")
              .WithUri("http://example.com")
              .HasTag("payment")
              .HasFrequency(60)
              .IsCritical();
          });
      });
  }
```

## Protecting health check endpoint 
The health check endpoint may be protected from malicious attacks by adding an authorization policy:
```C#
  public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
  {
      // Other middleware ... 
      app.UseHealthCheck(new HealthCheckOptions
      {
          Path = "/health", 
          AuthorizationPolicy = new AuthorizationPolicyBuilder()
                                  .RequireXxx()
                                  // More authorization requirements...
                                  .Build()
      });
  }
```
