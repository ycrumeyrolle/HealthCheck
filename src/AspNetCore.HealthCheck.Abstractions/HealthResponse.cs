using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNetCore.HealthCheck
{
    public class HealthResponse
    {
        public static readonly HealthResponse Empty = new HealthResponse();

        private HealthResponse()
        {
            Results = new HealthCheckResult[0];
        }

        public HealthResponse(IList<HealthCheckResult> results)
        {
            Results = new ReadOnlyCollection<HealthCheckResult>(results);

            DateTimeOffset? minTry = null;
            for (int i = 0; i < Results.Count; i++)
            {
                var result = Results[i];
                if (result.Status != HealthStatus.Healthy)
                {
                    HasErrors = true;
                    if (result.Critical)
                    {
                        HasCriticalErrors = true;
                    }
                }

                if (result.NextTry < minTry || !minTry.HasValue)
                {
                    minTry = result.NextTry;
                }
            }

            RetryAfter = minTry;
        }

        public IReadOnlyList<HealthCheckResult> Results { get; }

        public bool HasCriticalErrors { get; }

        public bool HasErrors { get; }

        public DateTimeOffset? RetryAfter { get; }

        public IEnumerable<HealthCheckResult> Errors
        {
            get
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    var result = Results[i];
                    if (result.Status != HealthStatus.Healthy && result.Critical)
                    {
                        yield return result;
                    }
                }

                yield break;
            }
        }
    }
}