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
        /// <param name="userId"></param>
        /// <returns></returns>
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

        //public bool IsMissionCompleted(string userId, byte missionId)
        //{
        //}

        #endregion Methods
    }
}
