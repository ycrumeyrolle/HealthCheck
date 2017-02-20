using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AspNetCore.HealthCheck.SqlServerDb
{
    public class SqlServerDbWatcher : HealthWatcher<SqlServerDbSettings>
    {
        private const string SelectOneCommandText = "SELECT 1";

        public override async Task CheckHealthAsync(HealthContext context, SqlServerDbSettings settings)
        {
            using (var connection = new SqlConnection(settings.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = SelectOneCommandText;
                    var result = (int)await command.ExecuteScalarAsync();
                    connection.Close();
                    if (result == 1)
                    {
                        context.Succeed();
                    }
                    else
                    {
                        context.Fail();
                    }
                }
            }
        }
    }
}