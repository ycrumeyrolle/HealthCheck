﻿#if !NETSTANDARD1_3
using Oracle.ManagedDataAccess.Client;
#endif
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.Oracle
{
    public class OracleCheck : HealthCheck<OracleCheckSettings>
    {
        private const string SelectOneSqlCommandText = "SELECT 1 FROM DUAL";

#if !NETSTANDARD1_3
        public override async Task CheckHealthAsync(HealthCheckContext context, OracleCheckSettings settings)
        {
            using (OracleConnection dbConnection = new OracleConnection(settings.ConnectionString))
            {
                await dbConnection.OpenAsync(context.CancellationToken);

                OracleCommand oracleCommand = dbConnection.CreateCommand();
                oracleCommand.CommandText = SelectOneSqlCommandText;
                await oracleCommand.ExecuteScalarAsync(context.CancellationToken);

                context.Succeed();
            }
        }
#endif

#if NETSTANDARD1_3
        public override Task CheckHealthAsync(HealthCheckContext context, OracleCheckSettings settings)
        {
            // OracleManaged not yet compatible with netcore 
            // check forced to failed since it's not supported

            context.Fail("Not Supported");
            return Task.FromResult(0);
        }
#endif
    }
}
