using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Core.Services.NumberFormat;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestPark.Duel.API.Infrastructure.Repositories.ScoreRankingRepository
{
    public class ScoreRankingRepository : IScoreRankingRepository
    {
        #region Private Variables

        private readonly IRepository<Tables.ScoreRanking> _scoreRankingRepository;
        private readonly IContestDateRepository _contestDateRepository;
        private readonly INumberFormatService _numberFormatService;

        #endregion Private Variables

        #region Constructor

        public ScoreRankingRepository(IRepository<Tables.ScoreRanking> scoreRankingRepository,
                                      IContestDateRepository contestDateRepository,
                                      INumberFormatService numberFormatService)
        {
            _scoreRankingRepository = scoreRankingRepository;
            _contestDateRepository = contestDateRepository;
            _numberFormatService = numberFormatService;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Her kategori için parametredeki yarışma id ve bakiye tipine göre ilk 3 kazananını verir
        /// </summary>
        /// <param name="contestDateId">Yarışma tarih id</param>
        /// <param name="balanceType">Bakiye tipi</param>
        /// <returns>Her kategori için kazanan ilk 3 kullanıcı</returns>
        public IEnumerable<WinnersModel> Winners(short contestDateId, BalanceTypes balanceType)
        {
            string sql = @"SELECT
                                (
                                SELECT          sr.UserId
                                                        FROM ScoreRankings sr
                                                        WHERE sr.ContestDateId = @contestDateId  AND sr.SubCategoryId=sc.SubCategoryId
                                                        ORDER BY
                                                        CASE
                                                            WHEN @balanceType=1 THEN sr.TotalGoldScore
                                                            WHEN @balanceType=2 THEN sr.TotalMoneyScore
                                                        END DESC
                                                        LIMIT 1
                                ) AS Premier,
                                (
                                SELECT          sr.UserId
                                                        FROM ScoreRankings sr
                                                        WHERE sr.ContestDateId = @contestDateId  AND sr.SubCategoryId=sc.SubCategoryId
                                                        ORDER BY
                                                        CASE
                                                            WHEN @balanceType=1 THEN sr.TotalGoldScore
                                                            WHEN @balanceType=2 THEN sr.TotalMoneyScore
                                                        END DESC
                                                        LIMIT 1,1
                                ) AS Secondary,
                                (
                                SELECT          sr.UserId
                                                        FROM ScoreRankings sr
                                                        WHERE sr.ContestDateId = @contestDateId  AND sr.SubCategoryId=sc.SubCategoryId
                                                        ORDER BY
                                                        CASE
                                                            WHEN @balanceType=1 THEN sr.TotalGoldScore
                                                            WHEN @balanceType=2 THEN sr.TotalMoneyScore
                                                        END DESC
                                                        LIMIT 2, 1
                                ) AS Third
                                FROM SubCategories sc";

            return _scoreRankingRepository.QueryMultiple<WinnersModel>(sql, new
            {
                contestDateId,
                balanceType
            })
              .Where(x => !string.IsNullOrEmpty(x.Premier) && !string.IsNullOrEmpty(x.Secondary) && !string.IsNullOrEmpty(x.Third));
        }

        /// <summary>
        /// Tüm zamanlardaki para ile skor sıralamasını getirir
        /// </summary>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Para kazananların sıralaması</returns>
        public ServiceModel<RankModel> AllTimes(PagingModel pagingModel)
        {
            return _scoreRankingRepository.ToServiceModel<RankModel>("SP_GetRankingAllTimes", new { }, pagingModel: pagingModel);
        }

        /// <summary>
        /// Alt kategori id ve bakiye tipine göre skor sıralama listesi verir
        /// Eğer contestDateId sıfır gelirse tüm zamanlardaki sıralamayı getirir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="balanceType">Bakiye tip</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori sıralaması</returns>
        public ServiceModel<RankModel> GetRankingBySubCategoryId(short subCategoryId, BalanceTypes balanceType, short contestDateId, PagingModel pagingModel)
        {
            return _scoreRankingRepository.ToServiceModel<RankModel>("SP_GetRankingBySubCategoryId", new
            {
                subCategoryId,
                balanceType,
                contestDateId,
            }, pagingModel);
        }

        /// <summary>
        /// Takip Takip edilen kullanıcıların ilgili alt kategori ve bakiye tipine göre olan sıralasını verir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="balanceType">Bakiye tipi</param>
        /// <param name="contestDateId">Aktiif yarışma id</param>
        /// <param name="followingUsers">Takip edilen kullanıcılar</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Takip edilen kullanıcıların sıralama listesi</returns>
        public ServiceModel<RankModel> GetFollowingRanking(short subCategoryId, BalanceTypes balanceType, short contestDateId, IEnumerable<string> followingUsers, PagingModel pagingModel)
        {
            string sql = @"SELECT CASE
                                  WHEN @balanceType=1 THEN sr.DisplayTotalGoldScore
                                  WHEN @balanceType=2 THEN sr.DisplayTotalMoneyScore
                                  END AS TotalScore,
                                  sr.UserId
                        FROM ScoreRankings sr
                        WHERE sr.ContestDateId = @contestDateId
                           AND sr.SubCategoryId = @subCategoryId
                           AND sr.UserId IN @followingUsers
                        ORDER BY
                        CASE
                            WHEN @balanceType=1 THEN sr.TotalGoldScore
                            WHEN @balanceType=2 THEN sr.TotalMoneyScore
                        END DESC";

            return _scoreRankingRepository.ToServiceModel<RankModel>(sql, new
            {
                subCategoryId,
                balanceType,
                contestDateId,
                followingUsers
            }, pagingModel: pagingModel);
        }

        /// <summary>
        /// Skor tablosunda güncelleme yapar
        /// </summary>
        /// <param name="userId">Kullanıcı id </param>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="balanceType">Bakiye tipi</param>
        /// <param name="score">Skor</param>
        /// <returns>Başarılı ise true değilse false</returns>
        public async Task<bool> UpdateScoreRank(string userId, short subCategoryId, BalanceTypes balanceType, byte score)
        {
            string sql = @"SELECT * FROM ScoreRankings sr WHERE sr.UserId = @userId AND sr.subCategoryId = @subCategoryId";
            Tables.ScoreRanking scoreRanking = _scoreRankingRepository.QuerySingleOrDefault<Tables.ScoreRanking>(sql, new
            {
                userId,
                subCategoryId
            });

            if (scoreRanking == null)
            {
                ContestDateModel contestDate = _contestDateRepository.ActiveContestDate();

                scoreRanking = new Tables.ScoreRanking
                {
                    UserId = userId,
                    SubCategoryId = subCategoryId,
                    ContestDateId = contestDate.ContestDateId,
                };
            }

            switch (balanceType)
            {
                case BalanceTypes.Gold:
                    scoreRanking.TotalGoldScore += score;
                    scoreRanking.DisplayTotalGoldScore = _numberFormatService.NumberFormating(scoreRanking.TotalGoldScore);
                    break;

                case BalanceTypes.Money:
                    scoreRanking.TotalMoneyScore += score;
                    scoreRanking.DisplayTotalMoneyScore = _numberFormatService.NumberFormating(scoreRanking.TotalMoneyScore);
                    break;

                default:
                    break;
            }

            if (scoreRanking.ScoreRankingId == 0)
            {
                int? scoreRankingId = await _scoreRankingRepository.AddAsync(scoreRanking);

                return scoreRankingId.HasValue;
            }

            return await _scoreRankingRepository.UpdateAsync(scoreRanking);
        }

        #endregion Methods
    }
}
