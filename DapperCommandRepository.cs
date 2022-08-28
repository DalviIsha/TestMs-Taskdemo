using Npgsql;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace TestMS
{
    public class DapperCommandRepository : IDapperCommandRepository
    {
        private readonly IConfiguration _config;
        private readonly string _connectionstring = "WriteDatabase";
        private readonly ILogger<DapperCommandRepository> _logger;

        public DapperCommandRepository(
            IConfiguration config,
            ILogger<DapperCommandRepository> logger)
        {
            _config = config;
            _logger = logger;
        }

        public void Dispose()
        {
        }

        public DbConnection GetDbconnection()
        {
            throw new NotImplementedException();
        }

        public async Task<int> Update<T>(string query, T data)
        {
            int result = 0;
            using var db = new NpgsqlConnection(_config.GetConnectionString(_connectionstring));
            try
            {
                if (db.State == ConnectionState.Closed)
                {
                    db.Open();
                }
                using var tran = db.BeginTransaction();
                try
                {
                    result = await db.ExecuteAsync(query, data, transaction: tran);
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Dapper Repository: Error while updating query {@Query} with exception", query);
                    tran.Rollback();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dapper Repository: Error on dapper repo {@Query}", query);
            }
            finally
            {
                if (db.State == ConnectionState.Open)
                {
                    db.Close();
                }
            }
            return result;
        }
    }
}
