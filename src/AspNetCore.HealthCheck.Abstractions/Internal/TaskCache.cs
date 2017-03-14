using System.Threading.Tasks;

namespace AspNetCore.HealthCheck
{
    public static class TaskCache
    {
#if NET452
        public static readonly Task CompletedTask = Task.FromResult(0);
#else
        public static readonly Task CompletedTask = Task.CompletedTask;
#endif
    }
}