using ApexWebFileMgr.core.Models;
using ApexWebFileMgr.DB.Dapper;
using ApexWebFileMgr.DB.StoreProcedures;
using Dapper;
using System.Data;

namespace ApexWebFileMgr.core.Services.DbCallService
{
    public class DbCallService : IDbCallService
    {
        private readonly IDapper _dapper;
        public DbCallService(IDapper dapper)
        {
            _dapper = dapper;
        }
        public async Task<ConfigurationModel> GetConfigurationAsync()
        {
            try
            {
                var dbparams = new DynamicParameters();
                var res = await _dapper.GetAsync<ConfigurationModel>(StoreProcedure.GetImagePath, null, commandType: CommandType.StoredProcedure);
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
