using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public interface IServerSwitch
    {
        Task CheckServerStateAsync(ServerSwitchContext context);
    }
}