using ContestPark.Core.Database.Interfaces;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.Duel
{
    public class DuelRepository : IDuelRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.Duel> _duelRepository;

        #endregion Private Variables

        #region Constructor

        public DuelRepository(IRepository<Tables.Duel> duelRepository)
        {
            _duelRepository = duelRepository;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Duello ekle
        /// </summary>
        /// <param name="duel">Duello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<int?> Insert(Tables.Duel duel)
        {
            return _duelRepository.AddAndGetIdAsync(duel);
        }

        /// <summary>
        /// Duello id'ye ait düelloyu verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello model</returns>
        public Tables.Duel GetDuelByDuelId(int duelId)
        {
            string sql = "SELECT * FROM Duels d WHERE d.DuelId = @duelId AND d.DuelType = @duelType";

            return _duelRepository.QuerySingleOrDefault<Tables.Duel>(sql, new
            {
                duelId,
                duelType = (byte)DuelTypes.Created
            });
        }

        /// <summary>
        /// Düello güncelle
        /// </summary>
        /// <param name="duel">Düello bilgileri</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> UpdateAsync(Tables.Duel duel)
        {
            return _duelRepository.UpdateAsync(duel);
        }

        /// <summary>
        /// Düello sonuç ekranı verir
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Düello sonuç ekranı</returns>
        public DuelResultModel DuelResultByDuelId(int duelId, string userId)
        {
            string sql = @"SELECT
                           d.FounderUserId,
                           d.OpponentUserId,
                           d.Bet AS Gold,
                           d.SubCategoryId,
                           d.FounderTotalScore AS FounderScore,
                           d.OpponentTotalScore AS OpponentScore,
                            CASE WHEN d.FounderUserId = @userId THEN d.FounderVictoryScore WHEN d.OpponentUserId = @userId THEN d.OpponentVictoryScore END AS VictoryBonus,
                            CASE WHEN d.FounderUserId = @userId THEN d.FounderFinshScore WHEN d.OpponentUserId = @userId THEN d.OpponentFinshScore END AS FinishBonus
                           FROM Duels d
                           WHERE d.DuelId = @duelId
                           LIMIT 1";

            return _duelRepository.QuerySingleOrDefault<DuelResultModel>(sql, new
            {
                duelId,
                userId
            });
        }

        /// <summary>
        /// Düello bitmiş mi kotnrol eder
        /// </summary>
        /// <param name="duelId">Düello id</param>
        /// <returns>Deüllo devam ediyor ise true bitmiş ise false</returns>
        public bool IsDuelFinish(int duelId)
        {
            if (duelId <= 0)
                return false;

            string sql = @"SELECT (CASE
                           WHEN EXISTS(
                           SELECT NULL
                           FROM Duels d WHERE d.DuelType = @duelType AND d.DuelId = @duelId)
                           THEN 1
                           ELSE 0
                           END)";

            return _duelRepository.QuerySingleOrDefault<bool>(sql, new
            {
                duelId,
                duelType = (byte)DuelTypes.Created
            });
        }

        /// <summary>
        /// Duello id göre düellodaki bahis miktarını ve bakiye tipini verir
        /// </summary>
        /// <param name="duelId">Duello id</param>
        /// <returns>Düellodaki bahis ve bakiye tipi</returns>
        public DuelBalanceInfoModel GetDuelBalanceInfoByDuelId(int duelId)
        {
            string sql = @"SELECT d.Bet, d.BalanceType, d.SubCategoryId, d.BetCommission
                           FROM Duels d
                           WHERE d.DuelId = @duelId";

            return _duelRepository.QuerySingleOrDefault<DuelBalanceInfoModel>(sql, new
            {
                duelId
            });
        }

        #endregion Methods
    }
}
