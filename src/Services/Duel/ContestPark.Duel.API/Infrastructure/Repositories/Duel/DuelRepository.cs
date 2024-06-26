﻿using ContestPark.Core.Database.Interfaces;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Models;
using System.Data;
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
            return _duelRepository.QuerySingleOrDefault<DuelResultModel>("SP_DuelResult", new
            {
                duelId,
                userId
            }, CommandType.StoredProcedure);
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
        /// Kazamma/kaybetme durumlarını verir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        public DuelWinStatusModel WinStatus(string userId, BalanceTypes balanceType)
        {
            return _duelRepository.QuerySingleOrDefault<DuelWinStatusModel>("SP_WinStatus", new
            {
                userId,
                balanceType
            }, CommandType.StoredProcedure);
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
        /// Kullanıcının oynanıyor durumundaki son duellosunu getirir
        /// </summary>
        /// <param name="userId">Kullanıcı id</param>
        /// <returns>Duello id</returns>
        public int LastPlayingDuel(string userId)
        {
            string sql = @"SELECT d.DuelId FROM Duels d
                           WHERE
                           (d.FounderUserId = @userId OR d.OpponentUserId = @userId)
                           AND d.FounderTotalScore IS NULL AND d.OpponentTotalScore  IS NULL
                           AND d.DuelType = @duelType
                           ORDER BY d.CreatedDate DESC
                           LIMIT 1";

            return _duelRepository.QuerySingleOrDefault<int>(sql, new
            {
                userId,
                duelType = (byte)DuelTypes.Created
            });
        }

        /// <summary>
        /// Son bir iki içinde iki oyuncu ikiden fazla düello yaptımı kontrol eder
        /// </summary>
        /// <param name="founderUserId">Kurucu kullanıcı id</param>
        /// <param name="opponentUserId">Rakip kullanıcı id</param>
        /// <returns>True ise 2 den fazla oynamış false ise oynamamıştır</returns>
        public bool PlaysInTheLastHour(string founderUserId, string opponentUserId)
        {
            string sql = @"SELECT
                           CASE
                           WHEN COUNT(d.DuelId) >= 1 THEN 1
                           ELSE 0
                           END
                           FROM Duels d
                           WHERE
                           d.CreatedDate >= DATE_SUB(NOW(), INTERVAL 1 HOUR) AND
                           ((d.FounderUserId = @founderUserId AND d.OpponentUserId = @opponentUserId)
                           OR (d.FounderUserId = @opponentUserId AND d.OpponentUserId = @founderUserId))
                           ORDER BY d.CreatedDate DESC
                           LIMIT 1";

            return _duelRepository.QuerySingleOrDefault<bool>(sql, new
            {
                founderUserId,
                opponentUserId
            });
        }

        #endregion Methods
    }
}
