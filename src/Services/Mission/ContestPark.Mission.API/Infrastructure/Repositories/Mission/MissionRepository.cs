using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Enums;
using ContestPark.Mission.API.Models;

namespace ContestPark.Mission.API.Infrastructure.Repositories.Mission
{
    public class MissionRepository : IMissionRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Mission> _missionRepository;

        #endregion Private Variables

        #region Constructor

        public MissionRepository(IRepository<Tables.Mission> missionRepository)
        {
            _missionRepository = missionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Görev listesini verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="language">Dil seçimi</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Görev listesi</returns>
        public ServiceModel<MissionModel> GetMissions(string userId, Languages language, PagingModel pagingModel)
        {
            return _missionRepository.ToSpServiceModel<MissionModel>("SP_Missions", new
            {
                userId,
                language
            }, pagingModel);
        }

        #endregion Methods
    }
}
