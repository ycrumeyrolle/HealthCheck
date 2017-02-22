using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public interface IServerSwitch
    {
        Task CheckServerState(ServerSwitchContext context);
    }
}