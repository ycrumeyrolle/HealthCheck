using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public interface IHealthWatcher
    {
        Task CheckHealthAsync(HealthContext context);
    }
}