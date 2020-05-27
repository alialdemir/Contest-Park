using ContestPark.Core.Database.Interfaces;
using System.Threading.Tasks;

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
        /// Tamamlanan görevi mission tablosuna ekler
        /// </summary>
        /// <param name="userId">Görevi tamamlayan kullanıcı id</param>
        /// <param name="missionId">Görev id</param>
        /// <returns>Ekleme işlemi başaralı ise true değilse false</returns>
        public async Task<bool> Add(string userId, byte missionId)
        {
            int? completedMissionId = await _completedMissionRepository.AddAsync(new Tables.CompletedMission
            {
                UserId = userId,
                MissionId = missionId
            });

            return completedMissionId.HasValue && completedMissionId.Value > 0;
        }

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
        public bool IsMissionCompleted(string userId, byte missionId)
        {
            return _completedMissionRepository.QuerySingleOrDefault<bool>("SP_Check_Mission", new
            {
                userId,
                missionId
            });
        }

        /// <summary>
        /// Görev altınını alır
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="missionId">Görev id</param>
        /// <returns>İşlem başarılı ise true değilse false</returns>
        public bool TakesMissionReward(string userId, byte missionId)
        {
            return _completedMissionRepository.QuerySingleOrDefault<bool>("SP_TakesMissionReward", new
            {
                userId,
                missionId
            });
        }

        #endregion Methods
    }
}
