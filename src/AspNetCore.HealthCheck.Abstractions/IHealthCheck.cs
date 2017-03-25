using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public interface IHealthCheck
    {
        Task CheckHealthAsync(HealthCheckContext context);
    }
}