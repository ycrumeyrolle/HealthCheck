﻿using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Builder
{
    public class HealthCheckOptions
    {
        public PathString Path { get; set; }

        public bool SendResults { get; set; } = true;
    }
}