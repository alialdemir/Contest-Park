using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Infrastructure.Cp.Entities;

namespace ContestPark.Infrastructure.Cp.Repositories.CpInfo
{
    public class CpInfoRepository : DapperRepositoryBase<CpInfoEntity>, ICpInfoRepository
    {
        #region Constructor

        public CpInfoRepository(ISettingsBase settingsBase) : base(settingsBase)
        {
        }

        #endregion Constructor
    }
}