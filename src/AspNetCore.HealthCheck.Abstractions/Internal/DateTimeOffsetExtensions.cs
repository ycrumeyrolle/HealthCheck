#if NET451 || NET452
using System;

namespace AspNetCore.HealthCheck
{
    public static class DateTimeOffsetExtensions
    {
        public static long ToUnixTimeSeconds(this DateTimeOffset dto)
        {
            return dto.UtcDateTime.Ticks / 10000000L - 62135596800L;
        }
    }
}
#endif