using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.HealthCheck.EntityFrameworkCore
{
    public class EntityFrameworkCoreWatcher<TDbContext> : HealthWatcher<IWatchSettings> where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        public EntityFrameworkCoreWatcher(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async override Task CheckHealthAsync(HealthContext context, IWatchSettings settings)
        {
            DbConnection connection = _dbContext.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();
                if (connection.State == ConnectionState.Open)
                {
                    context.Succeed();
                }
                else
                {
                    context.Fail();
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}