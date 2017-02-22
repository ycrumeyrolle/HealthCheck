using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public interface IHealthCheckService
    {
        Task<HealthResponse> CheckHealthAsync(HealthCheckPolicy policy);
    }
}