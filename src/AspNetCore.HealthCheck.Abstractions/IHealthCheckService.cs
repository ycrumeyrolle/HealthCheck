using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public interface IHealthCheckService
    {
        Task<HealthCheckResponse> CheckHealthAsync(HealthCheckPolicy policy);
    }
}