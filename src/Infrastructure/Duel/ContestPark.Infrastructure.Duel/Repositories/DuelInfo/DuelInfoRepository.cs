using ContestPark.Core.Dapper;
using ContestPark.Core.Interfaces;
using ContestPark.Infrastructure.Duel.Entities;

namespace ContestPark.Infrastructure.Duel.Repositories.DuelInfo
{
    public class DuelInfoRepository : DapperRepositoryBase<DuelInfoEntity>, IDuelInfoRepository
    {
        #region Constructor

        public DuelInfoRepository(ISettingsBase settings) : base(settings)
        {
        }

        #endregion Constructor
    }
}