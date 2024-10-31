using Dapper;
using System.Data;
using System.Data.Common;

namespace ApexWebFileMgr.DB.Dapper
{
    public interface IDapper
    {
        DbConnection GetDbconnection();
        Task<IEnumerable<T>> GetAllAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.StoredProcedure);
        Task<T> GetAsync<T>(string sp, DynamicParameters parms, CommandType commandType = CommandType.Text);
    }
}
