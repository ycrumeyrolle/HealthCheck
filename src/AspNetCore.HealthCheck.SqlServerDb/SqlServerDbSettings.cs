﻿using System.Collections.Generic;

namespace AspNetCore.HealthCheck.SqlServerDb
{
    public class SqlServerDbSettings : WatchSettings
    {
        public SqlServerDbSettings(string name, bool critical, int frequency, IEnumerable<string> tags, string connectionString)
            : base(name, critical, frequency, tags)
        {
            ConnectionString = connectionString;
        }

        public string ConnectionString { get; set; }
    }
}