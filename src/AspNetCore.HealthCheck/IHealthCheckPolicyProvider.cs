namespace AspNetCore.HealthCheck
{
    public interface IHealthCheckPolicyProvider
    {
        HealthCheckPolicy GetPolicy(string policyName);

        HealthCheckPolicy DefaultPolicy { get; }
    }
}