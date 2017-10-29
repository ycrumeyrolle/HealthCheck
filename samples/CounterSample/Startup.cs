using System.Threading.Tasks;
using AspNetCore.Counter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CounterSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalCounters()
                .AddRedisCounters("localhost");
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ICounterProvider counterFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var counter = counterFactory.GetLocalCounter("counter");
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World! " + counter.Value);
                counter.Increment();
            });
        }
    }

    public class CounterMiddleware
    {
        private readonly ICounter _counter;
        private readonly RequestDelegate _next;

        public CounterMiddleware(RequestDelegate next, ICounterProvider counterFactory)
        {
            _next = next;
            _counter = counterFactory.GetLocalCounter("CounterMiddleware");
        }

        public async Task Invoke(HttpContext context)
        {
            _counter.Increment();
            await context.Response.WriteAsync(_counter.Value.ToString());

            await _next(context);
        }
    }
}