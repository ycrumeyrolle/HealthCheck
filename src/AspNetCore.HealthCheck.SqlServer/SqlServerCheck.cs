using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.SqlServer
{
    public class SqlServerCheck : HealthCheck<SqlServerCheckSettings>
    {
        private const string SelectOneCommandText = "SELECT 1";

        public override async Task CheckHealthAsync(HealthCheckContext context, SqlServerCheckSettings settings)
        {
            using (var connection = new SqlConnection(settings.ConnectionString))
            {
                await connection.OpenAsync(context.CancellationToken);
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = SelectOneCommandText;
                    await command.ExecuteScalarAsync(context.CancellationToken);
                    connection.Close();
                    context.Succeed();
                }
            }
        }
    }
}