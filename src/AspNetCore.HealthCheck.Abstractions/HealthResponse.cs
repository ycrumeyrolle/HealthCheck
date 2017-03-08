﻿using System.Collections.Generic;

namespace AspNetCore.HealthCheck
{
    public class HealthResponse
    {
        public static readonly HealthResponse Empty = new HealthResponse();

        private HealthResponse()
        {
            Results = new HealthCheckResult[0];
        }

        public HealthResponse(List<HealthCheckResult> results)
        {
            Results = results.AsReadOnly();
        }

        public IReadOnlyList<HealthCheckResult> Results { get; }

        public bool HasCriticalErrors
        {
            get
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    var result = Results[i];
                    if (result.Status != HealthStatus.OK && result.Critical)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool HasErrors
        {
            get
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    var result = Results[i];
                    if (result.Status != HealthStatus.OK)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public IEnumerable<HealthCheckResult> Errors
        {
            get
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    var result = Results[i];
                    if (result.Status != HealthStatus.OK && result.Critical)
                    {
                        yield return result;
                    }
                }

                yield break;
            }
        }
    }
}