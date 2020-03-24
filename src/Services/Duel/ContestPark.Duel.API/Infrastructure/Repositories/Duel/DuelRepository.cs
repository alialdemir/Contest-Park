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
        /// Duello sonunda skorları duel tablosuna kayıt eder
        /// </summary>
        /// <param name="duelId">Duello id</param>
        /// <param name="founderTotalScore">Kurucu total skor</param>
        /// <param name="opponentTotalScore">Rakip total skor</param>
        /// <param name="founderFinishScore">Kurucu bitirme bonusu</param>
        /// <param name="opponentFinishScore">Rakip bitirme bonusu</param>
        /// <param name="founderVictoryScore">Kurucu kazanma bonusu</param>
        /// <param name="opponentVictoryScore">Rakip kazanma bonusu</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public Task<bool> UpdateDuelScores(int duelId, DuelTypes duelType, byte founderTotalScore, byte opponentTotalScore, byte founderFinishScore, byte opponentFinishScore, byte founderVictoryScore, byte opponentVictoryScore)
        {
            string sql = @"UPDATE Duels SET
                                    FounderTotalScore = @founderTotalScore,
                                    OpponentTotalScore = @opponentTotalScore,
                                    FounderFinishScore = @founderFinishScore,
                                    OpponentFinishScore = @opponentFinishScore,
                                    FounderVictoryScore = @founderVictoryScore,
                                    OpponentVictoryScore = @opponentVictoryScore,
                                    DuelType = @duelType,
                                    ModifiedDate = CURRENT_TIMESTAMP()
                                    WHERE DuelId = @duelId";

            return _duelRepository.ExecuteAsync(sql, new
            {
                founderTotalScore,
                opponentTotalScore,
                founderFinishScore,
                opponentFinishScore,
                founderVictoryScore,
                opponentVictoryScore,
                duelId,
                duelType
            }, System.Data.CommandType.Text);
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
                           d.Bet * 2 AS Gold,
                           d.SubCategoryId,
                           d.BalanceType,
                           d.FounderTotalScore AS FounderScore,
                           d.OpponentTotalScore AS OpponentScore,
                            CASE WHEN d.FounderUserId = @userId THEN d.FounderVictoryScore WHEN d.OpponentUserId = @userId THEN d.OpponentVictoryScore END AS VictoryBonus,
                            CASE WHEN d.FounderUserId = @userId THEN d.FounderFinishScore WHEN d.OpponentUserId = @userId THEN d.OpponentFinishScore END AS FinishBonus
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

        /// <summary>
        /// Kazamma durumunu verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <param name="balanceType">Bakiyee tipi</param>
        public bool WinStatus(string userId)
        {
            return _duelRepository.QuerySingleOrDefault<bool>("FNC_WinStatus", new
            {
                userId,
            }, System.Data.CommandType.StoredProcedure);
        }

        #endregion Methods
    }
}
