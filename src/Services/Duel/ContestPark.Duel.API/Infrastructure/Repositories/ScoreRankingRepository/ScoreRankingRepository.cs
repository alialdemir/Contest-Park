using ContestPark.Core.Database.Interfaces;
using ContestPark.Core.Database.Models;
using ContestPark.Duel.API.Enums;
using ContestPark.Duel.API.Infrastructure.Repositories.ContestDate;
using ContestPark.Duel.API.Models;
using ContestPark.Duel.API.Services.NumberFormat;
using System.Collections.Generic;
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
        /// Alt kategori id ve bakiye tipine göre skor sıralama listesi verir
        /// </summary>
        /// <param name="subCategoryId">Alt kategori id</param>
        /// <param name="balanceType">Bakiye tip</param>
        /// <param name="pagingModel">Sayfalama</param>
        /// <returns>Alt kategori sıralaması</returns>
        public ServiceModel<RankModel> GetRankingBySubCategoryId(short subCategoryId, BalanceTypes balanceType, short contestDateId, PagingModel pagingModel)
        {
            string sql = @"SELECT CASE
                                  WHEN @balanceType=1 THEN sr.TotalGoldScore
                                  WHEN @balanceType=2 THEN sr.TotalMoneyScore
                                  END AS TotalScore,
                                  sr.UserId
                        FROM ScoreRankings sr
                        WHERE sr.ContestDateId = @contestDateId
                           AND sr.SubCategoryId = @subCategoryId
                        ORDER BY
                        CASE
                            WHEN @balanceType=1 THEN sr.TotalGoldScore
                            WHEN @balanceType=2 THEN sr.TotalMoneyScore
                        END DESC";

            return _scoreRankingRepository.ToServiceModel<RankModel>(sql, new
            {
                subCategoryId,
                balanceType,
                contestDateId
            }, pagingModel: pagingModel);
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
                                  WHEN @balanceType=1 THEN sr.TotalGoldScore
                                  WHEN @balanceType=2 THEN sr.TotalMoneyScore
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
        public Task<bool> UpdateScoreRank(string userId, short subCategoryId, BalanceTypes balanceType, byte score)
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
                return _scoreRankingRepository.AddAsync(scoreRanking);
            }

            return _scoreRankingRepository.UpdateAsync(scoreRanking);
        }

        #endregion Methods
    }
}
