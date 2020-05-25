using ContestPark.Core.Database.Interfaces;

namespace ContestPark.Mission.API.Infrastructure.Repositories.CompletedMission
{
    public class CompletedMissionRepository : ICompletedMissionRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.CompletedMission> _completedMissionRepository;

        #endregion Private Variables

        #region Constructor

        public CompletedMissionRepository(IRepository<Tables.CompletedMission> completedMissionRepository)
        {
            _completedMissionRepository = completedMissionRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Kullanıcının tamamlanmış görev sayısını verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Tamamlanan görev sayısı</returns>
        public byte CompletedMissionCount(string userId)
        {
            string sql = @"SELECT
                           COUNT(cm.UserId) as CompleteMissionCount
                           FROM CompletedMissions cm
                           WHERE cm.UserId = @userId";

            return _completedMissionRepository.QuerySingleOrDefault<byte>(sql, new
            {
                userId
            });
        }

        /// <summary>
        /// Görev tamamlandımı kontrol eder
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="missionId">Görev id</param>
        public void IsMissionCompleted(string userId, byte missionId)
        {
            _completedMissionRepository.QuerySingleOrDefault<bool>("SP_Check_Mission", new
            {
                userId,
                missionId
            });
        }

        #endregion Methods
    }
}
