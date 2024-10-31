using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace ApexWebFileMgr.DB.Dapper
{
    public class Dapper : IDapper
    {
        private readonly IConfiguration _config;
        private string ConnectionstringName = "DefaultConnection";


        public Dapper(IConfiguration config)
        {
            _config = config;
        }

        public void Dispose()
        {

        }
        public DbConnection GetDbconnection()
        {
            return new SqlConnection(_config.GetConnectionString(GetConnectionStr()));
        }
        public async Task<IEnumerable<T>> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure)
        {
            using IDbConnection db = new SqlConnection(_config.GetConnectionString(GetConnectionStr()));
            return await db.QueryAsync<T>(sp, parms, commandType: commandType);
        }
        public async Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text)
        {
            using IDbConnection db = new SqlConnection(GetConnectionStr());
            return await db.QuerySingleOrDefaultAsync<T>(sp, parms, commandType: commandType);
        }
        private string GetConnectionStr()
        {
            return _config.GetConnectionString(ConnectionstringName);
        }
    }
}
