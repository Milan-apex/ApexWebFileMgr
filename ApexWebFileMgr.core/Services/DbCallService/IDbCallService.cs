using ApexWebFileMgr.core.Models;

namespace ApexWebFileMgr.core.Services.DbCallService
{
    public interface IDbCallService
    {
        Task<ConfigurationModel> GetConfigurationAsync();
    }
}
